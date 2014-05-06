using System.Collections.Generic;
using System.Linq;

namespace Rock.IO
{
    public class ExpirableBucketAdapter : IExpirableBucket
    {
        private readonly IBucket _bucket;

        public ExpirableBucketAdapter(IBucket bucket)
        {
            _bucket = bucket;
        }

        public string Name
        {
            get { return _bucket.Name; }
        }

        IEnumerable<IBucketItem> IBucket.GetItems()
        {
            return GetItems();
        }

        public IEnumerable<IExpirableBucketItem> GetItems()
        {
            return _bucket.GetItems().Select(item => item as IExpirableBucketItem ?? new ExpirableBucketItemAdapter(item));
        }

        IBucketItem IBucket.GetItem(string key)
        {
            return GetItem(key);
        }

        public IExpirableBucketItem GetItem(string key)
        {
            var item = _bucket.GetItem(key);
            return item as IExpirableBucketItem ?? new ExpirableBucketItemAdapter(item);
        }
    }
}