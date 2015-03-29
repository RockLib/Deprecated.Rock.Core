using System.Dynamic;
using Moq;
using NUnit.Framework;
using Rock.Conversion;

// ReSharper disable once CheckNamespace
namespace ToExpandoObjectExtensionTests
{
    public class TheToExpandoObjectExtensionMethod
    {
        private Mock<IConvertsTo<ExpandoObject>> _mockConverter;

        [SetUp]
        public void Setup()
        {
            _mockConverter = new Mock<IConvertsTo<ExpandoObject>>();

            ToExpandoObjectExtension.UnlockConverter();
            ToExpandoObjectExtension.SetConverter(_mockConverter.Object);
        }

        [TearDown]
        public void TearDown()
        {
            ToExpandoObjectExtension.ResetConverter();
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