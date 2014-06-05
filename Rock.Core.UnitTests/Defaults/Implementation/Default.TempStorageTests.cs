using Rock.KeyValueStores;

namespace DefaultHelperTests.Implementation
{
    public class Default_TempStorageTests : DefaultTestBase<IKeyValueStore, FileKeyValueStore>
    {
        protected override string PropertyName
        {
            get { return "TempStorage"; }
        }
    }
}