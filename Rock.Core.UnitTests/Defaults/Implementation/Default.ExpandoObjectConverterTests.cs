using System.Dynamic;
using Rock.Conversion;

namespace DefaultHelperTests.Implementation
{
    public class Default_ExpandoObjectConverterTests : DefaultTestBase<IConverter<ExpandoObject>, ExpandoObjectConverter>
    {
        protected override string PropertyName
        {
            get { return "ExpandoObjectConverter"; }
        }
    }
}