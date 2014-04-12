using System;
using System.Collections.Generic;

namespace Rock.Collections
{
    public class FunctionalKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private readonly Func<TItem, TKey> _getKey;

        public FunctionalKeyedCollection(Func<TItem, TKey> getKey, IEnumerable<TItem> items = null)
        {
            _getKey = getKey;

            if (items != null)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return _getKey(item);
        }
    }
}
