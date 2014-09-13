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
