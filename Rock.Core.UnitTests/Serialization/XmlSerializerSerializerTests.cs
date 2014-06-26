using Rock.Core.UnitTests.Serialization;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace XmlSerializerSerializerTests
{
    public class UponRoundTripSerialization : UponRoundTripSerializationBase<XmlSerializerSerializer, Foo>
    {
    }

    public class Foo : IFoo
    {
        public string Bar { get; set; }
    }
}