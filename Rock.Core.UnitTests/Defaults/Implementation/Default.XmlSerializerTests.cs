using Rock.Serialization;

namespace DefaultHelperTests.Implementation
{
    internal class Default_XmlSerializerTests : DefaultTestBase<ISerializer, XmlSerializerSerializer>
    {
        protected override string PropertyName
        {
            get { return "XmlSerializer"; }
        }
    }
}