namespace Rock.IO
{
    public interface IExpirableKeyValueStoreAdapterFactory
    {
        IExpirableKeyValueStore Create(IKeyValueStore keyValueStore);
    }
}