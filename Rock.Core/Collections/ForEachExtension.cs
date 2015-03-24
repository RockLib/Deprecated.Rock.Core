using System;
using System.Collections.Generic;

namespace Rock.Collections
{
    /// <summary>
    /// Provides a <see cref="ForEach{TSource}"/> extension method.
    /// </summary>
    public static class ForEachExtension
    {
        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">
        /// An <see cref="IEnumerable{T}"/> that contains the elements to perform an action on.
        /// </param>
        /// <param name="action">
        /// The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}"/>.
        ///</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
