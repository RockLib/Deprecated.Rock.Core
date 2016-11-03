using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using Rock.Reflection.Emit;
using Rock.Serialization;

namespace Rock.Core.UnitTests.Serialization
{
    public class XmlDeserializationProxyEngineConstructorComparerTests
    {
        public interface IFoo { }
        public class Foo : IFoo { }
        public interface IBar { }
        public class Bar : IBar { }
        public interface IBaz { }

        [Test]
        public void Rule2_CompareA()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void Rule2_CompareB()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.GreaterThan(0));
        }

        [Test]
        public void Rule2_Equals()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var equals = comparer.Equals(constructors.Item1, constructors.Item2);

            Assert.That(equals, Is.False);
        }

        [Test]
        public void Rule3_CompareA()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(null, typeof(IBar), "bar") });

            var comparer = GetConstructorComparer(new Foo());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void Rule3_CompareB()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(null, typeof(IBar), "bar") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.GreaterThan(0));
        }

        [Test]
        public void Rule3_Equals()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(null, typeof(IBar), "bar") });

            var comparer = GetConstructorComparer(new Foo());

            var equals = comparer.Equals(constructors.Item1, constructors.Item2);

            Assert.That(equals, Is.False);
        }

        [Test]
        public void Rule4_CompareA()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(Foo), "foo") });

            var comparer = GetConstructorComparer(new Foo());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.GreaterThan(0));
        }

        [Test]
        public void Rule4_CompareB()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(Foo), "foo") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void Rule4_Equals()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(Foo), "foo") });

            var comparer = GetConstructorComparer(new Foo());

            var equals = comparer.Equals(constructors.Item1, constructors.Item2);

            Assert.That(equals, Is.False);
        }

        [Test]
        public void Rule2vsRule3_CompareA()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar"), new Parameter(null, typeof(IBaz), "baz") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void Rule2vsRule3_CompareB()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar"), new Parameter(null, typeof(IBaz), "baz") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.GreaterThan(0));
        }

        [Test]
        public void Rule2vsRule3_Equals()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar"), new Parameter(null, typeof(IBaz), "baz") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var equals = comparer.Equals(constructors.Item1, constructors.Item2);

            Assert.That(equals, Is.False);
        }

        [Test]
        public void Rule2vsRule4_CompareA()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar") },
                new[] { new Parameter(typeof(Foo), "foo") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void Rule2vsRule4_CompareB()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.GreaterThan(0));
        }

        [Test]
        public void Rule2vsRule4_Equals()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo"), new Parameter(typeof(IBar), "bar") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo(), new Bar());

            var equals = comparer.Equals(constructors.Item1, constructors.Item2);

            Assert.That(equals, Is.False);
        }

        [Test]
        public void Rule3vsRule4_CompareA()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(Foo), "foo"), new Parameter(null, typeof(IBar), "bar") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.LessThan(0));
        }

        [Test]
        public void Rule3vsRule4_CompareB()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(IFoo), "foo") },
                new[] { new Parameter(typeof(Foo), "foo"), new Parameter(null, typeof(IBar), "bar") });

            var comparer = GetConstructorComparer(new Foo());

            var comparison = comparer.Compare(constructors.Item1, constructors.Item2);

            Assert.That(comparison, Is.GreaterThan(0));
        }

        [Test]
        public void Rule3vsRule4_Equals()
        {
            var constructors = GetConstructors(
                new[] { new Parameter(typeof(Foo), "foo"), new Parameter(null, typeof(IBar), "bar") },
                new[] { new Parameter(typeof(IFoo), "foo") });

            var comparer = GetConstructorComparer(new Foo());

            var equals = comparer.Equals(constructors.Item1, constructors.Item2);

            Assert.That(equals, Is.False);
        }

        private Tuple<ConstructorInfo, ConstructorInfo> GetConstructors(Parameter[] constructor1, Parameter[] constructor2)
        {
            var type = Create.Class(
                Define.Constructor(constructor1),
                Define.Constructor(constructor2));

            var constructors = type.GetConstructors();

            return Tuple.Create(constructors[0], constructors[1]);
        }

        private XmlDeserializationProxyEngine<object>.ConstructorComparer GetConstructorComparer(
            params object[] availableObjects)
        {
            var engine = new XmlDeserializationProxyEngine<object>(new object(), typeof(object), null);
            engine.AdditionalXElements.AddRange(availableObjects.Select(o => new XElement(o.GetType().Name, new XAttribute("type", o.GetType().AssemblyQualifiedName))));
            var comparer = new XmlDeserializationProxyEngine<object>.ConstructorComparer(engine, null);
            return comparer;
        }
    }
}