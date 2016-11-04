using System.Configuration;
using System.Xml.Serialization;
using NUnit.Framework;
using Rock.Configuration;

namespace Rock.Core.IntegrationTests.Configuration
{
    public class LateBoundConfigurationElementSection : ConfigurationSection
    {
        [ConfigurationProperty("foo", IsRequired = true)]
        public FooConfigurationElement FooFactory
        {
            get
            {
                return (FooConfigurationElement)this["foo"];
            }
            set
            {
                this["foo"] = value;
            }
        }
    }

    public class FooConfigurationElement : LateBoundConfigurationElement<IFoo>
    {
        public FooConfigurationElement()
            : base(typeof(Foo))
        {
        }
    }

    public interface IFoo
    {
    }

    public class Foo : IFoo
    {
        public string Bar { get; set; }
        public Baz Baz { get; set; }
    }

    public class Baz
    {
        [XmlAttribute("qux")]
        public int Qux { get; set; }
        [XmlAttribute("garply")]
        public bool Garply { get; set; }
    }

    public class AnotherFoo : IFoo
    {
        public string Qux { get; set; }
    }

    public class LateBoundConfigurationElementTests
    {
        [Test]
        public void CanGetInstanceOfDefaultType()
        {
            var config = (LateBoundConfigurationElementSection)ConfigurationManager.GetSection("LateBound1");

            var foo = config.FooFactory.CreateInstance() as Foo;

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Bar, Is.EqualTo("abc"));
            Assert.That(foo.Baz.Qux, Is.EqualTo(123));
            Assert.That(foo.Baz.Garply, Is.True);
        }

        [Test]
        public void CanGetInstanceOfSpecifiedType()
        {
            var config = (LateBoundConfigurationElementSection)ConfigurationManager.GetSection("LateBound2");

            var foo = config.FooFactory.CreateInstance() as AnotherFoo;

            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.Qux, Is.EqualTo("Hello, world!"));
        }
    }
}