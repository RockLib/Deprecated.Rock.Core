using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Rock.Reflection;

namespace Rock.Collections
{
    public class DeepEqualityComparer : IEqualityComparer
    {
        private const StringComparison _defaultStringComparison = StringComparison.Ordinal;
        private static readonly IMemberLocator _defaultMemberLocator = new DefaultMemberLocator();
        private static readonly IConfiguration _defaultConfiguration = new DeepEqualityComparerConfiguration { StringComparison = _defaultStringComparison, MemberLocator = _defaultMemberLocator };

        private static readonly ConcurrentDictionary<ImplementationKey, Implementation> _implementations = new ConcurrentDictionary<ImplementationKey, Implementation>(); 

        private readonly IEqualityComparer _implementation;

        public DeepEqualityComparer()
            : this(_defaultConfiguration)
        {
        }

        public DeepEqualityComparer(IConfiguration configuration)
        {
            configuration = configuration ?? _defaultConfiguration;
            var memberLocator = configuration.MemberLocator ?? _defaultMemberLocator;

            _implementation =
                _implementations.GetOrAdd(
                    new ImplementationKey(configuration.StringComparison, memberLocator),
                    key => new Implementation(GetStringComparer(configuration.StringComparison), memberLocator));
        }

        public new bool Equals(object x, object y)
        {
            return _implementation.Equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return _implementation.GetHashCode(obj);
        }

        private static StringComparer GetStringComparer(StringComparison stringComparison)
        {
            switch (stringComparison)
            {
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
                default:
                    throw new ArgumentOutOfRangeException("stringComparison");
            }
        }

        // ReSharper disable PossibleMultipleEnumeration
        private class Implementation : IEqualityComparer
        {
            private readonly ConcurrentDictionary<Type, Func<object, object, bool>> _equalsFuncs = new ConcurrentDictionary<Type, Func<object, object, bool>>();
            private readonly ConcurrentDictionary<Type, Func<object, int>> _getHashCodeFuncs = new ConcurrentDictionary<Type, Func<object, int>>();

            private readonly StringComparer _stringComparer;
            private readonly IMemberLocator _memberLocator;
            private readonly IEqualityComparer _this;

            public Implementation(StringComparer stringComparer, IMemberLocator memberLocator)
            {
                _stringComparer = stringComparer;
                _memberLocator = memberLocator;
                _this = this;
            }

            bool IEqualityComparer.Equals(object lhs, object rhs)
            {
                if (ReferenceEquals(lhs, rhs))
                {
                    return true;
                }

                if (ReferenceEquals(lhs, null))
                {
                    return false;
                }

                Type type;

                if (ReferenceEquals(rhs, null)
                    || !ReferenceEquals(type = lhs.GetType(), rhs.GetType()))
                {
                    return false;
                }

                var equalsFunc = RetrieveEqualsFunc(type, Type.EmptyTypes);
                return equalsFunc(lhs, rhs);
            }

            private Func<object, object, bool> RetrieveEqualsFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                return
                    _equalsFuncs.GetOrAdd(
                        type,
                        t =>
                        {
                            if (t == typeof(string))
                            {
                                return _stringComparer.Equals;
                            }

                            var equatableType = t.GetClosedGenericType(typeof(IEquatable<>), new[] { t });
                            if (equatableType != null)
                            {
                                return CreateEquatableEqualsFunc(equatableType);
                            }

                            if (t.IsPrimitivish() || HasOverriddenEqualsMethod(t))
                            {
                                return (l, r) => l.Equals(r);
                            }

                            if (typeof(IEnumerable).IsAssignableFrom(t))
                            {
                                var genericIDictionaryType = t.GetClosedGenericType(typeof(IDictionary<,>));
                                if (genericIDictionaryType != null)
                                {
                                    var keyType = genericIDictionaryType.GetGenericArguments()[0];
                                    var valueType = genericIDictionaryType.GetGenericArguments()[1];
                                    return CreateAreEqualDictionariesFunc(keyType, valueType, typesCurrentlyUnderConstruction);
                                }

                                if (typeof(IDictionary).IsAssignableFrom(t))
                                {
                                    return CreateAreEqualDictionariesFunc();
                                }

                                if (typeof(ICollection).IsAssignableFrom(t))
                                {
                                    return CreateAreEqualCollectionsFunc(t, typesCurrentlyUnderConstruction);
                                }

                                var genericICollectionType = t.GetClosedGenericType(typeof(ICollection<>));
                                if (genericICollectionType != null)
                                {
                                    var itemType = genericICollectionType.GetGenericArguments()[0];
                                    return CreateAreEqualCollectionsFunc(t, itemType, typesCurrentlyUnderConstruction);
                                }

                                return CreateAreEqualEnumerablesFunc(t, typesCurrentlyUnderConstruction);
                            }

                            return CreateAreEqualObjectsFunc(t, typesCurrentlyUnderConstruction.Concat(t));
                        });
            }

