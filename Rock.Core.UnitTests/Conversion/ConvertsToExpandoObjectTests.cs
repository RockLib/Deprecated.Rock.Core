using System.Collections.Generic;
using System.Dynamic;
using NUnit.Framework;
using Rock.Conversion;

// ReSharper disable once CheckNamespace
namespace ExpandoObjectConverterTests
{
    public class TheConvertMethod
    {
        private ConvertsToExpandoObject _converter;
        private object _obj;

        [SetUp]
        public void Setup()
        {
            _converter = new ConvertsToExpandoObject();
        }

        [Test]
        public void WhenObjectIsNullThenReturnNull()
        {
            _obj = null;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void WhenObjectIsPrimitivishThenThrowException()
        {
            _obj = 123;

            Assert.That(() => _converter.Convert(_obj), Throws.Exception);
        }

        [Test]
        public void WhenObjectIsAnExpandoObjectThenReturnIt()
        {
            dynamic d = new ExpandoObject();
            d.Bar = "abc";
            _obj = d;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result, Is.SameAs(_obj));
        }

        [Test]
        public void WhenObjectIsAnIDictionaryOfStringToObjectThenReturnANewExpandoObjectWithValuesCopiedFromTheDictionary()
        {
            var d = new Dictionary<string, object>();
            d["Bar"] = "abc";
            _obj = d;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result.Bar, Is.EqualTo(d["Bar"]));
        }

        [Test]
        public void WhenObjectIsAnIDictionaryOfStringToAnythingThenReturnANewExpandoObjectWithValuesCopiedFromTheDictionary()
        {
            var d = new Dictionary<string, string>();
            d["Bar"] = "abc";
            _obj = d;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result.Bar, Is.EqualTo(d["Bar"]));
        }

        [Test]
        public void WhenObjectIsAnyOtherTypeThenReturnANewExpandoObjectWithValuesCopiedFromPropertiesOfTheObject()
        {
            var f = new Foo { Bar = "abc" };
            _obj = f;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result.Bar, Is.EqualTo(f.Bar));
        }

        [Test]
        public void WhenAPropertyValueOfTheObjectIsNullThenTheCorrespondingPropertyOfTheReturnedExpandoObjectIsNull()
        {
            var f = new Foo { Bar = null };
            _obj = f;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result.Bar, Is.Null);
        }

        [Test]
        public void WhenAPropertyValueOfTheObjectIsPrimitivishThenTheCorrespondingPropertyOfTheReturnedExpandoObjectIsTheSameValue()
        {
            var f = new Foo { Bar = "abc" };
            _obj = f;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result.Bar, Is.EqualTo(f.Bar));
        }

        [Test]
        public void WhenAPropertyValueOfTheObjectIsNotNullNorPrimitivishThenTheCorrespondingPropertyOfTheReturnedExpandoObjectIsAnExpandoObjectThatRepresentsThatValue()
        {
            var f = new Foo { Bar = new Bar { Baz = "xyz" } };
            _obj = f;

            dynamic result = _converter.Convert(_obj);

            Assert.That(result.Bar, Is.InstanceOf<ExpandoObject>());
            Assert.That(result.Bar.Baz, Is.EqualTo("xyz"));
        }

        public class Foo
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public object Bar { get; set; }
        }

        public class Bar
        {
            public string Baz { get; set; }
        }
    }
}