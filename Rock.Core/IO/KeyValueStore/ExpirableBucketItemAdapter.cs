namespace Rock.IO
{
    public class ExpirableBucketItemAdapter : IExpirableBucketItem
    {
        // notes on implementation
        // there should be a static dictionary of some sort that holds the expiration information for an item
        // there should be a background thread that checks items to see if one or more have expired.
        // the value of this dictionary should probably be some sort of action, where the action contains a closure over the item to remove, and calls that item's Delete() method.

        private readonly IBucketItem _bucketItem;

        public ExpirableBucketItemAdapter(IBucketItem bucketItem)
        {
            _bucketItem = bucketItem;
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
            SetValue(value, Expiry.None);
        }

        public void Delete()
        {
            _bucketItem.Delete();
        }

        public void Touch()
        {
            // TODO: implement touch logic.
        }

        public void SetValue<T>(T value, Expiry expiry)
        {
            // TODO: implement expiry logic
            _bucketItem.SetValue(value);
        }
    }
}