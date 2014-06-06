using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Rock.Utilities;

// ReSharper disable once CheckNamespace
namespace AttributeLocatorTests
{
    public class TheFindMembersDecoratedWithMethod
    {
        [Test]
        public void FindsPublicClasses()
        {
            var result = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly);

            Assert.That(result, Contains.Item(typeof(PublicClass)));
        }

        [Test]
        public void FindsNonPublicClasses()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = (Type)typeof(PublicClass).GetMember("PrivateClass", BindingFlags.NonPublic).Single();

            Assert.That(results, Contains.Item(expected));
        }

        [Test]
        public void FindsPublicStructs()
        {
            var result = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly);

            Assert.That(result, Contains.Item(typeof(PublicStruct)));
        }

        [Test]
        public void FindsNonPublicStructs()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = (Type)typeof(PublicClass).GetMember("PrivateStruct", BindingFlags.NonPublic).Single();

            Assert.That(results, Contains.Item(expected));
        }

        [Test]
        public void FindsPublicDelegates()
        {
            var result = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly);

            Assert.That(result, Contains.Item(typeof(PublicDelegate)));
        }

        [Test]
        public void FindsNonPublicDelegates()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = (Type)typeof(PublicClass).GetMember("PrivateDelegate", BindingFlags.NonPublic).Single();

            Assert.That(results, Contains.Item(expected));
        }

        [Test]
        public void FindsPublicConstructors()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetConstructors().Single();

            Assert.That(results.OfType<ConstructorInfo>().SingleOrDefault(c => AreEqual(c, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsNonPublicConstructors()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();

            Assert.That(results.OfType<ConstructorInfo>().SingleOrDefault(c => AreEqual(c, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsPublicMethods()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetMethod("PublicMethod");

            Assert.That(results.OfType<MethodInfo>().SingleOrDefault(m => AreEqual(m, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsNonPublicMethods()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(results.OfType<MethodInfo>().SingleOrDefault(m => AreEqual(m, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsPublicProperties()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetProperty("PublicProperty");

            Assert.That(results.OfType<PropertyInfo>().SingleOrDefault(p => AreEqual(p, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsNonPublicProperties()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetProperty("PrivateProperty", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(results.OfType<PropertyInfo>().SingleOrDefault(p => AreEqual(p, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsPublicFields()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetField("PublicField");

            Assert.That(results.OfType<FieldInfo>().SingleOrDefault(f => AreEqual(f, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsNonPublicFields()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetField("PrivateField", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(results.OfType<FieldInfo>().SingleOrDefault(f => AreEqual(f, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsPublicEvents()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetEvent("PublicEvent");

            Assert.That(results.OfType<EventInfo>().SingleOrDefault(e => AreEqual(e, expected)), Is.Not.Null);
        }

        [Test]
        public void FindsNonPublicEvents()
        {
            var results = AttributeLocator.FindMembersDecoratedWith<ExampleAttribute>(GetType().Assembly).ToList();

            var expected = typeof(PublicClass).GetEvent("PrivateEvent", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(results.OfType<EventInfo>().SingleOrDefault(e => AreEqual(e, expected)), Is.Not.Null);
        }

        private static bool AreEqual(MethodBase method, MethodBase expected)
        {
            return
                method.Name == expected.Name
                && method.DeclaringType == expected.DeclaringType
                && method.GetParameters().Select(p => p.ParameterType)
                    .SequenceEqual(
                        expected.GetParameters().Select(p => p.ParameterType));
        }

        private static bool AreEqual(PropertyInfo property, PropertyInfo expected)
        {
            return property.Name == expected.Name
                   && property.PropertyType == expected.PropertyType
                   && property.DeclaringType == expected.DeclaringType;
        }

        private static bool AreEqual(FieldInfo property, FieldInfo expected)
        {
            return property.Name == expected.Name
                   && property.FieldType == expected.FieldType
                   && property.DeclaringType == expected.DeclaringType;
        }

        private static bool AreEqual(EventInfo @event, EventInfo expected)
        {
            return @event.Name == expected.Name
                   && @event.EventHandlerType == expected.EventHandlerType
                   && @event.DeclaringType == expected.DeclaringType;
        }
    }


    // ReSharper disable UnusedParameter.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedField.Compiler
    // ReSharper disable EventNeverInvoked
    // ReSharper disable EventNeverSubscribedTo.Local
    // ReSharper disable InconsistentNaming
    [Example]
    public class PublicClass
    {
        [Example]
        private class PrivateClass
        {
        }

        [Example]
        private struct PrivateStruct
        {
        }

        [Example]
        private delegate void PrivateDelegate();

        [Example]
        public PublicClass(int i)
        {
        }

        [Example]
        private PublicClass(string s)
        {
        }

        [Example]
        public void PublicMethod()
        {
        }

        [Example]
        private void PrivateMethod()
        {
        }

        [Example]
        public string PublicProperty { get; set; }

        [Example]
        private string PrivateProperty { get; set; }

        [Example]
        public string PublicField;

        [Example]
        private string PrivateField;

        [Example]
        public event EventHandler PublicEvent;

        [Example]
        private event EventHandler PrivateEvent;
    }
    // ReSharper restore UnusedParameter.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedField.Compiler
    // ReSharper restore EventNeverInvoked
    // ReSharper restore EventNeverSubscribedTo.Local
    // ReSharper restore InconsistentNaming

    [Example]
    public struct PublicStruct
    {
    }

    [Example]
    public delegate void PublicDelegate();

    public class ExampleAttribute : Attribute
    {
    }

}
