using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

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

        private static readonly ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> _getMemberFuncs = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string, Type>, Action<object, object>> _setMemberActions = new ConcurrentDictionary<Tuple<Type, string, Type>, Action<object, object>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string>, ReadOnlyCollection<InvokeMemberCandidate>> _invokeMemberFuncs = new ConcurrentDictionary<Tuple<Type, string>, ReadOnlyCollection<InvokeMemberCandidate>>();

        private readonly Lazy<ReadOnlyCollection<CreateInstanceCandidate>> _createInstanceCandidates;

        private readonly object _instance;
        private readonly Type _type;
        private readonly IEnumerable<string> _memberNames;

        private UniversalMemberAccessor(Type type)
        {
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
        public static dynamic GetStatic(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

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
        public static dynamic GetStatic<T>()
        {
            return GetStatic(typeof(T));
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
        public static dynamic GetStatic(string assemblyQualifiedName)
        {
            return GetStatic(Type.GetType(assemblyQualifiedName, true));
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
            Func<object, object> getMember;

            if (!TryGetGetMemberFunc(binder.Name, out getMember))
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

        private bool TryGetGetMemberFunc(string name, out Func<object, object> getMember)
        {
            getMember =
                _getMemberFuncs.GetOrAdd(
                    Tuple.Create(_type, name),
                    t => CreateGetMemberFunc(t.Item2));

            return getMember != null;
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
            Action<object, object> setMember;
            var valueType = GetValueType(value);

            if (!TryGetSetMemberAction(binder.Name, valueType,  out setMember))
            {
                return base.TrySetMember(binder, value);
            }

            setMember(_instance, value);
            return true;
        }

        private static Type GetValueType(object value)
        {
            return 
                value == null
                    ? null
                    : value is UniversalMemberAccessor
                        ? ((UniversalMemberAccessor)value)._instance.GetType()
                        : value.GetType();
        }

        private bool TryGetSetMemberAction(string name, Type valueType, out Action<object, object> setMember)
        {
            setMember = _setMemberActions.GetOrAdd(
                Tuple.Create(_type, name, valueType),
                t => CreateSetMemberAction(t.Item2, t.Item3));

            return setMember != null;
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

        internal bool TryCreateInstance(object[] args, out object result)
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
                Func<object, object> getMember;

                if (TryGetGetMemberFunc((string)indexes[0], out getMember))
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
                Action<object, object> setMember;
                var valueType = GetValueType(value);

                if (TryGetSetMemberAction((string)indexes[0], valueType, out setMember))
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
            var parameter = Expression.Parameter(typeof(object), "instance");
            var convertParameter = Expression.Convert(parameter, _type);

            Expression propertyOrField;

            var propertyInfo = _type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (propertyInfo != null)
            {
                propertyOrField = Expression.Property(IsStatic(propertyInfo) ? null : convertParameter, propertyInfo);
            }
            else
            {
                var fieldInfo = _type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (fieldInfo != null)
                {
                    propertyOrField = Expression.Field(fieldInfo.IsStatic ? null : convertParameter, fieldInfo);
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

            return obj => Get(func(obj));
        }

        private Action<object, object> CreateSetMemberAction(string name, Type valueType)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var valueParameter = Expression.Parameter(typeof(object), "value");

            var convertInstanceParameter = Expression.Convert(instanceParameter, _type);

            Action<object, object> action = null;

            Expression propertyOrField;

            var propertyInfo = _type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (propertyInfo != null)
            {
                if (valueType != null)
                {
                    if (!CanBeAssigned(propertyInfo.PropertyType, valueType))
                    {
                        return null;
                    }
                }
                else if (!CanBeAssignedNull(propertyInfo.PropertyType))
                {
                    return null;
                }

                propertyOrField = Expression.Property(IsStatic(propertyInfo) ? null : convertInstanceParameter, propertyInfo);
            }
            else
            {
                var fieldInfo = _type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (fieldInfo != null)
                {
                    if (valueType != null)
                    {
                        if (!CanBeAssigned(fieldInfo.FieldType, valueType))
                        {
                            return null;
                        }
                    }
                    else if (!CanBeAssignedNull(fieldInfo.FieldType))
                    {
                        return null;
                    }

                    if (fieldInfo.IsInitOnly)
                    {
                        var dynamicMethod = new DynamicMethod("", typeof(void),
                            new[] { typeof(object), typeof(object) }, fieldInfo.DeclaringType);

                        var il = dynamicMethod.GetILGenerator();

                        if (!fieldInfo.IsStatic)
                        {
                            // Load the first arg (should not do for static field)
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);
                        }

                        // Load the second arg
                        il.Emit(OpCodes.Ldarg_1);

                        if (fieldInfo.FieldType.IsValueType)
                        {
                            // Unbox value types.
                            if (valueType == fieldInfo.FieldType || valueType == null)
                            {
                                // Unbox as the field type when value type matches or is null.
                                il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                            }
                            else
                            {
                                // If value type is different than field type, unbox as the
                                // value type. Otherwise we get an invalid cast exception.
                                il.Emit(OpCodes.Unbox_Any, valueType);
                            }
                        }
                        else
                        {
                            // Cast reference types.
                            il.Emit(OpCodes.Castclass, fieldInfo.FieldType);
                        }

                        // Different op code for static vs. instance.
                        il.Emit(fieldInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, fieldInfo);
                        il.Emit(OpCodes.Ret);

                        action = (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
                        propertyOrField = null;
                    }
                    else
                    {
                        propertyOrField = Expression.Field(fieldInfo.IsStatic ? null : convertInstanceParameter, fieldInfo);
                    }
                }
                else
                {
                    return null;
                }
            }

            if (action == null)
            {
                Expression newValue;

                if (propertyOrField.Type.IsValueType)
                {
                    if (valueType == propertyOrField.Type || valueType == null)
                    {
                        newValue = Expression.Unbox(valueParameter, propertyOrField.Type);
                    }
                    else
                    {
                        newValue = Expression.Convert(
                            Expression.Unbox(valueParameter, valueType),
                            propertyOrField.Type);
                    }
                }
                else
                {
                    newValue = Expression.Convert(valueParameter, propertyOrField.Type);
                }

                var lambda =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Assign(propertyOrField, newValue),
                        new[] { instanceParameter, valueParameter });

                action = lambda.Compile();
            }

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

        private Lazy<ReadOnlyCollection<CreateInstanceCandidate>> GetLazyCreateInstanceCandidates()
        {
            return new Lazy<ReadOnlyCollection<CreateInstanceCandidate>>(() =>
                new ReadOnlyCollection<CreateInstanceCandidate>(
                    GetCreateInstanceCandidates().ToList()));
        }

        private IEnumerable<CreateInstanceCandidate> GetCreateInstanceCandidates()
        {
            if (_type.IsValueType)
            {
                if (ShouldReturnRawValue(_type))
                {
                    yield return new CreateInstanceCandidate(Enumerable.Empty<ParameterInfo>(), args => FormatterServices.GetUninitializedObject(_type));
                }
                else
                {
                    yield return new CreateInstanceCandidate(Enumerable.Empty<ParameterInfo>(), args => Get(FormatterServices.GetUninitializedObject(_type)));
                }
            }

            var constructorInfos = _type.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var constructor in constructorInfos)
            {
                var argsParameter = Expression.Parameter(typeof(object[]), "args");

                var constructorParameters = constructor.GetParameters();
                var newArguments = GetArguments(constructorParameters, argsParameter);

                Expression body = Expression.New(constructor, newArguments);

                if (constructor.DeclaringType.IsValueType)
                {
                    body = Expression.Convert(body, typeof(object));
                }

                var lambda = Expression.Lambda<Func<object[], object>>(body, new[] { argsParameter });

                var func = lambda.Compile();

                if (ShouldReturnRawValue(constructor.DeclaringType))
                {
                    yield return new CreateInstanceCandidate(constructorParameters, args => func(UnwrapArgs(args)));
                }
                else
                {
                    yield return new CreateInstanceCandidate(constructorParameters, args => Get(func(UnwrapArgs(args))));
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
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var argsParameter = Expression.Parameter(typeof(object[]), "args");

            var methodInfoParameters = methodInfo.GetParameters();
            var callArguments = GetArguments(methodInfoParameters, argsParameter);

            var localVariables = new List<ParameterExpression>();
            var assignToVariables = new List<Expression>();
            var assignToArgs = new List<Expression>();

            for (int i = 0; i < methodInfoParameters.Length; i++)
            {
                if (methodInfoParameters[i].ParameterType.IsByRef)
                {
                    // need a local variable
                    var localVariable = Expression.Variable(callArguments[i].Type, "local" + i);

                    var convertItem = callArguments[i];
                    var item = Expression.ArrayAccess(argsParameter, Expression.Constant(i));

                    localVariables.Add(localVariable);
                    callArguments[i] = localVariable;

                    if (!methodInfoParameters[i].IsOut)
                    {
                        // need to assign to local variable
                        assignToVariables.Add(Expression.Assign(localVariable, convertItem));
                    }

                    assignToArgs.Add(Expression.Assign(item, Expression.Convert(localVariable, typeof(object))));
                }
            }

            Expression call;

            if (methodInfo.IsStatic)
            {
                call = Expression.Call(
                    methodInfo,
                    callArguments);
            }
            else
            {
                call = Expression.Call(
                    Expression.Convert(instanceParameter, _type),
                    methodInfo,
                    callArguments);
            }

            Expression body;

            if (localVariables.Count == 0)
            {
                body = call;

                if (methodInfo.ReturnType == typeof(void))
                {
                    body = Expression.Block(body, Expression.Constant(null));
                }
                else if (methodInfo.ReturnType.IsValueType)
                {
                    body = Expression.Convert(body, typeof(object));
                }
            }
            else
            {
                var blockExpressions = assignToVariables.ToList();
                
                ParameterExpression returnValue = null;

                if (methodInfo.ReturnType != typeof(void))
                {
                    returnValue = Expression.Variable(methodInfo.ReturnType, "returnValue");
                    localVariables.Add(returnValue);
                    call = Expression.Assign(returnValue, call);
                }

                blockExpressions.Add(call);
                blockExpressions.AddRange(assignToArgs);

                if (returnValue != null)
                {
                    blockExpressions.Add(
                        methodInfo.ReturnType.IsValueType
                            ? (Expression)Expression.Convert(returnValue, typeof(object))
                            : returnValue);
                }

                body = Expression.Block(localVariables, blockExpressions);
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

            return new InvokeMemberCandidate(methodInfoParameters, (obj, args) => Get(func(obj, UnwrapArgs(args))));
        }

        private static Expression[] GetArguments(ParameterInfo[] parameters, ParameterExpression argsParameter)
        {
            var arguments = new Expression[parameters.Length];

            for (var i = 0; i < arguments.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;

                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }

                if (parameterType.IsValueType)
                {
                    var item = Expression.ArrayAccess(argsParameter, Expression.Constant(i));

                    arguments[i] = Expression.Condition(
                        Expression.TypeIs(item, parameterType),
                        Expression.Unbox(item, parameterType),
                        Expression.Convert(
                            Expression.Call(
                                typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) }),
                                item,
                                Expression.Constant(parameterType)),
                            parameterType));
                }
                else
                {
                    arguments[i] =
                        Expression.Convert(
                            Expression.ArrayAccess(argsParameter, Expression.Constant(i)),
                            parameterType);
                }
            }

            return arguments;
        }

        private static bool ShouldReturnRawValue(Type type)
        {
            return
                type == typeof(string)
                || typeof(Delegate).IsAssignableFrom(type)
                || IsValue(type)
                || (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && IsValue(type.GetGenericArguments()[0]));
        }

        private static bool IsValue(Type type)
        {
            return
                type.IsPrimitive
                || type.IsEnum
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
            if (getMethod != null && getMethod.IsStatic)
            {
                return true;
            }

            var setMethod = propertyInfo.GetSetMethod(true);
            return setMethod != null && setMethod.IsStatic;
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

                if (thisParameter == otherParameter)
                {
                    return;
                }

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

                // Object is always last
                if (otherParameter == typeof(object))
                {
                    score++;
                    return;
                }

                if (thisParameter == typeof(object))
                {
                    score -= short.MaxValue;
                    return;
                }

                if (IsNumeric(argType))
                {
                    var thisAncestorDistance = GetAncestorDistance(argType, thisParameter);
                    var otherAncestorDistance = GetAncestorDistance(argType, otherParameter);

                    if (thisAncestorDistance < otherAncestorDistance)
                    {
                        score++;
                    }
                    else if (otherAncestorDistance < thisAncestorDistance)
                    {
                        score -= short.MaxValue;
                    }

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
                    }
                    else if (otherAncestorDistance < thisAncestorDistance)
                    {
                        score -= short.MaxValue;
                    }
                }
                else
                {
                    if (thisHasAncestor)
                    {
                        score++;
                    }
                    else if (otherHasAncestor)
                    {
                        score -= short.MaxValue;
                    }
                }
            }

            private static bool IsNumeric(Type type)
            {
                return type == typeof(sbyte)
                       || type == typeof(byte)
                       || type == typeof(short)
                       || type == typeof(ushort)
                       || type == typeof(int)
                       || type == typeof(uint)
                       || type == typeof(long)
                       || type == typeof(ulong)
                       || type == typeof(float)
                       || type == typeof(double)
                       || type == typeof(decimal);
            }

            private static readonly Type[] _signedComparisons =
            {
                typeof(sbyte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(decimal)
            };

            private static readonly Type[] _unsignedComparisons =
            {
                typeof(byte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal)
            };

            private static int GetAncestorDistance(Type concreteType, Type ancestorType)
            {
                if ((IsNumeric(concreteType) || concreteType == typeof(char))
                    && IsNumeric(ancestorType))
                {
                    Type[] comparisons;

                    if (concreteType == typeof(sbyte)
                        || concreteType == typeof(short)
                        || concreteType == typeof(int)
                        || concreteType == typeof(long))
                    {
                        comparisons = _signedComparisons;
                    }
                    else
                    {
                        comparisons = _unsignedComparisons;
                    }

                    if (concreteType == typeof(char))
                    {
                        concreteType = typeof(short);
                    }

                    var concreteIndex = -1;
                    var ancestorIndex = -1;

                    for (var i = 0; i < comparisons.Length; i++)
                    {
                        if (comparisons[i] == concreteType)
                        {
                            concreteIndex = i;
                        }

                        if (comparisons[i] == ancestorType)
                        {
                            ancestorIndex = i;
                        }
                    }

                    System.Diagnostics.Debug.Assert(concreteIndex != -1);
                    System.Diagnostics.Debug.Assert(ancestorIndex != -1);
                    System.Diagnostics.Debug.Assert(concreteIndex <= ancestorIndex);

                    return ancestorIndex - concreteIndex;

                    // Signed Value
                    //
                    // short
                    // int
                    // long
                    // float
                    // double
                    // decimal*

                    // Unsigned Value
                    //
                    // short
                    // ushort
                    // int
                    // uint
                    // long
                    // ulong
                    // float
                    // double
                    // decimal*

                    // *float and double do not convert to decimal, nor does decimal convert to
                    //  them. If 1) the given value is an integer type, 2) there is no method
                    //  with a legal integer type parameter, 3) there is a method with a decimal
                    //  parameter, and 4) there is another method with either a double or a float
                    //  parameter (or two methods with both types); then there is an ambiguous
                    //  match (CS0121).
                }

                if (ancestorType.IsInterface)
                {
                    // if type's interfaces contain ancestorType, check to see if type.BaseType's
                    // interfaces contain it, and so on. Each time, add one to the distance.

                    if (!concreteType.GetInterfaces().Contains(ancestorType))
                    {
                        return ushort.MaxValue;
                    }

                    var distance = 0;

                    while (true)
                    {
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
                    var arg = args[i];

                    if (arg != null)
                    {
                        if (arg is UniversalMemberAccessor)
                        {
                            arg = ((UniversalMemberAccessor)arg)._instance;
                        }

                        if (!CanBeAssigned(_parameters[i], arg.GetType()))
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

        private static bool CanBeAssigned(Type targetType, Type valueType)
        {
            return _canBeAssignedCache.GetOrAdd(
                Tuple.Create(targetType, valueType),
                tuple => GetCanBeAssignedValue(tuple.Item1, tuple.Item2));
        }

        private static bool GetCanBeAssignedValue(Type targetType, Type valueType)
        {
            // Nullable target type needs to be unwrapped.
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            if (targetType.IsByRef)
            {
                targetType = targetType.GetElementType();
            }

            if (targetType.IsAssignableFrom(valueType))
            {
                return true;
            }

            // sbyte to short, int, long, float, double, or decimal
            if (valueType == typeof(sbyte))
            {
                return targetType == typeof(short)
                       || targetType == typeof(int)
                       || targetType == typeof(long)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // byte to short, ushort, int, uint, long, ulong, float, double, or decimal
            if (valueType == typeof(byte))
            {
                return targetType == typeof(short)
                       || targetType == typeof(ushort)
                       || targetType == typeof(int)
                       || targetType == typeof(uint)
                       || targetType == typeof(long)
                       || targetType == typeof(ulong)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // short to int, long, float, double, or decimal
            if (valueType == typeof(short))
            {
                return targetType == typeof(int)
                       || targetType == typeof(long)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // ushort to int, uint, long, ulong, float, double, or decimal
            if (valueType == typeof(ushort))
            {
                return targetType == typeof(int)
                       || targetType == typeof(uint)
                       || targetType == typeof(long)
                       || targetType == typeof(ulong)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // int to long, float, double, or decimal
            if (valueType == typeof(int))
            {
                return targetType == typeof(long)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // uint to long, ulong, float, double, or decimal
            if (valueType == typeof(uint))
            {
                return targetType == typeof(long)
                       || targetType == typeof(ulong)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // long to float, double, or decimal
            if (valueType == typeof(long))
            {
                return targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // ulong to float, double, or decimal
            if (valueType == typeof(ulong))
            {
                return targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // char to ushort, int, uint, long, ulong, float, double, or decimal
            if (valueType == typeof(char))
            {
                return targetType == typeof(ushort)
                       || targetType == typeof(int)
                       || targetType == typeof(uint)
                       || targetType == typeof(long)
                       || targetType == typeof(ulong)
                       || targetType == typeof(float)
                       || targetType == typeof(double)
                       || targetType == typeof(decimal);
            }

            // float to double
            if (valueType == typeof(float))
            {
                return targetType == typeof(double);
            }

            return false;
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
