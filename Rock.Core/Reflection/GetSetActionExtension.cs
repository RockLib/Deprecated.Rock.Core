using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Rock.Reflection
{
    /// <summary>
    /// Provides extension methods on a PropertyInfo to get optimized reflection methods.
    /// </summary>
    public static class GetSetActionExtension
    {
        /// <summary>
        /// Gets an <see cref="Action{T1,T2}"/> with generic arguments of type
        /// <see cref="object"/> and <see cref="object"/> that, when invoked, sets the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> that represents the property whose value 
        /// is set by the return function.
        /// </param>
        /// <returns>
        /// An <see cref="Action{T1,T2}"/> with generic arguments of types <see cref="object"/>
        /// and <see cref="object"/> that, when invoked, returns the value of the
        /// property represented by <paramref name="propertyInfo"/>.
        /// </returns>
        public static Action<object, object> GetSetAction(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetAction<object, object>();
        }

        /// <summary>
        /// Gets an <see cref="Action{TInstance, TProperty}"/> that, when invoked, sets the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> that represents the property whose value 
        /// is set when the return action is invoked.
        /// </param>
        /// <typeparam name="TInstance">
        /// The type of the instance parameter of the resulting <see cref="Action{TInstance, TProperty}"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the value parameter of the resulting <see cref="Action{TInstance, TProperty}"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="Action{TInstance, TProperty}"/> that, when invoked, sets the value
        /// of the property represented by <paramref name="propertyInfo"/>.
        /// </returns>
        public static Action<TInstance, TProperty> GetSetAction<TInstance, TProperty>(this PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(TInstance), "instance");
            var valueParameter = Expression.Parameter(typeof(TProperty), "value");

            var property = 
                Expression.Property(
                    instanceParameter.EnsureConvertableTo(propertyInfo.DeclaringType),
                    propertyInfo);

            var assign = Expression.Assign(
                property,
                valueParameter.EnsureConvertableTo(propertyInfo.PropertyType));

            var lambda =
                Expression.Lambda<Action<TInstance, TProperty>>(
                    assign,
                    "Set" + propertyInfo.Name,
                    new[] { instanceParameter, valueParameter });

            return lambda.Compile();
        }
    }
}
