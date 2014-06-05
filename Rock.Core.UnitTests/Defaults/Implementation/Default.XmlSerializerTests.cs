using Rock.Serialization;

namespace DefaultHelperTests.Implementation
{
    public class Default_XmlSerializerTests : DefaultTestBase<ISerializer, XSerializerSerializer>
    {
        protected override string PropertyName
        {
            get { return "XmlSerializer"; }
        }
    }
}