using System;
using System.Collections.Generic;

namespace Rock.Collections
{
    /// <summary>
    /// Provides the abstract base class for a collection whose keys are embedded in the values.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    public abstract class KeyedCollection<TKey, TItem>
        : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>,
          IKeyedEnumerable<TKey, TItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedCollection{TKey,TItem}"/> class that
        /// uses the specified equality comparer and creates a lookup dictionary when the specified
        /// threshold is exceeded.
        /// </summary>
        /// <param name="comparer">
        /// The implementation of the <see cref="IEqualityComparer{T}"/> generic interface to use
        /// when comparing keys, or null to use the default equality comparer for the type of the key, 
        /// obtained from <see cref="EqualityComparer{T}.Default"/>.
        /// </param>
        /// <param name="dictionaryCreationThreshold">
        /// The number of elements the collection can hold without creating a lookup dictionary 
        /// (0 creates the lookup dictionary when the first item is added), or –1 to specify that a 
        /// lookup dictionary is never created.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="dictionaryCreationThreshold"/> is less than –1
        /// </exception>
        protected KeyedCollection(IEqualityComparer<TKey> comparer = null, int dictionaryCreationThreshold = 0)
            : base(comparer, dictionaryCreationThreshold)
        {
        }
    }
}