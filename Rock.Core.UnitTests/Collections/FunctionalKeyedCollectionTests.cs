using NUnit.Framework;
using Rock.Collections;

// ReSharper disable once CheckNamespace
namespace FunctionalKeyedCollectionTests
{
    public class TheFunctionalKeyedCollectionConstructor
    {
        [Test]
        public void UsesTheFunctionPassedInToDetermineTheKeyOfAnObject()
        {
            var item = new Foo {Bar = "a", Baz = "z"};
            var collection = new FunctionalKeyedCollection<string, Foo>(foo => foo.Bar) { item };

            Assert.That(collection["a"], Is.SameAs(item));
            Assert.That(() => collection["z"], Throws.Exception);
        }

        [Test]
        public void ThrowsAnExceptionIfTheFunctionPassedInIsNull()
        {
            Assert.That(() => new FunctionalKeyedCollection<string, Foo>(null), Throws.Exception);
        }

        private class Foo
        {
            public string Bar { get; set; }
            public string Baz { get; set; }
        }
    }
}