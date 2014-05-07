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

        IBucket IKeyValueStore.GetOrAddBucket(string bucketName)
        {
            return GetOrAddBucket(bucketName);
        }

        public IExpirableBucket GetOrAddBucket(string bucketName)
        {
            var bucket = _keyValueStore.GetOrAddBucket(bucketName);
            return bucket as IExpirableBucket ?? new ExpirableBucketAdapter(bucket, _expirableAdapterHelper);
        }
    }
}