using System;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Rock.DependencyInjection;
using Rock.DependencyInjection.Heuristics;

// ReSharper disable once CheckNamespace
namespace ConstructorSelectorTests
{
    public class ConstructorSelectorTests
    {
        protected Mock<IResolver> MockResolver { get; private set; }

        [SetUp]
        public void Setup()
        {
            MockResolver = new Mock<IResolver>();
        }

        protected void SetResolvable(Type type)
        {
            MockResolver
                .Setup(m => m.CanGet(It.Is<Type>(t => t == type)))
                .Returns(true);
        }

        public class TheTryGetConstructorMethod : ConstructorSelectorTests
        {
            [Test]
            public void ReturnsTrueWhenAResolvableConstructorExists()
            {
                var sut = new ConstructorSelector();

                ConstructorInfo dummy;
                Assert.That(sut.TryGetConstructor(typeof(Class5), MockResolver.Object, out dummy), Is.True);
            }

            [Test]
            public void SetsTheOutParameterToTheResolvableConstructorWhenOneExists()
            {
                var sut = new ConstructorSelector();

                ConstructorInfo ctor;
                sut.TryGetConstructor(typeof(Class5), MockResolver.Object, out ctor);

                Assert.That(ctor, Is.Not.Null);
            }

            [Test]
            public void ReturnsFalseWhenAResolvableConstructorDoesNotExist()
            {
                var sut = new ConstructorSelector();

                ConstructorInfo dummy;
                Assert.That(sut.TryGetConstructor(typeof(Class1), MockResolver.Object, out dummy), Is.False);
            }

            [Test]
            public void SetsTheOutParameterToNullWhenNoResolvableConstructorExists()
            {
                var sut = new ConstructorSelector();

                ConstructorInfo ctor;
                sut.TryGetConstructor(typeof(Class1), MockResolver.Object, out ctor);

                Assert.That(ctor, Is.Null);
            }

            [TestCase(typeof(AbstractClass))]
            [TestCase(typeof(IInterface1))]
            public void ReturnsFalseIfTheTypeIsAbstract(Type abstractType)
            {
                var sut = new ConstructorSelector();

                ConstructorInfo ctor;
                var result = sut.TryGetConstructor(abstractType, MockResolver.Object, out ctor);

                Assert.That(result, Is.False);
            }

            [Test]
            public void ChoosesTheResolvableConstructorWhenThereAreMoreThanOnceResolvableConstructorsAndOnlyOneIsResolvable()
            {
                SetResolvable(typeof(IInterface1));
                SetResolvable(typeof(IInterface2));

                var sut = new ConstructorSelector();

                ConstructorInfo ctor;
                sut.TryGetConstructor(typeof(Class1), MockResolver.Object, out ctor);

                Assert.That(ctor.GetParameters().All(p => MockResolver.Object.CanGet(p.ParameterType) || p.HasDefaultValue), Is.True);
            }

            [Test]
            public void ChoosesTheResolvableConstructorWithTheMostParameters()
            {
                SetResolvable(typeof(IInterface1));
                SetResolvable(typeof(IInterface2));
                SetResolvable(typeof(IInterface3));

                var sut = new ConstructorSelector();

                var ctor = sut.GetConstructor(typeof(Class2), MockResolver.Object);

                Assert.That(ctor.GetParameters().Length, Is.EqualTo(2));
            }

            [Test]
            public void NeverSelectsAConstructorWithANonDefaultPrimitivishParameter()
            {
                SetResolvable(typeof(IInterface1));
                SetResolvable(typeof(IInterface2));
                SetResolvable(typeof(IInterface3));

                var sut = new ConstructorSelector();

                var ctor = sut.GetConstructor(typeof(Class3), MockResolver.Object);

                Assert.That(ctor.GetParameters().Length, Is.EqualTo(1));
            }

            public class GivenTwoResolvableConstructorsWithTheSameNumberOfParameters : ConstructorSelectorTests
            {
                [Test]
                public void IfNoParametersAreDefaultOnEitherReturnsFalse()
                {
                    SetResolvable(typeof(IInterface1));
                    SetResolvable(typeof(IInterface2));
                    SetResolvable(typeof(IInterface3));
                    SetResolvable(typeof(IInterface4));

                    var sut = new ConstructorSelector();

                    ConstructorInfo dummy;
                    Assert.That(sut.TryGetConstructor(typeof(Class1), MockResolver.Object, out dummy), Is.False);
                }

                [Test]
                public void TheConstructorWithTheFewestParametersWithDefaultValuesIsSelected()
                {
                    SetResolvable(typeof(IInterface1));
                    SetResolvable(typeof(IInterface2));
                    SetResolvable(typeof(IInterface3));
                    SetResolvable(typeof(IInterface4));

                    var sut = new ConstructorSelector();

                    ConstructorInfo ctor;
                    sut.TryGetConstructor(typeof(Class4), MockResolver.Object, out ctor);

                    Assert.That(ctor.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(IInterface1), typeof(IInterface2) }));
                }
            }
        }

        protected interface IInterface1 {}
        protected interface IInterface2 {}
        protected interface IInterface3 {}
        protected interface IInterface4 {}

        protected class Class1
        {
            public Class1(IInterface1 p1, IInterface2 p2) {}
            public Class1(IInterface3 p1, IInterface4 p2) {}
        }

        protected class Class2
        {
            public Class2(IInterface1 p1, IInterface2 p2) {}
            public Class2(IInterface3 p1) { }
        }

        protected class Class3
        {
            public Class3(IInterface1 p1, IInterface2 p2, int p3) {}
            public Class3(IInterface3 p1) {}
        }

        protected class Class4
        {
            public Class4(IInterface1 p1, IInterface2 p2) {}
            public Class4(IInterface3 p1, IInterface4 p2 = null) {}
        }

        protected class Class5
        {
        }

        protected abstract class AbstractClass
        {
        }
    }
}