            private Func<object, object, bool> CreateEquatableEqualsFunc(Type equatableType)
            {
                var lhsParameter = Expression.Parameter(typeof(object), "lhs");
                var rhsParameter = Expression.Parameter(typeof(object), "rhs");

                var lambda =
                    Expression.Lambda<Func<object, object, bool>>(
                        Expression.Call(
                            Expression.Convert(lhsParameter, equatableType),
                            equatableType.GetMethod("Equals"),
                            new Expression[] { Expression.Convert(rhsParameter, equatableType.GetGenericArguments()[0]) }),
                        lhsParameter,
                        rhsParameter);

                return lambda.Compile();
            }

            private static bool HasOverriddenEqualsMethod(Type type)
            {
                var equalsMethod = type.GetMethod("Equals", new[] {typeof(object)});
                return equalsMethod != null && equalsMethod.DeclaringType == type;
            }

            private Func<object, object, bool> CreateAreEqualDictionariesFunc(Type keyType, Type valueType, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
                var iDictionaryType = typeof(IDictionary<,>).MakeGenericType(keyType, valueType);

                var getEnumerator = GetGetEnumeratorFunc();
                var getKeyFromKeyValuePair = GetPropertyAccessorFunc(keyValuePairType, "Key");
                var tryGetValue = GetTryGetValueFunc(keyType, valueType, iDictionaryType);
                var getValueFromKeyValuePair = GetPropertyAccessorFunc(keyValuePairType, "Value");

                Func<object, object, bool> valueEquals;

                if (valueType.IsSealed
                    && !CanCreateEqualsCycle(valueType, typesCurrentlyUnderConstruction))
                {
                    valueEquals = GetOptimizedEqualsFunc(valueType, typesCurrentlyUnderConstruction);
                }
                else
                {
                    valueEquals = _this.Equals;
                }

                return
                    (lhsObject, rhsObject) =>
                    {
                        var lhsEnumerator = getEnumerator(lhsObject);

                        while (lhsEnumerator.MoveNext())
                        {
                            var lhsKey = getKeyFromKeyValuePair(lhsEnumerator.Current);

                            object rhsValue;
                            if (!tryGetValue(rhsObject, lhsKey, out rhsValue))
                            {
                                return false;
                            }

                            if (!valueEquals(getValueFromKeyValuePair(lhsEnumerator.Current), rhsValue))
                            {
                                return false;
                            }
                        }

                        return true;
                    };
            }

            private Func<object, object, bool> CreateAreEqualDictionariesFunc()
            {
                return
                    (lhsObject, rhsObject) =>
                    {
                        var lhs = (IDictionary)lhsObject;
                        var rhs = (IDictionary)rhsObject;

                        if (lhs.Count != rhs.Count)
                        {
                            return false;
                        }

                        var lhsEnumerator = lhs.GetEnumerator();
                        while (lhsEnumerator.MoveNext())
                        {
                            if (!rhs.Contains(lhsEnumerator.Key))
                            {
                                return false;
                            }

                            var rhsValue = rhs[lhsEnumerator.Key];
                            if (!_this.Equals(lhsEnumerator.Value, rhsValue))
                            {
                                return false;
                            }
                        }

                        return true;
                    };
            }

