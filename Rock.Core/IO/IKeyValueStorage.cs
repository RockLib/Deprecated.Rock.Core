using System.Collections.Generic;

namespace Rock.IO
{
    public interface IKeyValueStorage
    {
        IEnumerable<IBucket> GetBuckets();
        IBucket GetBucket(string bucketName);
    }
}