using System;
using NUnit.Framework;
using Rock.Reflection;

// ReSharper disable once CheckNamespace
namespace Rock.Core.Reflection.GetSetActionExtensionTests
{
    public class TheNonGenericMethod
    {
        [Test]
        public void SuccessfullySetsTheTargetProperty()
        {
            var barProperty = typeof(Foo).GetProperty("Bar");
            var setBar = barProperty.GetSetAction();

            var foo = new Foo();
            setBar(foo, "abc");

            Assert.That(foo.Bar, Is.EqualTo("abc"));
        }

        [Test]
        public void ThrowsWhenTheInstanceParameterIsAnInvalidType()
        {
            var barProperty = typeof(Foo).GetProperty("Bar");
            var setBar = barProperty.GetSetAction();

            var baz = new Baz();
            
            Assert.That(() => setBar(baz, "abc"), Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void ThrowsWhenTheValueParameterIsAnInvalidType()
        {
            var barProperty = typeof(Foo).GetProperty("Bar");
            var setBar = barProperty.GetSetAction();

            var foo = new Foo();

            Assert.That(() => setBar(foo, 123), Throws.InstanceOf<InvalidCastException>());
            Assert.That(foo.Bar, Is.Null);
        }
    }

    public class TheGenericMethod
    {
        [Test]
        public void SuccessfullySetsTheTargetProperty()
        {
            var barProperty = typeof(Foo).GetProperty("Bar");
            var setBar = barProperty.GetSetAction<Foo, string>();

            var foo = new Foo();
            setBar(foo, "abc");

            Assert.That(foo.Bar, Is.EqualTo("abc"));
        }
    }

    public class Foo
    {
        public string Bar { get; set; }
    }

    public class Baz
    {
    }
}