            private static TryFunc<object, object, object> GetTryGetValueFunc(Type keyType, Type valueType, Type iDictionaryType)
            {
                var objParameter = Expression.Parameter(typeof(object), "obj");
                var argParameter = Expression.Parameter(typeof(object), "arg");
                var resultParameter = Expression.Parameter(typeof(object).MakeByRefType(), "result");

                var valueVariable = Expression.Variable(valueType, "value");
                var foundVariable = Expression.Variable(typeof(bool), "found");

                var tryGetValue =
                    Expression.Assign(
                        foundVariable,
                        Expression.Call(
                            Expression.Convert(objParameter, iDictionaryType),
                            iDictionaryType.GetMethod("TryGetValue"),
                            Expression.Convert(argParameter, keyType),
                            valueVariable));

                var ifTrue =
                    Expression.Assign(
                        resultParameter,
                        BoxIfNecessary(valueVariable));

                var ifFalse =
                    Expression.Assign(
                        resultParameter,
                        BoxIfNecessary(Expression.Default(valueType)));

                var ifThenElse =
                    Expression.IfThenElse(
                        tryGetValue,
                        ifTrue,
                        ifFalse);

                var blockExpression =
                    Expression.Block(
                        typeof(bool),
                        new[] { valueVariable, foundVariable },
                        ifThenElse,
                        foundVariable);

                var lambda =
                    Expression.Lambda<TryFunc<object, object, object>>(
                        blockExpression,
                        objParameter,
                        argParameter,
                        resultParameter);

                return lambda.Compile();
            }

            private static Func<object, IEnumerator> GetGetEnumeratorFunc()
            {
                var objParameter = Expression.Parameter(typeof(object), "obj");

                var lambda =
                    Expression.Lambda<Func<object, IEnumerator>>(
                        Expression.Call(
                            Expression.Convert(objParameter, typeof(IEnumerable)),
                            typeof(IEnumerable).GetMethod("GetEnumerator")),
                        objParameter);

                return lambda.Compile();
            }

            private static Func<object, object> GetPropertyAccessorFunc(Type type, string propertyName)
            {
                var objParameter = Expression.Parameter(typeof(object), "obj");

                var lambda =
                    Expression.Lambda<Func<object, object>>(
                        BoxIfNecessary(
                            Expression.Property(
                                Expression.Convert(objParameter, type),
                                type.GetProperty(propertyName))),
                        objParameter);

                return lambda.Compile();
            }

            private Func<object, object, bool> CreateAreEqualCollectionsFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var equalsFunc = GetTopLevelEqualsFunc(type, typesCurrentlyUnderConstruction);

                return
                    (lhsObject, rhsObject) =>
                    {
                        var lhs = (ICollection)lhsObject;
                        var rhs = (ICollection)rhsObject;

                        if (lhs.Count != rhs.Count)
                        {
                            return false;
                        }

                        var lhsEnumerator = lhs.GetEnumerator();
                        var rhsEnumerator = rhs.GetEnumerator();

                        do
                        {
                            bool hasNext = lhsEnumerator.MoveNext();

                            if (!hasNext)
                            {
                                // If we're at the end of both collections, we're equal.
                                // We don't have to call rhsEnumerator.MoveNext() because
                                // we already made sure they had the same number of items.
                                return true;
                            }

                            // But if we're not at the end of the collections, we still need
                            // to advance rhs's enumerator.
                            rhsEnumerator.MoveNext();

                            if (!equalsFunc(lhsEnumerator.Current, rhsEnumerator.Current))
                            {
                                // If the left item isn't equal to the right item, we're not equal.
                                return false;
                            }
                        } while (true);
                    };
            }

            private Func<object, object, bool> CreateAreEqualCollectionsFunc(
                Type type,
                Type itemType,
                IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var createAreEqualGenericCollectionsFuncMethod =
                    typeof(Implementation).GetMethod(
                        "CreateAreEqualGenericCollectionsFunc",
                        BindingFlags.NonPublic | BindingFlags.Instance)
                        .MakeGenericMethod(itemType);

                return
                    (Func<object, object, bool>)createAreEqualGenericCollectionsFuncMethod
                        .Invoke(this, new object[] { type, typesCurrentlyUnderConstruction });
            }

            // ReSharper disable once UnusedMember.Local
            private Func<object, object, bool> CreateAreEqualGenericCollectionsFunc<T>(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var equalsFunc = GetTopLevelEqualsFunc(type, typesCurrentlyUnderConstruction);

                return
                    (lhsObject, rhsObject) =>
                    {
                        var lhs = (ICollection<T>)lhsObject;
                        var rhs = (ICollection<T>)rhsObject;

                        if (lhs.Count != rhs.Count)
                        {
                            return false;
                        }

                        var lhsEnumerator = lhs.GetEnumerator();
                        var rhsEnumerator = rhs.GetEnumerator();

                        do
                        {
                            bool hasNext = lhsEnumerator.MoveNext();

                            if (!hasNext)
                            {
                                // If we're at the end of both collections, we're equal.
                                // We don't have to call rhsEnumerator.MoveNext() because
                                // we already made sure they had the same number of items.
                                return true;
                            }

                            // But if we're not at the end of the collections, we still need
                            // to advance rhs's enumerator.
                            rhsEnumerator.MoveNext();

                            if (!equalsFunc(lhsEnumerator.Current, rhsEnumerator.Current))
                            {
                                // If the left item isn't equal to the right item, we're not equal.
                                return false;
                            }
                        } while (true);
                    };
            }

