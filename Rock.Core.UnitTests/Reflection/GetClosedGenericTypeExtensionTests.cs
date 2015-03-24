using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rock.Reflection;
using _a;

namespace TheGetClosedGenericTypeExtensionClass
{
    namespace TheGetClosedGenericTypeMethod
    {
        public class ThrowsAnArgumentException
        {
            [Test]
            public void WhenTheTargetTypeParameterIsNull()
            {
                Assert.That(
                    () => ((Type)null).GetClosedGenericType(typeof(IEnumerable<>)),
                    Throws.TypeOf<ArgumentNullException>());
            }

            [Test]
            public void WhenTheTargetTypeParameterIsAGenericTypeDefinition()
            {
                Assert.That(
                    () => typeof(IEnumerable<>).GetClosedGenericType(typeof(IEnumerable<>)),
                    Throws.ArgumentException);
            }

            [Test]
            public void WhenTheOpenGenericTypeParameterIsNull()
            {
                Assert.That(
                    () => typeof(List<int>).GetClosedGenericType(null),
                    Throws.TypeOf<ArgumentNullException>());
            }

            [Test]
            public void WhenTheOpenGenericTypeParameterIsNotAGenericTypeDefinition()
            {
                Assert.That(
                    () => typeof(List<int>).GetClosedGenericType(typeof(IEnumerable<int>)),
                    Throws.ArgumentException);
            }

            [Test]
            public void WhenTheNumberOfTypeArgumentsIsNotEqualToTheNumberOfGenericArguments()
            {
                Assert.That(
                    () => typeof(Dictionary<string, int>).GetClosedGenericType(typeof(IDictionary<,>), new[] { typeof(int) }),
                    Throws.ArgumentException);
            }
        }

        public class ReturnsNull
        {
            [Test]
            public void GivenNoMatchAndTheOpenGenericTypeIsAnInterface()
            {
                Assert.That(typeof(int).GetClosedGenericType(typeof(IFoo<>)), Is.Null);
            }

            [Test]
            public void GivenNoMatchAndTheOpenGenericTypeIsAnAbstractClass()
            {
                Assert.That(typeof(int).GetClosedGenericType(typeof(FooBase<>)), Is.Null);
            }

            [Test]
            public void GivenNoMatchAndTheOpenGenericTypeIsAConcreteClass()
            {
                Assert.That(typeof(int).GetClosedGenericType(typeof(Foo<>)), Is.Null);
            }
        }

        namespace GivenTheTargetIsAnyClosedGenericType
        {
            public class WhenTheOpenGenericTypeIsTheTypeDefinitionOfTheTargetType
            {
                [TestCase(typeof(IFoo<int>), typeof(IFoo<>), TestName = "Arity 1 closed generic interface")]
                [TestCase(typeof(FooBase<int>), typeof(FooBase<>), TestName = "Arity 1 closed generic abstract class")]
                [TestCase(typeof(Foo<int>), typeof(Foo<>), TestName = "Arity 1 closed generic concrete class")]
                [TestCase(typeof(IFoo<int, string>), typeof(IFoo<,>), TestName = "Arity 2 closed generic interface")]
                [TestCase(typeof(FooBase<int, string>), typeof(FooBase<,>), TestName = "Arity 2 closed generic abstract class")]
                [TestCase(typeof(Foo<int, string>), typeof(Foo<,>), TestName = "Arity 2 closed generic concrete class")]
                public void ThenTheResultIsTheSameAsTheTarget(Type targetType, Type openGenericType)
                {
                    AssertOpenGeneric.For(targetType, openGenericType, targetType);
                }
            }
        }

        namespace GivenTheOpenGenericTypeIsAbstract
        {
            public class WhenTheTargetIsAConcreteClosedGenericType
            {
                [TestCase(typeof(FooArity1BaseArity1<int>), typeof(IFoo<>), typeof(IFoo<int>), TestName = "Arity 1 open generic interface")]
                [TestCase(typeof(FooArity1BaseArity1<int>), typeof(FooBase<>), typeof(FooBase<int>), TestName = "Arity 1 open generic abstract class")]
                [TestCase(typeof(FooArity2FooBaseArity2<int, string>), typeof(IFoo<,>), typeof(IFoo<int, string>), TestName = "Arity 2 open generic interface")]
                [TestCase(typeof(FooArity2FooBaseArity2<int, string>), typeof(FooBase<,>), typeof(FooBase<int, string>), TestName = "Arity 2 open generic abstract class")]
                public void ThenTheResultIsAnAbstractClosedGenericType(Type targetType, Type openGenericType, Type expectedClosedGenericType)
                {
                    AssertOpenGeneric.For(targetType, openGenericType, expectedClosedGenericType);
                }
            }

