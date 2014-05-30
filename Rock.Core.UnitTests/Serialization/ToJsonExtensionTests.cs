using NUnit.Framework;
using Rock.Defaults.Implementation;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace ToJsonExtensionTests
{
    public class TheToJsonExtensionMethod
    {
        [Test]
        public void ThrowsAnExceptionIfTheExtensionParameterIsNull()
        {
            Assert.That(() => ((Foo)null).ToJson(), Throws.Exception);
        }

        [Test]
        public void ReturnsWhatTheSerializeToStringInterfaceMethodOnTheDefaultJsonSerializerReturns()
        {
            var foo = new Foo { Bar = "abc123" };

            var result = foo.ToJson();

            var expectedResult = Default.JsonSerializer.SerializeToString(foo, foo.GetType());

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        public class Foo
        {
            public string Bar { get; set; }
        }
    }
}
