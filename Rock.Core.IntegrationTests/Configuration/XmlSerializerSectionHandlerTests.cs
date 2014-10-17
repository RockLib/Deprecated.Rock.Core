using System;
using System.Configuration;
using System.Xml.Serialization;
using Example;
using NUnit.Framework;
using Rock.Configuration;
using Rock.Extensions;

namespace XmlSerializerSectionHandlerTests
{
    public class SadPaths
    {
        [Test]
        public void MissingTypeAttribute()
        {
            AssertInvalidConfigurationExceptionThrownWhenAccessingSection("Broken1");
        }

        [Test]
        public void InvalidTypeAttribute()
        {
            AssertInvalidConfigurationExceptionThrownWhenAccessingSection("Broken2");
        }

        [Test]
        public void SerializationError()
        {
            AssertInvalidConfigurationExceptionThrownWhenAccessingSection("Broken3");
        }

        private void AssertInvalidConfigurationExceptionThrownWhenAccessingSection(string sectionName)
        {
            try
            {
                ConfigurationManager.GetSection(sectionName);
                Assert.Fail("Expected: exception of type InvalidConfigurationException to be throw. Actual: no exception thrown.");
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    Assert.Fail("Expected: exception witn inner exception of type InvalidConfigurationException to be throw. Actual inner exception: null.");
                }

                if (!(ex.InnerException is InvalidConfigurationException))
                {
                    Assert.Fail("Expected: exception witn inner exception of type InvalidConfigurationException to be throw. Actual inner exception: exception of type '" + ex.InnerException.GetType() + "'.");
                }

                Console.WriteLine(ex.FormatToString());
            }
        }
    }

    public class TheGenericXmlSerializerSectionHandlerClass
    {
        [Test]
        public void CanBeLoadedFromConfig()
        {
            var config = (MutableConfig)ConfigurationManager.GetSection("GenericXmlSerializerSectionHandler");

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Foo, Is.EqualTo(Foo.First));
            Assert.That(config.Bar.Baz, Is.EqualTo("abc"));
            Assert.That(config.Bar.Qux, Is.EqualTo(123));
        }
    }

    public class TheNonGenericXmlSerializerSectionHandlerClass
    {
        [Test]
        public void CanBeLoadedFromConfig()
        {
            var config = (MutableConfig)ConfigurationManager.GetSection("NonGenericXmlSerializerSectionHandler");

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Foo, Is.EqualTo(Foo.Second));
            Assert.That(config.Bar.Baz, Is.EqualTo("bcd"));
            Assert.That(config.Bar.Qux, Is.EqualTo(234));
        }
    }

    public class AnInheritorOfTheNonGenericXmlSerializerSectionHandlerClass
    {
        [Test]
        public void CanBeLoadedFromConfig()
        {
            var config = (MutableConfig)ConfigurationManager.GetSection("CustomXmlSerializerSectionHandler");

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Foo, Is.EqualTo(Foo.Third));
            Assert.That(config.Bar.Baz, Is.EqualTo("cde"));
            Assert.That(config.Bar.Qux, Is.EqualTo(345));
        }
    }

    public class TheGenericXSerializerSectionHandlerClass
    {
        [Test]
        public void CanBeLoadedFromConfig()
        {
            var config = (ImmutableConfig)ConfigurationManager.GetSection("GenericXSerializerSectionHandler");

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Foo, Is.EqualTo(Foo.Fourth));
            Assert.That(config.Bar.Baz, Is.EqualTo("def"));
            Assert.That(config.Bar.Qux, Is.EqualTo(456));
        }
    }

    public class TheNonGenericXSerializerSectionHandlerClass
    {
        [Test]
        public void CanBeLoadedFromConfig()
        {
            var config = (ImmutableConfig)ConfigurationManager.GetSection("NonGenericXSerializerSectionHandler");

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Foo, Is.EqualTo(Foo.Fifth));
            Assert.That(config.Bar.Baz, Is.EqualTo("efg"));
            Assert.That(config.Bar.Qux, Is.EqualTo(567));
        }
    }

    public class AnInheritorOfTheNonGenericXSerializerSectionHandlerClass
    {
        [Test]
        public void CanBeLoadedFromConfig()
        {
            var config = (ImmutableConfig)ConfigurationManager.GetSection("CustomXSerializerSectionHandler");

            Assert.That(config, Is.Not.Null);
            Assert.That(config.Foo, Is.EqualTo(Foo.Sixth));
            Assert.That(config.Bar.Baz, Is.EqualTo("fgh"));
            Assert.That(config.Bar.Qux, Is.EqualTo(678));
        }
    }
}

// ReSharper disable once CheckNamespace
namespace Example
{
    public class CustomXmlSerializerSectionHandler : XmlSerializerSectionHandler<MutableConfig>
    {
    }

    public class CustomXSerializerSectionHandler : XSerializerSectionHandler<ImmutableConfig>
    {
    }

    public class MutableConfig
    {
        [XmlAttribute]
        public Foo Foo { get; set; }

        public MutableBar Bar { get; set; }
    }

    public class MutableBar
    {
        [XmlAttribute]
        public string Baz { get; set; }

        [XmlText]
        public int Qux { get; set; }
    }

    public class ImmutableConfig
    {
        private readonly Foo _foo;
        private readonly ImmutableBar _bar;

        public ImmutableConfig(Foo foo, ImmutableBar bar)
        {
            _foo = foo;
            _bar = bar;
        }

        [XmlAttribute]
        public Foo Foo { get { return _foo; } }

        public ImmutableBar Bar { get { return _bar; } }
    }

    public class ImmutableBar
    {
        private readonly string _baz;
        private readonly int _qux;

        public ImmutableBar(string baz, int qux)
        {
            _baz = baz;
            _qux = qux;
        }

        [XmlAttribute]
        public string Baz { get { return _baz; } }

        [XmlText]
        public int Qux { get { return _qux; } }
    }

    public enum Foo
    {
        First, Second, Third, Fourth, Fifth, Sixth
    }
}
