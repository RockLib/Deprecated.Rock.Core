using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using System.Reflection;
using RockLib.Dynamic.UnitTests.TypeCreator;
using UMA=RockLib.Dynamic.UniversalMemberAccessor;

namespace RockLib.Dynamic.UnitTests
{
    public class UniversalMemberAccessorTests
    {
        [Test]
        public void UnlockExtensionMethodAndGetMethodReturnTheSameObject()
        {
            var type = Create.Class();

            var instance = Activator.CreateInstance(type);

            var unlock = instance.Unlock();
            var get = UniversalMemberAccessor.Get(instance);

            Assert.That(unlock, Is.SameAs(get));
        }

        [Test]
        public void UnlockExtensionMethodAndGetStaticMethodReturnTheSameObject()
        {
            var type = Create.Class();

            var unlock = type.Unlock();
            var getStatic = UniversalMemberAccessor.GetStatic(type);

            Assert.That(unlock, Is.SameAs(getStatic));
        }

        [Test]
        public void GetDynamicMemberNamesReturnsAllTheMemberNamesOfTheType()
        {
            var type = Create.Class();

            var instance = type.New();

            var allMemberNames = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => !(m is ConstructorInfo))
                .Select(m => m.Name);

            var dynamicMemberNames = instance.GetDynamicMemberNames();

            Assert.That(dynamicMemberNames, Is.EqualTo(allMemberNames));
        }

        [Test]
        public void CannotCallGetStaticWithNullType()
        {
            Assert.That(() => UMA.GetStatic((Type)null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void CanGetBackingInstanceWithInstance()
        {
            var foo = new Foo(123).Unlock();

            var backingFoo = foo.Instance;

            Assert.That(foo, Is.InstanceOf<UMA>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CanGetBackingInstanceWithObject()
        {
            var foo = new Foo(123).Unlock();

            var backingFoo = foo.Object;

            Assert.That(foo, Is.InstanceOf<UMA>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CanGetBackingInstanceWithValue()
        {
            var foo = new Foo(123).Unlock();

            var backingFoo = foo.Value;

            Assert.That(foo, Is.InstanceOf<UMA>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CanImplicitlyConvert()
        {
            var foo = new Foo(123).Unlock();

            Foo backingFoo = foo;

            Assert.That(foo, Is.InstanceOf<UMA>());
            Assert.That(backingFoo, Is.InstanceOf<Foo>());
        }

        [Test]
        public void CannotImplicitlyConvertToWrongType()
        {
            var foo = new Foo(123).Unlock();

            Bar bar;

            Assert.That(() => bar = foo,
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "Cannot implicitly convert type 'RockLib.Dynamic.UnitTests.Foo' to 'RockLib.Dynamic.UnitTests.Bar'"));
        }

        [Test]
        public void CannotGetBackingInstanceFromStaticAccessorWithInstance()
        {
            var foo = UMA.GetStatic<Foo>();

            Assert.That(() => foo.Instance, Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void CannotGetBackingInstanceFromStaticAccessorWithObject()
        {
            var foo = UMA.GetStatic<Foo>();

            Assert.That(() => foo.Object, Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void CannotGetBackingInstanceFromStaticAccessorWithValue()
        {
            var foo = UMA.GetStatic<Foo>();

            Assert.That(() => foo.Value, Throws.InstanceOf<RuntimeBinderException>());
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
        public void CannotSetValueOfPropertyThatDoesNotExist()
        {
            var foo = new Foo().Unlock();

            Assert.That(() => foo.DoesNotExist = "abc",
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "'RockLib.Dynamic.UniversalMemberAccessor' does not contain a definition for 'DoesNotExist'"));
        }

        [Test]
        public void CannotSetWrongTypeForField()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(int)));

            var foo = type.New();

            Assert.That(() => foo._bar = "abc",
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "Cannot implicitly convert type 'System.String' to 'System.Int32'"));
        }

        [Test]
        public void CannotSetWrongTypeForProperty()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(int)));

            var foo = type.New();

            Assert.That(() => foo.Bar = "abc",
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "Cannot implicitly convert type 'System.String' to 'System.Int32'"));
        }

        [Test]
        public void CannotSetNullForNonNullableValueTypeField()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(int)));

            var foo = type.New();

            Assert.That(() => foo._bar = null,
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "Cannot convert null to 'System.Int32' because it is a non-nullable value type"));
        }

