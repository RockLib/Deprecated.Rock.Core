using Rock;

namespace DefaultHelperTests.Implementation
{
    internal class Default_ApplicationInfoTests : DefaultTestBase<IApplicationInfo, DefaultApplicationInfo>
    {
        protected override string PropertyName
        {
            get { return "ApplicationInfo"; }
        }
    }
}