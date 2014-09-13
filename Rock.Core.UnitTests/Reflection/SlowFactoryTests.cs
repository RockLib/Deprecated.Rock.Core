using NUnit.Framework;
using Rock.Reflection;

namespace Rock.Core.Reflection.SlowFactoryTests
{
    public class TheCreateInstanceMethod
    {
        [Test]
        public void ReturnsAnInstanceOfATypeThatHasAPublicParameterlessConstructor()
        {
            var assemblyQualifiedName = typeof(Foo).AssemblyQualifiedName;

            Assert.That(SlowFactory.CreateInstance<IFoo>(assemblyQualifiedName), Is.InstanceOf<Foo>());
        }

        [Test]
        public void ReturnsAnInstanceOfATypeThatHasAPublicConstructorWhoseParametersAllHaveADefaultValue()
        {
            var assemblyQualifiedName = typeof(Bar).AssemblyQualifiedName;

            Assert.That(SlowFactory.CreateInstance<IBar>(assemblyQualifiedName), Is.InstanceOf<Bar>());
        }

        [Test]
        public void ThrowsAnExceptionWhenTheAssemblyQualifiedNameCanNotBeLocated()
        {
            const string assemblyQualifiedName = "this string is not the assembly qualified name of any type";

            Assert.That(
                () => SlowFactory.CreateInstance<IFoo>(assemblyQualifiedName),
                Throws.Exception.Message.ContainsSubstring("Unable to locate a type with the name of"));
        }

        [Test]
        public void ThrowsAnExceptionWhenTheTypeIsAbstract()
        {
            var assemblyQualifiedName = typeof(IFoo).AssemblyQualifiedName;

            Assert.That(
                () => SlowFactory.CreateInstance<IFoo>(assemblyQualifiedName),
                Throws.Exception.Message.ContainsSubstring("Unable to create instance of abstract type"));
        }

        [Test]
        public void ThrowsAnExceptionWhenTheTypeIsNotAssignableToTheGenericTypeArgument()
        {
            var assemblyQualifiedName = typeof(Foo).AssemblyQualifiedName;

            Assert.That(
                () => SlowFactory.CreateInstance<IBar>(assemblyQualifiedName),
                Throws.Exception.Message.ContainsSubstring("type is not assignable to the"));
        }

        [Test]
        public void ThrowsAnExceptionWhenTheTypeDoesNotHaveAPublicParameterlessConstructorOrAConstructorWhoseParametersAllHaveADefaultValue()
        {
            var assemblyQualifiedName = typeof(Baz).AssemblyQualifiedName;

            Assert.That(
                () => SlowFactory.CreateInstance<IBaz>(assemblyQualifiedName),
                Throws.Exception.Message.ContainsSubstring("Unable to find suitable constructor for"));
        }
    }

    public interface IFoo
    {
    }

    public class Foo : IFoo
    {
    }

    public interface IBar
    {
    }

    public class Bar : IBar
    {
        public Bar(int i = 0, string j = "abc", object o = null)
        {
        }
    }

    public interface IBaz
    {
    }

    public class Baz : IBaz
    {
        public Baz(int i, string j, object o)
        {
        }
    }
}
