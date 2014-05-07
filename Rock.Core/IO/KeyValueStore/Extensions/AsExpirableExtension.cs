using System.Runtime.CompilerServices;

namespace Rock.IO
{
    public static class AsExpirableExtension
    {
        private static readonly ConditionalWeakTable<IKeyValueStore, ExpirableKeyValueStoreAdapter> _adapters = new ConditionalWeakTable<IKeyValueStore, ExpirableKeyValueStoreAdapter>();

        public static IExpirableKeyValueStore AsExpirable(this IKeyValueStore keyValueStore)
        {
            return keyValueStore as IExpirableKeyValueStore ?? _adapters.GetValue(keyValueStore, CreateAdapter);
        }

        private static ExpirableKeyValueStoreAdapter CreateAdapter(IKeyValueStore keyValueStore)
        {
            return new ExpirableKeyValueStoreAdapter(keyValueStore);
        }
    }
}