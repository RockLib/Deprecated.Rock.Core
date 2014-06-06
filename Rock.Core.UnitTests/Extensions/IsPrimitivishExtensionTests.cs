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
        public class WithoutExtraPrimitivishTypesParameter : IsPrimitivishExtensionTestsBase
        {
            [SetUp]
            public void Setup()
            {
                Default.SetExtraPrimitivishTypes(GetExtraPrimitivishTypes);
            }

            [TearDown]
            public void TearDown()
            {
                Default.RestoreDefaultExtraPrimitivishTypes();
            }

            protected override bool IsPrimitivish(Type type)
            {
                return type.IsPrimitivish();
            }
        }

        public class WithExtraPrimitivishTypesParameter : IsPrimitivishExtensionTestsBase
        {
            protected override bool IsPrimitivish(Type type)
            {
                return type.IsPrimitivish(GetExtraPrimitivishTypes());
            }
        }

        public abstract class IsPrimitivishExtensionTestsBase
        {
            // The primitive types, according to http://msdn.microsoft.com/en-us/library/system.type.isprimitive.aspx
            private static readonly Type[] _primitiveTypes =
            {
                typeof(Boolean), typeof(Byte), typeof(SByte),
                typeof(Int16), typeof(UInt16), typeof(Int32),
                typeof(UInt32), typeof(Int64), typeof(UInt64),
                typeof(IntPtr), typeof(UIntPtr), typeof(Char),
                typeof(Double), typeof(Single)
            };

            /// <summary>
            /// When overridden in an inheriting class, makes a call to the IsPrimitivish extension method.
            /// </summary>
            protected abstract bool IsPrimitivish(Type type);

            [TestCaseSource("GetPrimitiveTypes")]
            public void ReturnsTrueIfTheTypeIsPrimitive(Type primitiveType)
            {
                Assert.That(IsPrimitivish(primitiveType), Is.True);
            }

            [Test]
            public void ReturnsTrueIfTheTypeIsAnEnum()
            {
                Assert.That(IsPrimitivish(typeof(BazEnum)), Is.True);
            }

            [TestCaseSource("GetDefaultPrimitivishTypes")]
            public void ReturnsTrueIfTheTypeIsOneOfTheDefaultPrimitivishTypes(Type defaultPrimitivishType)
            {
                Assert.That(IsPrimitivish(defaultPrimitivishType), Is.True);
            }

            [TestCaseSource("GetExtraPrimitivishTypes")]
            public void ReturnsTrueIfTheTypeIsOneOfTheExtraPrimitivishTypes(Type extraPrimitivishType)
            {
                Assert.That(IsPrimitivish(extraPrimitivishType), Is.True);
            }

            [TestCaseSource("GetNullablePrimitiveTypes")]
            public void ReturnsTrueIfTheTypeIsNullableOfTypePrimitive(Type nullablePrimitiveType)
            {
                Assert.That(IsPrimitivish(nullablePrimitiveType), Is.True);
            }

            [Test]
            public void ReturnsTrueIfTheTypeIsANullableEnum()
            {
                Assert.That(IsPrimitivish(typeof(BazEnum?)), Is.True);
            }

            [TestCaseSource("GetNullableDefaultPrimitivishTypes")]
            public void ReturnsTrueIfTheTypeIsANullableOfOneOfTheDefaultPrimitivishTypes(Type nullableDefaultPrimitivishType)
            {
                Assert.That(IsPrimitivish(nullableDefaultPrimitivishType), Is.True);
            }

            [TestCaseSource("GetNullableExtraPrimitivishTypes")]
            public void ReturnsTrueIfTheTypeIsANullableOfOneOfTheExtraPrimitivishTypes(Type nullableExtraPrimitivishType)
            {
                Assert.That(IsPrimitivish(nullableExtraPrimitivishType), Is.True);
            }

            [TestCase(typeof(Bar))]
            [TestCase(typeof(Bar?))]
            [TestCase(typeof(Exception))]
            public void ReturnsFalseIfTheTypeDoesNotMeetAnyOfTheOtherConditions(Type nonPrimitivishType)
            {
                Assert.That(IsPrimitivish(nonPrimitivishType), Is.False);
            }

            protected IEnumerable<Type> GetPrimitiveTypes()
            {
                return _primitiveTypes;
            }

            protected IEnumerable<Type> GetDefaultPrimitivishTypes()
            {
                return IsPrimitivishExtension._defaultPrimitivishTypes;
            }

            protected IEnumerable<Type> GetExtraPrimitivishTypes()
            {
                yield return typeof(Foo);
            }

            protected IEnumerable<Type> GetNullablePrimitiveTypes()
            {
                return _primitiveTypes.Select(t => typeof(Nullable<>).MakeGenericType(t));
            }

            protected IEnumerable<Type> GetNullableDefaultPrimitivishTypes()
            {
                return GetDefaultPrimitivishTypes().Where(t => t.IsValueType).Select(t => typeof(Nullable<>).MakeGenericType(t));
            }

            protected IEnumerable<Type> GetNullableExtraPrimitivishTypes()
            {
                return GetExtraPrimitivishTypes().Where(t => t.IsValueType).Select(t => typeof(Nullable<>).MakeGenericType(t));
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
}
