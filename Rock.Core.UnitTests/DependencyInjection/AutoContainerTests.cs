using System;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Rock.Defaults.Implementation;
using Rock.DependencyInjection;
using Rock.DependencyInjection.Heuristics;

// ReSharper disable once CheckNamespace
namespace AutoContainerTests
{
    public class AutoContainerTests
    {
        public class TheConstructorWithoutAnIConstructorSelectorParameter
        {
            [Test]
            public void PassesTheDefaultConstructorSelectorToTheOtherPublicConstructor()
            {
                try
                {
                    var mockConstructorSelector = new Mock<IConstructorSelector>();

                    Default.SetConstructorSelector(() => mockConstructorSelector.Object);

                    var container = new AutoContainer();
                    container.CanGet(typeof(Foo)); // This method ends up accessing the constructor selector.

                    ConstructorInfo dummy;
                    mockConstructorSelector.Verify(
                        m => m.TryGetConstructor(It.Is<Type>(t => t == typeof(Foo)), It.IsAny<IResolver>(), out dummy));
                }
                finally
                {
                    Default.RestoreDefaultConstructorSelector();
                }
            }

            [Test]
            public void PassesTheInstancesCollectionToTheOtherPublicConstructor()
            {
                var container = new AutoContainer(new Foo(0));

                Assert.That(container.CanGet(typeof(Foo)), Is.True);
            }
        }

        public class TheCanGetMethod
        {
            [Test]
            public void ReturnsTrueWhenTheTypeRequestedIsTheTypeOfAConcreteInstance()
            {
                // Foo and AnotherFoo implement IFoo and Foo and YetAnotherFoo inherit FooBase,
                // making IFoo and FooBase ineligible. Nothing else in Foo's type hierarchy
                // other than the Foo type itself is still eligible. And Foo itself cannot be
                // resolved because of its constructor. So the only way we'll be able to get a
                // Foo from the container is by asking for a Foo.
                var container = new AutoContainer(new Foo(0), new AnotherFoo(), new YetAnotherFoo());

                var result = container.CanGet(typeof(Foo));

                Assert.That(result, Is.True);
            }

            [Test]
            public void ReturnsTrueWhenTheTypeRequestedIsTheBaseTypeOfTheConcreteInstance()
            {
                var container = new AutoContainer(new Foo(0));

                var result = container.CanGet(typeof(FooBase));

                Assert.That(result, Is.True);
            }

            [Test]
            public void ReturnsTrueWhenTheTypeRequestedIsAnInterfaceThatTheConcreteInstanceInheritsFrom()
            {
                var container = new AutoContainer(new Foo(0));

                var result = container.CanGet(typeof(IFoo));

                Assert.That(result, Is.True);
            }

