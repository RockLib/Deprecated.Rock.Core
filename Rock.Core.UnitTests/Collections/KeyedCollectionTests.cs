using NUnit.Framework;
using Rock.Collections;

namespace Rock.Core.UnitTests.Collections
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