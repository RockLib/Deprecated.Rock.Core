using System.Collections.Generic;

namespace Rock.IO
{
    public interface IExpirableBucket : IBucket
    {
        new IEnumerable<IExpirableBucketItem> GetItems();
        new IExpirableBucketItem GetItem(string key);
    }
}