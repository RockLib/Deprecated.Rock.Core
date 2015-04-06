using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Rock.Reflection
{
    /// <summary>
    /// Provides extension methods for the <see cref=" PropertyInfo"/> type, creating
    /// optimized functions that retrieve the value of a <see cref="PropertyInfo"/>.
    /// </summary>
    public static class GetGetFuncExtension
    {
        /// <summary>
        /// Gets a Func&gt;object, object&lt; that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> that represents the property whose value 
        /// is returned by the return function.
        /// </param>
        /// <returns>
        /// A Func&gt;object, object&lt; that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </returns>
        public static Func<object, object> GetGetFunc(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetFunc<object, object>();
        }

        /// <summary>
        /// Gets a <see cref="Func{T, TResult}"/> that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> that represents the property whose value 
        /// is returned by the return function.
        /// </param>
        /// <typeparam name="TInstance">
        /// The type of the input parameter of the resulting <see cref="Func{T, TResult}"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The return type of the resulting <see cref="Func{T, TResult}"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Func{T, TResult}"/> that, when invoked, returns the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </returns>
        public static Func<TInstance, TProperty> GetGetFunc<TInstance, TProperty>(this PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(TInstance), "instance");

            var body =
                Expression.Property(
                    instanceParameter.EnsureConvertableTo(propertyInfo.ReflectedType),
                    propertyInfo);

            var lambda =
                Expression.Lambda<Func<TInstance, TProperty>>(
                    body.EnsureConvertableTo<TProperty>(),
                    "Get" + propertyInfo.Name,
                    new[] { instanceParameter });

            return lambda.Compile();
        }
    }
}
