using System.Runtime.Serialization;
using Rock.Core.UnitTests.Serialization;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace DataContractJsonSerializerSerializerTests
{
    public class UponRoundTripSerialization : UponRoundTripSerializationBase<DataContractJsonSerializerSerializer, Foo>
    {
    }

    [DataContract]
    public class Foo : IFoo
    {
        [DataMember]
        public string Bar { get; set; }
    }
}