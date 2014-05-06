using System.Collections.Generic;

namespace Rock.IO
{
    public interface IBucket
    {
        string Name { get; }

        IEnumerable<IBucketItem> GetItems();
        IBucketItem GetItem(string key);
    }
}