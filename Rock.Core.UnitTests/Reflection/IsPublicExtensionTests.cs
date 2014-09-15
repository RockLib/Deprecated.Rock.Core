using System.Reflection;
using NUnit.Framework;
using Rock.Reflection;

namespace Rock.Core.Reflection.IsPublicExtensionTestsTests
{
    public class IsPublicExtensionTests
    {
        [Test]
        public void ReturnsTrueForAPropertyMarkedWithPublic()
        {
            var barProperty = typeof(Foo).GetProperty("Bar");
            Assert.That(barProperty.IsPublic(), Is.True);
        }

        [Test]
        public void ReturnsFalseForAPropertyMarkedWithInternal()
        {
            var bazProperty = typeof(Foo).GetProperty("Baz", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(bazProperty.IsPublic(), Is.False);
        }

        [Test]
        public void ReturnsFalseForAPropertyMarkedWithProtected()
        {
            var quxProperty = typeof(Foo).GetProperty("Qux", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(quxProperty.IsPublic(), Is.False);
        }

        [Test]
        public void ReturnsFalseForAPropertyMarkedWithPrivate()
        {
            var corgeProperty = typeof(Foo).GetProperty("Corge", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(corgeProperty.IsPublic(), Is.False);
        }

        public class Foo
        {
            public string Bar { get; set; }
            internal string Baz { get; set; }
            protected string Qux { get; set; }
            private string Corge { get; set; }
        }
    }
}
