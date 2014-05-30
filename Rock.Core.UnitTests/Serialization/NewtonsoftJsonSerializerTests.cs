using Rock.Core.UnitTests.Serialization;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace NewtonsoftJsonSerializerTests
{
    public class UponRoundTripSerialization : UponRoundTripSerializationBase<NewtonsoftJsonSerializer, Foo>
    {
    }

    public class Foo : IFoo
    {
        public string Bar { get; set; }
    }
}