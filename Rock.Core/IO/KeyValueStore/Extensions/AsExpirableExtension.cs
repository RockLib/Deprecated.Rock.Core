namespace Rock.IO
{
    public static class AsExpirableExtension
    {
        public static IExpirableKeyValueStore AsExpirable(this IKeyValueStore keyValueStore)
        {
            return keyValueStore as IExpirableKeyValueStore ?? new ExpirableKeyValueStoreAdapter(keyValueStore);
        }
    }
}