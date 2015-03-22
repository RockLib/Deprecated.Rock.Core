using System.Dynamic;
using Rock.Conversion;

namespace DefaultHelperTests.Implementation
{
    internal class Default_ExpandoObjectConverterTests : DefaultTestBase<IConvertsTo<ExpandoObject>, ConvertsToExpandoObject>
    {
        protected override string PropertyName
        {
            get { return "ObjectToExpandoObjectConverter"; }
        }
    }
}