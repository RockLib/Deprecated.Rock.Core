using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rock.Reflection
{
    /// <summary>
    /// Provides extension methods for obtaining optimized reflection method
    /// for invoking a type's default constructor.
    /// </summary>
    public static class GetConstructorFuncExtension
    {
        /// <summary>
        /// Gets a function with a return type of <see cref="object"/> that returns
        /// a new instance of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <returns>
        /// A function with a return type of <see cref="object"/> that returns
        /// a new instance of <paramref name="type"/>.
        /// </returns>
        public static Func<object> GetConstructorFunc(this Type type)
        {
            return type.GetConstructorFunc<object>();
        }

        /// <summary>
        /// Gets a function with a return type of <typeparamref name="T"/>
        /// that returns a new instance of <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="T">The return type of the resulting function.</typeparam>
        /// <param name="type">The type of object to create.</param>
        /// <returns>
        /// A function with a return type of <typeparamref name="T"/>
        /// that returns a new instance of <paramref name="type"/>.
        /// </returns>
        public static Func<T> GetConstructorFunc<T>(this Type type)
        {
            if (type.IsAbstract)
            {
                throw new ArgumentException(string.Format("'type', {0}, must not be abstract.", type), "type");
            }

            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format("'typeof(T)', {0}, must be assignable from 'type', {1}.", typeof(T), type), "type");
            }

            if (!type.IsValueType && !type.GetConstructors().Any(IsDefaultConstructor))
            {
                throw new ArgumentException(string.Format("'type', {0}, must have a public parameterless constructor.", type), "type");
            }

            var lambda =
                Expression.Lambda<Func<T>>(
                    Expression.New(type).EnsureConvertableTo<T>(),
                    "Create" + type.Name,
                    Enumerable.Empty<ParameterExpression>());

            return lambda.Compile();
        }

        private static bool IsDefaultConstructor(ConstructorInfo ctor)
        {
            return ctor.GetParameters().Length == 0;
        }
    }
}
