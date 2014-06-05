using System;
using Moq;
using NUnit.Framework;
using Rock.Defaults.Implementation;

namespace DefaultHelperTests.Implementation
{
    public abstract class DefaultTestBase<T, TDefault>
        where T : class
        where TDefault : class, T
    {
        [SetUp]
        public void Setup()
        {
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException("DefaultTestBase must have a generic type parameter that is an interface");
            }

            RestoreDefault();
        }

        [TearDown]
        public void TearDown()
        {
            RestoreDefault();
        }

        protected abstract string PropertyName { get; }

        [Test]
        public void HasAPropertyWhoseNameMatchesTheTypeInQuestion()
        {
            var property = typeof(Default).GetProperty(PropertyName, typeof(T));

            Assert.That(property, Is.Not.Null);
        }

        [Test]
        public void HasADefaultPropertyWhoseNameMatchesTheTypeInQuestion()
        {
            var property = typeof(Default).GetProperty("Default" + PropertyName, typeof(T));

            Assert.That(property, Is.Not.Null);
        }

        [Test]
        public void HasASetMethodWhoseNameMatchesTheTypeInQuestionAndAParameterThatMatchesTheTypeInQuestion()
        {
            var method = typeof(Default).GetMethod("Set" + PropertyName, new [] { typeof(Func<>).MakeGenericType(typeof(T)) });

            Assert.That(method, Is.Not.Null);
            Assert.That(method.ReturnType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void HasARestoreDefaultMethodWhoseNameMatchesTheTypeInQuestionAndHasNoParameters()
        {
            var method = typeof(Default).GetMethod("RestoreDefault" + PropertyName, Type.EmptyTypes);

            Assert.That(method, Is.Not.Null);
            Assert.That(method.ReturnType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void TheDefaultValueIsTheCorrectType()
        {
            var defaultValue = GetDefaultValue();

            Assert.That(defaultValue.GetType(), Is.EqualTo(typeof(TDefault)));
        }

        [Test]
        public void TheInitialValueIsTheSameInstanceAsTheDefaultValue()
        {
            var initialValue = GetValue();
            var defaultValue = GetDefaultValue();

            Assert.That(initialValue, Is.SameAs(defaultValue));
        }

        [Test]
        public void CanSetTheCurrentValue()
        {
            var mockValue = new Mock<T>();

            SetValue(() => mockValue.Object);

            var value = GetValue();

            Assert.That(value, Is.SameAs(mockValue.Object));
        }

        [Test]
        public void CanRestoreTheDefaultValue()
        {
            var mockValue = new Mock<T>();

            SetValue(() => mockValue.Object);

            var value = GetValue();

            Assert.That(value, Is.SameAs(mockValue.Object)); // Sanity check

            RestoreDefault();

            value = GetValue();
            var defaultValue = GetDefaultValue();

            Assert.That(value, Is.SameAs(defaultValue));
        }

        protected object GetValue()
        {
            var valueProperty = typeof(Default).GetProperty(PropertyName, typeof(T));

            if (valueProperty != null)
            {
                return valueProperty.GetValue(null);
            }

            return new object();
        }

        protected object GetDefaultValue()
        {
            var defaultValueProperty = typeof(Default).GetProperty("Default" + PropertyName, typeof(T));

            if (defaultValueProperty != null)
            {
                return defaultValueProperty.GetValue(null);
            }

            return new object();
        }

        protected void SetValue(Func<T> getValue)
        {
            var setValueMethod = typeof(Default).GetMethod("Set" + PropertyName, new [] { typeof(Func<>).MakeGenericType(typeof(T)) });

            if (setValueMethod != null)
            {
                setValueMethod.Invoke(null, new object[] { getValue });
            }
        }

        protected void RestoreDefault()
        {
            var restoreDefaultMethod = typeof(Default).GetMethod("RestoreDefault" + PropertyName, Type.EmptyTypes);

            if (restoreDefaultMethod != null)
            {
                restoreDefaultMethod.Invoke(null, null);
            }
        }
    }
}