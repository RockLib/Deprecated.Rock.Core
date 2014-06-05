using System.Dynamic;
using Rock.Conversion;

namespace DefaultHelperTests.Implementation
{
    public class Default_ConvertToExpandoObjectTests : DefaultTestBase<IConvertTo<ExpandoObject>, ReflectinatorConvertToExpandoObject>
    {
        protected override string PropertyName
        {
            get { return "ConvertToExpandoObject"; }
        }
    }
}