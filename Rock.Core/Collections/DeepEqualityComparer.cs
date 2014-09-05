using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Rock.Extensions;

namespace Rock.Collections
{
    // ReSharper disable PossibleMultipleEnumeration
    public class DeepEqualityComparer : IEqualityComparer
    {
        private static readonly IEqualityComparer _instance = new DeepEqualityComparer();

        private readonly ConcurrentDictionary<Type, Func<object, object, bool>> _equalsFuncs = new ConcurrentDictionary<Type, Func<object, object, bool>>();
        private readonly ConcurrentDictionary<Type, Func<object, int>> _getHashCodeFuncs = new ConcurrentDictionary<Type, Func<object, int>>();

        private readonly IEqualityComparer _this;

        private DeepEqualityComparer()
        {
            _this = this;
        }

        public static IEqualityComparer Instance
        {
            get { return _instance; }
        }

        public static IEqualityComparer<T> GetInstance<T>()
        {
            return DeepEqualityComparer<T>.Instance;
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

            var equalsFunc = RetrieveEqualsFunc(type, Enumerable.Empty<Type>());
            return equalsFunc(lhs, rhs);
        }

        private Func<object, object, bool> RetrieveEqualsFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
        {
            return
                _equalsFuncs.GetOrAdd(
                    type,
                    t =>
                    {
                        if (t.IsPrimitivish())
                        {
                            return (l, r) => l.Equals(r);
                        }

                        // TODO: Determine if IDictionary or IDictionary<,> need special handling. If so, do that here. Probably.

                        if (typeof(IEnumerable).IsAssignableFrom(t))
                        {
                            if (typeof(ICollection).IsAssignableFrom(t))
                            {
                                return CreateAreEqualCollectionsFunc(t, typesCurrentlyUnderConstruction);
                            }

                            var genericICollectionType =
                                t.GetInterfaces()
                                    .FirstOrDefault(i =>
                                        i.IsGenericType
                                        && i.GetGenericTypeDefinition() == typeof(ICollection<>));

                            if (genericICollectionType != null)
                            {
                                var createAreEqualGenericCollectionsFuncMethod =
                                    typeof(DeepEqualityComparer).GetMethod(
                                        "CreateAreEqualGenericCollectionsFunc",
                                        BindingFlags.NonPublic | BindingFlags.Instance)
                                    .MakeGenericMethod(genericICollectionType.GetGenericArguments()[0]);

                                return
                                    (Func<object, object, bool>)createAreEqualGenericCollectionsFuncMethod
                                        .Invoke(this, new object[] { t, typesCurrentlyUnderConstruction });
                            }

                            return CreateAreEqualEnumerablesFunc(t, typesCurrentlyUnderConstruction);
                        }

                        return CreateAreEqualObjectsFunc(t, typesCurrentlyUnderConstruction.Concat(t));
                    });
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

            var closedIEnumerable =
                type.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

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
            var properties = GetReadableProperties(type);

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
                        accumulatedExpression == null
                        // Only the first item will have a null accumulatedExpression...
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
            PropertyInfo property,
            Expression lhsParameterExpression,
            Expression rhsParameterExpression,
            IEnumerable<Type> typesCurrentlyUnderConstruction)
        {
            var lhsPropertyValueExpression =
                BoxIfNecessary(Expression.Property(lhsParameterExpression, property));
            var rhsPropertyValueExpression =
                BoxIfNecessary(Expression.Property(rhsParameterExpression, property));

            if (property.PropertyType.IsSealed
                && !CanCreateEqualsCycle(property.PropertyType, typesCurrentlyUnderConstruction))
            {
                var equalsFunc = GetOptimizedEqualsFunc(property.PropertyType, Enumerable.Empty<Type>());
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

            var getHashCodeFunc = RetrieveGetHashCodeFunc(instance.GetType(), Enumerable.Empty<Type>());
            return getHashCodeFunc(instance);
        }

        private Func<object, int> RetrieveGetHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
        {
            return
                _getHashCodeFuncs.GetOrAdd(
                    type,
                    t =>
                    {
                        if (t.IsPrimitivish())
                        {
                            return obj => obj.GetHashCode();
                        }

                        if (typeof(IEnumerable).IsAssignableFrom(t))
                        {
                            return CreateGetAggregatedHashCodeFunc(t, typesCurrentlyUnderConstruction);
                        }

                        return CreateObjectGetHashCodeFunc(t, typesCurrentlyUnderConstruction.Concat(t));
                    });
        }

        private Func<object, int> CreateGetAggregatedHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
        {
            Func<object, int> getHashCodeFunc = _this.GetHashCode;

            var closedIEnumerable =
                type.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (closedIEnumerable != null)
            {
                var itemType = closedIEnumerable.GetGenericArguments()[0];

                if (itemType.IsSealed)
                {
                    getHashCodeFunc = GetTopLevelGetHashCodeFunc(itemType, typesCurrentlyUnderConstruction);
                }
            }

            return
                obj =>
                {
                    unchecked
                    {
                        return ((IEnumerable)obj).Cast<object>().Aggregate(0, (hashCode, x) => AccumulateHashCode(hashCode, x, getHashCodeFunc));
                    }
                };
        }

        private Func<object, int> CreateObjectGetHashCodeFunc(Type type, IEnumerable<Type> typesCurrentlyUnderConstruction)
        {
            var parameterExpression = Expression.Parameter(typeof(object), "obj");

            var properties = GetReadableProperties(type);

            if (!properties.Any())
            {
                return obj => 0;
            }

            var instanceVariableExpression = Expression.Variable(type, "instance");

            var assignToInstanceVariableExpression =
                Expression.Assign(
                    instanceVariableExpression,
                    Expression.Convert(parameterExpression, type));

            var accumulateHashCodeMethod =
                typeof(DeepEqualityComparer)
                    .GetMethod("AccumulateHashCode", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(int), typeof(object), typeof(Func<object, int>) }, null);

            var getAggregatedHashCodeExpression =
                properties
                    .Select(property => new
                    {
                        Property = property,
                        GetHashCodeFunc = GetTopLevelGetHashCodeFunc(property.PropertyType, typesCurrentlyUnderConstruction)
                    })
                    .Aggregate(
                        (Expression)Expression.Constant(0),
                        (currentHashCodeExpression, item) =>
                            Expression.Call(
                                accumulateHashCodeMethod,
                                currentHashCodeExpression,
                                BoxIfNecessary(Expression.Property(instanceVariableExpression, item.Property)),
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

        private static List<PropertyInfo> GetReadableProperties(Type type)
        {
            return type.GetProperties()
                .Where(p =>
                    p.CanRead
                    && p.GetGetMethod() != null
                    && p.GetGetMethod().IsPublic)
                .ToList();
        }
    }
    // ReSharper restore PossibleMultipleEnumeration
}
