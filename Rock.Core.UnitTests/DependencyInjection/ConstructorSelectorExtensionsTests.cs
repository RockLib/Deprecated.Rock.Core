using System;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Rock.DependencyInjection;
using Rock.DependencyInjection.Heuristics;

// ReSharper disable once CheckNamespace
namespace ConstructorSelectorExtensionsTests
{
    public class TheGetConstructorExtensionMethod
    {
        [Test]
        public void WhenTryGetConstructorReturnsTrueReturnsTheValueOfTheOutParameter()
        {
            var mockConstructorSelector = new Mock<IConstructorSelector>();

            // ReSharper disable once RedundantAssignment
            var returnCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            mockConstructorSelector
                .Setup(m => m.TryGetConstructor(typeof(object), It.IsAny<IResolver>(), out returnCtor))
                .Returns(true);

            var ctor = mockConstructorSelector.Object.GetConstructor(typeof(object), null);
            Assert.That(ctor, Is.EqualTo(returnCtor));
        }

        [Test]
        public void WhenTryGetConstructorReturnsFalseThrowsAnException()
        {
            var mockConstructorSelector = new Mock<IConstructorSelector>();

            // ReSharper disable once RedundantAssignment
            ConstructorInfo returnCtor = null;
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
            var mockConstructorSelector = new Mock<IConstructorSelector>();

            // ReSharper disable once RedundantAssignment
            ConstructorInfo returnCtor = null;
            mockConstructorSelector
                .Setup(m => m.TryGetConstructor(typeof(object), It.IsAny<IResolver>(), out returnCtor))
                .Returns(whatTheTryGetConstructorMethodReturns);

            var result = mockConstructorSelector.Object.CanGetConstructor(typeof(object), null);

            Assert.That(result, Is.EqualTo(whatTheTryGetConstructorMethodReturns));
        }
    }
}