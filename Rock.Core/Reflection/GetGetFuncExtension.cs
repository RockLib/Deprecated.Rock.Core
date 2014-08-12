using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

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
            return PropertyFuncCache<object, object>.GetGetFuncFor(propertyInfo);
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
            return PropertyFuncCache<TInstance, TProperty>.GetGetFuncFor(propertyInfo);
        }

        private static Func<TInstance, TProperty> CreateGetterFunc<TInstance, TProperty>(PropertyInfo propertyInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(TInstance), "instance");

            var body =
                Expression.Property(
                    CheckParameterExpression<TInstance>(instanceParameter, propertyInfo),
                    propertyInfo);

            var lambda =
                Expression.Lambda<Func<TInstance, TProperty>>(
                    CheckReturnExpression<TProperty>(body, propertyInfo),
                    instanceParameter);

            return lambda.Compile();
        }

        private static Expression CheckParameterExpression<TInstance>(Expression expression, PropertyInfo propertyInfo)
        {
            if (typeof(TInstance).IsLessSpecificThan(propertyInfo.DeclaringType))
            {
                return Expression.Convert(expression, propertyInfo.DeclaringType);
            }

            return expression;
        }

        private static Expression CheckReturnExpression<TProperty>(Expression expression, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsLessSpecificThan(typeof(TProperty))
                || propertyInfo.PropertyType.RequiresBoxingWhenConvertingTo(typeof(TProperty)))
            {
                return Expression.Convert(expression, typeof(TProperty));
            }

            return expression;
        }

        private static bool IsLessSpecificThan(this Type thisType, Type comparisonType)
        {
            return thisType != comparisonType && thisType.IsAssignableFrom(comparisonType);
        }

        private static bool RequiresBoxingWhenConvertingTo(this Type fromType, Type toType)
        {
            return fromType.IsValueType && !toType.IsValueType;
        }

        private static class PropertyFuncCache<TInstance, TProperty>
        {
            private static readonly ConditionalWeakTable<PropertyInfo, Func<TInstance, TProperty>> _getterTable = new ConditionalWeakTable<PropertyInfo, Func<TInstance, TProperty>>();

            public static Func<TInstance, TProperty> GetGetFuncFor(PropertyInfo propertyInfo)
            {
                return _getterTable.GetValue(propertyInfo, CreateGetterFunc<TInstance, TProperty>);
            }
        }
    }
}
