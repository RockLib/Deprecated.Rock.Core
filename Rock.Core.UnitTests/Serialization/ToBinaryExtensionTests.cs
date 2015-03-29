using System;
using NUnit.Framework;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace ToBinaryExtensionTests
{
    public class TheToBinaryExtensionMethod
    {
        [Test]
        public void ThrowsAnExceptionIfTheExtensionParameterIsNull()
        {
            Assert.That(() => ((Foo)null).ToBinary(), Throws.Exception);
        }

        [Test]
        public void ReturnsWhatTheSerializeToByteArrayExtensionMethodOnTheDefaultBinarySerializerReturns()
        {
            var foo = new Foo { Bar = "abc123" };

            var result = foo.ToBinary();

            var expectedResult = DefaultBinarySerializer.Current.SerializeToByteArray(foo, foo.GetType());

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Serializable]
        public class Foo
        {
            public string Bar { get; set; }
        }
    }
}
