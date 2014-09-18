using System.Collections.Generic;
using NUnit.Framework;
using Rock.Conversion;

// ReSharper disable once CheckNamespace
namespace ConvertsToDictionaryOfStringToTValueTests
{
    public class GivenANullObject
    {
        [Test]
        public void WhenObjectIsNullReturnsNull()
        {
            var converter1 = new ConvertsToDictionaryOfStringTo<string>();
            var converter2 = new ConvertsToDictionaryOfStringTo<object>();
            var converter3 = new ConvertsToDictionaryOfStringTo<Foo>();

            Assert.That(converter1.Convert(null), Is.Null);
            Assert.That(converter2.Convert(null), Is.Null);
            Assert.That(converter3.Convert(null), Is.Null);
        }
    }

    namespace GivenAGenericArgumentOfTypeString
    {
        public class TheConvertMethod
        {
            private ConvertsToDictionaryOfStringTo<string> _converter;

            [SetUp]
            public void Setup()
            {
                _converter = new ConvertsToDictionaryOfStringTo<string>();
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToStringReturnsIt()
            {
                var dictionary =
                    new Dictionary<string, string>
                    {
                        { "foo", "bar" },
                        { "baz", "qux" },
                        { "corge", null },
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result, Is.SameAs(dictionary));
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToObjectCopiesKeysAndConvertedValuesToNewDictionary()
            {
                var dictionary =
                    new Dictionary<string, object>
                    {
                        { "foo", 123 },
                        { "bar", "baz" },
                        { "qux", null }
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result.Count, Is.EqualTo(dictionary.Count));
                Assert.That(result["foo"], Is.EqualTo(dictionary["foo"].ToString()));
                Assert.That(result["bar"], Is.EqualTo(dictionary["bar"].ToString()));
                Assert.That(result["qux"], Is.Null);
            }

            [Test]
            public void WhenTheObjectIsNotAnIDictionaryOfStringToAnythingCopiesPropertyValuesToNewDictionary()
            {
                var bar = new Bar
                {
                    Foo = 123,
                    Baz = "abc",
                };

                var result = _converter.Convert(bar);

                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result["Foo"], Is.EqualTo("123"));
                Assert.That(result["Baz"], Is.EqualTo("abc"));
            }
        }
    }

    namespace GivenAGenericArgumentOfTypeObject
    {
        public class TheConvertMethod
        {
            private ConvertsToDictionaryOfStringTo<object> _converter;

            [SetUp]
            public void Setup()
            {
                _converter = new ConvertsToDictionaryOfStringTo<object>();
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToobjectReturnsIt()
            {
                var dictionary =
                    new Dictionary<string, object>
                    {
                        { "foo", 123 },
                        { "bar", "baz" },
                        { "qux", null }
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result, Is.SameAs(dictionary));
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToStringCopiesKeysAndValuesToNewDictionary()
            {
                var dictionary =
                    new Dictionary<string, string>
                    {
                        { "foo", "bar" },
                        { "baz", "qux" },
                        { "corge", null },
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result.Count, Is.EqualTo(dictionary.Count));
                Assert.That(result["foo"], Is.EqualTo(dictionary["foo"]));
                Assert.That(result["baz"], Is.EqualTo(dictionary["baz"]));
                Assert.That(result["corge"], Is.Null);
            }
        }
    }

    namespace GivenAGenericArgumentOfAnArbitraryReferenceType
    {
        public class TheConvertMethod
        {
            private ConvertsToDictionaryOfStringTo<Foo> _converter;

            [SetUp]
            public void Setup()
            {
                _converter = new ConvertsToDictionaryOfStringTo<Foo>();
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToTValueReturnsIt()
            {
                var dictionary =
                    new Dictionary<string, Foo>
                    {
                        { "foo", new Foo { Bar = "abc" } },
                        { "bar", new FooDerived { Bar = "xyz", Baz = 123 } },
                        { "qux", null }
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result, Is.SameAs(dictionary));
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToAClassThatInheritsFromTValueCopyCopiesKeysAndValuesToNewDictionary()
            {
                var dictionary =
                    new Dictionary<string, FooDerived>
                    {
                        { "foo", new FooDerived { Bar = "abc" } },
                        { "bar", null }
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result.Count, Is.EqualTo(dictionary.Count));
                Assert.That(result["foo"], Is.EqualTo(dictionary["foo"]));
                Assert.That(result["bar"], Is.Null);
            }

            [Test]
            public void WhenTheObjectIsNotAnIDictionaryOfStringToAnythingCopiesPropertyValuesToNewDictionary()
            {
                var baz = new Baz
                {
                    Foo = new FooDerived()
                };

                var result = _converter.Convert(baz);

                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result["Foo"], Is.SameAs(baz.Foo));
            }
        }
    }

    namespace GivenAGenericArgumentOfAnArbitraryValueType
    {
        public class TheConvertMethod
        {
            private ConvertsToDictionaryOfStringTo<MyInt32> _converter;

            [SetUp]
            public void Setup()
            {
                _converter = new ConvertsToDictionaryOfStringTo<MyInt32>();
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToTValueReturnsIt()
            {
                var dictionary =
                    new Dictionary<string, MyInt32>
                    {
                        { "foo", new MyInt32(1) },
                        { "bar", new MyInt32(2) }
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result, Is.SameAs(dictionary));
            }

            [Test]
            public void WhenTheObjectIsAnIDictionaryOfStringToATypeThatCanBeConvertedToTValueCopyCopiesKeysAndValuesToNewDictionary()
            {
                var dictionary =
                    new Dictionary<string, int>
                    {
                        { "foo", 1 },
                        { "bar", 2 }
                    };

                var result = _converter.Convert(dictionary);

                Assert.That(result.Count, Is.EqualTo(dictionary.Count));
                Assert.That(result["foo"].Value, Is.EqualTo(dictionary["foo"]));
                Assert.That(result["bar"].Value, Is.EqualTo(dictionary["bar"]));
            }

            [Test]
            public void WhenTheObjectIsNotAnIDictionaryOfStringToAnythingCopiesPropertyValuesToNewDictionary()
            {
                var qux = new Qux
                {
                    Foo = new MyInt32(123)
                };

                var result = _converter.Convert(qux);

                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result["Foo"], Is.InstanceOf<MyInt32>());
                Assert.That(result["Foo"].Value, Is.EqualTo(123));
            }
        }
    }

    public class Foo
    {
        public string Bar { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Bar == ((Foo)obj).Bar;
        }
    }

    public class FooDerived : Foo
    {
        public int Baz { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Bar == ((FooDerived)obj).Bar && Baz == ((FooDerived)obj).Baz;
        }
    }

    public class Bar
    {
        public int Foo { get; set; }
        public string Baz { get; set; }
    }

    public class Baz
    {
        public FooDerived Foo { get; set; }
    }

    public class Qux
    {
        public MyInt32 Foo { get; set; }
    }

    public struct MyInt32
    {
        private readonly int _value;

        public MyInt32(int value)
        {
            _value = value;
        }

        public int Value { get { return _value; } }

        public static explicit operator int(MyInt32 i)
        {
            return i.Value;
        }

        public static explicit operator MyInt32(int i)
        {
            return new MyInt32(i);
        }
    }
}