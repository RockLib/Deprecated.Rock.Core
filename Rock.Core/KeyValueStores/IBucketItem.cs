namespace Rock.KeyValueStores
{
    public interface IBucketItem
    {
        string BucketName { get; }
        string Key { get; }

        bool TryGet<T>(out T value);
        void Put<T>(T value);
        void Delete();
    }
}