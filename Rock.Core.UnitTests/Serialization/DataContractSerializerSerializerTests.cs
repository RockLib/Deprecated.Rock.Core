using System.Runtime.Serialization;
using Rock.Core.UnitTests.Serialization;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace DataContractSerializerSerializerTests
{
    public class UponRoundTripSerialization : UponRoundTripSerializationBase<DataContractSerializerSerializer, Foo>
    {
    }

    [DataContract]
    public class Foo : IFoo
    {
        [DataMember]
        public string Bar { get; set; }
    }
}