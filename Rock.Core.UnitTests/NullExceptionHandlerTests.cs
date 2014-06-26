using System;
using NUnit.Framework;
using Rock;

namespace NullExceptionHandlerTests
{
    public class TheNullExceptionHandlerClass
    {
        [Test]
        public void HasNoPublicConstructors()
        {
            var publicConstructors = typeof(NullExceptionHandler).GetConstructors();

            Assert.That(publicConstructors, Is.Empty);
        }

        [Test]
        public void IsASingleton()
        {
            var first = NullExceptionHandler.Instance;
            var second = NullExceptionHandler.Instance;

            Assert.That(first, Is.SameAs(second));
        }
    }

    public class TheHandleExceptionMethod
    {
        [Test]
        public void DoesNothing()
        {
            Assert.That(() => NullExceptionHandler.Instance.HandleException(new Exception()), Throws.Nothing);
        }
    }
}