            public class WhenTheTargetIsAConcreteNonGenericType
            {
                [TestCase(typeof(FooArity0FooBaseArity1), typeof(IFoo<>), typeof(IFoo<int>), TestName = "Arity 1 open generic interface")]
                [TestCase(typeof(FooArity0FooBaseArity1), typeof(FooBase<>), typeof(FooBase<int>), TestName = "Arity 1 open generic abstract class")]
                [TestCase(typeof(FooArity0FooBaseArity2), typeof(IFoo<,>), typeof(IFoo<int, string>), TestName = "Arity 2 open generic interface")]
                [TestCase(typeof(FooArity0FooBaseArity2), typeof(FooBase<,>), typeof(FooBase<int, string>), TestName = "Arity 2 open generic abstract class")]
                public void ThenTheResultIsAnAbstractClosedGenericType(Type targetType, Type openGenericType, Type expectedClosedGenericType)
                {
                    AssertOpenGeneric.For(targetType, openGenericType, expectedClosedGenericType);
                }
            }

            public class WhenTheTargetIsAConcreteClosedPartiallyAppliedGenericType
            {
                [TestCase(typeof(FooArity1FooBaseArity2A<string>), typeof(IFoo<,>), typeof(IFoo<int, string>), TestName="Partial generic application variant A interface")]
                [TestCase(typeof(FooArity1FooBaseArity2A<string>), typeof(FooBase<,>), typeof(FooBase<int, string>), TestName = "Partial generic application variant A abstract class")]
                [TestCase(typeof(FooArity1FooBaseArity2B<int>), typeof(IFoo<,>), typeof(IFoo<int, string>), TestName = "Partial generic application variant B interface")]
                [TestCase(typeof(FooArity1FooBaseArity2B<int>), typeof(FooBase<,>), typeof(FooBase<int, string>), TestName = "Partial generic application variant B abstract class")]
                public void ThenTheResultIsAnAbstractClosedGenericType(Type targetType, Type openGenericType, Type expectedClosedGenericType)
                {
                    AssertOpenGeneric.For(targetType, openGenericType, expectedClosedGenericType);
                }
            }

            public class WhenTheTargetIsAnAbstractNonGenericType
            {
                [TestCase(typeof(IFoo), typeof(IFoo<>), typeof(IFoo<int>), TestName = "Arity 1 interface")]
                [TestCase(typeof(FooBase), typeof(FooBase<>), typeof(FooBase<int>), TestName = "Arity 1 abstract class")]
                [TestCase(typeof(IFoo), typeof(IFoo<,>), typeof(IFoo<int, string>), TestName = "Arity 2 interface")]
                [TestCase(typeof(FooBase), typeof(FooBase<,>), typeof(FooBase<int, string>), TestName = "Arity 2 abstract class")]
                public void ThenTheResultIsAnAbstractClosedGenericType(Type targetType, Type openGenericType, Type expectedClosedGenericType)
                {
                    AssertOpenGeneric.For(targetType, openGenericType, expectedClosedGenericType);
                }
            }

            public class WhenTheTargetIsANonGenericInheritingFromMultipleAbstractGenericTypes
            {
                public class AndNoTypeArgumentHintsAreProvided : WhenTheTargetIsANonGenericInheritingFromMultipleAbstractGenericTypes
                {
                    [TestCase(typeof(IMultiFoo), TestName="Interface")]
                    [TestCase(typeof(MultiFooBase), TestName = "Absract class")]
                    [TestCase(typeof(MultiFoo), TestName = "Concrete class")]
                    public void ThenTheResultIsTheFirstAbstractClosedImplementationFound(Type targetType)
                    {
                        Assert.That(targetType.GetClosedGenericType(typeof(IFoo<>)), Is.EqualTo(typeof(IFoo<int>)).Or.EqualTo(typeof(IFoo<string>)));
                    }
                }

