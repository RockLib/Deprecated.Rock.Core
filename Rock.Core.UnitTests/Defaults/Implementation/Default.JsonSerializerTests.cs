using Rock.Serialization;

namespace DefaultHelperTests.Implementation
{
    internal class Default_JsonSerializerTests : DefaultTestBase<ISerializer, DataContractJsonSerializerSerializer>
    {
        protected override string PropertyName
        {
            get { return "JsonSerializer"; }
        }
    }
}