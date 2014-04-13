using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rock.DependencyInjection
{
    public partial class AutoContainer : IResolver
    {
        private readonly Lazy<MethodInfo> _getMethod;
        private readonly ConcurrentDictionary<Type, Func<object>> _bindings;

        private AutoContainer(ConcurrentDictionary<Type, Func<object>> bindings)
        {
            _getMethod = new Lazy<MethodInfo>(() => GetType().GetMethod("Get", Type.EmptyTypes));
            _bindings = bindings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        protected AutoContainer(AutoContainer parentContainer)
            : this(parentContainer._bindings)
        {
        }

        public AutoContainer(params object[] instances)
            : this(new ConcurrentDictionary<Type, Func<object>>())
        {
            foreach (var instance in instances.Where(x => x != null))
            {
                BindConstant(instance.GetType(), instance);
            }
        }

        public virtual bool CanResolve(Type type)
        {
            return _bindings.ContainsKey(type) || CanCreateInstance(type);
        }

        public T Get<T>()
        {
            var instance = Get(typeof(T));

            if (instance == null)
            {
                return default(T);
            }

            return (T)instance;
        }

        public virtual object Get(Type type)
        {
            Func<object> getInstance;

            if (!_bindings.TryGetValue(type, out getInstance))
            {
                getInstance =
                    CanCreateInstance(type)
                    ? GetCreateInstanceFunc(type)
                    : null;

                _bindings[type] = getInstance;
            }

            if (getInstance == null)
            {
                return null;
            }

            return getInstance();
        }

        IResolver IResolver.MergeWith(IResolver otherContainer)
        {
            return MergeWith(otherContainer);
        }

        public virtual AutoContainer MergeWith(IResolver otherContainer)
        {
            return new MergedAutoContainer(this, otherContainer);
        }

        private Func<object> GetCreateInstanceFunc(Type type)
        {
            var ctor =
                type.GetConstructors()
                    .Where(AllParametersAreResolvable)
                    .GroupBy(c => c.GetParameters().Length)
                        .OrderByDescending(g => g.Key)
                        .Where(g => g.Count() == 1) // This is debatable. What happens if there is more than one valid constructor with the same number of parameters? Here, we're ignoring all constructors of the same length, hoping that another constructor with fewer parameters will successfully resolve.
                        .Select(g => g.Single())
                    .FirstOrDefault();

            if (ctor == null)
            {
                return null;
            }

            var thisExpression = Expression.Constant(this);

            var createInstanceExpression =
                Expression.Lambda<Func<object>>(
                    Expression.New(
                        ctor,
                        ctor.GetParameters()
                            .Select(
                                p =>
                                    CanResolve(p.ParameterType)
                                        ? (Expression)Expression.Call(
                                            thisExpression,
                                            _getMethod.Value.MakeGenericMethod(p.ParameterType))
                                        : Expression.Constant(p.DefaultValue, p.ParameterType))));

            return createInstanceExpression.Compile();
        }

        public AutoContainer BindInstance<TContract, TImplementation>()
        {
            return BindInstance(typeof(TContract), typeof(TImplementation));
        }

        public AutoContainer BindInstance<TContract>(Func<TContract> createInstance)
        {
            return BindInstance(typeof(TContract), () => createInstance());
        }

        public AutoContainer BindInstance(Type contractType, Type implementationType)
        {
            if (!contractType.IsAssignableFrom(implementationType))
            {
                return this;
            }

            if (!CanCreateInstance(implementationType))
            {
                return this;
            }

            var createInstance = GetCreateInstanceFunc(implementationType);
            return BindInstance(contractType, createInstance);
        }

        public AutoContainer BindInstance(Type contractType, Func<object> createInstance)
        {
            foreach (var type in GetTypeHierarchy(contractType))
            {
                AddInstanceMapping(type, createInstance);
            }

            return this;
        }

        private void AddInstanceMapping(Type contractType, Func<object> createInstance)
        {
            _bindings[contractType] = createInstance;
        }

        public AutoContainer BindSingleton<TContract, TImplementation>()
        {
            return BindSingleton(typeof(TContract), typeof(TImplementation));
        }

        public AutoContainer BindSingleton<TContract>(Func<TContract> createInstance)
        {
            return BindSingleton(typeof(TContract), () => createInstance());
        }

        public AutoContainer BindSingleton(Type contractType, Type implementationType)
        {
            if (!contractType.IsAssignableFrom(implementationType))
            {
                return this;
            }

            if (!CanCreateInstance(implementationType))
            {
                return this;
            }

            var createInstance = GetCreateInstanceFunc(implementationType);
            return BindSingleton(contractType, createInstance);
        }

        public AutoContainer BindSingleton(Type contractType, Func<object> createInstance)
        {
            var lazyInstance = new Lazy<object>(createInstance);

            foreach (var type in GetTypeHierarchy(contractType))
            {
                AddSingletonMapping(type, lazyInstance);
            }

            return this;
        }

        private void AddSingletonMapping(Type contractType, Lazy<object> lazyInstance)
        {
            _bindings[contractType] = () => lazyInstance.Value;
        }

        public AutoContainer BindConstant<TContract>(TContract instance)
        {
            return BindConstant(typeof(TContract), instance);
        }

        public AutoContainer BindConstant(Type contractType, object instance)
        {
            if (instance == null)
            {
                return this;
            }

            foreach (var type in GetTypeHierarchy(contractType))
            {
                AddConstantMapping(type, instance);
            }

            return this;
        }

        private bool CanCreateInstance(Type type)
        {
            return
                !type.IsAbstract
                && type.GetConstructors().Any(AllParametersAreResolvable);
        }

        private bool AllParametersAreResolvable(ConstructorInfo constructor)
        {
            return constructor.GetParameters().All(p => CanResolve(p.ParameterType) || p.HasDefaultValue);
        }

        private void AddConstantMapping(Type type, object instance)
        {
            _bindings[type] = () => instance;
        }

        private static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            var types = Enumerable.Repeat(type, 1);

            if (type == typeof(object))
            {
                return types;
            }

            if (!type.IsInterface)
            {
                types = types.Concat(GetTypeHierarchy(type.BaseType));
            }

            types = type.GetInterfaces().Aggregate(types, (typesSoFar, @interface) => typesSoFar.Concat(GetTypeHierarchy(@interface)));

            return types.Distinct();
        }
    }
}