using System;
using Rock.Core.UnitTests.Serialization;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace BinaryFormatterSerializerTests
{
    public class UponRoundTripSerialization : UponRoundTripSerializationBase<BinaryFormatterSerializer, Foo>
    {
    }

    [Serializable]
    public class Foo : IFoo
    {
        public string Bar { get; set; }
    }
}