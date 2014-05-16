using System;
using NUnit.Framework;
using Rock.Defaults;

namespace Rock.Core.UnitTests.Defaults
{
    public class DefaultHelperTests
    {
        public class TheDefaultInstanceProperty
        {
            [Test]
            public void ReturnsTheObjectThatIsReturnedByTheFunctionThatIsPassedInToTheConstructor()
            {
                var defaultInstance = new Foo();

                var defaultHelper = new DefaultHelper<Foo>(() => defaultInstance);

                Assert.That(defaultHelper.DefaultInstance, Is.SameAs(defaultInstance));
            }
        }

        public class TheCurrentProperty
        {
            [Test]
            public void ReturnsTheDefaultInstanceIfSetCurrentHasNotBeenCalled()
            {
                var defaultHelper = new DefaultHelper<Foo>(() => new Foo());

                Assert.That(defaultHelper.Current, Is.SameAs(defaultHelper.DefaultInstance));
            }

            [Test]
            public void DoesNotReturnTheDefaultInstanceIfSetCurrentHasBeenCalledWithANonNullFunction()
            {
                var defaultHelper = new DefaultHelper<Foo>(() => new Foo());

                defaultHelper.SetCurrent(() => new Foo());

                Assert.That(defaultHelper.Current, Is.Not.SameAs(defaultHelper.DefaultInstance));
            }
        }

        public class TheSetCurrentMethod
        {
            [Test]
            public void WhenPassedAFunctionCausesTheCurrentPropertyToReturnWhatTheFunctionReturns()
            {
                var defaultHelper = new DefaultHelper<Foo>(() => new Foo());

                var current = new Foo();
                defaultHelper.SetCurrent(() => current);

                Assert.That(defaultHelper.Current, Is.SameAs(current));
            }

            [Test]
            public void WhenPassedNullCausesTheCurrentPropertyToReturnTheValueOfTheDefaultInstanceProperty()
            {
                var defaultHelper = new DefaultHelper<Foo>(() => new Foo());

                defaultHelper.SetCurrent(() => new Foo());
                defaultHelper.SetCurrent(null);

                Assert.That(defaultHelper.Current, Is.SameAs(defaultHelper.DefaultInstance));
            }
        }

        private class Foo
        {
        }
    }
}