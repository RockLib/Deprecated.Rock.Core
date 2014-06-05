using Rock;

namespace DefaultHelperTests.Implementation
{
    public class Default_ApplicationInfoTests : DefaultTestBase<IApplicationInfo, EntryAssemblyApplicationInfo>
    {
        protected override string PropertyName
        {
            get { return "ApplicationInfo"; }
        }
    }
}