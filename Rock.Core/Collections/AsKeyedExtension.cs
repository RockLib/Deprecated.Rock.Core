using System;
using System.Collections.Generic;

namespace Rock.Collections
{
    public static class AsKeyedExtension
    {
        public static IKeyedEnumerable<TKey, TItem> AsKeyed<TKey, TItem>(
            this IEnumerable<TItem> items,
            Func<TItem, TKey> getKey)
        {
            return new FunctionalKeyedCollection<TKey, TItem>(getKey, items);
        }
    }
}
