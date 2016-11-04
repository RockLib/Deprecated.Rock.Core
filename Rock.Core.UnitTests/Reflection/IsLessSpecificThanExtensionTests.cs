using System;
using NUnit.Framework;
using Rock.Reflection;

namespace Rock.Core.Reflection.IsLessSpecificThanExtensionTests
{
    public class TheIsLessSpecificThanExtensionMethod
    {
        [Test]
        public void ReturnTrueWhenTheComparisonParameterImplementsTheThisParameter()
        {
            Assert.That(typeof(IFoo).IsLessSpecificThan(typeof(Foo)), Is.True);
        }

        [Test]
        public void ReturnTrueWhenTheComparisonParameterInheritsFromTheThisParameter()
        {
            Assert.That(typeof(FooBase).IsLessSpecificThan(typeof(Foo)), Is.True);
        }

        [Test]
        public void ReturnFalseWhenTheComparisonParameterIsTheSameAsTheThisParameter()
        {
            Assert.That(typeof(Foo).IsLessSpecificThan(typeof(Foo)), Is.False);
        }

        [Test]
        public void ReturnFalseWhenTheTwoTypesAreCompletelyUnrelated()
        {
            Assert.That(typeof(IFoo).IsLessSpecificThan(typeof(Bar)), Is.False);
        }

        [TestCase(typeof(short), typeof(byte))]
        [TestCase(typeof(short), typeof(sbyte))]
        [TestCase(typeof(ushort), typeof(byte))]
        [TestCase(typeof(int), typeof(byte))]
        [TestCase(typeof(int), typeof(sbyte))]
        [TestCase(typeof(int), typeof(short))]
        [TestCase(typeof(int), typeof(ushort))]
        [TestCase(typeof(uint), typeof(byte))]
        [TestCase(typeof(uint), typeof(ushort))]
        [TestCase(typeof(long), typeof(byte))]
        [TestCase(typeof(long), typeof(sbyte))]
        [TestCase(typeof(long), typeof(short))]
        [TestCase(typeof(long), typeof(ushort))]
        [TestCase(typeof(long), typeof(int))]
        [TestCase(typeof(long), typeof(uint))]
        [TestCase(typeof(ulong), typeof(byte))]
        [TestCase(typeof(ulong), typeof(ushort))]
        [TestCase(typeof(ulong), typeof(uint))]
        [TestCase(typeof(float), typeof(byte))]
        [TestCase(typeof(float), typeof(sbyte))]
        [TestCase(typeof(float), typeof(short))]
        [TestCase(typeof(float), typeof(ushort))]
        [TestCase(typeof(float), typeof(int))]
        [TestCase(typeof(float), typeof(uint))]
        [TestCase(typeof(float), typeof(long))]
        [TestCase(typeof(float), typeof(ulong))]
        [TestCase(typeof(double), typeof(byte))]
        [TestCase(typeof(double), typeof(sbyte))]
        [TestCase(typeof(double), typeof(short))]
        [TestCase(typeof(double), typeof(ushort))]
        [TestCase(typeof(double), typeof(int))]
        [TestCase(typeof(double), typeof(uint))]
        [TestCase(typeof(double), typeof(long))]
        [TestCase(typeof(double), typeof(ulong))]
        [TestCase(typeof(double), typeof(float))]
        public void ReturnTrueForNumericTypesThatCanBeDirectlyAssigned(Type thisType, Type comparisonType)
        {
            var nullableThisType = typeof(Nullable<>).MakeGenericType(thisType);
            var nullableComparisonType = typeof(Nullable<>).MakeGenericType(comparisonType);

            Assert.That(thisType.IsLessSpecificThan(comparisonType), Is.True);
            Assert.That(nullableThisType.IsLessSpecificThan(comparisonType), Is.True);
            Assert.That(thisType.IsLessSpecificThan(nullableComparisonType), Is.True);
            Assert.That(nullableThisType.IsLessSpecificThan(nullableComparisonType), Is.True);
        }

