using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Rock.Defaults.Implementation;
using Rock.DependencyInjection.Heuristics;

namespace Rock.DependencyInjection
{
    public partial class AutoContainer : IResolver
    {
        private readonly Lazy<MethodInfo> _getMethod;
        private readonly ConcurrentDictionary<Type, Func<object>> _bindings;
        private readonly IConstructorSelector _constructorSelector;

        private AutoContainer(
            ConcurrentDictionary<Type, Func<object>> bindings,
            IConstructorSelector constructorSelector)
        {
            _getMethod = new Lazy<MethodInfo>(() => GetType().GetMethod("Get", Type.EmptyTypes));
            _constructorSelector = constructorSelector;
            _bindings = bindings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        protected AutoContainer(AutoContainer parentContainer)
            : this(parentContainer._bindings, parentContainer._constructorSelector)
        {
        }

        public AutoContainer(params object[] instances)
            : this(Default.ConstructorSelector, instances)
        {
        }

        public AutoContainer(IConstructorSelector constructorSelector, IEnumerable<object> instances)
            : this(new ConcurrentDictionary<Type, Func<object>>(), constructorSelector)
        {
            foreach (var instance in instances.Where(x => x != null))
            {
                var localInstance = instance;
                foreach (var type in GetTypeHierarchy(instance.GetType()))
                {
                    if (_bindings.ContainsKey(type))
                    {
                        _bindings[type] = null;
                    }
                    else
                    {
                        _bindings[type] = () => localInstance;
                    }
                }
            }
        }

        public virtual bool CanGet(Type type)
        {
            Func<object> binding;
            if (_bindings.TryGetValue(type, out binding))
            {
                return binding != null;
            }

            return CanGetConstructor(type);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public virtual object Get(Type type)
        {
            var getInstance =
                _bindings.GetOrAdd(
                    type,
                    t =>
                    {
                        ConstructorInfo ctor;
                        return
                            _constructorSelector.TryGetConstructor(type, this, out ctor)
                                ? GetCreateInstanceFunc(ctor)
                                : null;
                    });

            if (getInstance == null)
            {
                throw new ResolveException("Cannot resolve type: " + type);
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

        private bool CanGetConstructor(Type type)
        {
            return
                type != typeof(object)
                && _constructorSelector.CanGetConstructor(type, this);
        }

        private Func<object> GetCreateInstanceFunc(ConstructorInfo ctor)
        {
            // We're going to be creating a lambda expression of type Func<object>.
            // Then we'lle compile that expression and return the resulting Func<object>.

            // We want a closure around 'this' so that we can access its Get<> method.
            var thisExpression = Expression.Constant(this);

            // The following is roughly what's going on in the lambda expression below.
            // Note that the select statement will be fully realized at expression creation time.
            // () =>
            //     new instance_type(
            //         for each of the constructor's parameters
            //             if the parameter's type can be resolved
            //                 use this.Get<parameter_type>()
            //             else
            //                 use the parameter's default value)

            var createInstanceExpression =
                Expression.Lambda<Func<object>>(
                    Expression.New(ctor,
                        ctor.GetParameters()
                            .Select(p =>
                                CanGet(p.ParameterType)
                                    ? (Expression)Expression.Call(
                                        thisExpression,
                                        _getMethod.Value.MakeGenericMethod(p.ParameterType))
                                    : Expression.Constant(p.DefaultValue, p.ParameterType))));

            return createInstanceExpression.Compile();
        }

        private static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            return GetConcreteHierarchy(type).Concat(type.GetInterfaces());
        }

        private static IEnumerable<Type> GetConcreteHierarchy(Type type)
        {
            while (type != null && type != typeof(object))
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}