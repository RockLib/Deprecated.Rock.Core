using NUnit.Framework;
using Rock.Reflection;

namespace Rock.Core.Reflection.IsPublicExtensionTestsTests
{
    public class IsStaticExtensionTests
    {
        [Test]
        public void ReturnsFalseForInstanceProperties()
        {
            var barProperty = typeof(Foo).GetProperty("Bar");
            Assert.That(barProperty.IsStatic(), Is.False);
        }

        [Test]
        public void ReturnsTrueForStaticProperties()
        {
            var bazProperty = typeof(Foo).GetProperty("Baz");
            Assert.That(bazProperty.IsStatic(), Is.True);
        }

        public class Foo
        {
            public string Bar { get; set; }
            public static string Baz { get; set; }
        }
    }
}
