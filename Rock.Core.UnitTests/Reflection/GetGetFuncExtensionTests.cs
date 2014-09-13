using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rock.Reflection;

// ReSharper disable once CheckNamespace
namespace Rock.Core.Reflection.GetGetFuncExtensionTests
{
    namespace TheNonGenericGetGetFuncExtensionMethod
    {
        public class WhenPropertyTypeIsAReferenceTypeAndNotObject
            : TestBaseSuccess<ClassWithReferenceTypeProperty, Foo>
        {
            protected override Foo GetPropertyValue()
            {
                return new Foo();
            }
        }

        public class WhenPropertyTypeIsObject
            : TestBaseSuccess<ClassWithObjectProperty, object>
        {
            protected override object GetPropertyValue()
            {
                return new object();
            }
        }

        public class WhenPropertyTypeIsAValueType
            : TestBaseSuccess<ClassWithValueTypeProperty, int>
        {
            protected override int GetPropertyValue()
            {
                return 123;
            }
        }
    }

    namespace TheGenericGetGetFuncExtensionMethod
    {
        public class WhenTInstanceIsLessSpecificThanDeclaringType
            : TestBaseSuccess<object, int, ClassWithValueTypeProperty, int>
        {
            protected override int GetPropertyValue()
            {
                return 123;
            }
        }

        public class WhenTInstanceIsNotLessSpecificThanDeclaringType
            : TestBaseSuccess<ClassWithValueTypeProperty, int, ClassWithValueTypeProperty, int>
        {
            protected override int GetPropertyValue()
            {
                return 123;
            }
        }

        public class WhenTInstanceIsUnrelatedToDeclaringType
            : TestBaseFailure<Attribute, int, ClassWithValueTypeProperty, int>
        {
        }

        public class WhenPropertyTypeIsLessSpecificThanTProperty
            : TestBaseSuccess<ClassWithReferenceTypeProperty, FooDerived, ClassWithReferenceTypeProperty, Foo>
        {
            protected override Foo GetPropertyValue()
            {
                return new FooDerived();
            }
        }

        public class WhenPropertyTypeRequiresBoxingWhenConvertingToTProperty
            : TestBaseSuccess<ClassWithValueTypeProperty, object, ClassWithValueTypeProperty, int>
        {
            protected override int GetPropertyValue()
            {
                return 123;
            }
        }

        public class WhenPropertyTypeIsNotLessSpecificThanAndDoesNotRequireBoxingWhenConvertingToTProperty
            : TestBaseSuccess<ClassWithValueTypeProperty, int, ClassWithValueTypeProperty, int>
        {
            protected override int GetPropertyValue()
            {
                return 123;
            }
        }

        public class WhenPropertyTypeIsUnrelatedToTProperty
            : TestBaseFailure<ClassWithValueTypeProperty, Attribute, ClassWithValueTypeProperty, int>
        {
        }
    }

    public abstract class TestBase<TDeclaringType, TPropertyType>
        where TDeclaringType : IHasValue<TPropertyType>, new()
    {
        protected PropertyInfo GetPropertyInfo()
        {
            return typeof(IHasValue<TPropertyType>).GetProperties().First();
        }

        protected TDeclaringType GetInstance(TPropertyType value)
        {
            return new TDeclaringType { Value = value };
        }

        protected Func<object, object> GetNonGenericFunc(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo.DeclaringType == typeof(TDeclaringType) || (propertyInfo.DeclaringType.IsInterface && typeof(TDeclaringType).GetInterfaces().Any(i => i == propertyInfo.DeclaringType)));
            Debug.Assert(propertyInfo.PropertyType == typeof(TPropertyType));

            return propertyInfo.GetGetFunc();
        }
    }

    public abstract class TestBaseSuccess<TDeclaringType, TPropertyType>
        : TestBase<TDeclaringType, TPropertyType>
        where TDeclaringType : IHasValue<TPropertyType>, new()
    {
        [Test]
        public void CallingGetGetFuncThrowsNoException()
        {
            Assert.That(() => GetNonGenericFunc(GetPropertyInfo()), Throws.Nothing);
        }

        [Test]
        public void InvokingTheFunctionReturnedByTheGetGetFuncReturnsTheValueOfTheObjectsProperty()
        {
            var instance = GetInstance(GetPropertyValue());

            var getPropertyValue = GetNonGenericFunc(GetPropertyInfo());

            var propertyValue = getPropertyValue(instance);

            Assert.That(propertyValue, Is.EqualTo(instance.Value));
        }

        protected abstract TPropertyType GetPropertyValue();
    }

    public abstract class TestBaseFailure<TDeclaringType, TPropertyType>
        : TestBase<TDeclaringType, TPropertyType>
        where TDeclaringType : IHasValue<TPropertyType>, new()
    {
        [Test]
        public void CallingGetGetFuncThrowsAnException()
        {
            Assert.That(() => GetNonGenericFunc(GetPropertyInfo()), Throws.Exception);
        }
    }

    public abstract class TestBase<TInstance, TProperty, TDeclaringType, TPropertyType>
        : TestBase<TDeclaringType, TPropertyType>
        where TDeclaringType : IHasValue<TPropertyType>, new()
    {
        protected Func<TInstance, TProperty> GetGenericFunc(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo.DeclaringType == typeof(TDeclaringType) || (propertyInfo.DeclaringType.IsInterface && typeof(TDeclaringType).GetInterfaces().Any(i => i == propertyInfo.DeclaringType)));
            Debug.Assert(propertyInfo.PropertyType == typeof(TPropertyType));

            return propertyInfo.GetGetFunc<TInstance, TProperty>();
        }
    }

    public abstract class TestBaseSuccess<TInstance, TProperty, TDeclaringType, TPropertyType>
        : TestBase<TInstance, TProperty, TDeclaringType, TPropertyType>
        where TDeclaringType : IHasValue<TPropertyType>, TInstance, new()
    {
        [Test]
        public void CallingGetGetFuncThrowsNoException()
        {
            Assert.That(() => GetNonGenericFunc(GetPropertyInfo()), Throws.Nothing);
        }

        [Test]
        public void InvokingTheFunctionReturnedByTheGetGetFuncReturnsTheValueOfTheObjectsProperty()
        {
            var instance = GetInstance(GetPropertyValue());

            var getPropertyValue = GetGenericFunc(GetPropertyInfo());

            var propertyValue = getPropertyValue(instance);

            Assert.That(propertyValue, Is.EqualTo(instance.Value));
        }

        protected abstract TPropertyType GetPropertyValue();
    }

    public abstract class TestBaseFailure<TInstance, TProperty, TDeclaringType, TPropertyType>
        : TestBase<TInstance, TProperty, TDeclaringType, TPropertyType>
        where TDeclaringType : IHasValue<TPropertyType>, new()
    {
        [Test]
        public void CallingGetGetFuncThrowsAnException()
        {
            Assert.That(() => GetGenericFunc(GetPropertyInfo()), Throws.Exception);
        }
    }

    public interface IHasValue<T>
    {
        T Value { get; set; }
    }

    public class ClassWithValueTypeProperty : IHasValue<int>
    {
        public int Value { get; set; }
    }

    public class ClassWithReferenceTypeProperty : IHasValue<Foo>
    {
        public Foo Value { get; set; }
    }

    public class ClassWithObjectProperty : IHasValue<object>
    {
        public object Value { get; set; }
    }

    public class Foo
    {
    }

    public class FooDerived : Foo
    {
    }
}