        [TestCase(typeof(byte), typeof(byte))]
        [TestCase(typeof(byte), typeof(sbyte))]
        [TestCase(typeof(byte), typeof(short))]
        [TestCase(typeof(byte), typeof(ushort))]
        [TestCase(typeof(byte), typeof(int))]
        [TestCase(typeof(byte), typeof(uint))]
        [TestCase(typeof(byte), typeof(long))]
        [TestCase(typeof(byte), typeof(ulong))]
        [TestCase(typeof(byte), typeof(float))]
        [TestCase(typeof(byte), typeof(double))]
        [TestCase(typeof(sbyte), typeof(byte))]
        [TestCase(typeof(sbyte), typeof(sbyte))]
        [TestCase(typeof(sbyte), typeof(short))]
        [TestCase(typeof(sbyte), typeof(ushort))]
        [TestCase(typeof(sbyte), typeof(int))]
        [TestCase(typeof(sbyte), typeof(uint))]
        [TestCase(typeof(sbyte), typeof(long))]
        [TestCase(typeof(sbyte), typeof(ulong))]
        [TestCase(typeof(sbyte), typeof(float))]
        [TestCase(typeof(sbyte), typeof(double))]
        [TestCase(typeof(short), typeof(short))]
        [TestCase(typeof(short), typeof(ushort))]
        [TestCase(typeof(short), typeof(int))]
        [TestCase(typeof(short), typeof(uint))]
        [TestCase(typeof(short), typeof(long))]
        [TestCase(typeof(short), typeof(ulong))]
        [TestCase(typeof(short), typeof(float))]
        [TestCase(typeof(short), typeof(double))]
        [TestCase(typeof(ushort), typeof(sbyte))]
        [TestCase(typeof(ushort), typeof(short))]
        [TestCase(typeof(ushort), typeof(ushort))]
        [TestCase(typeof(ushort), typeof(int))]
        [TestCase(typeof(ushort), typeof(uint))]
        [TestCase(typeof(ushort), typeof(long))]
        [TestCase(typeof(ushort), typeof(ulong))]
        [TestCase(typeof(ushort), typeof(float))]
        [TestCase(typeof(ushort), typeof(double))]
        [TestCase(typeof(int), typeof(int))]
        [TestCase(typeof(int), typeof(uint))]
        [TestCase(typeof(int), typeof(long))]
        [TestCase(typeof(int), typeof(ulong))]
        [TestCase(typeof(int), typeof(float))]
        [TestCase(typeof(int), typeof(double))]
        [TestCase(typeof(uint), typeof(sbyte))]
        [TestCase(typeof(uint), typeof(short))]
        [TestCase(typeof(uint), typeof(uint))]
        [TestCase(typeof(uint), typeof(int))]
        [TestCase(typeof(uint), typeof(ulong))]
        [TestCase(typeof(uint), typeof(float))]
        [TestCase(typeof(uint), typeof(double))]
        [TestCase(typeof(long), typeof(long))]
        [TestCase(typeof(long), typeof(ulong))]
        [TestCase(typeof(long), typeof(float))]
        [TestCase(typeof(long), typeof(double))]
        [TestCase(typeof(ulong), typeof(sbyte))]
        [TestCase(typeof(ulong), typeof(short))]
        [TestCase(typeof(ulong), typeof(int))]
        [TestCase(typeof(ulong), typeof(long))]
        [TestCase(typeof(ulong), typeof(ulong))]
        [TestCase(typeof(ulong), typeof(float))]
        [TestCase(typeof(ulong), typeof(double))]
        [TestCase(typeof(float), typeof(float))]
        [TestCase(typeof(float), typeof(double))]
        [TestCase(typeof(double), typeof(double))]
        public void ReturnFalseForNumericTypesThatCannotBeDirectlyAssigned(Type thisType, Type comparisonType)
        {
            var nullableThisType = typeof(Nullable<>).MakeGenericType(thisType);
            var nullableComparisonType = typeof(Nullable<>).MakeGenericType(comparisonType);

            Assert.That(thisType.IsLessSpecificThan(comparisonType), Is.False);
            Assert.That(nullableThisType.IsLessSpecificThan(comparisonType), Is.False);
            Assert.That(thisType.IsLessSpecificThan(nullableComparisonType), Is.False);
            Assert.That(nullableThisType.IsLessSpecificThan(nullableComparisonType), Is.False);
        }
    }

    public interface IFoo
    {
    }

    public abstract class FooBase
    {
    }

    public class Foo : FooBase, IFoo
    {
    }

    public class Bar
    {
    }
}
