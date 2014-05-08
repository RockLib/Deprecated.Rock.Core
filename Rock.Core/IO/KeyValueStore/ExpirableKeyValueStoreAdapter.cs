using System.Collections.Generic;
using System.Linq;

namespace Rock.IO
{
    internal class ExpirableKeyValueStoreAdapter : IExpirableKeyValueStore
    {
        private readonly ExpirableAdapterHelper _expirableAdapterHelper = new ExpirableAdapterHelper();
        private readonly IKeyValueStore _keyValueStore;

        public ExpirableKeyValueStoreAdapter(IKeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
        }

        IEnumerable<IBucket> IKeyValueStore.GetBuckets()
        {
            return GetBuckets();
        }

        public IEnumerable<IExpirableBucket> GetBuckets()
        {
            return _keyValueStore.GetBuckets().Select(bucket => bucket as IExpirableBucket ?? new ExpirableBucketAdapter(bucket, _expirableAdapterHelper));
        }

        IBucket IKeyValueStore.GetBucket(string bucketName)
        {
            return GetBucket(bucketName);
        }

        public IExpirableBucket GetBucket(string bucketName)
        {
            var bucket = _keyValueStore.GetBucket(bucketName);
            return bucket as IExpirableBucket ?? new ExpirableBucketAdapter(bucket, _expirableAdapterHelper);
        }
    }
}