using NUnit.Framework;
using Rock.Collections;

namespace Rock.Core.UnitTests.Collections
{
    public class AnInstanceOfFunctionalKeyedCollection
    {
        [Test]
        public void UsesTheFunctionPassedInToTheConstructorToDetermineItsKey()
        {
            var item = new Foo {Bar = "a", Baz = "z"};
            var collection = new FunctionalKeyedCollection<string, Foo>(foo => foo.Bar) { item };

            Assert.That(collection["a"], Is.SameAs(item));
            Assert.That(() => collection["z"], Throws.Exception);
        }

        private class Foo
        {
            public string Bar { get; set; }
            public string Baz { get; set; }
        }
    }
}