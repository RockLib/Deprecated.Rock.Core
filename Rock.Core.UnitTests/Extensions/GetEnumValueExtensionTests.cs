using NUnit.Framework;
using Rock.Extensions;

// ReSharper disable once CheckNamespace
namespace GetEnumValueExtensionTests
{
    public class TheGetEnumValueExtensionMethod
    {
        [TestCase("Bar", Foo.Bar, TestName="Case Match")]
        [TestCase("bar", Foo.Bar, TestName="Case Mismatch")]
        public void ReturnsAnEnumValueThatCorrespondsToTheGivenStringValue(string stringValue, Foo expectedValue)
        {
            var value = stringValue.GetEnumValue<Foo>();

            Assert.That(value, Is.EqualTo(Foo.Bar));
        }

        [Test]
        public void ThrowsAnExceptionIfNoEnumValueCorrespondsToTheGivenStringValue()
        {
            const string value = "wtf";

            Assert.That(() => value.GetEnumValue<Foo>(), Throws.Exception);
        }

        [Test]
        public void ThrowsAnExceptionIfTheGenericTypeIsNotAnEnum()
        {
            const string value = "Baz";

            Assert.That(() => value.GetEnumValue<Bar>(), Throws.Exception);
            Assert.That(() => value.GetEnumValue<Bar>(), Throws.Exception);
        }

        public enum Foo
        {
            Bar,
            Baz
        }

        public struct Bar
        {
            public int Baz;
        }
    }
}