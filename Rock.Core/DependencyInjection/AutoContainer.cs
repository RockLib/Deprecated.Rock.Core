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
    /// <summary>
    /// An implementation of <see cref="IResolver"/> that registers, at constructor-time,
    /// a collection of object instances. When the <see cref="Get{T}"/> or <see cref="Get"/>
    /// methods are called, these instances are available to be passed as a constructor
    /// arguments if the instance satisfies the constructor arg's contract.
    /// </summary>
    public partial class AutoContainer : IResolver
    {
        private const Func<object> _getInstanceFuncNotFound = null;

        private static readonly MethodInfo _genericGetMethod;

        private readonly IEnumerable<object> _instances;
        private readonly ConcurrentDictionary<Type, Func<object>> _bindings;
        private readonly IResolverConstructorSelector _constructorSelector;

        static AutoContainer()
        {
            Expression<Func<AutoContainer, int>> expression = container => container.Get<int>();
            _genericGetMethod = ((MethodCallExpression)expression.Body).Method.GetGenericMethodDefinition();
        }

        private AutoContainer(
            IEnumerable<object> instances,
            ConcurrentDictionary<Type, Func<object>> bindings,
            IResolverConstructorSelector constructorSelector)
        {
            if (instances == null) { throw new ArgumentNullException("instances"); }
            if (bindings == null) { throw new ArgumentNullException("bindings"); }
            if (constructorSelector == null) { throw new ArgumentNullException("constructorSelector"); }

            _instances = instances;
            _bindings = bindings;
            _constructorSelector = constructorSelector;
        }

        /// <summary>
        /// Copy constructor. Initializes an instance of an inheritor of <see cref="AutoContainer"/>
        /// to have the same values for its private backing fields as <paramref name="parentContainer"/>.
        /// NOTE: The fields that are copied are the fields defined in in the <see cref="AutoContainer"/>
        /// class. In other words, fields defined in an inheritor of <see cref="AutoContainer"/> will not
        /// be copied by this constructor.
        /// </summary>
        protected AutoContainer(AutoContainer parentContainer)
            : this(parentContainer._instances, parentContainer._bindings, parentContainer._constructorSelector)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AutoContainer"/>, using the
        /// <paramref name="instances"/> as its registered dependencies. These
        /// depenendencies will be resolvable by this instance of
        /// <see cref="AutoContainer"/> via any type that exactly one dependency
        /// equals, implements, or inherits from. This instance of <see cref="AutoContainer"/>
        /// will use <see cref="Default.ResolverConstructorSelector"/> internally to determine
        /// which constructor of an arbitrary type will be selected for invocation when
        /// <see cref="Get{T}"/> or <see cref="Get"/> methods are called.
        /// </summary>
        /// <param name="instances">The objects to use as registered dependencies.</param>
        public AutoContainer(params object[] instances)
            : this(Default.ResolverConstructorSelector, instances)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AutoContainer"/>, using the
        /// <paramref name="instances"/> as its registered dependencies. These
        /// depenendencies will be resolvable by this instance of
        /// <see cref="AutoContainer"/> via any type that exactly one dependency
        /// equals, implements, or inherits from. This instance of <see cref="AutoContainer"/>
        /// will use the <see cref="IResolverConstructorSelector"/> specified by
        /// <paramref name="constructorSelector"/> internally to determine
        /// which constructor of an arbitrary type will be selected for invocation when
        /// <see cref="Get{T}"/> or <see cref="Get"/> methods are called.
        /// </summary>
        /// <param name="constructorSelector">The </param>
        /// <param name="instances"></param>
        public AutoContainer(IResolverConstructorSelector constructorSelector, IEnumerable<object> instances)
            : this(
                (instances == null // I don't know why someone specifically provided null for instances...
                    ? Enumerable.Empty<object>() // ...but whatever, we'll just use an empty list.
                    : instances.Where(x => x != null)) // Otherwise, be sure to ignore any null values.
                .ToList(), // Fully evaluate the instances collection to ensure fast enumeration.
                new ConcurrentDictionary<Type, Func<object>>(),
                constructorSelector)
        {
        }

        /// <summary>
        /// Returns whether this instance of <see cref="AutoContainer"/> can get an instance of the specified
        /// type.
        /// </summary>
        /// <param name="type">The type to determine whether this instance is able to get an instance of.</param>
        /// <returns>True, if this instance can get an instance of the specified type. False, otherwise.</returns>
        public virtual bool CanGet(Type type)
        {
            return GetGetInstanceFunc(type) != _getInstanceFuncNotFound;
        }

        /// <summary>
        /// Gets an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <returns>An instance of type <typeparamref name="T"/>.</returns>
        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Gets an instance of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object to return.</param>
        /// <returns>An instance of type <paramref name="type"/></returns>
        public virtual object Get(Type type)
        {
            var getInstanceFunc = GetGetInstanceFunc(type);

            if (getInstanceFunc == _getInstanceFuncNotFound)
            {
                throw new ResolveException("Cannot resolve type: " + type);
            }

            return getInstanceFunc();
        }

        IResolver IResolver.MergeWith(IResolver otherContainer)
        {
            return MergeWith(otherContainer);
        }

        /// <summary>
        /// Returns a new instance of <see cref="AutoContainer"/> that is the result of a merge operation between
        /// this instance of <see cref="AutoContainer"/> and <paramref name="secondaryResolver"/>.
        /// </summary>
        /// <param name="secondaryResolver">A secondary <see cref="IResolver"/>.</param>
        /// <returns>An instance of <see cref="AutoContainer"/> resulting from the merge operation.</returns>
        public virtual AutoContainer MergeWith(IResolver secondaryResolver)
        {
            return new MergedAutoContainer(this, secondaryResolver);
        }

        /// <summary>
        /// Returns <see cref="_getInstanceFuncNotFound"/> if the type is unresolvable.
        /// </summary>
        private Func<object> GetGetInstanceFunc(Type type)
        {
            return
                _bindings.GetOrAdd(
                    type,
                    t =>
                    {
                        if (t == typeof(object))
                        {
                            return null;
                        }

                        object instance;
                        if (TryGetInstance(t, out instance))
                        {
                            return () => instance;
                        }

                        ConstructorInfo constructor;
                        if (_constructorSelector.TryGetConstructor(t, this, out constructor))
                        {
                            return GetCreateInstanceFunc(constructor);
                        }

                        return _getInstanceFuncNotFound;
                    });
        }

        private bool TryGetInstance(Type type, out object instance)
        {
            if (_instances.Count(type.IsInstanceOfType) == 1)
            {
                instance = _instances.First(type.IsInstanceOfType);
                return true;
            }

            instance = null;
            return false;
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
                                        _genericGetMethod.MakeGenericMethod(p.ParameterType))
                                    : Expression.Constant(p.DefaultValue, p.ParameterType))));

            return createInstanceExpression.Compile();
        }
    }
}