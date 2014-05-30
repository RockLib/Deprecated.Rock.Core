using System.IO;
using NUnit.Framework;
using Rock.Serialization;

namespace Rock.Core.UnitTests.Serialization
{
    public interface IFoo
    {
        string Bar { get; set; }
    }

    public abstract class UponRoundTripSerializationBase<TSerializer, TFoo>
            where TSerializer : ISerializer, new()
            where TFoo : IFoo, new()
    {
        [Test]
        public void VerifyNonEmptySerializationAndCorrectDeserializationUsingStream()
        {
            var foo = new TFoo { Bar = "abcd" };

            var serializer = new TSerializer();

            byte[] data;

            using (var stream = new MemoryStream())
            {
                serializer.SerializeToStream(stream, foo, typeof(TFoo));
                stream.Flush();
                data = stream.ToArray();
            }

            Assert.That(data, Is.Not.Null.Or.Empty);

            TFoo roundTripFoo;

            using (var stream = new MemoryStream(data))
            {
                roundTripFoo = (TFoo)serializer.DeserializeFromStream(stream, typeof(TFoo));
            }

            Assert.That(roundTripFoo.Bar, Is.EqualTo(foo.Bar));
        }

        [Test]
        public void VerifyNonEmptySerializationAndCorrectDeserializationUsingString()
        {
            var foo = new TFoo { Bar = "abc" };

            var serializer = new TSerializer();

            var data = serializer.SerializeToString(foo, typeof(TFoo));

            Assert.That(data, Is.Not.Null.Or.Empty);

            var roundTripFoo = (TFoo)serializer.DeserializeFromString(data, typeof(TFoo));

            Assert.That(roundTripFoo.Bar, Is.EqualTo(foo.Bar));
        }
    }
}