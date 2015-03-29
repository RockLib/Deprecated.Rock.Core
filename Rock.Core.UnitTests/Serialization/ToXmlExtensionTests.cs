using NUnit.Framework;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace ToXmlExtensionTests
{
    public class TheToXmlExtensionMethod
    {
        [Test]
        public void ThrowsAnExceptionIfTheExtensionParameterIsNull()
        {
            Assert.That(() => ((Foo)null).ToXml(), Throws.Exception);
        }

        [Test]
        public void ReturnsWhatTheSerializeToStringInterfaceMethodOnTheDefaultXmlSerializerReturns()
        {
            var foo = new Foo { Bar = "abc123" };

            var result = foo.ToXml();

            var expectedResult = DefaultXmlSerializer.Current.SerializeToString(foo, foo.GetType());

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public class Foo
        {
            public string Bar { get; set; }
        }
    }
}
