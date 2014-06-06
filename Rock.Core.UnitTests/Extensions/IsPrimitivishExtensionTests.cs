using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rock.Defaults.Implementation;
using Rock.Extensions;

// ReSharper disable once CheckNamespace
namespace IsPrimitivishExtensionTests
{
    public class TheIsPrimitivishExtensionMethod
    {
        // The primitive types, according to http://msdn.microsoft.com/en-us/library/system.type.isprimitive.aspx
        private readonly Type[] _primitiveTypes = { typeof(Boolean), typeof(Byte), typeof(SByte), typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32), typeof(Int64), typeof(UInt64), typeof(IntPtr), typeof(UIntPtr), typeof(Char), typeof(Double), typeof(Single) };

        private readonly Type[] _customPrimitivishTypes = { typeof(Foo), typeof(Guid), typeof(string) };

        [SetUp]
        public void Setup()
        {
            Default.SetPrimitivishTypes(GetCustomPrimitivishTypes);
        }

        [TearDown]
        public void TearDown()
        {
            Default.RestoreDefaultPrimitivishTypes();
        }

        [TestCaseSource("GetPrimitiveTypes")]
        public void ReturnsTrueIfTheTypeIsPrimitive(Type primitiveType)
        {
            Assert.That(primitiveType.IsPrimitivish(), Is.True);
        }

        [Test]
        public void ReturnsTrueIfTheTypeIsAnEnum()
        {
            Assert.That(typeof(BazEnum).IsPrimitivish(), Is.True);
        }

        [TestCaseSource("GetCustomPrimitivishTypes")]
        public void ReturnsTrueIfTheTypeIsOneOfTheDefaultPrimitivishTypes(Type customPrimitivishType)
        {
            Assert.That(customPrimitivishType.IsPrimitivish(), Is.True);
        }

        [TestCaseSource("GetNullablePrimitiveTypes")]
        public void ReturnsTrueIfTheTypeIsNullableOfTypePrimitive(Type nullablePrimitiveType)
        {
            Assert.That(nullablePrimitiveType.IsPrimitivish(), Is.True);
        }

        [Test]
        public void ReturnsTrueIfTheTypeIsANullableEnum()
        {
            Assert.That(typeof(BazEnum?).IsPrimitivish(), Is.True);
        }

        [TestCaseSource("GetNullableCustomPrimitivishTypes")]
        public void ReturnsTrueIfTheTypeIsANullableOfOneOfTheDefaultPrimitivishTypes(Type nullableCustomPrimitivishType)
        {
            Assert.That(nullableCustomPrimitivishType.IsPrimitivish(), Is.True);
        }

        [TestCase(typeof(Bar))]
        [TestCase(typeof(Bar?))]
        [TestCase(typeof(Exception))]
        public void ReturnsFalseIfTheTypeDoesNotMeetAnyOfTheOtherConditions(Type nonPrimitivishType)
        {
            Assert.That(nonPrimitivishType.IsPrimitivish(), Is.False);
        }

        private IEnumerable<Type> GetPrimitiveTypes()
        {
            return _primitiveTypes;
        }

        private IEnumerable<Type> GetCustomPrimitivishTypes()
        {
            return _customPrimitivishTypes;
        }

        private IEnumerable<Type> GetNullablePrimitiveTypes()
        {
            return _primitiveTypes.Select(t => typeof(Nullable<>).MakeGenericType(t));
        }

        private IEnumerable<Type> GetNullableCustomPrimitivishTypes()
        {
            return _customPrimitivishTypes.Where(t => t.IsValueType).Select(t => typeof(Nullable<>).MakeGenericType(t));
        }

        public struct Foo
        {
        }

        public struct Bar
        {
        }

        public enum BazEnum
        {
        }
    }
}
