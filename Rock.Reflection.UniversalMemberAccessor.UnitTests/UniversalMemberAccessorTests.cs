using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using System.Reflection;

namespace Rock.Reflection.UnitTests
{
    public class UniversalMemberAccessorTests
    {
        [Test]
        public void CanGetBackingInstanceWithInstance()
        {
            var foo = new Foo(123).Unlock();

            var backingFoo = foo.Instance;

            Assert.That(foo, Is.InstanceOf<UniversalMemberAccessor>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CanGetBackingInstanceWithObject()
        {
            var foo = new Foo(123).Unlock();

            var backingFoo = foo.Object;

            Assert.That(foo, Is.InstanceOf<UniversalMemberAccessor>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CanGetBackingInstanceWithValue()
        {
            var foo = new Foo(123).Unlock();

            var backingFoo = foo.Value;

            Assert.That(foo, Is.InstanceOf<UniversalMemberAccessor>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CanGetValueOfPrivateInstanceField()
        {
            var foo = new Foo(123).Unlock();

            int bar = foo._bar;

            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanGetValueOfPrivateInstanceProperty()
        {
            var foo = new Foo(123).Unlock();

            int bar = foo.Bar;

            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateInstanceField()
        {
            var foo = new Foo().Unlock();

            foo._bar = 123;

            Assert.That(foo._bar, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateInstanceProperty()
        {
            var foo = new Foo().Unlock();

            foo.Bar = 123;

            Assert.That(foo.Bar, Is.EqualTo(123));
        }

        [Test]
        public void CanGetValueOfPrivateStaticFieldThroughInstanceAccessor()
        {
            Foo.Reset();

            var foo = new Foo().Unlock();

            int baz = foo._baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanGetValueOfPrivateStaticPropertyThroughInstanceAccessor()
        {
            Foo.Reset();

            var foo = new Foo().Unlock();

            int baz = foo.Baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanSetValueOfPrivateStaticFieldThroughInstanceAccessor()
        {
            var foo = new Foo().Unlock();

            foo._baz = 123;

            Assert.That(foo._baz, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateStaticPropertyThroughInstanceAccessor()
        {
            var foo = new Foo().Unlock();

            foo.Baz = 123;

            Assert.That(foo.Baz, Is.EqualTo(123));
        }

        [Test]
        public void CanGetValueOfPrivateStaticFieldThroughStaticAccessor()
        {
            Foo.Reset();

            var foo = UniversalMemberAccessor.Get<Foo>();

            int baz = foo._baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanGetValueOfPrivateStaticPropertyThroughStaticAccessor()
        {
            Foo.Reset();

            var foo = UniversalMemberAccessor.Get<Foo>();

            int baz = foo.Baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanSetValueOfPrivateStaticFieldThroughStaticAccessor()
        {
            var foo = UniversalMemberAccessor.Get<Foo>();

            foo._baz = 123;

            Assert.That(foo._baz, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateStaticPropertyThroughStaticAccessor()
        {
            var foo = UniversalMemberAccessor.Get<Foo>();

            foo.Baz = 123;

            Assert.That(foo.Baz, Is.EqualTo(123));
        }

        [Test]
        public void CanRegisterAndDeregisterPrivateInstanceEvent()
        {
            var bar = new Bar().Unlock();

            var invocationCount = 0;

            EventHandler eventHandler = (sender, args) => invocationCount++;

            bar.Foo += eventHandler;

            bar.InvokeFoo();

            Assert.That(invocationCount, Is.EqualTo(1));

            bar.Foo -= eventHandler;

            bar.InvokeFoo();

            Assert.That(invocationCount, Is.EqualTo(1));
        }

        [Test]
        public void CanRegisterAndDeregisterPrivateStaticEventThroughInstanceAccessor()
        {
            var bar = new Bar().Unlock();

            var invocationCount = 0;

            EventHandler eventHandler = (sender, args) => invocationCount++;

            bar.Baz += eventHandler;

            Bar.InvokeBaz();

            Assert.That(invocationCount, Is.EqualTo(1));

            bar.Baz -= eventHandler;

            Bar.InvokeBaz();

            Assert.That(invocationCount, Is.EqualTo(1));
        }

        [Test]
        public void CanRegisterAndDeregisterPrivateStaticEventThroughStaticAccessor()
        {
            var bar = UniversalMemberAccessor.Get<Bar>();

            var invocationCount = 0;

            EventHandler eventHandler = (sender, args) => invocationCount++;

            bar.Baz += eventHandler;

            Bar.InvokeBaz();

            Assert.That(invocationCount, Is.EqualTo(1));

            bar.Baz -= eventHandler;

            Bar.InvokeBaz();

            Assert.That(invocationCount, Is.EqualTo(1));
        }

        [Test]
        public void CanIllegallyInvokeEvents()
        {
            var bar = new Bar().Unlock();

            var eventHandler1Count = 0;
            var eventHandler2Count = 0;

            EventHandler eventHandler1 = (sender, args) => eventHandler1Count++;
            EventHandler eventHandler2 = (sender, args) => eventHandler2Count++;

            bar.Qux += eventHandler1;
            bar.Qux += eventHandler2;

            // If this were a bar object, we would get a compiler error here:
            // "The event 'Rock.Reflection.UnitTests.Bar.Qux' can only appear on the left hand side
            // of += or -= (except when used from within the type 'Rock.Reflection.UnitTests.Bar')"

            // What is returned is a delegate, so it can be invoked directly...
            bar.Qux(this, EventArgs.Empty);

            // ...or the delegate's Invoke method can be called.
            bar.Qux.Invoke(this, EventArgs.Empty);

            // You can also assign the event to a delegate variable.
            EventHandler invokingEventHandler = bar.Qux;
            invokingEventHandler(this, EventArgs.Empty);

            Assert.That(eventHandler1Count, Is.EqualTo(3));
            Assert.That(eventHandler2Count, Is.EqualTo(3));
        }

        [Test]
        public void CanCallPrivateInstanceMethods()
        {
            var foo = new Foo().Unlock();

            Assert.That(foo.Qux(123, "abc"), Is.EqualTo("Qux(int i, string s)"));
        }

        [Test]
        public void CanCallPrivateStaticMethodsThroughInstanceAccessor()
        {
            var foo = new Foo().Unlock();

            Assert.That(foo.Grault(123), Is.EqualTo("Grault(int i)"));
        }

        [Test]
        public void CanCallPrivateStaticMethodsThroughStaticAccessor()
        {
            var foo = UniversalMemberAccessor.Get<Foo>();

            Assert.That(foo.Grault(123), Is.EqualTo("Grault(int i)"));
        }

        [Test]
        public void CanResolveOverloadedMethods()
        {
            var foo = new Foo().Unlock();

            Assert.That(foo.Garply(), Is.EqualTo("Garply()"));
            Assert.That(foo.Garply(123), Is.EqualTo("Garply(int i)"));
            Assert.That(foo.Garply("abc"), Is.EqualTo("Garply(string s)"));
            Assert.That(foo.Garply(new Baz()), Is.EqualTo("Garply(IBaz b)"));
            Assert.That(foo.Garply(123, null), Is.EqualTo("Garply(int i, string s)"));
        }

        [Test]
        public void AmbiguousInvocationThrowsRuntimeBinderException()
        {
            var foo = new Foo().Unlock();

            Assert.That(() => foo.Garply(null), Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithNew()
        {
            var quxFactory = UniversalMemberAccessor.Get<Qux>();

            Qux qux = quxFactory.New();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithCreate()
        {
            var quxFactory = UniversalMemberAccessor.Get<Qux>();

            Qux qux = quxFactory.Create();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithNewInstance()
        {
            var quxFactory = UniversalMemberAccessor.Get<Qux>();

            Qux qux = quxFactory.NewInstance();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithCreateInstance()
        {
            var quxFactory = UniversalMemberAccessor.Get<Qux>();

            Qux qux = quxFactory.CreateInstance();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanResolveMultipleConstructors()
        {
            var garplyFactory = UniversalMemberAccessor.Get<Garply>();

            Assert.That(garplyFactory.New().Value, Is.EqualTo("Garply()"));
            Assert.That(garplyFactory.New(123).Value, Is.EqualTo("Garply(int i)"));
            Assert.That(garplyFactory.New("abc").Value, Is.EqualTo("Garply(string s)"));
        }

        [Test]
        public void CanGetAndSetDelegateValue()
        {
            var waldo = new Waldo().Unlock();

            var foo = waldo._foo;

            Assert.That(foo, Is.InstanceOf<EventHandler>());
        }

        [Test]
        public void CanGetAndSetEnumValue()
        {
            var waldo = new Waldo().Unlock();

            var wobble = waldo._wobble;

            Assert.That(wobble, Is.InstanceOf<Waldo.Wobble>());
            Assert.That(wobble, Is.EqualTo(Waldo.Wobble.Wubble));

            waldo._wobble = Waldo.Wobble.Wibble;
            wobble = waldo._wobble;

            Assert.That(wobble, Is.EqualTo(Waldo.Wobble.Wibble));
        }

        [Test]
        public void CanGetPrivateEnumValue()
        {
            var wibbleType = typeof(Waldo).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public).Single(t => t.Name == "Wibble");
            var Wibble = UniversalMemberAccessor.Get(wibbleType);

            // Note that these variables are declared as object. (see below)
            object wubble = Wibble.Wubble;
            object wobble = Wibble.Wobble;

            Assert.That(wubble.GetType(), Is.EqualTo(wibbleType));
            Assert.That(wobble.GetType(), Is.EqualTo(wibbleType));

            // If the wubble and wobble variables had been declared dynamic,
            // then these conversions would fail.
            var wubbleInt = (int)wubble;
            var wobbleInt = (int)wobble;

            Assert.That(wubbleInt, Is.EqualTo(0));
            Assert.That(wobbleInt, Is.EqualTo(1));
        }

        [Test]
        public void CanGetDefaultValueOfPrivateEnum()
        {
            var wibbleType = typeof(Waldo).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public).Single(t => t.Name == "Wibble");
            var Wibble = UniversalMemberAccessor.Get(wibbleType);

            // Note that the variable is declared as object. (see below)
            object defaultWibble = Wibble.New();

            Assert.That(defaultWibble.GetType(), Is.EqualTo(wibbleType));

            // If the defaultWibble variable had been declared dynamic,
            // then this conversion would fail.
            var defaultInt = (int)defaultWibble;

            Assert.That(defaultInt, Is.EqualTo(0));
        }

        [Test]
        public void GetImplicitConvertionMethodsReturnsNoMethodsWhenNoneAreDefined()
        {
            var f = UniversalMemberAccessor.Get<UniversalMemberAccessor>();

            Type parameterType = typeof(Thud);
            Type valueType = typeof(Fred);

            IEnumerable<MethodInfo> implicitConvertionMethods =
                f.GetImplicitConvertionMethods(parameterType, valueType);

            Assert.That(implicitConvertionMethods.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetImplicitConvertionMethodsReturnsOneMethodWhenOneIsDefined()
        {
            var f = UniversalMemberAccessor.Get<UniversalMemberAccessor>();

            Type parameterType = typeof(Fred);
            Type valueType = typeof(Waldo);

            IEnumerable<MethodInfo> implicitConvertionMethods =
                f.GetImplicitConvertionMethods(parameterType, valueType);

            Assert.That(implicitConvertionMethods.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetImplicitConvertionMethodsReturnsTwoMethodsWhenTwoAreDefined()
        {
            var f = UniversalMemberAccessor.Get<UniversalMemberAccessor>();

            Type parameterType = typeof(Fred);
            Type valueType = typeof(Thud);

            IEnumerable<MethodInfo> implicitConvertionMethods =
                f.GetImplicitConvertionMethods(parameterType, valueType);

            Assert.That(implicitConvertionMethods.Count(), Is.EqualTo(2));
        }

        [TestCase(typeof(Pork), typeof(Pork), 0)]
        [TestCase(typeof(Pork), typeof(IPork), 1)]
        [TestCase(typeof(Ham), typeof(Ham), 0)]
        [TestCase(typeof(Ham), typeof(Pork), 1)] // «═╗ Potential
        [TestCase(typeof(Ham), typeof(IHam), 1)] // «═╝ conflict
        [TestCase(typeof(Ham), typeof(IPork), 2)]
        [TestCase(typeof(CountryHam), typeof(CountryHam), 0)]
        [TestCase(typeof(CountryHam), typeof(ICountryHam), 1)] // «═╗ Potential
        [TestCase(typeof(CountryHam), typeof(Ham), 1)] // «═════════╝ conflict
        [TestCase(typeof(CountryHam), typeof(Pork), 2)] // «═╗ Potential
        [TestCase(typeof(CountryHam), typeof(IHam), 2)] // «═╝ conflict
        [TestCase(typeof(CountryHam), typeof(IPork), 3)]
        public void AncestorDistanceIsCalculatedCorrectlyForInterfacesAndClasses(Type type, Type ancestorType, int expectedDistance)
        {
            var candidate =
                UniversalMemberAccessor.Get(
                    "Rock.Reflection.UniversalMemberAccessor+Candidate");

            var distance = candidate.GetAncestorDistance(type, ancestorType);

            Assert.That(distance, Is.EqualTo(expectedDistance));
        }

        [TestCase(typeof(sbyte), typeof(sbyte), 0)]
        [TestCase(typeof(sbyte), typeof(short), 1)]
        [TestCase(typeof(sbyte), typeof(int), 2)]
        [TestCase(typeof(sbyte), typeof(long), 3)]
        [TestCase(typeof(sbyte), typeof(float), 4)]
        [TestCase(typeof(sbyte), typeof(double), 5)]
        [TestCase(typeof(sbyte), typeof(decimal), 6)]

        [TestCase(typeof(byte), typeof(byte), 0)]
        [TestCase(typeof(byte), typeof(short), 1)]
        [TestCase(typeof(byte), typeof(ushort), 2)]
        [TestCase(typeof(byte), typeof(int), 3)]
        [TestCase(typeof(byte), typeof(uint), 4)]
        [TestCase(typeof(byte), typeof(long), 5)]
        [TestCase(typeof(byte), typeof(ulong), 6)]
        [TestCase(typeof(byte), typeof(float), 7)]
        [TestCase(typeof(byte), typeof(double), 8)]
        [TestCase(typeof(byte), typeof(decimal), 9)]

        [TestCase(typeof(short), typeof(short), 0)]
        [TestCase(typeof(short), typeof(int), 1)]
        [TestCase(typeof(short), typeof(long), 2)]
        [TestCase(typeof(short), typeof(float), 3)]
        [TestCase(typeof(short), typeof(double), 4)]
        [TestCase(typeof(short), typeof(decimal), 5)]

        [TestCase(typeof(ushort), typeof(ushort), 0)]
        [TestCase(typeof(ushort), typeof(int), 1)]
        [TestCase(typeof(ushort), typeof(uint), 2)]
        [TestCase(typeof(ushort), typeof(long), 3)]
        [TestCase(typeof(ushort), typeof(ulong), 4)]
        [TestCase(typeof(ushort), typeof(float), 5)]
        [TestCase(typeof(ushort), typeof(double), 6)]
        [TestCase(typeof(ushort), typeof(decimal), 7)]

        [TestCase(typeof(char), typeof(ushort), 1)]
        [TestCase(typeof(char), typeof(int), 2)]
        [TestCase(typeof(char), typeof(uint), 3)]
        [TestCase(typeof(char), typeof(long), 4)]
        [TestCase(typeof(char), typeof(ulong), 5)]
        [TestCase(typeof(char), typeof(float), 6)]
        [TestCase(typeof(char), typeof(double), 7)]
        [TestCase(typeof(char), typeof(decimal), 8)]

        [TestCase(typeof(int), typeof(int), 0)]
        [TestCase(typeof(int), typeof(long), 1)]
        [TestCase(typeof(int), typeof(float), 2)]
        [TestCase(typeof(int), typeof(double), 3)]
        [TestCase(typeof(int), typeof(decimal), 4)]

        [TestCase(typeof(uint), typeof(uint), 0)]
        [TestCase(typeof(uint), typeof(long), 1)]
        [TestCase(typeof(uint), typeof(ulong), 2)]
        [TestCase(typeof(uint), typeof(float), 3)]
        [TestCase(typeof(uint), typeof(double), 4)]
        [TestCase(typeof(uint), typeof(decimal), 5)]

        [TestCase(typeof(long), typeof(long), 0)]
        [TestCase(typeof(long), typeof(float), 1)]
        [TestCase(typeof(long), typeof(double), 2)]
        [TestCase(typeof(long), typeof(decimal), 3)]

        [TestCase(typeof(ulong), typeof(ulong), 0)]
        [TestCase(typeof(ulong), typeof(float), 1)]
        [TestCase(typeof(ulong), typeof(double), 2)]
        [TestCase(typeof(ulong), typeof(decimal), 3)]

        [TestCase(typeof(float), typeof(float), 0)]
        [TestCase(typeof(float), typeof(double), 1)]
        [TestCase(typeof(float), typeof(decimal), 2)]

        [TestCase(typeof(double), typeof(double), 0)]
        [TestCase(typeof(double), typeof(decimal), 1)]

        [TestCase(typeof(decimal), typeof(decimal), 0)]
        public void AncestorDistanceIsCalculatedCorrectlyForNumericTypes(Type type, Type ancestorType, int expectedDistance)
        {
            var candidate =
                UniversalMemberAccessor.Get(
                    "Rock.Reflection.UniversalMemberAccessor+Candidate");

            var distance = candidate.GetAncestorDistance(type, ancestorType);

            Assert.That(distance, Is.EqualTo(expectedDistance));
        }

        [Test]
        public void WhenResolvingMethodsAnExceptionIsThrownWhenTheAncestorDistanceIsTheSame1()
        {
            var spam = new Spam();

            // Demonstrate behavior in dynamic variable that points to a regular object.
            dynamic lockedSpam = spam;

            // Call each method with a good match.
            Assert.That(() => lockedSpam.PublicFoo(new Pork()), Throws.Nothing);
            Assert.That(() => lockedSpam.PublicFoo(new BadActor()), Throws.Nothing);

            // Ambiguous match - Ham has the same ancestor distance to Pork and IHam.
            Assert.That(() => lockedSpam.PublicFoo(new Ham()), Throws.InstanceOf<RuntimeBinderException>());

            // Unlock the object and verify that calling its private methods exhibits identical behavior.
            dynamic unlockedSpam = spam.Unlock();

            Assert.That(() => unlockedSpam.PrivateFoo(new Ham()), Throws.InstanceOf<RuntimeBinderException>());
            Assert.That(() => unlockedSpam.PrivateFoo(new Pork()), Throws.Nothing);
            Assert.That(() => unlockedSpam.PrivateFoo(new BadActor()), Throws.Nothing);
        }

        [Test]
        public void WhenResolvingMethodsAnExceptionIsThrownWhenTheAncestorDistanceIsTheSame2()
        {
            var spam = new Spam();

            // Demonstrate behavior in dynamic variable that points to a regular object.
            dynamic lockedSpam = spam;

            Assert.That(() => lockedSpam.PublicFoo(new CountryHam()), Throws.InstanceOf<RuntimeBinderException>());
            Assert.That(() => lockedSpam.PublicFoo(new Pork()), Throws.Nothing);
            Assert.That(() => lockedSpam.PublicFoo(new BadActor()), Throws.Nothing);

            // Unlock the object and verify that calling its private methods exhibits identical behavior.
            dynamic unlockedSpam = new Spam().Unlock();

            Assert.That(() => unlockedSpam.PrivateFoo(new CountryHam()), Throws.InstanceOf<RuntimeBinderException>());
            Assert.That(() => unlockedSpam.PrivateFoo(new Pork()), Throws.Nothing);
            Assert.That(() => unlockedSpam.PrivateFoo(new BadActor()), Throws.Nothing);
        }

        [Test]
        public void WhenResolvingMethodsAnExceptionIsThrownWhenTheAncestorDistanceIsTheSame3()
        {
            var spam = new Spam();

            // Demonstrate behavior in dynamic variable that points to a regular object.
            dynamic lockedSpam = spam;

            Assert.That(() => lockedSpam.PublicBar(new CountryHam()), Throws.InstanceOf<RuntimeBinderException>());
            Assert.That(() => lockedSpam.PublicBar(new Ham()), Throws.Nothing);
            Assert.That(() => lockedSpam.PublicBar(new Prosciutto()), Throws.Nothing);

            // Unlock the object and verify that calling its private methods exhibits identical behavior.
            dynamic unlockedSpam = new Spam().Unlock();

            Assert.That(() => unlockedSpam.PrivateBar(new CountryHam()), Throws.InstanceOf<RuntimeBinderException>());
            Assert.That(() => unlockedSpam.PrivateBar(new Ham()), Throws.Nothing);
            Assert.That(() => unlockedSpam.PrivateBar(new Prosciutto()), Throws.Nothing);
        }

        public interface IPork { }
        public interface IHam : IPork { }
        public interface ICountryHam : IHam { }

        public class Pork : IPork { }
        public class Ham : Pork, IHam { }
        public class CountryHam : Ham, ICountryHam { }

        public class BadActor : IHam { }
        public class Prosciutto : ICountryHam { }

        public class Spam
        {
            private string PrivateFoo(Pork pork) { return null; }
            private string PrivateFoo(IHam ham) { return null; }
            public string PublicFoo(Pork pork) { return null; }
            public string PublicFoo(IHam ham) { return null; }

            private string PrivateBar(ICountryHam countryHam) { return null; }
            private string PrivateBar(Ham ham) { return null; }
            public string PublicBar(ICountryHam countryHam) { return null; }
            public string PublicBar(Ham ham) { return null; }
        }
    }

    // ReSharper disable UnusedParameter.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable ConvertToAutoProperty
    // ReSharper disable EventNeverSubscribedTo.Local
    public class Foo
    {
        private int _bar;
        private static int _baz = -1;

        public Foo()
        {
        }

        public Foo(int bar)
        {
            _bar = bar;
        }

        protected int Bar { get { return _bar; } set { _bar = value; } }
        protected static int Baz { get { return _baz; } set { _baz = value; } }

        public static void Reset()
        {
            _baz = -1;
        }

        private string Qux(int i, string s)
        {
            return "Qux(int i, string s)";
        }

        private string Garply()
        {
            return "Garply()";
        }

        private string Garply(int i)
        {
            return "Garply(int i)";
        }

        private string Garply(string s)
        {
            return "Garply(string s)";
        }

        private string Garply(IBaz b)
        {
            return "Garply(IBaz b)";
        }

        private string Garply(int i, string s)
        {
            return "Garply(int i, string s)";
        }

        private static string Grault(int i)
        {
            return "Grault(int i)";
        }

        public void Fred(object o)
        {
        }

        public void Fred(IBaz b)
        {
        }

        public void Fred(Baz b)
        {
        }
    }

    public class Bar
    {
        private event EventHandler Foo;
        private static event EventHandler Baz;
        public event EventHandler Qux;
        
        public void InvokeFoo()
        {
            var handler = Foo;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public static void InvokeBaz()
        {
            var handler = Baz;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }
    }

    public interface IBaz
    {
    }

    public class Baz : IBaz
    {
    }

    public class AnotherBaz : IBaz
    {
    }

    public class Qux
    {
        private Qux()
        {
        }
    }

    public class Garply
    {
        public readonly string Value;

        private Garply()
        {
            Value = "Garply()";
        }

        private Garply(int i)
        {
            Value = "Garply(int i)";
        }

        private Garply(string s)
        {
            Value = "Garply(string s)";
        }
    }

    public static class Grault
    {
    }

    public class Fred
    {
        public static implicit operator Fred(Waldo x)
        {
            return new Fred();
        }

        // Dulicate implicit conversion
        public static implicit operator Fred(Thud x)
        {
            return new Fred();
        }
    }

    public class Waldo
    {
        private EventHandler _foo = FooHandler;
        private Wobble _wobble;

        public enum Wobble
        {
            Wubble,
            Wibble
        }

        private enum Wibble
        {
            Wubble,
            Wobble
        }

        private static void FooHandler(object sender, EventArgs args)
        {
        }
    }

    public class Thud
    {
        // Dulicate implicit conversion
        public static implicit operator Fred(Thud x)
        {
            return new Fred();
        }
    }
    // ReSharper restore EventNeverSubscribedTo.Local
    // ReSharper restore ConvertToAutoProperty
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}
