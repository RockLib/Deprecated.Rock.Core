using NUnit.Framework;
using Rock.Reflection;

// ReSharper disable once CheckNamespace
namespace Rock.Core.Reflection.GetConstructorFuncExtensionTests
{
    public class TheNonGenericMethod
    {
        [Test]
        public void ReturnsANewInstanceOfTheSpecifiedType()
        {
            var createFoo = typeof(Foo).GetConstructorFunc();

            var foo = createFoo();

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void WorksWithStructs()
        {
            var createQux = typeof(Qux).GetConstructorFunc();

            var qux = createQux();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void ThrowsWhenTheSpecifiedTypeIsAbstract()
        {
            Assert.That(() => typeof(Bar).GetConstructorFunc(), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsWhenTheSpecifiedTypeHasNoDefaultConstructor()
        {
            Assert.That(() => typeof(Baz).GetConstructorFunc(), Throws.ArgumentException);
        }
    }

    public class TheGenericMethod
    {
        [Test]
        public void ReturnsANewInstanceOfTheSpecifiedType()
        {
            var createFoo = typeof(Foo).GetConstructorFunc<Foo>();

            var foo = createFoo();

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void WorksWithStructs()
        {
            var createQux = typeof(Qux).GetConstructorFunc<Qux>();

            var qux = createQux();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void ThrowsWhenTheSpecifiedTypeIsAbstract()
        {
            Assert.That(() => typeof(Bar).GetConstructorFunc<Bar>(), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsWhenTheSpecifiedTypeIsNotAssignableToTheGenericArg()
        {
            Assert.That(() => typeof(Foo).GetConstructorFunc<Bar>(), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsWhenTheSpecifiedTypeHasNoDefaultConstructor()
        {
            Assert.That(() => typeof(Baz).GetConstructorFunc<Baz>(), Throws.ArgumentException);
        }
    }

    public class Foo
    {
    }

    public abstract class Bar
    {
    }

    public class Baz
    {
        private Baz()
        {
        }
    }

    public struct Qux
    {
    }
}