        [Test]
        public void CannotSetNullForNonNullableValueTypeProperty()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(int)));

            var foo = type.New();

            Assert.That(() => foo.Bar = null,
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "Cannot convert null to 'System.Int32' because it is a non-nullable value type"));
        }

        [Test]
        public void CanSetFieldValueWhenValueIsWrappedInUniversalMemberAccessor()
        {
            var barType = Create.Class("Bar", Define.Field("_baz", typeof(int)));
            var fooType = Create.Class("Foo", Define.Field("_bar", barType));

            var bar = barType.New();
            bar._baz = 123;

            var foo = fooType.New();
            foo._bar = bar;

            int baz = foo._bar._baz;

            Assert.That(baz, Is.EqualTo(123));
        }

        [Test]
        public void CanCreateInstanceWhenConstructorArgIsUniversalMemberAccessor()
        {
            var barType = Create.Class("Bar", Define.Field("_baz", typeof(int)),
                Define.Constructor(new Parameter(typeof(int), "_baz")));
            var fooType = Create.Class("Foo", Define.Field("_bar", barType),
                Define.Constructor(new Parameter(barType, "_bar")));

            object bar = barType.New(123);

            var foo = fooType.New(bar);

            int baz = foo._bar._baz;

            Assert.That(baz, Is.EqualTo(123));
        }

        [Test]
        public void CanCreateInstanceOfStruct()
        {
            var type = Create.Struct("FooStruct");

            var foo = type.New();

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Value.GetType(), Is.EqualTo(type));
        }

        [Test]
        public void CanCreateInstanceOfStructWithDeclaredConstructor()
        {
            var type = Create.Struct("FooStruct", Define.Constructor(typeof(int)));

            var foo = type.New(123);

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Value.GetType(), Is.EqualTo(type));
        }

        [Test]
        public void CanCreateInstanceOfString()
        {
            var type = typeof(string);

            var foo = type.New('a', 3);

            Assert.That(foo, Is.EqualTo("aaa"));
        }

        [Test]
        public void CanSetNullableFieldToValue()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(int?)));

            var foo = type.New();

            foo._bar = 123;

            var bar = foo._bar;
            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanSetNullableFieldToNull()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(int?)));

            var foo = type.New();

            foo._bar = null;

            var bar = foo._bar;
            Assert.That(bar, Is.Null);
        }

        [Test]
        public void CanSetNullablePropertyToValue()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(int?)));

            var foo = type.New();

            foo.Bar = 123;

            var bar = foo.Bar;
            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanSetNullablePropertyToNull()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(int?)));

            var foo = type.New();

            foo.Bar = null;

            var bar = foo.Bar;
            Assert.That(bar, Is.Null);
        }

        [Test]
        public void CanSetMoreSpecificNumericType()
        {
            var foo = new Foo().Unlock();

            Assert.That(() => foo.Bar = (byte)123, Throws.Nothing);
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

            var foo = UMA.GetStatic<Foo>();

            int baz = foo._baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanGetValueOfPrivateStaticPropertyThroughStaticAccessor()
        {
            Foo.Reset();

            var foo = UMA.GetStatic<Foo>();

            int baz = foo.Baz;

            Assert.That(baz, Is.EqualTo(-1));
        }

        [Test]
        public void CanSetValueOfPrivateStaticFieldThroughStaticAccessor()
        {
            var foo = UMA.GetStatic<Foo>();

            foo._baz = 123;

            Assert.That(foo._baz, Is.EqualTo(123));
        }

        [Test]
        public void CanSetValueOfPrivateStaticPropertyThroughStaticAccessor()
        {
            var foo = UMA.GetStatic<Foo>();

            foo.Baz = 123;

            Assert.That(foo.Baz, Is.EqualTo(123));
        }

        [Test]
        public void CanCreateInstanceWithNewExtensionMethodWithNoParameters()
        {
            var type = Create.Class("Foo");

            var foo = type.New();

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Value.GetType().Name, Is.EqualTo("Foo"));
        }

        [Test]
        public void CanCreateInstanceWithNewExtensionMethodWithParameters()
        {
            var type = Create.Class("Foo", Define.Constructor(typeof(int), typeof(string)));

            var foo = type.New(123, "abc");

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Value.GetType().Name, Is.EqualTo("Foo"));
        }

        [Test]
        public void CannotCreateInstanceOfObjectWithWrongNumberOfParameters()
        {
            var type = Create.Class("Foo");

            Assert.That(() => type.New(123, "abc"),
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "'Foo' does not contain a constructor that takes 2 arguments"));
        }

        [Test]
        public void CannotCreateInstanceOfObjectWithWrongParameterType()
        {
            var type = Create.Class("Foo", Define.Constructor(typeof(int)));

            Assert.That(() => type.New("abc"),
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "The best overloaded constructor match for 'Foo.Foo(Int32)' has some invalid arguments"));
        }

        [Test]
        public void CanChooseTheBestConstructor()
        {
            var type = Create.Class("Foo",
                Define.Constructor(new Parameter(typeof(long), "_long")),
                Define.Constructor(new Parameter(typeof(short), "_short")),
                Define.Field("_long", typeof(long), isReadOnly:true),
                Define.Field("_short", typeof(short), isReadOnly:true));

            long longValue;
            short shortValue;

            var foo1 = type.New((long)123); // exact match to long ctor
            longValue = foo1._long;
            shortValue = foo1._short;
            Assert.That(longValue, Is.EqualTo(123));
            Assert.That(shortValue, Is.EqualTo(0));

            var foo2 = type.New((short)123); // exact match to short ctor
            longValue = foo2._long;
            shortValue = foo2._short;
            Assert.That(longValue, Is.EqualTo(0));
            Assert.That(shortValue, Is.EqualTo(123));

            var foo3 = type.New((uint)123); // only long ctor is legal
            longValue = foo3._long;
            shortValue = foo3._short;
            Assert.That(longValue, Is.EqualTo(123));
            Assert.That(shortValue, Is.EqualTo(0));

            var foo4 = type.New((byte)123); // short is "closer" to byte than long
            longValue = foo4._long;
            shortValue = foo4._short;
            Assert.That(longValue, Is.EqualTo(0));
            Assert.That(shortValue, Is.EqualTo(123));
        }

        [Test]
        public void CannotChoseBestConstructorWhenChoiceIsAmbiguous()
        {
            var type = Create.Class(
                   Define.Constructor(typeof(ICountryHam)),
                   Define.Constructor(typeof(Ham)));
            
            Assert.That(() => type.New(new CountryHam()), Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void CannotCallMethodWithWrongNumberOfParameters()
        {
            var type = Create.Class("Foo", Define.EchoMethod("Bar", typeof(int)));

            var foo = type.New();

            Assert.That(() => foo.Bar(123, 456),
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "No overload for method 'Bar' takes 2 arguments"));
        }

        [Test]
        public void CannotCallMethodWithWrongParameters()
        {
            var type = Create.Class("Foo", Define.EchoMethod("Bar", typeof(int)));

            var foo = type.New();

            Assert.That(() => foo.Bar("abc"),
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "The best overloaded method match for 'Foo.Bar(Int32)' has some invalid arguments"));
        }

        [Test]
        public void CanCallMethodWithValueTypeRefParameterReturningVoid()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(int), true));

            var foo = type.New();

            var i = -1;
            foo.Bar(123, ref i);

            Assert.That(i, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithValueTypeRefParameterReturningValue()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(int), true));

            var foo = type.New();

            var i = -1;
            int dummy = foo.Bar(123, ref i);

            Assert.That(i, Is.EqualTo(123));
            Assert.That(dummy, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithValueTypeOutParameterReturningVoid()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(int), true));

            var foo = type.New();

            int i;
            foo.Bar(123, out i);

            Assert.That(i, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithValueTypeOutParameterReturningValue()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(int)));

            var foo = type.New();

            int i;
            int dummy = foo.Bar(123, out i);

            Assert.That(i, Is.EqualTo(123));
            Assert.That(dummy, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithNullableTypeRefParameterReturningVoid()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(int?), true));

            var foo = type.New();

            int? i = -1;
            foo.Bar(123, ref i);

            Assert.That(i, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithNullableTypeRefParameterReturningValue()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(int?), true));

            var foo = type.New();

            int? i = -1;
            int dummy = foo.Bar(123, ref i);

            Assert.That(i, Is.EqualTo(123));
            Assert.That(dummy, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithNullableTypeOutParameterReturningVoid()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(int?), true));

            var foo = type.New();

            int? i;
            foo.Bar(123, out i);

            Assert.That(i, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithNullableTypeOutParameterReturningValue()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(int?)));

            var foo = type.New();

            int? i;
            int dummy = foo.Bar(123, out i);

            Assert.That(i, Is.EqualTo(123));
            Assert.That(dummy, Is.EqualTo(123));
        }

        [Test]
        public void CanCallMethodWithNullableParameterWithNull()
        {
            var type = Create.Class("Foo", Define.EchoMethod("Bar", typeof(int?)));

            var foo = type.New();

            int? value = foo.Bar(null);

            Assert.That(value, Is.Null);
        }

        [Test]
        public void CanCallMethodWithReferenceTypeRefParameterReturningVoid()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(string), true));

            var foo = type.New();

            var s = "";
            foo.Bar("abc", ref s);

            Assert.That(s, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithReferenceTypeRefParameterReturningValue()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(string)));

            var foo = type.New();

            var s = "";
            string dummy = foo.Bar("abc", ref s);

            Assert.That(s, Is.EqualTo("abc"));
            Assert.That(dummy, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithReferenceTypeOutParameterReturningVoid()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(string), true));

            var foo = type.New();

            string s;
            foo.Bar("abc", out s);

            Assert.That(s, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithReferenceTypeOutParameterReturningValue()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(string)));

            var foo = type.New();

            string s;
            string dummy = foo.Bar("abc", out s);

            Assert.That(s, Is.EqualTo("abc"));
            Assert.That(dummy, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithRefParameterUsingOutKeyword()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(string)));

            var foo = type.New();

            string s;
            string dummy = foo.Bar("abc", out s);

            Assert.That(s, Is.EqualTo("abc"));
            Assert.That(dummy, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithOutParameterUsingRefKeyword()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(string)));

            var foo = type.New();

            string s = "";
            string dummy = foo.Bar("abc", ref s);

            Assert.That(s, Is.EqualTo("abc"));
            Assert.That(dummy, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithRefParameterWithoutUsingRefKeywordButVariableIsNotChanged()
        {
            var type = Create.Class("Foo", Define.EchoRefMethod("Bar", typeof(string)));

            var foo = type.New();

            string s = null;
            string dummy = foo.Bar("abc", s);

            Assert.That(s, Is.Null);
            Assert.That(dummy, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallMethodWithOutParameterWithoutUsingOutKeywordButVariableIsNotChanged()
        {
            var type = Create.Class("Foo", Define.EchoOutMethod("Bar", typeof(string)));

            var foo = type.New();

            string s = null;
            string dummy = foo.Bar("abc", s);

            Assert.That(s, Is.Null);
            Assert.That(dummy, Is.EqualTo("abc"));
        }

        [Test]
        public void CanCallComplexMethodWithMixtureOfRefOutAndRegularParameters()
        {
            var foo = new ComplexMethodWithMixtureOfRefOutAndRegularParameters().Unlock();

            double d;
            var s = "Hello, world!";
            int result = foo.Bar(123, out d, ref s);

            Assert.That(d, Is.EqualTo(184.5));
            Assert.That(s, Is.EqualTo("Hello, world! 5.481"));
            Assert.That(result, Is.EqualTo(246));
        }

        public class ComplexMethodWithMixtureOfRefOutAndRegularParameters
        {
            private int Bar(int i, out double d, ref string s)
            {
                d = i * 1.5;
                s += " " + new string(d.ToString().Reverse().ToArray());
                return i * 2;
            }
        }

        [Test]
        public void CanChooseTheBestMethodOverload()
        {
            var type = Create.Class("Foo",
                Define.EchoMethod("Bar", typeof(long)),
                Define.EchoMethod("Bar", typeof(short)));

            var foo = type.New();

            object result;

            result = foo.Bar((long)123); // exact match to long overload
            Assert.That(result, Is.InstanceOf<long>());

            result = foo.Bar((short)123); // exact match to short overload
            Assert.That(result, Is.InstanceOf<short>());

            result = foo.Bar((uint)123); // only long overload is legal
            Assert.That(result, Is.InstanceOf<long>());

            result = foo.Bar((byte)123); // short is "closer" to byte than long
            Assert.That(result, Is.InstanceOf<short>());
        }

        [Test]
        public void CannotChoseBestMethodOverloadWhenChoiceIsAmbiguous()
        {
            var type = Create.Class("Foo",
                Define.EchoMethod("Bar", typeof(ICountryHam)),
                Define.EchoMethod("Bar", typeof(Ham)));

            var foo = type.New();

            Assert.That(() => foo.Bar(new CountryHam()), Throws.InstanceOf<RuntimeBinderException>());
        }

        [TestCase(typeof(int), 123)]
        [TestCase(typeof(int), (short)456)]
        [TestCase(typeof(string), "abc")]
        public void CanSetReadonlyInstanceField(Type fieldType, object fieldValue)
        {
            var type = Create.Class("Foo", Define.Field("_bar", fieldType, false, true));

            var foo = type.New();

            foo._bar = fieldValue;

            var bar = foo._bar;
            Assert.That(bar, Is.EqualTo(fieldValue));
        }

        [Test]
        public void CanSetReadonlyReferenceTypeStaticField()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(string), true, true));

            var foo = UMA.GetStatic(type);

            foo._bar = "abc";

            var bar = foo._bar;
            Assert.That(bar, Is.EqualTo("abc"));
        }

