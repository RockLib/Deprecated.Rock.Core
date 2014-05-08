using System.Runtime.CompilerServices;
using Rock.Defaults.Implementation;

namespace Rock.IO
{
    public static class AsExpirableExtension
    {
        private static readonly ConditionalWeakTable<IKeyValueStore, IExpirableKeyValueStore> _adapters = new ConditionalWeakTable<IKeyValueStore, IExpirableKeyValueStore>();

        public static IExpirableKeyValueStore AsExpirable(this IKeyValueStore keyValueStore)
        {
            return
                keyValueStore as IExpirableKeyValueStore
                ?? _adapters.GetValue(keyValueStore, Default.ExpirableKeyValueStoreAdapterFactory.Create);
        }
    }
}