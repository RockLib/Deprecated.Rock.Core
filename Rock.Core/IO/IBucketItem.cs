namespace Rock.IO
{
    public interface IBucketItem
    {
        string BucketName { get; }
        string Key { get; }

        T GetValue<T>();
        void SetValue<T>(T value);
        void Delete();
    }
}