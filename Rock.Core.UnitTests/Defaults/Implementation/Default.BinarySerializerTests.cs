using Rock.Serialization;

namespace DefaultHelperTests.Implementation
{
    internal class Default_BinarySerializerTests : DefaultTestBase<ISerializer, BinaryFormatterSerializer>
    {
        protected override string PropertyName
        {
            get { return "BinarySerializer"; }
        }
    }
}