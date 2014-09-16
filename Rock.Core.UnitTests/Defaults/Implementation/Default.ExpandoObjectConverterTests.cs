using System.Dynamic;
using Rock.Conversion;

namespace DefaultHelperTests.Implementation
{
    public class Default_ExpandoObjectConverterTests : DefaultTestBase<IConvertsTo<ExpandoObject>, ExpandoObjectConverter>
    {
        protected override string PropertyName
        {
            get { return "ExpandoObjectConverter"; }
        }
    }
}