            [Test]
            public void ReturnsFalseWhenTheTypeRequestedIsObject()
            {
                var conatiner = new AutoContainer(new Foo(0));

                var result = conatiner.CanGet(typeof(object));

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseWhenTheTypeRequestedIsEntirelyUnrelatedToAnyOfTheConcreteInstances()
            {
                var container = new AutoContainer(new Foo(0));

                var result = container.CanGet(typeof(IIsNotIFoo));

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseWhenTheTypeRequestedTypeHasBeenNullified()
            {
                // Foo and AnotherFoo both implement IFoo, so IFoo becomes ineligible.
                var container = new AutoContainer(new Foo(0), new AnotherFoo());

                var result = container.CanGet(typeof(IFoo));

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseWhenTheTypeRequestedHasNoResolvableConstructors()
            {
                var container = new AutoContainer();

                var result = container.CanGet(typeof(NeedsAnIFoo));

                Assert.That(result, Is.False);
            }
        }

        public abstract class GetMethodBase
        {
            protected AutoContainer _container;

            protected abstract T Get<T>();

            [Test]
            public void ThrowsAnExceptionWhenTheCanGetMethodReturnsFalse()
            {
                _container = new AutoContainer();

                Assert.That(() => Get<NeedsAnIFoo>(), Throws.Exception);
            }

            [Test]
            public void WhenTheRequestedTypeResolvesToOneOfTheConcreteInstancesThenTheSameInstanceIsReturnedEveryTime()
            {
                _container = new AutoContainer(new Foo(0));

                var first = Get<IFoo>();
                var second = Get<IFoo>();

                Assert.That(first, Is.SameAs(second));
            }

            [Test]
            public void WhenTheRequestedTypeIsResolvableButNotToOneOfTheConcreteInstancesThenANewInstanceIsReturnedEveryTime()
            {
                _container = new AutoContainer(new Foo(0));

                var first = Get<NeedsAnIFoo>();
                var second = Get<NeedsAnIFoo>();

                Assert.That(first, Is.Not.SameAs(second));
            }
        }

        public class TheGenericGetMethod : GetMethodBase
        {
            protected override T Get<T>()
            {
                return _container.Get<T>();
            }
        }

        public class TheNonGenericGetMethod : GetMethodBase
        {
            protected override T Get<T>()
            {
                return (T)_container.Get(typeof(T));
            }
        }

        public class TheAutoContainerReturnedByTheMergeMethod
        {
            public class HasACanGetMethod
            {
                [Test]
                public void ThatReturnsTrueIfTheCanGetMethodOfTheBaseAutoContainerReturnsTrue()
                {
                    var foo = new Foo(0);

                    var primary = new AutoContainer(foo);
                    var secondary = new AutoContainer();

                    // Sanity checks
                    Assert.That(primary.CanGet(typeof(NeedsAnIFoo)), Is.True);
                    Assert.That(secondary.CanGet(typeof(NeedsAnIFoo)), Is.False);

                    var merged = primary.MergeWith(secondary);

                    var result = merged.CanGet(typeof(NeedsAnIFoo));

                    Assert.That(result, Is.True);
                }

                [Test]
                public void ThatReturnsTrueIfTheCanGetMethodOfTheSecondaryResolverReturnsTrue()
                {
                    var foo = new Foo(0);

                    var primary = new AutoContainer();
                    var secondary = new AutoContainer(foo);

                    // Sanity checks
                    Assert.That(primary.CanGet(typeof(NeedsAnIFoo)), Is.False);
                    Assert.That(secondary.CanGet(typeof(NeedsAnIFoo)), Is.True);

                    var merged = primary.MergeWith(secondary);

                    var result = merged.CanGet(typeof(NeedsAnIFoo));

                    Assert.That(result, Is.True);
                }

                [Test]
                public void ThatReturnsFalseIfTheCanGetMethodFromBothTheBaseAutoContainerAndSecondaryResolverReturnFalse()
                {
                    var primary = new AutoContainer();
                    var secondary = new AutoContainer();

                    // Sanity checks
                    Assert.That(primary.CanGet(typeof(NeedsAnIFoo)), Is.False);
                    Assert.That(secondary.CanGet(typeof(NeedsAnIFoo)), Is.False);

                    var merged = primary.MergeWith(secondary);

                    var result = merged.CanGet(typeof(NeedsAnIFoo));

                    Assert.That(result, Is.False);
                }
            }

            public abstract class MergedGetMethodBase
            {
                protected AutoContainer _mergedContainer;

                protected abstract T Get<T>();

                [Test]
                public void ThatReturnsWhatTheBaseAutoContainerReturnsIfTheBaseAutoContainerCanResolve()
                {
                    var foo = new Foo(0);
                    var bar = new Bar();
                    var anotherFoo = new AnotherFoo();

                    var primary = new AutoContainer(foo);
                    var secondary = new AutoContainer(bar, anotherFoo);

                    _mergedContainer = primary.MergeWith(secondary);

                    var result = Get<NeedsAnIFooAndAnIBar>();

                    Assert.That(result.TheFoo, Is.SameAs(foo));
                    Assert.That(result.TheFoo, Is.Not.SameAs(anotherFoo));
                }

                [Test]
                public void ThatReturnsWhatTheSecondaryResolverReturnsIfTheBaseAutoContainerCannotResolve()
                {
                    var foo = new Foo(0);
                    var bar = new Bar();
                    var anotherFoo = new AnotherFoo();

                    var primary = new AutoContainer(foo);
                    var secondary = new AutoContainer(bar, anotherFoo);

                    _mergedContainer = primary.MergeWith(secondary);

                    var result = Get<NeedsAnIFooAndAnIBar>();

                    Assert.That(result.TheBar, Is.SameAs(bar));
                }

                [Test]
                public void ThatThrowsAnExceptionIfNeitherTheBaseAutoContainerNorTheSecondaryResolverCanResolve()
                {
                    var foo = new Foo(0);
                    var anotherFoo = new AnotherFoo();

                    var primary = new AutoContainer(foo);
                    var secondary = new AutoContainer(anotherFoo);

                    _mergedContainer = primary.MergeWith(secondary);

                    Assert.That(() => Get<NeedsAnIFooAndAnIBar>(), Throws.Exception);
                }
            }

            public class HasAGenericGetMethod : MergedGetMethodBase
            {
                protected override T Get<T>()
                {
                    return _mergedContainer.Get<T>();
                }
            }

            public class HasANonGenericGetMethod : MergedGetMethodBase
            {
                protected override T Get<T>()
                {
                    return (T)_mergedContainer.Get(typeof(T));
                }
            }
        }

        // ReSharper disable UnusedParameter.Local
        public interface IFoo
        {
        }

        public abstract class FooBase
        {
        }

        public class Foo : FooBase, IFoo
        {
            public Foo(object x)
            {
            }
        }

        public class AnotherFoo : IFoo
        {
        }

        public class YetAnotherFoo : FooBase
        {
        }

        public interface IIsNotIFoo
        {
        }

        public class NeedsAnIFoo
        {
            public NeedsAnIFoo(IFoo foo)
            {
            }
        }

        public interface IBar
        {
        }

        public class Bar : IBar
        {
        }

        public class NeedsAnIFooAndAnIBar
        {
            public NeedsAnIFooAndAnIBar(IFoo foo, IBar bar)
            {
                TheFoo = foo;
                TheBar = bar;
            }

            public IFoo TheFoo { get; private set; }
            public IBar TheBar { get; private set; }
        }
        // ReSharper restore UnusedParameter.Local
    }
}