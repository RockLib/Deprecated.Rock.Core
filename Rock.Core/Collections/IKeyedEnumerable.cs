using System.Collections.Generic;

namespace Rock.Collections
{
    /// <summary>
    /// Represents a collection whose keys are embedded in the values.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public interface IKeyedEnumerable<in TKey, out TItem> : IEnumerable<TItem>
    {
        /// <summary>
        /// Gets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The element with the specified key. If an element with the specified key is not found, an exception is thrown.</returns>
        TItem this[TKey key] { get; }

        /// <summary>
        /// Determines whether the collection contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IKeyedEnumerable{TKey,TItem}"/>.</param>
        /// <returns>true if the <see cref="IKeyedEnumerable{TKey,TItem}"/> contains an element with the specified key; otherwise, false.</returns>
        bool Contains(TKey key);
    }
}