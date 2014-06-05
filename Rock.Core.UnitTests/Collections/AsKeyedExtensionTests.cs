using NUnit.Framework;
using Rock.Collections;

// ReSharper disable once CheckNamespace
namespace AsKeyedExtensionMethodTests
{
    public class TheAsKeyedExtensionMethod
    {
        [Test]
        public void ReturnsAnInstanceOfKeyedCollection()
        {
            var enumerable = new[] { new Foo { Bar = "abc" } };

            var keyedEnumerable = enumerable.AsKeyed(f => f.Bar);

            Assert.That(keyedEnumerable, Is.InstanceOf<KeyedCollection<string, Foo>>());
        }

        private class Foo
        {
            public string Bar { get; set; }
        }
    }
}