using System.Dynamic;
using Moq;
using NUnit.Framework;
using Rock.Conversion;
using Rock.Defaults.Implementation;

// ReSharper disable once CheckNamespace
namespace ToExpandoObjectExtensionTests
{
    public class TheToExpandoObjectExtensionMethod
    {
        private Mock<IConverter<ExpandoObject>> _mockConverter;

        [SetUp]
        public void Setup()
        {
            _mockConverter = new Mock<IConverter<ExpandoObject>>();

            Default.SetExpandoObjectConverter(() => _mockConverter.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Default.RestoreDefaultExpandoObjectConverter();
        }

        [Test]
        public void CallsTheConvertMethodOfTheDefaultExpandoObjectConverter()
        {
            var foo = new Foo();
            foo.ToExpandoObject();

            _mockConverter.Verify(m => m.Convert(It.Is<Foo>(f => f == foo)), Times.Once);
        }

        public class Foo
        {
        }
    }
}