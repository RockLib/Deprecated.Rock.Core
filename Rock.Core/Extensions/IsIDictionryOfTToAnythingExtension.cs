using System;
using System.Collections.Generic;
using System.Linq;

namespace Rock.Extensions
{
    /// <summary>
    /// Provides an extension method that determines whether a type implements
    /// <see cref="IDictionary{TKey,TValue}"/> where the type of the key is known
    /// but the type of the value is unknown.
    /// </summary>
    public static class IsIDictionryOfTToAnythingExtension
    {
        /// <summary>
        /// Determines whether the given type is or implements IDictionary&lt;TKey,&gt;,
        /// where the type of the key of the dictionary is <typeparamref name="TKey"/>
        /// and the type of the value is anything.
        /// </summary>
        /// <typeparam name="TKey">The type of the Key of a dictionary.</typeparam>
        /// <param name="type">The type to check whether is is or implements IDictionary&lt;TKey,&gt;.</param>
        /// <returns>Whether the given type is or implements IDictionary&lt;TKey,&gt;.</returns>
        public static bool IsIDictionaryOfTToAnything<TKey>(this Type type)
        {
            return
                type.EqualsIDictionaryOfTToAnything<TKey>()
                    || type.GetInterfaces().Any(EqualsIDictionaryOfTToAnything<TKey>);
        }

        private static bool EqualsIDictionaryOfTToAnything<TKey>(this Type type)
        {
            return
                type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)
                   && type.GetGenericArguments()[0] == typeof(TKey);
        }
    }
}