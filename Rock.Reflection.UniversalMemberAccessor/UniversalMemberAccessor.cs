using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Rock.Reflection
{
    /// <summary>
    /// A proxy object that enables access to all members of a type, both public and non-public.
    /// </summary>
    /// <remarks>This is a very dangerous class - use with caution.</remarks>
    public class UniversalMemberAccessor : DynamicObject
    {
        private static readonly ConditionalWeakTable<object, UniversalMemberAccessor> _instanceMap = new ConditionalWeakTable<object, UniversalMemberAccessor>();
        private static readonly ConcurrentDictionary<Type, UniversalMemberAccessor> _staticCache = new ConcurrentDictionary<Type, UniversalMemberAccessor>();
        private static readonly ConcurrentDictionary<Type, IEnumerable<string>> _instanceMemberNamesCache = new ConcurrentDictionary<Type, IEnumerable<string>>();
        private static readonly ConcurrentDictionary<Type, IEnumerable<string>> _staticMemberNamesCache = new ConcurrentDictionary<Type, IEnumerable<string>>();

        private static readonly ConcurrentDictionary<Type, bool> _canBeAssignedNullCache = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> _canBeAssignedCache = new ConcurrentDictionary<Tuple<Type, Type>, bool>();

        private static readonly ConcurrentDictionary<Tuple<Type, string>, Func<object, Object>> _getMemberFuncs = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string>, Action<object, object>> _setMemberActions = new ConcurrentDictionary<Tuple<Type, string>, Action<object, object>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string>, ReadOnlyCollection<InvokeMemberCandidate>> _invokeMemberFuncs = new ConcurrentDictionary<Tuple<Type, string>, ReadOnlyCollection<InvokeMemberCandidate>>();

        private readonly Lazy<ReadOnlyCollection<CreateInstanceCandidate>> _createInstanceCandidates;

        private readonly object _instance;
        private readonly Type _type;
        private readonly IEnumerable<string> _memberNames;

        private UniversalMemberAccessor(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            _type = type;

            _memberNames = _staticMemberNamesCache.GetOrAdd(_type, t =>
                t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(m => !(m is ConstructorInfo) && !(m is EventInfo) && !(m is Type))
                    .Select(m => m.Name)
                    .ToList()
                    .AsReadOnly());

            _createInstanceCandidates = GetLazyCreateInstanceCandidates();
        }

        private UniversalMemberAccessor(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            _instance = instance;
            _type = _instance.GetType();

            _memberNames = _instanceMemberNamesCache.GetOrAdd(_type, t =>
                t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => !(m is ConstructorInfo) && !(m is EventInfo) && !(m is Type))
                    .Select(m => m.Name)
                    .ToList()
                    .AsReadOnly());
        }

        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the given object. Returns null if <paramref name="instance"/> is null.
        /// </summary>
        /// <remarks>
        /// If this method is called more than once with the same object, then the value returned
        /// is the same instance of <see cref="UniversalMemberAccessor"/> each time.
        /// </remarks>
        /// <param name="instance">An object.</param>
        /// <returns>
        /// A dynamic proxy object enabling access to all members of the given instance, or null
        /// if <paramref name="instance"/> is null.
        /// </returns>
        /// <remarks>This is a very dangerous method - use with caution.</remarks>
        public static dynamic Get(object instance)
        {
            if (instance == null)
            {
                return null;
            }

            if (instance.GetType().IsValueType)
            {
                return new UniversalMemberAccessor(instance);
            }

            return _instanceMap.GetValue(instance, obj => new UniversalMemberAccessor(obj));
        }

        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the static members of the given type.
        /// </summary>
        /// <param name="type">The type whose static members will be exposed by the resulting dynamic proxy object.</param>
        /// <returns>A dynamic proxy object enabling access to all static members of the given type.</returns>
        /// <remarks>This is a very dangerous method - use with caution.</remarks>
        public static dynamic Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return _staticCache.GetOrAdd(type, t => new UniversalMemberAccessor(t));
        }

        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the static members of the type of the <typeparamref name="T"/> type parameter.
        /// </summary>
        /// <typeparam name="T">
        /// The type whose static members will be exposed by the resulting dynamic proxy object.
        /// </typeparam>
        /// <returns>A dynamic proxy object enabling access to all static members of the given type.</returns>
        /// <remarks>This is a very dangerous method - use with caution.</remarks>
        public static dynamic Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the static members of the type described by the <paramref name="assemblyQualifiedName"/>
        /// parameter.
        /// </summary>
        /// <param name="assemblyQualifiedName">
        /// The assembly qualified name of a type whose static members will be exposed by the
        /// resulting dynamic proxy object.
        /// </param>
        /// <returns>A dynamic proxy object enabling access to all static members of the given type.</returns>
        /// <remarks>This is a very dangerous method - use with caution.</remarks>
        public static dynamic Get(string assemblyQualifiedName)
        {
            return Get(Type.GetType(assemblyQualifiedName, true));
        }

        /// <summary>
        /// Provides the implementation for operations that get member values.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="result">The result of the get operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var getMember =
                _getMemberFuncs.GetOrAdd(
                    Tuple.Create(_type, binder.Name),
                    t => CreateGetMemberFunc(t.Item2));

            if (getMember == null)
            {
                switch (binder.Name)
                {
                    case "Instance":
                    case "Object":
                    case "Value":
                        if (_instance != null)
                        {
                            result = _instance;
                            return true;
                        }
                        break;
                }

                return base.TryGetMember(binder, out result);
            }

            result = getMember(_instance);
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="value">The value to set to the member.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var setMember =
                _setMemberActions.GetOrAdd(
                    Tuple.Create(_type, binder.Name),
                    t => CreateSetMemberAction(t.Item2));

            if (setMember == null)
            {
                return base.TrySetMember(binder, value);
            }

            setMember(_instance, value);
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <param name="binder">Provides information about the dynamic operation.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation.</param>
        /// <param name="result">The result of the member invocation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryInvokeMember(
            InvokeMemberBinder binder,
            object[] args,
            out object result)
        {
            var invokeMemberCandidates =
                _invokeMemberFuncs.GetOrAdd(
                    Tuple.Create(_type, binder.Name),
                    t => new ReadOnlyCollection<InvokeMemberCandidate>(CreateInvokeMemberCandidates(t.Item2).ToList()));

            var legalCandidates = invokeMemberCandidates.Where(c => c.IsLegal(args)).ToList();

            if (legalCandidates.Count == 1)
            {
                result = legalCandidates[0].InvokeMember(_instance, args);
                return true;
            }

            if (legalCandidates.Count == 0)
            {
                switch (binder.Name)
                {
                    case "New":
                    case "Create":
                    case "CreateInstance":
                    case "NewInstance":
                        if (TryCreateInstance(args, out result))
                        {
                            return true;
                        }
                        break;
                }
            }
            else
            {
                var betterMethods = GetBetterMethods(args, legalCandidates);

                if (betterMethods.Count == 1)
                {
                    result = betterMethods[0].InvokeMember(_instance, args);
                    return true;
                }
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        private bool TryCreateInstance(object[] args, out object result)
        {
            var legalCandidates = _createInstanceCandidates.Value.Where(c => c.IsLegal(args)).ToList();

            if (legalCandidates.Count == 1)
            {
                result = legalCandidates[0].CreateInstance(args);
                return true;
            }

            if (legalCandidates.Count == 0)
            {
                result = null;
                return false;
            }

            var betterMethods = GetBetterMethods(args, legalCandidates);

            if (betterMethods.Count == 1)
            {
                result = betterMethods[0].CreateInstance(args);
                return true;
            }

            result = null;
            return false;
        }

        private static IList<TCandidate> GetBetterMethods<TCandidate>(object[] args, IList<TCandidate> legalCandidates)
            where TCandidate : Candidate
        {
            var isBestCandidate = Enumerable.Repeat(true, legalCandidates.Count).ToList();

            for (int i = 0; i < legalCandidates.Count; i++)
            {
                var candidate = legalCandidates[i];

                for (int j = 0; j < legalCandidates.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var score = candidate.GetBetterScore(legalCandidates[j], args);

                    if (score < 1)
                    {
                        isBestCandidate[i] = false;
                    }
                }
            }

            var betterMethods =
                isBestCandidate
                    .Select((isBest, index) => new { isBest, index })
                    .Where(x => x.isBest)
                    .Select(x => legalCandidates[x.index])
                    .ToList();

            return betterMethods;
        }

        /// <summary>
        /// Provides the implementation for operations that get a value by index.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation.</param>
        /// <param name="result">The result of the index operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TryGetIndex(
            GetIndexBinder binder,
            object[] indexes,
            out object result)
        {
            if (indexes.Length == 1
                && indexes[0] is string)
            {
                var getMember =
                    _getMemberFuncs.GetOrAdd(
                        Tuple.Create(_type, (string)indexes[0]),
                        t => CreateGetMemberFunc(t.Item2));

                if (getMember != null)
                {
                    result = getMember(_instance);
                    return true;
                }
            }

            return base.TryGetIndex(binder, indexes, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that set a value by index.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation.</param>
        /// <param name="value">The value to set to the object that has the specified index.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TrySetIndex(
            SetIndexBinder binder,
            object[] indexes,
            object value)
        {
            if (indexes.Length == 1
                && indexes[0] is string)
            {
                var setMember =
                    _setMemberActions.GetOrAdd(
                        Tuple.Create(_type, (string)indexes[0]),
                        t => CreateSetMemberAction(t.Item2));

                if (setMember != null)
                {
                    setMember(_instance, value);
                    return true;
                }
            }

            return base.TrySetIndex(binder, indexes, value);
        }

        /// <summary>
        /// Provides implementation for type conversion operations.
        /// </summary>
        /// <param name="binder">Provides information about the conversion operation.</param>
        /// <param name="result">The result of the type conversion operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// </returns>
        public override bool TryConvert(
            ConvertBinder binder,
            out object result)
        {
            if (!binder.ReturnType.IsAssignableFrom(_type))
            {
                return base.TryConvert(binder, out result);
            }

            result = _instance;
            return true;
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names. 
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _memberNames;
        }

        private Func<object, object> CreateGetMemberFunc(string name)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(object), "instance");
                var convertParameter = Expression.Convert(parameter, _type);

                Expression propertyOrField;
                bool isDelegate;

                var propertyInfo = _type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (propertyInfo != null)
                {
                    propertyOrField = Expression.Property(IsStatic(propertyInfo) ? null : convertParameter, propertyInfo);
                    isDelegate = typeof(Delegate).IsAssignableFrom(propertyInfo.PropertyType);
                }
                else
                {
                    var fieldInfo = _type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (fieldInfo != null)
                    {
                        propertyOrField = Expression.Field(fieldInfo.IsStatic ? null : convertParameter, fieldInfo);
                        isDelegate = typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType);
                    }
                    else
                    {
                        return null;
                    }
                }

                var type = propertyOrField.Type;

                if (type.IsValueType)
                {
                    propertyOrField = Expression.Convert(propertyOrField, typeof(object));
                }

                var lambda =
                    Expression.Lambda<Func<object, object>>(
                        propertyOrField,
                        new[] { parameter });

                var func = lambda.Compile();

                if (ShouldReturnRawValue(type))
                {
                    return func;
                }

                return
                    isDelegate
                        ? func // Don't unlock delegate values.
                        : obj => func(obj).Unlock();
            }
            catch
            {
                return null;
            }
        }

        private Action<object, object> CreateSetMemberAction(string name)
        {
            try
            {
                var instanceParameter = Expression.Parameter(typeof(object), "instance");
                var valueParameter = Expression.Parameter(typeof(object), "value");

                var convertInstanceParameter = Expression.Convert(instanceParameter, _type);

                Expression propertyOrField;

                var propertyInfo = _type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (propertyInfo != null)
                {
                    propertyOrField = Expression.Property(IsStatic(propertyInfo) ? null : convertInstanceParameter, propertyInfo);
                }
                else
                {
                    var fieldInfo = _type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (fieldInfo != null)
                    {
                        propertyOrField = Expression.Field(fieldInfo.IsStatic ? null : convertInstanceParameter, fieldInfo);
                    }
                    else
                    {
                        return null;
                    }
                }

                var lambda =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Assign(propertyOrField, Expression.Convert(valueParameter, propertyOrField.Type)),
                        new[] { instanceParameter, valueParameter });

                var action = lambda.Compile();

                return (obj, value) =>
                {
                    var universalMemberAccessor = value as UniversalMemberAccessor;

                    if (universalMemberAccessor != null)
                    {
                        // Unwrap the value if it is a UniversalMemberAccessor.
                        value = universalMemberAccessor._instance;
                    }

                    action(obj, value);
                };
            }
            catch
            {
                return null;
            }
        }

        private Lazy<ReadOnlyCollection<CreateInstanceCandidate>> GetLazyCreateInstanceCandidates()
        {
            return new Lazy<ReadOnlyCollection<CreateInstanceCandidate>>(() =>
                new ReadOnlyCollection<CreateInstanceCandidate>(
                    GetCreateInstanceCandidates().ToList()));
        }

        private IEnumerable<CreateInstanceCandidate> GetCreateInstanceCandidates()
        {
            var constructorInfos = _type.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var constructor in constructorInfos)
            {
                var argsParameter = Expression.Parameter(typeof(object[]), "args");

                var methodInfoParameters = constructor.GetParameters();
                var newArguments = new Expression[methodInfoParameters.Length];

                for (var i = 0; i < newArguments.Length; i++)
                {
                    newArguments[i] =
                        Expression.Convert(
                            Expression.ArrayAccess(argsParameter, Expression.Constant(i)),
                            methodInfoParameters[i].ParameterType);
                }

                Expression body =
                    Expression.New(
                        constructor,
                        newArguments);

                if (constructor.DeclaringType.IsValueType)
                {
                    body = Expression.Convert(body, typeof(object));
                }

                var lambda =
                    Expression.Lambda<Func<object[], object>>(
                        body,
                        new[] { argsParameter });

                var func = lambda.Compile();

                if (ShouldReturnRawValue(constructor.DeclaringType))
                {
                    yield return new CreateInstanceCandidate(methodInfoParameters, args => func(UnwrapArgs(args)));
                }
                else
                {
                    yield return new CreateInstanceCandidate(methodInfoParameters, args => func(UnwrapArgs(args)).Unlock());
                }
            }
        }

        private IEnumerable<InvokeMemberCandidate> CreateInvokeMemberCandidates(string name)
        {
            var methodInfos =
                _type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => m.Name == name);

            return methodInfos.Select(CreateInvokeMemberCandidate).Where(func => func != null);
        }

        private InvokeMemberCandidate CreateInvokeMemberCandidate(MethodInfo methodInfo)
        {
            try
            {
                var instanceParameter = Expression.Parameter(typeof(object), "instance");
                var argsParameter = Expression.Parameter(typeof(object[]), "args");

                var methodInfoParameters = methodInfo.GetParameters();
                var callArguments = new Expression[methodInfoParameters.Length];

                for (var i = 0; i < callArguments.Length; i++)
                {
                    callArguments[i] =
                        Expression.Convert(
                            Expression.ArrayAccess(argsParameter, Expression.Constant(i)),
                            methodInfoParameters[i].ParameterType);
                }

                Expression body;

                if (methodInfo.IsStatic)
                {
                    body = Expression.Call(
                        methodInfo,
                        callArguments);
                }
                else
                {
                    body = Expression.Call(
                        Expression.Convert(instanceParameter, _type),
                        methodInfo,
                        callArguments);
                }

                if (methodInfo.ReturnType == typeof(void))
                {
                    body =
                        Expression.Block(
                            body,
                            Expression.Constant(null));
                }
                else if (methodInfo.ReturnType.IsValueType)
                {
                    body = Expression.Convert(body, typeof(object));
                }

                var lambda =
                    Expression.Lambda<Func<object, object[], object>>(
                        body,
                        new[] { instanceParameter, argsParameter });

                var func = lambda.Compile();

                if (ShouldReturnRawValue(methodInfo.ReturnType))
                {
                    return new InvokeMemberCandidate(methodInfoParameters, (obj, args) => func(obj, UnwrapArgs(args)));
                }

                return new InvokeMemberCandidate(methodInfoParameters, (obj, args) => func(obj, UnwrapArgs(args)).Unlock());
            }
            catch
            {
                return null;
            }
        }

        private static bool ShouldReturnRawValue(Type type)
        {
            return
                IsValue(type)
                || (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && IsValue(type.GetGenericArguments()[0]));
        }

        private static bool IsValue(Type type)
        {
            return
                type.IsPrimitive
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || type == typeof(DateTimeOffset);
        }

        // Potentially modifies the items of the array and returns it.
        private static object[] UnwrapArgs(object[] args)
        {
            // Unwrap any unprivates before sending to func.
            for (int i = 0; i < args.Length; i++)
            {
                var universalMemberAccessor = args[i] as UniversalMemberAccessor;
                if (universalMemberAccessor != null)
                {
                    args[i] = universalMemberAccessor._instance;
                }
            }

            return args;
        }

        private static bool IsStatic(PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod(true);
            var isStatic = (getMethod != null && getMethod.IsStatic) || propertyInfo.GetSetMethod(true).IsStatic;
            return isStatic;
        }

        private abstract class Candidate
        {
            private readonly Type[] _parameters;

            protected Candidate(IEnumerable<ParameterInfo> parameters)
            {
                _parameters = parameters.Select(p => p.ParameterType).ToArray();
            }

            public int GetBetterScore(Candidate other, object[] args)
            {
                int score = 0;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == null
                        || _parameters[i] == other._parameters[i])
                    {
                        continue;
                    }

                    AccumulateScore(_parameters[i], other._parameters[i], args[i].GetType(), ref score);
                }

                return score;
            }

            private static void AccumulateScore(Type thisParameter, Type otherParameter, Type argType, ref int score)
            {
                // sbyte to short, int, long, float, double, or decimal
                // byte to short, ushort, int, uint, long, ulong, float, double, or decimal
                // short to int, long, float, double, or decimal
                // ushort to int, uint, long, ulong, float, double, or decimal
                // int to long, float, double, or decimal
                // uint to long, ulong, float, double, or decimal
                // long to float, double, or decimal
                // ulong to float, double, or decimal
                // char to ushort, int, uint, long, ulong, float, double, or decimal
                // float to double
                // Nullable type conversion
                // Reference type to object
                // Interface to base interface
                // Array to array when arrays have the same number of dimensions, there is an implicit conversion from the source element type to the destination element type and the source element type and the destination element type are reference types
                // Array type to System.Array
                // Array type to IList<> and its base interfaces
                // Delegate type to System.Delegate
                // Boxing conversion
                // Enum type to System.Enum
                // User defined conversion (op_implicit)

                // Identity
                if (thisParameter == argType && otherParameter != argType)
                {
                    score++;
                    return;
                }

                if (otherParameter == argType && thisParameter != argType)
                {
                    score -= short.MaxValue;
                    return;
                }

                // Derived class to base class
                // Class to implemented interface
                var thisHasAncestor = HasAncestor(argType, thisParameter);
                var otherHasAncestor = HasAncestor(argType, otherParameter);

                if (thisHasAncestor && otherHasAncestor)
                {
                    var thisAncestorDistance = GetAncestorDistance(argType, thisParameter);
                    var otherAncestorDistance = GetAncestorDistance(argType, otherParameter);

                    if (thisAncestorDistance < otherAncestorDistance)
                    {
                        score++;
                        return;
                    }

                    if (otherAncestorDistance < thisAncestorDistance)
                    {
                        score -= short.MaxValue;
                        return;
                    }
                }
                else
                {
                    if (thisHasAncestor)
                    {
                        score++;
                        return;
                    }

                    if (otherHasAncestor)
                    {
                        score -= short.MaxValue;
                        return;
                    }
                }
            }

            private static int GetAncestorDistance(Type concreteType, Type ancestorType)
            {
                if (ancestorType.IsInterface)
                {
                    // if type's interfaces contain ancestorType, check to see if type.BaseType's interfaces contain it, and so one. Each time, add one to the distance.

                    var distance = 0;

                    while (true)
                    {
                        if (concreteType.BaseType == null)
                        {
                            break;
                        }

                        if (concreteType.BaseType.GetInterfaces().Contains(ancestorType))
                        {
                            distance++;
                        }
                        else
                        {
                            break;
                        }

                        concreteType = concreteType.BaseType;
                    }

                    var typeInterfaces = concreteType.GetInterfaces();

                    return distance + typeInterfaces.Count(
                            typeInterface =>
                                typeInterface == ancestorType
                                || typeInterface.GetInterfaces().Any(typeInterfaceInterface =>
                                    typeInterfaceInterface == ancestorType));
                }

                if (ancestorType.IsClass)
                {
                    var count = 0;

                    while (true)
                    {
                        if (concreteType == ancestorType)
                        {
                            return count;
                        }

                        count++;

                        if (concreteType == typeof(object) || concreteType == null)
                        {
                            return ushort.MaxValue;
                        }

                        concreteType = concreteType.BaseType;
                    }
                }

                return ushort.MaxValue;
            }

            private static bool HasAncestor(Type type, Type ancestorType)
            {
                if (ancestorType == typeof(object))
                {
                    return false;
                }

                if (ancestorType.IsInterface)
                {
                    return type.GetInterfaces().Any(i => i == ancestorType);
                }

                if (ancestorType.IsClass)
                {
                    while (true)
                    {
                        type = type.BaseType;

                        if (type == ancestorType)
                        {
                            return true;
                        }

                        if (type == typeof(object) || type == null)
                        {
                            return false;
                        }
                    }
                }

                return false;
            }

            public bool IsLegal(object[] args)
            {
                if (args.Length != _parameters.Length)
                {
                    return false;
                }

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] != null)
                    {
                        if (!CanBeAssigned(_parameters[i], args[i].GetType()))
                        {
                            return false;
                        }
                    }
                    else if (!CanBeAssignedNull(_parameters[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static bool CanBeAssignedNull(Type type)
        {
            return _canBeAssignedNullCache.GetOrAdd(type, t =>
                !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        private static bool CanBeAssigned(Type parameterType, Type valueType)
        {
            return _canBeAssignedCache.GetOrAdd(
                Tuple.Create(parameterType, valueType),
                tuple => GetCanBeAssignedValue(tuple.Item1, tuple.Item2, false));
        }

        private static bool GetCanBeAssignedValue(Type parameterType, Type valueType, bool skipImplicitConversionSearch)
        {
            // Identity
            if (valueType == parameterType)
            {
                return true;
            }

            // Anything to object
            if (parameterType == typeof(object))
            {
                return true;
            }

            // Nullable type conversion
            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                parameterType = parameterType.GetGenericArguments()[0];

                // Recheck identity
                if (valueType == parameterType)
                {
                    return true;
                }
            }

            // sbyte to short, int, long, float, double, or decimal
            if (valueType == typeof(sbyte))
            {
                return parameterType == typeof(short)
                       || parameterType == typeof(int)
                       || parameterType == typeof(long)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // byte to short, ushort, int, uint, long, ulong, float, double, or decimal
            if (valueType == typeof(byte))
            {
                return parameterType == typeof(short)
                       || parameterType == typeof(ushort)
                       || parameterType == typeof(int)
                       || parameterType == typeof(uint)
                       || parameterType == typeof(long)
                       || parameterType == typeof(ulong)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // short to int, long, float, double, or decimal
            if (valueType == typeof(short))
            {
                return parameterType == typeof(int)
                       || parameterType == typeof(long)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // ushort to int, uint, long, ulong, float, double, or decimal
            if (valueType == typeof(ushort))
            {
                return parameterType == typeof(int)
                       || parameterType == typeof(uint)
                       || parameterType == typeof(long)
                       || parameterType == typeof(ulong)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // int to long, float, double, or decimal
            if (valueType == typeof(int))
            {
                return parameterType == typeof(long)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // uint to long, ulong, float, double, or decimal
            if (valueType == typeof(uint))
            {
                return parameterType == typeof(long)
                       || parameterType == typeof(ulong)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // long to float, double, or decimal
            if (valueType == typeof(double))
            {
                return parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // ulong to float, double, or decimal
            if (valueType == typeof(ulong))
            {
                return parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // char to ushort, int, uint, long, ulong, float, double, or decimal
            if (valueType == typeof(char))
            {
                return parameterType == typeof(ushort)
                       || parameterType == typeof(int)
                       || parameterType == typeof(uint)
                       || parameterType == typeof(long)
                       || parameterType == typeof(ulong)
                       || parameterType == typeof(float)
                       || parameterType == typeof(double)
                       || parameterType == typeof(decimal);
            }

            // float to double
            if (valueType == typeof(float))
            {
                return parameterType == typeof(double);
            }

            // Derived class to base class
            // Array type to System.Array
            // Delegate type to System.Delegate
            // Enum type to System.Enum
            if (IsDerivedFrom(valueType, parameterType))
            {
                return true;
            }

            // Class to implemented interface
            if (parameterType.IsInterface && valueType.GetInterfaces().Any(i => i == parameterType))
            {
                return true;
            }

            // Interface to base interface (not necessary - arg is a concrete type)

            if (valueType.IsArray)
            {
                if (parameterType.IsArray)
                {
                    // Array to array when arrays have the same number of dimensions, there is an implicit
                    // conversion from the source element type to the destination element type and the source
                    // element type and the destination element type are reference types
                    if (valueType.GetArrayRank() == parameterType.GetArrayRank())
                    {
                        var parameterElementType = parameterType.GetElementType();
                        var valueElementType = valueType.GetElementType();

                        if (!parameterElementType.IsValueType
                            && !valueElementType.IsValueType
                            && CanBeAssigned(parameterElementType, valueElementType))
                        {
                            return true;
                        }
                    }
                }
                else if (parameterType.IsInterface && valueType.GetArrayRank() == 1)
                {
                    // Array type to IList<> and its base interfaces
                    if (parameterType.IsGenericType)
                    {
                        var typeDefinition = parameterType.GetGenericTypeDefinition();

                        if (typeDefinition == typeof(IList<>)
                            || typeDefinition == typeof(ICollection<>)
                            || typeDefinition == typeof(IEnumerable<>))
                        {
                            var parameterGenericArgument = parameterType.GetGenericArguments()[0];
                            var valueElementType = valueType.GetElementType();

                            if (CanBeAssigned(parameterGenericArgument, valueElementType))
                            {
                                return true;
                            }
                        }
                    }
                    else if (parameterType == typeof(IList)
                             || parameterType == typeof(ICollection)
                             || parameterType == typeof(IEnumerable))
                    {
                        return true;
                    }
                }

                return false;
            }

            // Boxing conversion (not necessary)

            if (!skipImplicitConversionSearch)
            {
                // User defined conversion (op_implicit)
                var implicitConvertionMethods = GetImplicitConvertionMethods(parameterType, valueType);

                if (implicitConvertionMethods.Count() == 1)
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<MethodInfo> GetImplicitConvertionMethods(Type parameterType, Type valueType)
        {
            var allImplicitConversionMethods =
                GetImplicitConvertionMethods(parameterType).Concat(GetImplicitConvertionMethods(valueType));

            var projection =
                allImplicitConversionMethods
                    .Select(m => new { MethodInfo = m, m.ReturnType, m.GetParameters()[0].ParameterType });

            var filter = projection
                .Where(method => GetCanBeAssignedValue(parameterType, method.ReturnType, true)
                                 && GetCanBeAssignedValue(method.ParameterType, valueType, true));

            var implicitConvertionMethods = filter.Select(method => method.MethodInfo);

            return implicitConvertionMethods;
        }

        private static IEnumerable<MethodInfo> GetImplicitConvertionMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "op_Implicit");
        }

        private static bool IsDerivedFrom(Type type, Type baseType)
        {
            while (true)
            {
                type = type.BaseType;

                if (type == baseType)
                {
                    return true;
                } 
                
                if (type == typeof(object) || type == null)
                {
                    return false;
                }
            }
        }

        private class CreateInstanceCandidate : Candidate
        {
            private readonly Func<object[], object> _createInstanceFunc;

            public CreateInstanceCandidate(IEnumerable<ParameterInfo> parameters, Func<object[], object> createInstanceFunc)
                : base(parameters)
            {
                _createInstanceFunc = createInstanceFunc;
            }

            public object CreateInstance(object[] args)
            {
                return _createInstanceFunc(args);
            }
        }

        private class InvokeMemberCandidate : Candidate
        {
            private readonly Func<object, object[], object> _invokeMemberFunc;

            public InvokeMemberCandidate(IEnumerable<ParameterInfo> parameters, Func<object, object[], object> invokeMemberFunc)
                : base(parameters)
            {
                _invokeMemberFunc = invokeMemberFunc;
            }

            public object InvokeMember(object instance, object[] args)
            {
                return _invokeMemberFunc(instance, args);
            }
        }
    }
}
