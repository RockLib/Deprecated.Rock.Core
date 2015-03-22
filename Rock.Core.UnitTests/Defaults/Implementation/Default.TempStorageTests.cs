using Rock.KeyValueStores;

namespace DefaultHelperTests.Implementation
{
    internal class Default_TempStorageTests : DefaultTestBase<IKeyValueStore, FileKeyValueStore>
    {
        protected override string PropertyName
        {
            get { return "TempStorage"; }
        }
    }
}