            private Func<object, object, bool> CreateAreEqualEnumerablesFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var equalsFunc = GetTopLevelEqualsFunc(type, typesCurrentlyUnderConstruction);

                return
                    (lhsObject, rhsObject) =>
                    {
                        var lhs = (IEnumerable)lhsObject;
                        var rhs = (IEnumerable)rhsObject;

                        var lhsEnumerator = lhs.GetEnumerator();
                        var rhsEnumerator = rhs.GetEnumerator();

                        do
                        {
                            bool hasNext = lhsEnumerator.MoveNext();

                            if (hasNext != rhsEnumerator.MoveNext())
                            {
                                // If they have a different number of items, we're not equal.
                                return false;
                            }

                            if (!hasNext)
                            {
                                // If we're at the end of both collections, we're equal.
                                return true;
                            }

                            if (!equalsFunc(lhsEnumerator.Current, rhsEnumerator.Current))
                            {
                                // If the left item isn't equal to the right item, we're not equal.
                                return false;
                            }
                        } while (true);
                    };
            }

            private Func<object, object, bool> GetTopLevelEqualsFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                Func<object, object, bool> equalsFunc = _this.Equals;

                var closedIEnumerable = type.GetClosedGenericType(typeof(IEnumerable<>));
                if (closedIEnumerable != null)
                {
                    var itemType = closedIEnumerable.GetGenericArguments()[0];

                    if (itemType.IsSealed
                        && !CanCreateEqualsCycle(itemType, typesCurrentlyUnderConstruction))
                    {
                        equalsFunc = GetOptimizedEqualsFunc(itemType, typesCurrentlyUnderConstruction.Concat(itemType));
                    }
                }

                return equalsFunc;
            }

            private Func<object, object, bool> CreateAreEqualObjectsFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var properties = _memberLocator.GetFieldsAndProperties(type).ToList();

                if (!properties.Any())
                {
                    return (lhs, rhs) => true;
                }

                var lhsParameterExpression = Expression.Parameter(typeof(object), "lhs");
                var rhsParameterExpression = Expression.Parameter(typeof(object), "rhs");

                var lhsInstanceExpression = Expression.Variable(type, "lhsInstance");
                var rhsInstanceExpression = Expression.Variable(type, "rhsInstance");

                var assignLhsInstanceExpression = Expression.Assign(lhsInstanceExpression,
                    Expression.Convert(lhsParameterExpression, type));
                var assignRhsInstanceExpression = Expression.Assign(rhsInstanceExpression,
                    Expression.Convert(rhsParameterExpression, type));

                var areEqualPropertyValuesExpressions =
                    properties
                        .Select(p =>
                            GetAreEqualPropertyValuesExpression(
                                p,
                                lhsInstanceExpression,
                                rhsInstanceExpression,
                                typesCurrentlyUnderConstruction));

                var areAllPropertyValuesTrueExpression =
                    areEqualPropertyValuesExpressions.Aggregate(
                        (Expression)null,
                        (accumulatedExpression, currentExpression) =>
                            accumulatedExpression == null // Only the first item will have a null accumulatedExpression...
                                ? currentExpression // ...so just return currentExpression when it is the first item.
                                : Expression.AndAlso(accumulatedExpression, currentExpression)); // The rest of the items accumulate with Expression.AndAlso.

                var block =
                    Expression.Block(
                        typeof(bool),
                        new[] { lhsInstanceExpression, rhsInstanceExpression },
                        assignLhsInstanceExpression,
                        assignRhsInstanceExpression,
                        areAllPropertyValuesTrueExpression);

                var lambda =
                    Expression.Lambda<Func<object, object, bool>>(
                        block,
                        lhsParameterExpression,
                        rhsParameterExpression);

                return lambda.Compile();
            }

            private Func<object, object, bool> GetOptimizedEqualsFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var equalsFunc = RetrieveEqualsFunc(type, typesCurrentlyUnderConstruction);

                if (type.IsValueType)
                {
                    return
                        (lhs, rhs) =>
                            ReferenceEquals(lhs.GetType(), rhs.GetType())
                            && equalsFunc(lhs, rhs);
                }

                return
                    (lhs, rhs) =>
                    {
                        if (ReferenceEquals(lhs, rhs))
                        {
                            return true;
                        }

                        if (ReferenceEquals(lhs, null)
                            || ReferenceEquals(rhs, null)
                            || !ReferenceEquals(lhs.GetType(), rhs.GetType()))
                        {
                            return false;
                        }

                        return equalsFunc(lhs, rhs);
                    };
            }

            private Expression GetAreEqualPropertyValuesExpression(
                PropertyOrField property,
                Expression lhsParameterExpression,
                Expression rhsParameterExpression,
                IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var lhsPropertyValueExpression =
                    BoxIfNecessary(Expression.PropertyOrField(lhsParameterExpression, property.Name));
                var rhsPropertyValueExpression =
                    BoxIfNecessary(Expression.PropertyOrField(rhsParameterExpression, property.Name));

                if (property.PropertyOrFieldType.IsSealed
                    && !CanCreateEqualsCycle(property.PropertyOrFieldType, typesCurrentlyUnderConstruction))
                {
                    var equalsFunc = GetOptimizedEqualsFunc(property.PropertyOrFieldType, Type.EmptyTypes);
                    return
                        Expression.Invoke(
                            Expression.Constant(equalsFunc),
                            lhsPropertyValueExpression,
                            rhsPropertyValueExpression);
                }

                var thisClosure = Expression.Constant(this);
                var equalsMethod = typeof(IEqualityComparer).GetMethod("Equals");

                return
                    Expression.Call(
                        thisClosure,
                        equalsMethod,
                        lhsPropertyValueExpression,
                        rhsPropertyValueExpression);
            }

            private bool CanCreateEqualsCycle(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                return typesCurrentlyUnderConstruction.Contains(type) && !_equalsFuncs.ContainsKey(type);
            }

            int IEqualityComparer.GetHashCode(object instance)
            {
                if (instance == null)
                {
                    return 0;
                }

                var getHashCodeFunc = RetrieveGetHashCodeFunc(instance.GetType(), Type.EmptyTypes);
                return getHashCodeFunc(instance);
            }

            private Func<object, int> RetrieveGetHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                return
                    _getHashCodeFuncs.GetOrAdd(
                        type,
                        t =>
                        {
                            if (t == typeof(string))
                            {
                                return _stringComparer.GetHashCode;
                            }

                            if (t.IsPrimitivish() || HasOverriddenGetHashCodeMethod(t))
                            {
                                return obj => obj.GetHashCode();
                            }

                            var iDictionaryType =
                                t.GetClosedGenericType(typeof(IDictionary<,>))
                                ?? (typeof(IDictionary).IsAssignableFrom(t)
                                    ? typeof(IDictionary)
                                    : null);

                            if (iDictionaryType != null)
                            {
                                return CreateGetDictionaryHashCodeFunc(t, iDictionaryType, typesCurrentlyUnderConstruction);
                            }

                            if (typeof(IEnumerable).IsAssignableFrom(t))
                            {
                                return CreateGetEnumerableHashCodeFunc(t, typesCurrentlyUnderConstruction);
                            }

                            return CreateObjectGetHashCodeFunc(t, typesCurrentlyUnderConstruction.Concat(t));
                        });
            }

            private Func<object, int> CreateGetDictionaryHashCodeFunc(Type type, Type iDictionaryType, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                Func<object, int> getKeyHashCodeFunc = _this.GetHashCode;
                Func<object, int> getValueHashCodeFunc = _this.GetHashCode;

                Type dictionaryItemType;

                var needsNullCheck = true;

                var closedIDictionary = type.GetClosedGenericType(typeof(IDictionary<,>));
                if (closedIDictionary != null)
                {
                    var keyType = closedIDictionary.GetGenericArguments()[0];
                    var valueType = closedIDictionary.GetGenericArguments()[1];

                    needsNullCheck = !valueType.IsValueType;

                    if (keyType.IsSealed)
                    {
                        getKeyHashCodeFunc = GetTopLevelGetHashCodeFunc(keyType, typesCurrentlyUnderConstruction);
                    }

                    if (valueType.IsSealed)
                    {
                        getValueHashCodeFunc = GetTopLevelGetHashCodeFunc(valueType, typesCurrentlyUnderConstruction);
                    }

                    dictionaryItemType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
                }
                else
                {
                    dictionaryItemType = typeof(DictionaryEntry);
                }

                var getKeyFunc = GetPropertyAccessorFunc(dictionaryItemType, "Key");
                var getValueFunc = GetPropertyAccessorFunc(dictionaryItemType, "Value");

                Func<int, object, int> accumulateHashCodes;

                if (needsNullCheck)
                {
                    accumulateHashCodes =
                        (hashCode, kvp) =>
                        {
                            unchecked
                            {
                                return
                                    hashCode ^
                                    ((getKeyHashCodeFunc(getKeyFunc(kvp)) * 397)
                                        ^ (getValueFunc(kvp) != null ? getValueHashCodeFunc(getValueFunc(kvp)) : 0));
                            }
                        };
                }
                else
                {
                    accumulateHashCodes =
                        (hashCode, kvp) =>
                        {
                            unchecked
                            {
                                return
                                    hashCode ^
                                    ((getKeyHashCodeFunc(getKeyFunc(kvp)) * 397)
                                        ^ getValueHashCodeFunc(getValueFunc(kvp)));
                            }
                        };
                }

                var objParameter = Expression.Parameter(typeof(object), "obj");

                var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(typeof(object));
                var getDictionaryItemCollection = Expression.Call(castMethod, Expression.Convert(objParameter, iDictionaryType));

                var hashCodeSeed = type.GetHashCode();

                var lambda =
                    Expression.Lambda<Func<object, int>>(
                        Expression.Call(
                            GetAggregateMethod().MakeGenericMethod(typeof(object), typeof(int)),
                            getDictionaryItemCollection,
                            Expression.Constant(hashCodeSeed),
                            Expression.Constant(accumulateHashCodes)),
                        objParameter);

                return lambda.Compile();
            }

            private static MethodInfo GetAggregateMethod()
            {
                return typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(
                        m =>
                        {
                            ParameterInfo[] parameters;
                            Type[] genericArguments;

                            if (m.Name != "Aggregate"
                                || (parameters = m.GetParameters()).Length != 3
                                || (genericArguments = m.GetGenericArguments()).Length != 2)
                            {
                                return false;
                            }

                            return
                                parameters[0].ParameterType == typeof(IEnumerable<>).MakeGenericType(genericArguments[0])
                                && parameters[1].ParameterType == genericArguments[1]
                                && parameters[2].ParameterType == typeof(Func<,,>).MakeGenericType(genericArguments[1], genericArguments[0], genericArguments[1]);
                        });
            }

            private static bool HasOverriddenGetHashCodeMethod(Type type)
            {
                var getHashCodeMethod = type.GetMethod("GetHashCode", Type.EmptyTypes);
                return getHashCodeMethod != null && getHashCodeMethod.DeclaringType == type;
            }

            private Func<object, int> CreateGetEnumerableHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                Func<object, int> getHashCodeFunc = _this.GetHashCode;

                var closedIEnumerable = type.GetClosedGenericType(typeof(IEnumerable<>));
                if (closedIEnumerable != null)
                {
                    var itemType = closedIEnumerable.GetGenericArguments()[0];

                    if (itemType.IsSealed)
                    {
                        getHashCodeFunc = GetTopLevelGetHashCodeFunc(itemType, typesCurrentlyUnderConstruction);
                    }
                }

                var hashCodeSeed = type.GetHashCode();

                return
                    obj =>
                    {
                        unchecked
                        {
                            return
                                ((IEnumerable)obj).Cast<object>()
                                    .Aggregate(
                                        hashCodeSeed,
                                        (currentHashCode, nextItem) =>
                                            AccumulateHashCode(currentHashCode, nextItem, getHashCodeFunc));
                        }
                    };
            }

            private Func<object, int> CreateObjectGetHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                var parameterExpression = Expression.Parameter(typeof(object), "obj");

                var hashCodeSeed = type.GetHashCode();

                var properties = _memberLocator.GetFieldsAndProperties(type).ToList();

                if (!properties.Any())
                {
                    return obj => hashCodeSeed;
                }

                var instanceVariableExpression = Expression.Variable(type, "instance");

                var assignToInstanceVariableExpression =
                    Expression.Assign(
                        instanceVariableExpression,
                        Expression.Convert(parameterExpression, type));

                var accumulateHashCodeMethod =
                    typeof(Implementation)
                        .GetMethod("AccumulateHashCode", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(int), typeof(object), typeof(Func<object, int>) }, null);

                var getAggregatedHashCodeExpression =
                    properties
                        .Select(property => new
                        {
                            Property = property,
                            GetHashCodeFunc = GetTopLevelGetHashCodeFunc(property.PropertyOrFieldType, typesCurrentlyUnderConstruction)
                        })
                        .Aggregate(
                            (Expression)Expression.Constant(hashCodeSeed),
                            (currentHashCodeExpression, item) =>
                                Expression.Call(
                                    accumulateHashCodeMethod,
                                    currentHashCodeExpression,
                                    BoxIfNecessary(Expression.PropertyOrField(instanceVariableExpression, item.Property.Name)),
                                    Expression.Constant(item.GetHashCodeFunc)));

                var block =
                    Expression.Block(
                        typeof(int),
                        new[] { instanceVariableExpression },
                        assignToInstanceVariableExpression,
                        getAggregatedHashCodeExpression);

                var lambda =
                    Expression.Lambda<Func<object, int>>(
                        block,
                        parameterExpression);

                return lambda.Compile();
            }

            private Func<object, int> GetTopLevelGetHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                if (CanCreateGetHashCodeCycle(type, typesCurrentlyUnderConstruction))
                {
                    return _this.GetHashCode;
                }

                var getHashCodeFunc = RetrieveGetHashCodeFunc(type, typesCurrentlyUnderConstruction);
                return
                    obj =>
                        obj != null
                            ? getHashCodeFunc(obj)
                            : 0;
            }

            private bool CanCreateGetHashCodeCycle(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
            {
                return typesCurrentlyUnderConstruction.Contains(type) && !_getHashCodeFuncs.ContainsKey(type);
            }

            private static Expression BoxIfNecessary(Expression expression)
            {
                return
                    expression.Type.IsValueType
                        ? Expression.Convert(expression, typeof(object))
                        : expression;
            }

            private static int AccumulateHashCode(int currentHashCode, object obj, Func<object, int> getHashCode)
            {
                return (currentHashCode * 397) ^ (obj != null ? getHashCode(obj) : 0);
            }
        }
        // ReSharper restore PossibleMultipleEnumeration

        private class ImplementationKey
        {
            private readonly StringComparison _stringComparison;
            private readonly IMemberLocator _memberLocator;
            private readonly int _hashCode;

            public ImplementationKey(StringComparison stringComparison, IMemberLocator memberLocator)
            {
                _stringComparison = stringComparison;
                _memberLocator = memberLocator;
                _hashCode = ((int)stringComparison * 397) ^ memberLocator.GetHashCode();
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                var other = (ImplementationKey)obj;
                return
                    _stringComparison == other._stringComparison
                    && _memberLocator.Equals(other._memberLocator);
            }
        }

        public class PropertyOrField
        {
            private readonly Type _propertyOrFieldType;
            private readonly string _name;

            public PropertyOrField(MemberInfo propertyOrField)
            {
                if (propertyOrField == null)
                {
                    throw new ArgumentNullException("propertyOrField");
                }

                if (!(propertyOrField is FieldInfo || propertyOrField is PropertyInfo))
                {
                    throw new ArgumentException("MemberInfo must be either a PropertyInfo or a FieldInfo.", "propertyOrField");
                }

                _propertyOrFieldType =
                    propertyOrField is PropertyInfo
                        ? ((PropertyInfo)propertyOrField).PropertyType
                        : ((FieldInfo)propertyOrField).FieldType;
                _name = propertyOrField.Name;
            }

            public Type PropertyOrFieldType
            {
                get { return _propertyOrFieldType; }
            }

            public string Name
            {
                get { return _name; }
            }
        }

        public interface IMemberLocator
        {
            IEnumerable<PropertyOrField> GetFieldsAndProperties(Type type);
        }

        public interface IConfiguration
        {
            StringComparison StringComparison { get; }
            IMemberLocator MemberLocator { get; }
        }
    }
}
