using NUnit.Framework;
using Rock.Collections;

namespace Rock.Core.UnitTests.Collections
{
    public class TheAsKeyedExtensionMethod
    {
        [Test]
        public void ReturnsAnInstanceOfKeyedCollection()
        {
            var enumerable = new[] { new Foo() { Bar = "abc" } };

            var keyedEnumerable = enumerable.AsKeyed(f => f.Bar);

            Assert.That(keyedEnumerable, Is.InstanceOf<KeyedCollection<string, Foo>>());
        }

        private class Foo
        {
            public string Bar { get; set; }
        }
    }
}