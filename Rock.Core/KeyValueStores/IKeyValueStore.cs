using System.Collections.Generic;

namespace Rock.KeyValueStores
{
    public interface IKeyValueStore
    {
        IEnumerable<IBucket> GetBuckets();
        IBucket GetBucket(string bucketName);
    }
}