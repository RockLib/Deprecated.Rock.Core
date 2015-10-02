using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private static readonly ConcurrentDictionary<Tuple<Type, string>, Func<object, Object>> _getMemberFuncs = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string>, Action<object, object>> _setMemberActions = new ConcurrentDictionary<Tuple<Type, string>, Action<object, object>>();
        private static readonly ConcurrentDictionary<Tuple<Type, string, int>, Func<object, object[], object>> _invokeMemberFuncs = new ConcurrentDictionary<Tuple<Type, string, int>, Func<object, object[], object>>();

        private readonly Lazy<IEnumerable<string>> _memberNames;

        private readonly object _instance;
        private readonly Type _type;

        private UniversalMemberAccessor(Type type)
        {
            _type = type;

            _memberNames = new Lazy<IEnumerable<string>>(() =>
                _type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(m => !(m is ConstructorInfo) && !(m is EventInfo) && !(m is Type))
                    .Select(m => m.Name)
                    .ToList()
                    .AsReadOnly());
        }

        private UniversalMemberAccessor(object instance)
        {
            _instance = instance;
            _type = _instance.GetType();

            _memberNames = new Lazy<IEnumerable<string>>(() =>
                _type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => !(m is ConstructorInfo) && !(m is EventInfo) && !(m is Type))
                    .Select(m => m.Name)
                    .ToList()
                    .AsReadOnly());
        }

        /// <summary>
        /// Gets a dynamic proxy object (specifically, an instance of <see cref="UniversalMemberAccessor"/>)
        /// for the given object.
        /// </summary>
        /// <remarks>
        /// If this method is called more than once with the same object, then the value returned
        /// is the same instance of <see cref="UniversalMemberAccessor"/> each time.
        /// </remarks>
        /// <param name="instance">An object.</param>
        /// <returns>A dynamic proxy object enabling access to all members of the given instance.</returns>
        /// <remarks>This is a very dangerous method - use with caution.</remarks>
        public static dynamic Get(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            return _instanceMap.GetValue(instance, o => new UniversalMemberAccessor(o));
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
            if (binder.Name == "LockNonPublicMembers" && binder.CallInfo.ArgumentCount == 0)
            {
                result = _instance;
                return true;
            }

            var invokeMember =
                _invokeMemberFuncs.GetOrAdd(
                    Tuple.Create(_type, binder.Name, binder.CallInfo.ArgumentCount),
                    t => CreateInvokeMemberFunc(t.Item2, t.Item3));

            if (invokeMember == null)
            {
                return base.TryInvokeMember(binder, args, out result);
            }

            result = invokeMember(_instance, args);
            return true;
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
            if (!_type.IsAssignableFrom(binder.ReturnType))
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
            return _memberNames.Value;
        }

        private Func<object, object> CreateGetMemberFunc(string name)
        {
            try
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

                return obj =>
                {
                    var value = func(obj);

                    return
                        value == null
                            ? (object)null
                            : value.UnlockNonPublicMembers(); // Unlock the return value.
                };
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
                    // Unwrap the value if it is a universalMemberAccessor.
                    var universalMemberAccessor = value as UniversalMemberAccessor;
                    if (universalMemberAccessor != null)
                    {
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

        private Func<object, object[], object> CreateInvokeMemberFunc(string name, int argCount)
        {
            try
            {
                var methodInfo =
                    _type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Single(m => m.Name == name && m.GetParameters().Length == argCount);

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

                Expression body =
                    Expression.Call(
                        Expression.Convert(instanceParameter, _type),
                        methodInfo,
                        callArguments);

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
                    return (obj, args) => func(obj, UnwrapArgs(args));
                }

                return (obj, args) =>
                {
                    var value = func(obj, UnwrapArgs(args));

                    return
                        value == null
                            ? (object)null
                            : value.UnlockNonPublicMembers(); // Unlock the return value.
                };
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
    }
}
