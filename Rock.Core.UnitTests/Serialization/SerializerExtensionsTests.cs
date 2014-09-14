using System;
using System.IO;
using NUnit.Framework;
using Rock.Serialization;

// ReSharper disable once CheckNamespace


namespace SerializerExtensionsTests
{
    [Serializable]
    public class Foo
    {
        public string Bar { get; set; }
    }

    public abstract class ExtensionMethodTestBase
    {
        protected object _object;
        protected Foo _foo;
        protected ISerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _object = _foo = new Foo { Bar = "abc123" };
            _serializer = new BinaryFormatterSerializer();
        }

        protected byte[] GetFooData()
        {
            using (var stream = new MemoryStream())
            {
                _serializer.SerializeToStream(stream, _object, _object.GetType());
                stream.Flush();
                return stream.ToArray();
            }
        }
    }

    public class TheGenericSerializeToStreamExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheResultOfTheNonGenericSerializeToStreamInterfaceMethod()
        {
            byte[] results;
            using (var stream = new MemoryStream())
            {
                // The extension method under test
                _serializer.SerializeToStream(stream, _foo);
                stream.Flush();
                results = stream.ToArray();
            }

            byte[] expectedResults;
            using (var stream = new MemoryStream())
            {
                // The interface method
                _serializer.SerializeToStream(stream, _object, _object.GetType());
                stream.Flush();
                expectedResults = stream.ToArray();
            }

            Assert.That(results, Is.EqualTo(expectedResults));
        }
    }

    public class TheGenericDeserializeFromStreamExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheResultOfTheNonGenericDeserializeFromStreamInterfaceMethod()
        {
            // We need to get something to deserialize first. Use the interface's SerializeToStream method to get it.
            var fooData = GetFooData();

            Foo result;
            using (var stream = new MemoryStream(fooData))
            {
                // The extension method under test
                result = _serializer.DeserializeFromStream<Foo>(stream);
            }

            Foo expectedResult;
            using (var stream = new MemoryStream(fooData))
            {
                // The interface method
                expectedResult = (Foo)_serializer.DeserializeFromStream(stream, typeof(Foo));
            }

            Assert.That(result.Bar, Is.EqualTo(expectedResult.Bar));
        }
    }

    public class TheNonGenericSerializeToByteArrayExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheByteArrayFromTheMemoryStreamPassedToTheSerializeToStreamInterfaceMethod()
        {
            // The extension method under test
            var result = _serializer.SerializeToByteArray(_object, _object.GetType());

            byte[] expectedResult;
            using (var stream = new MemoryStream())
            {
                // The interface method
                _serializer.SerializeToStream(stream, _object, _object.GetType());
                stream.Flush();
                expectedResult = stream.ToArray();
            }

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }

    public class TheNonGenericDeserializeFromByteArrayExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheObjectReturnedFromTheDeserializeFromStreamInterfaceMethodThatIsProvidedWithAMemoryStreamCreatedWithTheByteArray()
        {
            var fooData = GetFooData();

            // The extension method under test
            var result = (Foo)_serializer.DeserializeFromByteArray(fooData, typeof(Foo));

            Foo expectedResult;
            using (var stream = new MemoryStream(fooData))
            {
                // The interface method
                expectedResult = (Foo)_serializer.DeserializeFromStream(stream, typeof(Foo));
            }

            Assert.That(result.Bar, Is.EqualTo(expectedResult.Bar));
        }
    }

    public class TheGenericSerializeToByteArrayExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheByteArrayFromTheMemoryStreamPassedToTheSerializeToStreamInterfaceMethod()
        {
            // The extension method under test
            var result = _serializer.SerializeToByteArray(_foo);

            byte[] expectedResult;
            using (var stream = new MemoryStream())
            {
                // The interface method
                _serializer.SerializeToStream(stream, _foo, _foo.GetType());
                stream.Flush();
                expectedResult = stream.ToArray();
            }

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }

    public class TheGenericDeserializeFromByteArrayExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheObjectReturnedFromTheDeserializeFromStreamInterfaceMethodThatIsProvidedWithAMemoryStreamCreatedWithTheByteArray()
        {
            var fooData = GetFooData();

            // The extension method under test
            var result = _serializer.DeserializeFromByteArray<Foo>(fooData);

            Foo expectedResult;
            using (var stream = new MemoryStream(fooData))
            {
                // The interface method
                expectedResult = (Foo)_serializer.DeserializeFromStream(stream, typeof(Foo));
            }

            Assert.That(result.Bar, Is.EqualTo(expectedResult.Bar));
        }
    }

    public class TheGenericSerializeToStringExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheStringReturnedByTheNonGenericSerializeToStringInterfaceMethod()
        {
            // The extension method under test
            var result = _serializer.SerializeToString(_foo);

            // The interface method
            var expectedResult = _serializer.SerializeToString(_foo, _foo.GetType());

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }

    public class TheGenericDeserializeFromStringExtensionMethod : ExtensionMethodTestBase
    {
        [Test]
        public void ReturnsTheObjectReturnedByTheNonGenericDeserializeFromStringInterfaceMethod()
        {
            var fooString = _serializer.SerializeToString(_foo, _foo.GetType());

            // The extension method under test
            var result = _serializer.DeserializeFromString<Foo>(fooString);

            // The interface method
            var expectedResult = (Foo)_serializer.DeserializeFromString(fooString, typeof(Foo));

            Assert.That(result.Bar, Is.EqualTo(expectedResult.Bar));
        }
    }
}
