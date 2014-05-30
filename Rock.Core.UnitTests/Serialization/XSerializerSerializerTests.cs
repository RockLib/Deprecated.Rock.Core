using Rock.Core.UnitTests.Serialization;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace XSerializerSerializerTests
{
    public class UponRoundTripSerialization : UponRoundTripSerializationBase<XSerializerSerializer, Foo>
    {
    }

    public class Foo : IFoo
    {
        public string Bar { get; set; }
    }
}