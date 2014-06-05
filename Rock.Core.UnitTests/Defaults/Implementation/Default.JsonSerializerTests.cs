using Rock.Serialization;

namespace DefaultHelperTests.Implementation
{
    public class Default_JsonSerializerTests : DefaultTestBase<ISerializer, NewtonsoftJsonSerializer>
    {
        protected override string PropertyName
        {
            get { return "JsonSerializer"; }
        }
    }
}