using NUnit.Framework;
using Rock.Collections;

// ReSharper disable once CheckNamespace
namespace KeyedCollectionTests
{
    public class TheKeyedCollectionClass
    {
        [Test]
        public void InheritsFromSystemCollectionsObjectModelKeyedCollection()
        {
            var keyedCollectionType = typeof(KeyedCollection<int, string>);

            Assert.That(typeof(System.Collections.ObjectModel.KeyedCollection<int, string>).IsAssignableFrom(keyedCollectionType));
        }

        [Test]
        public void ImplementsIKeyedEnumerable()
        {
            var keyedCollectionType = typeof(KeyedCollection<int, string>);

            Assert.That(typeof(IKeyedEnumerable<int, string>).IsAssignableFrom(keyedCollectionType));
        }
    }
}