                public class AndATypeArgumentHintIsProvided : WhenTheTargetIsANonGenericInheritingFromMultipleAbstractGenericTypes
                {
                    [TestCase(typeof(IMultiFoo), typeof(int), typeof(IFoo<int>), TestName="Interface target and int type argument")]
                    [TestCase(typeof(MultiFooBase), typeof(int), typeof(IFoo<int>), TestName = "Abstract class target and int type argument")]
                    [TestCase(typeof(MultiFoo), typeof(int), typeof(IFoo<int>), TestName = "Concrete class target and int type argument")]
                    [TestCase(typeof(IMultiFoo), typeof(string), typeof(IFoo<string>), TestName = "Interface target and string type argument")]
                    [TestCase(typeof(MultiFooBase), typeof(string), typeof(IFoo<string>), TestName = "Abstract class target and string type argument")]
                    [TestCase(typeof(MultiFoo), typeof(string), typeof(IFoo<string>), TestName = "Concrete class target and string type argument")]
                    public void ThenTheResultIsTheMatchingAbstractClosedGenericType(Type targetType, Type typeArgument, Type expectedClosedGenericType)
                    {
                        Assert.That(targetType.GetClosedGenericType(typeof(IFoo<>), new[] { typeArgument }), Is.EqualTo(expectedClosedGenericType));
                    }
                }
            }
        }
    }
}

namespace _a
{
    public static class AssertOpenGeneric
    {
        public static void For(Type targetType, Type openGenericType, Type expectedClosedGenericType)
        {
            switch (openGenericType.GetGenericArguments().Length)
            {
                case 1:
                    Assert1ArityOpenGeneric(targetType, openGenericType, expectedClosedGenericType);
                    break;
                case 2:
                    Assert2ArityOpenGeneric(targetType, openGenericType, expectedClosedGenericType);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void Assert1ArityOpenGeneric(Type targetType, Type openGenericType, Type expectedClosedGenericType)
        {
            Assert.That(targetType.GetClosedGenericType(openGenericType), Is.EqualTo(expectedClosedGenericType));
            Assert.That(targetType.GetClosedGenericType(openGenericType, new Type[] { null }), Is.EqualTo(expectedClosedGenericType));
            Assert.That(targetType.GetClosedGenericType(openGenericType, new[] { typeof(int) }), Is.EqualTo(expectedClosedGenericType));
        }

        private static void Assert2ArityOpenGeneric(Type targetType, Type openGenericType, Type expectedClosedGenericType)
        {
            Assert.That(targetType.GetClosedGenericType(openGenericType), Is.EqualTo(expectedClosedGenericType));
            Assert.That(targetType.GetClosedGenericType(openGenericType, new Type[] { null, null }), Is.EqualTo(expectedClosedGenericType));
            Assert.That(targetType.GetClosedGenericType(openGenericType, new[] { typeof(int), null }), Is.EqualTo(expectedClosedGenericType));
            Assert.That(targetType.GetClosedGenericType(openGenericType, new[] { null, typeof(string) }), Is.EqualTo(expectedClosedGenericType));
            Assert.That(targetType.GetClosedGenericType(openGenericType, new[] { typeof(int), typeof(string) }), Is.EqualTo(expectedClosedGenericType));
        }
    }

    // ReSharper disable UnusedTypeParameter
    public interface IFoo<T>
    {
    }

    public interface IFoo<T1, T2>
    {
    }

    public interface IFoo : IFoo<int>, IFoo<int, string>
    {
    }

    public abstract class FooBase<T>
    {
    }

    public abstract class FooBase<T1, T2> : FooBase<T1>
    {
    }

    public abstract class FooBase : FooBase<int, string>
    {
    }

    public class Foo<T>
    {
    }

    public class Foo<T1, T2>
    {
    }

    public class FooArity1BaseArity1<T> : FooBase<T>, IFoo<T>
    {
    }

    public class FooArity2FooBaseArity2<T1, T2> : FooBase<T1, T2>, IFoo<T1, T2>
    {
    }

    public class FooArity0FooBaseArity1 : FooBase<int>, IFoo<int>
    {
    }

    public class FooArity1FooBaseArity2A<T> : FooBase<int, T>, IFoo<int, T>
    {
    }

    public class FooArity1FooBaseArity2B<T> : FooBase<T, string>, IFoo<T, string>
    {
    }

    public class FooArity0FooBaseArity2 : FooBase<int, string>, IFoo<int, string>
    {
    }

    public interface IMultiFoo : IFoo<int>, IFoo<string>
    {
    }

    public abstract class MultiFooBase : IFoo<int>, IFoo<string>
    {
    }

    public class MultiFoo : MultiFooBase
    {
    }
    // ReSharper restore UnusedTypeParameter
}