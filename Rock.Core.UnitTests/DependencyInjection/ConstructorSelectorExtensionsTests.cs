using System;
using Moq;
using NUnit.Framework;
using Rock.DependencyInjection;
using Rock.DependencyInjection.Heuristics;

namespace ConstructorSelectorExtensionsTests
{
    public class TheGetConstructorExtensionMethod
    {
        [Test]
        public void WhenTryGetConstructorReturnsTrueReturnsTheValueOfTheOutParameter()
        {
            // ReSharper disable once RedundantAssignment
            var returnCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            var mockConstructorSelector = new Mock<IConstructorSelector>();
            mockConstructorSelector
                .Setup(m => m.TryGetConstructor(typeof(object), It.IsAny<IResolver>(), out returnCtor))
                .Returns(true);

            var ctor = mockConstructorSelector.Object.GetConstructor(typeof(object), null);
            Assert.That(ctor, Is.EqualTo(returnCtor));
        }

        [Test]
        public void WhenTryGetConstructorReturnsFalseThrowsAnException()
        {
            // ReSharper disable once RedundantAssignment
            var returnCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            var mockConstructorSelector = new Mock<IConstructorSelector>();
            mockConstructorSelector
                .Setup(m => m.TryGetConstructor(typeof(object), It.IsAny<IResolver>(), out returnCtor))
                .Returns(false);

            Assert.That(() => mockConstructorSelector.Object.GetConstructor(typeof(object), null), Throws.Exception);
        }
    }

    public class TheCanGetConstructorExtensionMethod
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ReturnsWhatTheTryGetConstructorMethodFromTheConstructorSelectorReturns(bool whatTheTryGetConstructorMethodReturns)
        {
            // ReSharper disable once RedundantAssignment
            var returnCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            var mockConstructorSelector = new Mock<IConstructorSelector>();
            mockConstructorSelector
                .Setup(m => m.TryGetConstructor(typeof(object), It.IsAny<IResolver>(), out returnCtor))
                .Returns(whatTheTryGetConstructorMethodReturns);

            var result = mockConstructorSelector.Object.CanGetConstructor(typeof(object), null);

            Assert.That(result, Is.EqualTo(whatTheTryGetConstructorMethodReturns));
        }
    }
}