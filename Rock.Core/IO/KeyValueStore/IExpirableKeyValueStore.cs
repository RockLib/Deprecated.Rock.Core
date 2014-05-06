using System.Collections.Generic;

namespace Rock.IO
{
    public interface IExpirableKeyValueStore : IKeyValueStore
    {
        new IEnumerable<IExpirableBucket> GetBuckets();
        new IExpirableBucket GetOrAddBucket(string bucketName);
    }
}