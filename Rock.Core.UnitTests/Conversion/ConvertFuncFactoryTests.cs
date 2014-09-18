using System;
using System.ComponentModel;
using NUnit.Framework;
using Rock.Conversion;
using System.Globalization;

namespace ConvertFuncFactoryTests.cs
{
    public class TheGetConvertFuncMethod
    {
        [Test]
        public void WhenTheTypesAreUnrelatedReturnsAFuncThatReturnsTheInputObject()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Foo), typeof(double));

            var foo = new Foo();

            Assert.That(f(foo), Is.InstanceOf<Foo>());
        }

        [Test]
        public void WhenBothTypesAreTheSameReturnsAFuncThatReturnsTheInputObject()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(int), typeof(int));

            Assert.That(f(1), Is.InstanceOf<int>());
        }

        [Test]
        public void WhenDestinationTypeInheritsFromSourceTypeReturnsAFuncThatReturnsTheInputObject()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Foo), typeof(IFoo));

            var foo = new Foo();

            Assert.That(f(foo), Is.InstanceOf<Foo>());
        }

        [Test]
        public void WhenTheTypesAreUnrelatedButAConversionOperatorIsDefinedBetweenThemReturnsAFuncThatExecutesTheConversionOperator()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Foo), typeof(Bar));

            var foo = new Foo();

            Assert.That(f(foo), Is.InstanceOf<Bar>());
        }

        [Test]
        public void WhenTheTypesAreUnrelatedButATypeConverterIsDefinedForTheSourceTypeAndCanConvertToTheDestinationTypeReturnsAFuncThatReturnsTheResultOfTheTypeConvertersConvertToMethod()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Baz), typeof(Bar));

            var baz = new Baz();

            Assert.That(f(baz), Is.InstanceOf<Bar>());
        }

        [Test]
        public void WhenTheTypesAreUnrelatedButATypeConverterIsDefinedForTheDestinationTypeAndCanConvertFromTheSourceTypeReturnsAFuncThatReturnsTheResultOfTheTypeConvertersConvertFromMethod()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Foo), typeof(Baz));

            var foo = new Foo();

            Assert.That(f(foo), Is.InstanceOf<Baz>());
        }

        [Test]
        public void WhenTheTypesAreUnrelatedButTheSourceTypeImlementsIConvertibleReturnsAFuncThatReturnsTheResultOfACallToConvertChangeType()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Qux), typeof(Foo));

            var qux = new Qux();

            Assert.That(f(qux), Is.InstanceOf<Foo>());
        }

        [Test]
        public void WhenTheTypesAreUnrelatedButTheSourceTypeImplementsIConvertibleReturnsAFuncThatReturnsTheResultOfACallToConvertChangeButWhenThatCallThrowsAnExceptionReturnTheInputObject()
        {
            var f = ConvertFuncFactory.GetConvertFunc(typeof(Qux), typeof(Bar));

            var qux = new Qux();

            Assert.That(f(qux), Is.InstanceOf<Qux>());
        }

        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
        }

        public class Bar
        {
            public static explicit operator Bar(Foo foo)
            {
                return new Bar();
            }
        }

        [TypeConverter(typeof(BazTypeConverter))]
        public class Baz
        {
        }

        public class BazTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(Foo);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return new Baz();
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(Bar);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return new Bar();
            }
        }

        public class Qux : IConvertible
        {
            TypeCode IConvertible.GetTypeCode() { throw new NotImplementedException(); }
            bool IConvertible.ToBoolean(IFormatProvider provider) { throw new NotImplementedException(); }
            char IConvertible.ToChar(IFormatProvider provider) { throw new NotImplementedException(); }
            sbyte IConvertible.ToSByte(IFormatProvider provider) { throw new NotImplementedException(); }
            byte IConvertible.ToByte(IFormatProvider provider) { throw new NotImplementedException(); }
            short IConvertible.ToInt16(IFormatProvider provider) { throw new NotImplementedException(); }
            ushort IConvertible.ToUInt16(IFormatProvider provider) { throw new NotImplementedException(); }
            int IConvertible.ToInt32(IFormatProvider provider) { throw new NotImplementedException(); }
            uint IConvertible.ToUInt32(IFormatProvider provider) { throw new NotImplementedException(); }
            long IConvertible.ToInt64(IFormatProvider provider) { throw new NotImplementedException(); }
            ulong IConvertible.ToUInt64(IFormatProvider provider) { throw new NotImplementedException(); }
            float IConvertible.ToSingle(IFormatProvider provider) { throw new NotImplementedException(); }
            double IConvertible.ToDouble(IFormatProvider provider) { throw new NotImplementedException(); }
            decimal IConvertible.ToDecimal(IFormatProvider provider) { throw new NotImplementedException(); }
            DateTime IConvertible.ToDateTime(IFormatProvider provider) { throw new NotImplementedException(); }
            string IConvertible.ToString(IFormatProvider provider) { throw new NotImplementedException(); }

            object IConvertible.ToType(Type conversionType, IFormatProvider provider)
            {
                if (conversionType != typeof(Foo))
                {
                    throw new Exception();
                }

                return new Foo();
            }
        }
    }
}
