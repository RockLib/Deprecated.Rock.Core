using System.Collections.Generic;

namespace Rock.IO
{
    public interface IBucket
    {
        string Name { get; }

        IEnumerable<IBucketItem> GetItems();
        IBucketItem GetItem(string key);

        void AddItem<T>(string key, T value);
        void RemoveItem(string key);
    }
}