#if NET40
        [Test]
        public void CannotSetReadonlyValueTypeStaticField()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(int), true, true));

            var foo = UMA.GetStatic(type);

            Assert.That(() => foo._bar = 123,
                Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
                "The current runtime does not allow the (illegal) changing of readonly static value-type fields."));
        }
#else
        [Test]
        public void CanSetReadonlyValueTypeStaticField()
        {
            var type = Create.Class("Foo", Define.Field("_bar", typeof(int), true, true));

            var foo = UMA.GetStatic(type);

            foo._bar = 123;

            var bar = foo._bar;
            Assert.That(bar, Is.EqualTo(123));
        }
#endif

        [Test]
        public void CanReadFieldWithIllegalCSharpName()
        {
            var type = Create.Class("Foo", Define.Field("<Bar>", typeof(int)));

            var foo = type.New();

            int bar = foo["<Bar>"];
            Assert.That(bar, Is.EqualTo(0));
        }

        [Test]
        public void CanWriteFieldWithIllegalCSharpName()
        {
            var type = Create.Class("Foo", Define.Field("<Bar>", typeof(int)));

            var foo = type.New();

            foo["<Bar>"] = 123;

            int bar = foo["<Bar>"];
            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanReadFieldWithIllegalCSharpNameThatDoesNotExist()
        {
            var type = Create.Class("Foo");

            var foo = type.New();

            int bar;
            Assert.That(() => bar = foo["<Bar>"], Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void CanWriteFieldWithIllegalCSharpNameThatDoesNotExist()
        {
            var type = Create.Class("Foo");

            var foo = type.New();

            Assert.That(() => foo["<Bar>"] = 123, Throws.InstanceOf<RuntimeBinderException>());
        }

        [TestCase(typeof(int), typeof(int))]
        
        [TestCase(typeof(int?), typeof(int))]
        [TestCase(typeof(int?), typeof(byte))]

        [TestCase(typeof(object), typeof(int))]

        [TestCase(typeof(short), typeof(sbyte))]
        [TestCase(typeof(int), typeof(sbyte))]
        [TestCase(typeof(long), typeof(sbyte))]
        [TestCase(typeof(float), typeof(sbyte))]
        [TestCase(typeof(double), typeof(sbyte))]
        [TestCase(typeof(decimal), typeof(sbyte))]

        [TestCase(typeof(short), typeof(byte))]
        [TestCase(typeof(ushort), typeof(byte))]
        [TestCase(typeof(int), typeof(byte))]
        [TestCase(typeof(uint), typeof(byte))]
        [TestCase(typeof(long), typeof(byte))]
        [TestCase(typeof(ulong), typeof(byte))]
        [TestCase(typeof(float), typeof(byte))]
        [TestCase(typeof(double), typeof(byte))]
        [TestCase(typeof(decimal), typeof(byte))]

        [TestCase(typeof(int), typeof(short))]
        [TestCase(typeof(long), typeof(short))]
        [TestCase(typeof(float), typeof(short))]
        [TestCase(typeof(double), typeof(short))]
        [TestCase(typeof(decimal), typeof(short))]

        [TestCase(typeof(int), typeof(ushort))]
        [TestCase(typeof(uint), typeof(ushort))]
        [TestCase(typeof(long), typeof(ushort))]
        [TestCase(typeof(ulong), typeof(ushort))]
        [TestCase(typeof(float), typeof(ushort))]
        [TestCase(typeof(double), typeof(ushort))]
        [TestCase(typeof(decimal), typeof(ushort))]

        [TestCase(typeof(long), typeof(int))]
        [TestCase(typeof(float), typeof(int))]
        [TestCase(typeof(double), typeof(int))]
        [TestCase(typeof(decimal), typeof(int))]

        [TestCase(typeof(long), typeof(uint))]
        [TestCase(typeof(ulong), typeof(uint))]
        [TestCase(typeof(float), typeof(uint))]
        [TestCase(typeof(double), typeof(uint))]
        [TestCase(typeof(decimal), typeof(uint))]

        [TestCase(typeof(float), typeof(long))]
        [TestCase(typeof(double), typeof(long))]
        [TestCase(typeof(decimal), typeof(long))]

        [TestCase(typeof(float), typeof(ulong))]
        [TestCase(typeof(double), typeof(ulong))]
        [TestCase(typeof(decimal), typeof(ulong))]

        [TestCase(typeof(ushort), typeof(char))]
        [TestCase(typeof(int), typeof(char))]
        [TestCase(typeof(uint), typeof(char))]
        [TestCase(typeof(long), typeof(char))]
        [TestCase(typeof(ulong), typeof(char))]
        [TestCase(typeof(float), typeof(char))]
        [TestCase(typeof(double), typeof(char))]
        [TestCase(typeof(decimal), typeof(char))]

        [TestCase(typeof(double), typeof(float))]

        [TestCase(typeof(MyBase), typeof(MyDerived))]

        [TestCase(typeof(IMyInterface), typeof(MyDerived))]
        public void CanAssign(Type propertyType, Type valueType)
        {
            var value = Activator.CreateInstance(valueType);

            var type = Create.Class("Foo", Define.AutoProperty("Bar", propertyType));

            var foo = type.New();

            Assert.That(() => foo.Bar = value, Throws.Nothing);
        }

        [Test]
        public void CanAssignSpecificArrayToArray()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(Array)));

            var foo = type.New();

            Assert.That(() => foo.Bar = new int[0], Throws.Nothing);
        }

        [Test]
        public void CanAssignSpecificDelegateToDelegate()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(Delegate)));

            var foo = type.New();

            Assert.That(() => foo.Bar = (Action)(() => {}), Throws.Nothing);
        }

        [Test]
        public void CanAssignSpecificEnumToEnum()
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(Enum)));

            var foo = type.New();

            Assert.That(() => foo.Bar = MyEnum.First, Throws.Nothing);
        }

        [TestCase(typeof(IMyInterface[]))]

        [TestCase(typeof(IList<IMyInterface>))]
        [TestCase(typeof(ICollection<IMyInterface>))]
        [TestCase(typeof(IEnumerable<IMyInterface>))]

        [TestCase(typeof(IList))]
        [TestCase(typeof(ICollection))]
        [TestCase(typeof(IEnumerable))]
        public void CanAssignArrayToAllOfItsInterfaces(Type propertyType)
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", propertyType));

            var foo = type.New();

            Assert.That(() => foo.Bar = new MyDerived[0], Throws.Nothing);
        }

        [Test]
        public void CanAssignCovariant() // covariance
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(IEnumerable<IMyInterface>)));

            var foo = type.New();

            Assert.That(() => foo.Bar = new List<MyDerived>(), Throws.Nothing);
        }

        [Test]
        public void CanAssignContravariant() // contravariance
        {
            var type = Create.Class("Foo", Define.AutoProperty("Bar", typeof(Action<MyDerived>)));

            var foo = type.New();

            Assert.That(() => foo.Bar = (Action<MyBase>)(myBase => {}), Throws.Nothing);
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
            var bar = UMA.GetStatic<Bar>();

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
            var foo = UMA.GetStatic<Foo>();

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
            
            Assert.That(() => foo.Garply(null),
                Throws.InstanceOf<RuntimeBinderException>().With.Message.EqualTo(
                "The call is ambiguous between the following methods or properties: 'Garply(System.String)' and 'Garply(RockLib.Dynamic.UnitTests.IBaz)'"));
        }

        [Test]
        public void CanInvokePrivateConstructorsWithNew()
        {
            var quxFactory = UMA.GetStatic<Qux>();

            Qux qux = quxFactory.New();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithCreate()
        {
            var quxFactory = UMA.GetStatic<Qux>();

            Qux qux = quxFactory.Create();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithNewInstance()
        {
            var quxFactory = UMA.GetStatic<Qux>();

            Qux qux = quxFactory.NewInstance();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CanInvokePrivateConstructorsWithCreateInstance()
        {
            var quxFactory = UMA.GetStatic<Qux>();

            Qux qux = quxFactory.CreateInstance();

            Assert.That(qux, Is.InstanceOf<Qux>());
        }

        [Test]
        public void CannotInvokeConstructorsWithAliasWithIncorrectArgs()
        {
            var quxFactory = UMA.GetStatic<Qux>();

            Assert.That(() => quxFactory.CreateInstance("wrong", "args"), Throws.InstanceOf<RuntimeBinderException>());
        }

        [Test]
        public void CanResolveMultipleConstructors()
        {
            var garplyFactory = UMA.GetStatic<Garply>();

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
            var fooType = Create.Class("Foo", Define.NestedEnum("Bar", "Baz", "Qux"));
            var Foo = UMA.GetStatic(fooType);

            // Note that these variables are declared as object. (see below)
            object baz = Foo.Bar.Baz;
            object qux = Foo.Bar.Qux;

            // If the baz and qux variables had been declared dynamic,
            // then these conversions would fail.
            var bazInt = (int)baz;
            var quxInt = (int)qux;

            Assert.That(bazInt, Is.EqualTo(0));
            Assert.That(quxInt, Is.EqualTo(1));
        }

        [Test]
        public void CanGetDefaultValueOfPrivateEnum()
        {
            var fooType = Create.Class("Foo", Define.NestedEnum("Bar", "Baz", "Qux"));
            var Foo = UMA.GetStatic(fooType);

            // Note that the variable is declared as object. (see below)
            object defaultBar = Foo.Bar.New();

            // If the defaultBar variable had been declared dynamic,
            // then this conversion would fail.
            var defaultInt = (int)defaultBar;

            Assert.That(defaultInt, Is.EqualTo(0));
        }

        [Test]
        public void CanAccessNestedClass()
        {
            var fooType = Create.Class("Foo", Define.NestedClass("Bar"));

            var foo = UMA.GetStatic(fooType);

            var bar = foo.Bar.New();

            Assert.That(bar.Value.GetType().Name, Is.EqualTo("Bar"));
        }

        [Test]
        public void CanAccessDeeplyNestedClass()
        {
            var fooType =
                Create.Class("Foo",
                    Define.NestedClass("Bar",
                        Define.NestedClass("Baz",
                            Define.NestedClass("Qux",
                                Define.NestedClass("Grault",
                                    Define.NestedClass("Garply"))))));

            var foo = UMA.GetStatic(fooType);

            var garply = foo.Bar.Baz.Qux.Grault.Garply.New();

            Assert.That(garply.Value.GetType().Name, Is.EqualTo("Garply"));
        }

        [Test]
        public void CanAccessNestedStruct()
        {
            var fooType = Create.Class("Foo", Define.NestedStruct("Bar"));

            var foo = UMA.GetStatic(fooType);

            var bar = foo.Bar.New();

            Assert.That(bar.Value.GetType().Name, Is.EqualTo("Bar"));
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

        [TestCase(typeof(string), typeof(IHam), ushort.MaxValue)]
        [TestCase(typeof(string), typeof(Ham), ushort.MaxValue)]
        [TestCase(typeof(string), typeof(int), ushort.MaxValue)]
        public void AncestorDistanceIsCalculatedCorrectlyForInterfacesAndClasses(Type type, Type ancestorType, int expectedDistance)
        {
            var candidate = UMA.GetStatic<UMA>().Candidate;

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
            var candidate = UMA.GetStatic<UMA>().Candidate;

            var distance = candidate.GetAncestorDistance(type, ancestorType);

            Assert.That(distance, Is.EqualTo(expectedDistance));
        }

        [TestCase(typeof(string), typeof(object), false, TestName="object ancestor returns false")]
        [TestCase(typeof(string), typeof(int), false, TestName="struct ancestor returns false")]
        [TestCase(typeof(Foo), typeof(string), false, TestName="unrelated types returns false")]
        [TestCase(typeof(Ham), typeof(IHam), true, TestName="implemented interface returns true")]
        [TestCase(typeof(Ham), typeof(Pork), true, TestName="inherited class returns true")]
        public void HasAncestorReturnsTheCorrectValue(Type type, Type ancestorType, bool expectedValue)
        {
            var candidate = UMA.GetStatic<UMA>().Candidate;

            bool hasAncestor = candidate.HasAncestor(type, ancestorType);

            Assert.That(hasAncestor, Is.EqualTo(expectedValue));
        }

        [TestCase(typeof(int), typeof(int), typeof(int), 0)]
        [TestCase(typeof(int), typeof(short), typeof(int), 1)]
        [TestCase(typeof(short), typeof(int), typeof(int), -32767)]
        [TestCase(typeof(int), typeof(object), typeof(int), 1)]
        [TestCase(typeof(object), typeof(int), typeof(int), -32767)]
        [TestCase(typeof(Ham), typeof(object), typeof(CountryHam), 1)]
        [TestCase(typeof(object), typeof(Ham), typeof(CountryHam), -32767)]
        [TestCase(typeof(short), typeof(int), typeof(byte), 1)]
        [TestCase(typeof(int), typeof(short), typeof(byte), -32767)]
        [TestCase(typeof(Ham), typeof(Pork), typeof(CountryHam), 1)]
        [TestCase(typeof(Pork), typeof(Ham), typeof(CountryHam), -32767)]
        [TestCase(typeof(Ham), typeof(string), typeof(CountryHam), 1)]
        [TestCase(typeof(string), typeof(Ham), typeof(CountryHam), -32767)]
        public void AccumulateScoreModifiesScoreCorrectly(Type thisType, Type otherType, Type argType, int expectedScore)
        {
            var candidate = UMA.GetStatic<UMA>().Candidate;

            var score = 0;
            candidate.AccumulateScore(thisType, otherType, argType, ref score);

            Assert.That(score, Is.EqualTo(expectedScore));
        }

        [Test]
        public void IsLegalReturnsFalseIfNotEnoughRequiredArgumentsAreSupplied()
        {
            var type = Create.Class("Foo",
                Define.Constructor(
                    new Parameter(typeof(string)),
                    new Parameter(-1, typeof(int)),
                    new Parameter(true, typeof(bool))));

            var constructor = type.GetConstructors()[0];

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(constructor);

            bool isLegal = candidate.IsLegal(new Type[0], Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReturnsTrueIfAllArgumentsAreSupplied()
        {
            var type = Create.Class("Foo",
                Define.Constructor(
                    new Parameter(typeof(string)),
                    new Parameter(-1, typeof(int)),
                    new Parameter(true, typeof(bool))));

            var constructor = type.GetConstructors()[0];

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(constructor);

            bool isLegal = candidate.IsLegal(new[] { typeof(string), typeof(int), typeof(bool) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfNoOptionalArgumentsAreSupplied()
        {
            var type = Create.Class("Foo",
                Define.Constructor(
                    new Parameter(typeof(string)),
                    new Parameter(-1, typeof(int)),
                    new Parameter(true, typeof(bool))));

            var constructor = type.GetConstructors()[0];

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(constructor);

            bool isLegal = candidate.IsLegal(new[] { typeof(string) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfSomeOptionalArgumentsAreSupplied()
        {
            var type = Create.Class("Foo",
                Define.Constructor(
                    new Parameter(typeof(string)),
                    new Parameter(-1, typeof(int)),
                    new Parameter(true, typeof(bool))));

            var constructor = type.GetConstructors()[0];

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(constructor);

            bool isLegal = candidate.IsLegal(new[] { typeof(string), typeof(int) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        public void Generic<T>(T t)
        {
        }

        [Test]
        public void IsLegalReturnsFalseIfTheArgTypeIsNotAssignableToTheTypeArgument()
        {
            var method = GetType().GetMethod("Generic");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(int) }, new[] { typeof(byte) });

            Assert.That(isLegal, Is.False);
        }

        public void NewConstraint<T>(T t)
            where T : new()
        {
        }

        [Test]
        public void IsLegalReturnsTrueIfAnObjectWithADefaultConstructorIsMatchedWithTheNewContraint()
        {
            var method = GetType().GetMethod("NewConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Ham) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfATypeParameterClassWithADefaultConstructorIsMatchedWithTheNewContraint2()
        {
            var method = GetType().GetMethod("NewConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(CountryHam) }, new[] { typeof(Ham) });

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfAValueTypeValueIsMatchedWithTheNewContraint()
        {
            var method = GetType().GetMethod("NewConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(int) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfATypeParameterValueTypeIsMatchedWithTheNewContraint()
        {
            var method = GetType().GetMethod("NewConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(int) }, new[] { typeof(long) });

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsFalseIfAnObjectWithoutADefaultConstructorIsMatchedWithTheNewContraint()
        {
            var method = GetType().GetMethod("NewConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Lard) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReturnsFalseIfATypeParameterClassWithoutADefaultConstructorIsMatchedWithTheNewContraint()
        {
            var method = GetType().GetMethod("NewConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Bacon) }, new[] { typeof(Lard) });

            Assert.That(isLegal, Is.False);
        }

        public void ClassConstraint<T>(T t)
            where T : class
        {
        }

        [Test]
        public void IsLegalReturnsTrueIfAnObjectIsMatchedWithTheClassContraint()
        {
            var method = GetType().GetMethod("ClassConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(string) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfATypeParameterClassIsMatchedWithTheClassContraint()
        {
            var method = GetType().GetMethod("ClassConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(string) }, new[] { typeof(string) });

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsFalseIfAValueTypeValueIsMatchedWithTheClassContraint()
        {
            var method = GetType().GetMethod("ClassConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(int) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReturnsFalseIfTypeParameterStructIsMatchedWithTheClassContraint()
        {
            var method = GetType().GetMethod("ClassConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(byte) }, new[] { typeof(int) });

            Assert.That(isLegal, Is.False);
        }

        public void StructConstraint<T>(T t)
            where T : struct
        {
        }

        [Test]
        public void IsLegalReturnsTrueIfAValueTypeValueIsMatchedWithTheStructContraint()
        {
            var method = GetType().GetMethod("StructConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(int) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfATypeParameterValueTypeIsMatchedWithTheStructContraint()
        {
            var method = GetType().GetMethod("StructConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(int) }, new[] { typeof(int) });

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsFalseIfAnObjectIsMatchedWithTheStructContraint()
        {
            var method = GetType().GetMethod("StructConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(string) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReturnsFalseIfAnTypeParameterClassIsMatchedWithTheStructContraint()
        {
            var method = GetType().GetMethod("StructConstraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(string) }, new[] { typeof(string) });

            Assert.That(isLegal, Is.False);
        }

        public void BaseClassContraint<THam>(THam tHam)
            where THam : Ham
        {
        }

        [Test]
        public void IsLegalReturnsTrueIfADerivedObjectIsMatchedWithItsBaseClassContraint()
        {
            var method = GetType().GetMethod("BaseClassContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(CountryHam) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfATypeParameterDerivedClassIsMatchedWithItsBaseClassContraint()
        {
            var method = GetType().GetMethod("BaseClassContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(CountryHam) }, new[] { typeof(CountryHam) });

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsFalseIfAnUnrelatedObjectIsMatchedWithABaseClassContraint()
        {
            var method = GetType().GetMethod("BaseClassContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Spam) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReturnsFalseIfAnUnrelatedTypeParameterClassIsMatchedWithABaseClassContraint()
        {
            var method = GetType().GetMethod("BaseClassContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Spam) }, new[] { typeof(Spam) });

            Assert.That(isLegal, Is.False);
        }

        public void InterfaceContraint<THam>(THam tHam)
            where THam : IHam
        {
        }

        [Test]
        public void IsLegalReturnsTrueIfAnInterfaceImplementationObjectIsMatchedWithItsInterfaceContraint()
        {
            var method = GetType().GetMethod("InterfaceContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Ham) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueIfATypeParameterInterfaceImplementationIsMatchedWithItsInterfaceContraint()
        {
            var method = GetType().GetMethod("InterfaceContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Ham) }, new[] { typeof(Ham) });

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsFalseIfAnUnrelatedClassIsMatchedWithAnInterfaceContraint()
        {
            var method = GetType().GetMethod("InterfaceContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Spam) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void IsLegalReturnsFalseIfATypeParameterUnrelatedClassIsMatchedWithAnInterfaceContraint()
        {
            var method = GetType().GetMethod("InterfaceContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Spam) }, new[] { typeof(Spam) });

            Assert.That(isLegal, Is.False);
        }

        public void TypeParameterContraint<TBase, TDerived>(TBase tBase, TDerived tDerived)
            where TDerived : TBase
        {
        }

        [Test]
        public void IsLegalReturnsTrueInATypeParameterConstraintScenario1()
        {
            var method = GetType().GetMethod("TypeParameterContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Ham), typeof(CountryHam) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.True);
        }

        [Test]
        public void IsLegalReturnsTrueInATypeParameterConstraintScenario2()
        {
            var method = GetType().GetMethod("TypeParameterContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Ham), typeof(CountryHam) }, new[] { typeof(Ham), typeof(CountryHam) });

            Assert.That(isLegal, Is.True);
        }

        [Test, Ignore("Gives a false positive - no good (read: simple) way of knowing the relationship of one generic paramter to another.")]
        public void IsLegalReturnsFalseInANonTypeParameterConstraintScenario()
        {
            var method = GetType().GetMethod("TypeParameterContraint");

            var candidateFactory = UMA.GetStatic<UMA>().Candidate;

            var candidate = candidateFactory.New(method);

            bool isLegal = candidate.IsLegal(new[] { typeof(Ham), typeof(Spam) }, Type.EmptyTypes);

            Assert.That(isLegal, Is.False);
        }

        [Test]
        public void CanCallConstructorWithDefaultValueParameterWithAndWithoutSpecifyingIt()
        {
            var type = Create.Class("Foo",
                Define.Constructor(typeof(string), new Parameter(-1, typeof(int))));

            Assert.That(() => type.New("abc", 123), Throws.Nothing);
            Assert.That(() => type.New("abc"), Throws.Nothing);
        }

        [Test]
        public void CanFindTheBestConstructorWhenDefaultParametersAreInvolved()
        {
            var type = Create.Class("Foo",
                Define.Constructor(
                    new Parameter(typeof(string), "_bar"),
                    new Parameter(-1, typeof(int)),
                    new Parameter(true, typeof(bool))),
                Define.Constructor(
                    new Parameter(typeof(object), "_baz"),
                    new Parameter(-1, typeof(int))),
                Define.AutoProperty("Bar", typeof(string), backingFieldName:"_bar"),
                Define.AutoProperty("Baz", typeof(object), backingFieldName:"_baz"));

            object parameter = "abc";
            var foo1 = type.New(parameter);

            Assert.That(foo1.Bar, Is.EqualTo(parameter));
            Assert.That(foo1.Baz, Is.Null);

            parameter = 123.45; 
            var foo2 = type.New(parameter);

            Assert.That(foo2.Bar, Is.Null);
            Assert.That(foo2.Baz.Value, Is.EqualTo(parameter));
        }

        [Test]
        public void CanCallMethodWithDefaultValueParameterWithAndWithoutSpecifyingIt()
        {
            var type = Create.Class("Foo",
                Define.Method("Bar", typeof(string), new Parameter(-1, typeof(int))));

            var foo = type.New();

            Assert.That(() => foo.Bar("abc", 123), Throws.Nothing);
            Assert.That(() => foo.Bar("abc"), Throws.Nothing);
        }

        [Test]
        public void CanFindTheBestOverloadedMethodWhenDefaultParametersAreInvolved()
        {
            var type = Create.Class("Foo",
                Define.Method("Qux",
                    new Parameter(typeof(string), "_bar"),
                    new Parameter(-1, typeof(int)),
                    new Parameter(true, typeof(bool))),
                Define.Method("Qux",
                    new Parameter(typeof(object), "_baz"),
                    new Parameter(-1, typeof(int))),
                Define.AutoProperty("Bar", typeof(string), backingFieldName: "_bar"),
                Define.AutoProperty("Baz", typeof(object), backingFieldName: "_baz"));

            var foo = type.New();

            object parameter = "abc";
            foo.Qux(parameter);

            Assert.That(foo.Bar, Is.EqualTo(parameter));
            Assert.That(foo.Baz, Is.Null);

            foo = type.New();

            parameter = 123.45;
            foo.Qux(parameter);

            Assert.That(foo.Bar, Is.Null);
            Assert.That(foo.Baz.Value, Is.EqualTo(parameter));
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

        [Test]
        public void CreateInstanceDefinitionEqualsReturnsTrueWhenReferencesAreEqual()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = definition1;

            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void CreateInstanceDefinitionEqualsReturnsFalseWhenOtherTypeIsNotCreateInstanceDefinition()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = "abcd";

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void CreateInstanceDefinitionEqualsReturnsFalseWhenOtherHasDifferentType()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = type.New(typeof(string), new[] { typeof(string), typeof(bool) }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void CreateInstanceDefinitionEqualsReturnsFalseWhenOtherHasDifferentNumberOfArgTypes()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = type.New(typeof(int), new[] { typeof(string) }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void CreateInstanceDefinitionEqualsReturnsFalseWhenOtherHasDifferentArgTypes()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = type.New(typeof(int), new[] { typeof(string), typeof(DateTime) }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void CreateInstanceDefinitionEqualsReturnsTrueWhenTypeAndArgTypesAreTheSame()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;

            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void CreateInstanceDefinitionGetHashCodeIsTheSameForEqualInstances()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;

            Assert.That(definition1.GetHashCode(), Is.EqualTo(definition2.GetHashCode()));
            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void CreateInstanceDefinitionGetHashCodeIsTheSameForReferenceEqualInstances()
        {
            var type = UMA.GetStatic<UMA>().CreateInstanceDefinition;

            var definition1 = type.New(typeof(int), new[] { typeof(string), typeof(bool) }).Value;
            var definition2 = definition1;

            Assert.That(definition1.GetHashCode(), Is.EqualTo(definition2.GetHashCode()));
            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsTrueWhenReferencesAreEqual()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;
            var definition2 = definition1;

            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherTypeIsNotInvokeMethodDefinition()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;
            var definition2 = "abcd";

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherHasDifferentType()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(string), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherHasDifferentName()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Bar", Type.EmptyTypes, new object[] { "abc", true }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherHasDifferentNumberOfArgTypes()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc" }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherHasDifferentArgTypes()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Foo", Type.EmptyTypes, new object[] { "abc", DateTime.Now }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherHasDifferentNumberOfTypeArguments()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", new[] { typeof(int) }, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc" }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsFalseWhenOtherHasDifferentTypeArguments()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(int) }, new object[] { "abc", DateTime.Now }).Value;

            Assert.That(definition1.Equals(definition2), Is.False);
        }

        [Test]
        public void InvokeMethodDefinitionEqualsReturnsTrueWhenTypeAndArgTypesAreTheSame()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc", true }).Value;

            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void InvokeMethodDefinitionGetHashCodeIsTheSameForEqualInstances()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc", true }).Value;
            var definition2 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc", true }).Value;

            Assert.That(definition1.GetHashCode(), Is.EqualTo(definition2.GetHashCode()));
            Assert.That(definition1.Equals(definition2), Is.True);
        }

        [Test]
        public void InvokeMethodDefinitionGetHashCodeIsTheSameForReferenceEqualInstances()
        {
            var type = UMA.GetStatic<UMA>().InvokeMethodDefinition;

            var definition1 = type.New(typeof(int), "Foo", new[] { typeof(int), typeof(string) }, new object[] { "abc", true }).Value;
            var definition2 = definition1;

            Assert.That(definition1.GetHashCode(), Is.EqualTo(definition2.GetHashCode()));
            Assert.That(definition1.Equals(definition2), Is.True);
        }

        // TODO: Add "proper" support for nested type.
        // TODO: add feature: explicitly implemented interface members
        //       TODO: figure out how to deal with explicit member with same name as other member of same name
        //       http://stackoverflow.com/a/17854048/252004
        //       http://stackoverflow.com/questions/7379276/how-to-find-if-a-method-is-implementing-specific-interface
        // TODO: add feature: support for method hiding
        //       TODO: figure out how to deal with deciding which member (hiding or hidden) to select
        // TODO: add feature: support for calling extension methods dynamically. May need to have "specify namespace" functionality for this to work.

        [Test]
        public void CanCallGenericMethodWithExplicitStructGenericArgument()
        {
            var foo = new GenericFoo().Unlock();

            var bar = foo.Bar<int>("foo", 123);

            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanCallGenericMethodWithExplicitClassGenericArgument()
        {
            var foo = new GenericFoo().Unlock();

            var bar = foo.Bar<Ham>("foo", new Ham()).Value;

            Assert.That(bar, Is.InstanceOf<Ham>());
        }

        [Test]
        public void CanCallGenericMethodWithImplicitStructGenericArgument()
        {
            var foo = new GenericFoo().Unlock();

            var bar = foo.Bar("foo", 123);

            Assert.That(bar, Is.EqualTo(123));
        }

        [Test]
        public void CanCallGenericMethodWithImplicitClassGenericArgument()
        {
            var foo = new GenericFoo().Unlock();

            var bar = foo.Bar("foo", new Ham()).Value;

            Assert.That(bar, Is.InstanceOf<Ham>());
        }

        [Test]
        public void CanCallGenericMethodWithExplicitStructGenericArgumentAndNoGenericParameter()
        {
            var foo = new GenericFoo().Unlock();

            var ham = foo.Baz<int>();

            Assert.That(ham, Is.EqualTo(0));
        }

        [Test]
        public void CanCallGenericMethodWithExplicitClassGenericArgumentAndNoGenericParameter()
        {
            var foo = new GenericFoo().Unlock();

            var ham = foo.Baz<Ham>().Value;

            Assert.That(ham, Is.InstanceOf<Ham>());
        }

        [Test]
        public void CanCallGenericMethodWithExplicitStructGenericArgumentAndGenericOutParameter()
        {
            var foo = new GenericFoo().Unlock();

            int outBar;
            var bar = foo.Qux<int>("foo", 123, out outBar);

            Assert.That(bar, Is.EqualTo(123));
            Assert.That(outBar, Is.EqualTo(123));
        }

        [Test]
        public void CanCallGenericMethodWithExplicitStructGenericArgumentAndGenericRefParameter()
        {
            var foo = new GenericFoo().Unlock();

            var outBar = 456;
            var bar = foo.Qux<int>("foo", 123, ref outBar);

            Assert.That(bar, Is.EqualTo(123));
            Assert.That(outBar, Is.EqualTo(123));
        }

        [Test]
        public void CanCallGenericMethodWithExplicitClassGenericArgumentAndGenericOutParameter()
        {
            var foo = new GenericFoo().Unlock();

            var ham = new Ham();

            Ham outBar;
            var bar = foo.Qux<Ham>("foo", ham, out outBar).Value;

            Assert.That(bar, Is.SameAs(ham));
            Assert.That(outBar, Is.SameAs(ham));
        }

        [Test]
        public void CanCallGenericMethodWithExplicitClassGenericArgumentAndGenericRefParameter()
        {
            var foo = new GenericFoo().Unlock();

            var ham = new Ham();

            var outBar = new Ham();
            var bar = foo.Qux<Ham>("foo", ham, ref outBar).Value;

            Assert.That(bar, Is.SameAs(ham));
            Assert.That(outBar, Is.SameAs(ham));
        }

        [Test]
        public void CanCallGenericMethodWithImplicitStructGenericArgumentAndGenericOutParameter()
        {
            var foo = new GenericFoo().Unlock();

            int outBar;
            var bar = foo.Qux("foo", 123, out outBar);

            Assert.That(bar, Is.EqualTo(123));
            Assert.That(outBar, Is.EqualTo(123));
        }

        [Test]
        public void CanCallGenericMethodWithImplicitStructGenericArgumentAndGenericRefParameter()
        {
            var foo = new GenericFoo().Unlock();

            var outBar = 456;
            var bar = foo.Qux("foo", 123, ref outBar);

            Assert.That(bar, Is.EqualTo(123));
            Assert.That(outBar, Is.EqualTo(123));
        }

        [Test]
        public void CanCallGenericMethodWithImplicitClassGenericArgumentAndGenericOutParameter()
        {
            var foo = new GenericFoo().Unlock();

            var ham = new Ham();

            Ham outBar;
            var bar = foo.Qux("foo", ham, out outBar).Value;

            Assert.That(bar, Is.SameAs(ham));
            Assert.That(outBar, Is.SameAs(ham));
        }

        [Test]
        public void CanCallGenericMethodWithImplicitClassGenericArgumentAndGenericRefParameter()
        {
            var foo = new GenericFoo().Unlock();

            var ham = new Ham();

            var outBar = new Ham();
            var bar = foo.Qux("foo", ham, ref outBar).Value;

            Assert.That(bar, Is.SameAs(ham));
            Assert.That(outBar, Is.SameAs(ham));
        }

        public class GenericFoo
        {
            public T Bar<T>(string s, T t)
            {
                return t;
            }

            public T Baz<T>() where T : new()
            {
                return new T();
            }

            public T Qux<T>(string s, T tIn, ref T tOut)
            {
                tOut = tIn;
                return tIn;
            }
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

        public class Lard
        {
            public Lard(bool gross)
            {
            }
        }

        public class Bacon : Lard
        {
            public Bacon()
                : base(false)
            {
            }
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

    public class Waldo
    {
        private EventHandler _foo = FooHandler;
        private Wobble _wobble;

        public enum Wobble
        {
            Wubble,
            Wibble
        }

        private static void FooHandler(object sender, EventArgs args)
        {
        }
    }

    public class MyBase
    {
    }

    public interface IMyInterface
    {
    }

    public class MyDerived : MyBase, IMyInterface
    {
    }

    public enum MyEnum
    {
        First
    }

    // ReSharper restore EventNeverSubscribedTo.Local
    // ReSharper restore ConvertToAutoProperty
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}
