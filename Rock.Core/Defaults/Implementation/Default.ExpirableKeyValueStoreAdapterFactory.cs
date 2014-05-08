using System;
using Rock.IO;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IExpirableKeyValueStoreAdapterFactory> _expirableKeyValueStoreAdapterFactory = new DefaultHelper<IExpirableKeyValueStoreAdapterFactory>(() => new ExpirableKeyValueStoreAdapterFactory());

        public static IExpirableKeyValueStoreAdapterFactory ExpirableKeyValueStoreAdapterFactory
        {
            get { return _expirableKeyValueStoreAdapterFactory.Current; }
        }

        public static void SetExpirableKeyValueStoreAdapterFactory(Func<IExpirableKeyValueStoreAdapterFactory> getExpirableKeyValueStoreAdapterFactoryInstance)
        {
            _expirableKeyValueStoreAdapterFactory.SetCurrent(getExpirableKeyValueStoreAdapterFactoryInstance);
        }
    }
}
