using System;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

namespace Rock.Reflection.UnitTests
{
    public class UniversalMemberAccessorTests
    {
        [Test]
        public void CanGetValueOfPrivateInstanceField()
        {
            var foo = new Foo(123).UnlockNonPublicMembers();

            int bar = foo._bar;

            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanGetValueOfPrivateInstanceProperty()
        {
            var foo = new Foo(123).UnlockNonPublicMembers();

            int bar = foo.Bar;

            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateInstanceField()
        {
            var foo = new Foo().UnlockNonPublicMembers();

            foo._bar = 123;

            Assert.That(foo._bar, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateInstanceProperty()
        {
            var foo = new Foo().UnlockNonPublicMembers();

            foo.Bar = 123;

            Assert.That(foo.Bar, Is.EqualTo(123));
        }

        [Test]
        public void CanGetValueOfPrivateStaticFieldThroughInstanceAccessor()
        {
            Foo.Reset();

            var foo = new Foo().UnlockNonPublicMembers();

            int baz = foo._baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanGetValueOfPrivateStaticPropertyThroughInstanceAccessor()
        {
            Foo.Reset();

            var foo = new Foo().UnlockNonPublicMembers();

            int baz = foo.Baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanSetValueOfPrivateStaticFieldThroughInstanceAccessor()
        {
            var foo = new Foo().UnlockNonPublicMembers();

            foo._baz = 123;

            Assert.That(foo._baz, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateStaticPropertyThroughInstanceAccessor()
        {
            var foo = new Foo().UnlockNonPublicMembers();

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
            var bar = new Bar().UnlockNonPublicMembers();

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
            var bar = new Bar().UnlockNonPublicMembers();

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
        public void CanCallPrivateMethod()
        {
            var foo = new Foo().UnlockNonPublicMembers();

            Assert.That(foo.Qux(123, "abc"), Is.EqualTo("Qux(int i, string s)"));
        }

        [Test]
        public void CanResolveOverloadedMethods()
        {
            var foo = new Foo().UnlockNonPublicMembers();

            Assert.That(foo.Garply(), Is.EqualTo("Garply()"));
            Assert.That(foo.Garply(123), Is.EqualTo("Garply(int i)"));
            Assert.That(foo.Garply("abc"), Is.EqualTo("Garply(string s)"));
            Assert.That(foo.Garply(new Baz()), Is.EqualTo("Garply(IBaz b)"));
        }

        [Test]
        public void AmbiguousInvocationThrowsRuntimeBinderException()
        {
            var foo = new Foo().UnlockNonPublicMembers();

            Assert.That(() => foo.Garply(null), Throws.InstanceOf<RuntimeBinderException>());
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
    }

    public class Bar
    {
        private event EventHandler Foo;
        private static event EventHandler Baz;
        
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
    // ReSharper restore EventNeverSubscribedTo.Local
    // ReSharper restore ConvertToAutoProperty
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}
