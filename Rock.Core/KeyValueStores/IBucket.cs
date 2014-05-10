using System.Collections.Generic;

namespace Rock.KeyValueStores
{
    public interface IBucket
    {
        string Name { get; }

        IEnumerable<IBucketItem> GetItems();
        IBucketItem GetItem(string key);
    }
}