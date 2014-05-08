namespace Rock.IO
{
    public class ExpirableKeyValueStoreAdapterFactory : IExpirableKeyValueStoreAdapterFactory
    {
        public IExpirableKeyValueStore Create(IKeyValueStore keyValueStore)
        {
            return new ExpirableKeyValueStoreAdapter(keyValueStore);
        }
    }
}