using System.Linq;
using NUnit.Framework;
using Rock.Collections;

namespace Rock.Core.UnitTests.Collections
{
    public class TheConcatMethodThatExtendsT
    {
        [Test]
        public void ReturnsAnEnumerableWithTheInstanceOfTAtTheBeginning()
        {
            var collection = 0.Concat(new[] { 1, 2, 3 });

            Assert.That(collection.First(), Is.EqualTo(0));
        }
    }

    public class TheConcatMethodThatExtendsIEnumerableOfT
    {
        [Test]
        public void ReturnsAnEnumerableWithTheAdditionalItemsAtTheEnd()
        {
            var collection = new[] { 1, 2, 3 }.Concat(4);

            Assert.That(collection.Last(), Is.EqualTo(4));
        }
    }
}