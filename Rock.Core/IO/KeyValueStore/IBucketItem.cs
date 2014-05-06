namespace Rock.IO
{
    public interface IBucketItem
    {
        string BucketName { get; }
        string Key { get; }

        bool TryGetValue<T>(out T value);
        void SetValue<T>(T value);
        void Delete();
    }

    public interface IExpirableBucketItem : IBucketItem
    {
        void Touch();
        void SetValue<T>(T value, Expiry expiry);
    }
}