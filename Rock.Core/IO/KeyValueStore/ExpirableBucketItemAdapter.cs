using System;

namespace Rock.IO
{
    public class ExpirableBucketItemAdapter : IExpirableBucketItem
    {
        private readonly IBucketItem _bucketItem;
        private readonly ExpirableAdapterHelper _expirableAdapterHelper;

        private TimeSpan _expiry;
        private DateTime _lastTouched;

        public ExpirableBucketItemAdapter(IBucketItem bucketItem, ExpirableAdapterHelper expirableAdapterHelper)
        {
            _bucketItem = bucketItem;
            _expirableAdapterHelper = expirableAdapterHelper;
        }

        public string BucketName
        {
            get { return _bucketItem.BucketName; }
        }

        public string Key
        {
            get { return _bucketItem.Key; }
        }

        public bool TryGetValue<T>(out T value)
        {
            return _bucketItem.TryGetValue(out value);
        }

        public void SetValue<T>(T value)
        {
            SetValue(value, null);
        }

        public void SetValue<T>(T value, TimeSpan? expiry)
        {
            _bucketItem.SetValue(value);

            if (expiry == null)
            {
                _expirableAdapterHelper.RemoveItem(this);
                return;
            }

            _expiry = expiry.Value;
            _lastTouched = DateTime.UtcNow;

            _expirableAdapterHelper.RegisterItem(this);
        }

        public void Touch()
        {
            _lastTouched = DateTime.UtcNow;
        }

        public void Delete()
        {
            _bucketItem.Delete();
        }

        public DateTime GetExpirationDate()
        {
            return _lastTouched + _expiry;
        }
    }
}