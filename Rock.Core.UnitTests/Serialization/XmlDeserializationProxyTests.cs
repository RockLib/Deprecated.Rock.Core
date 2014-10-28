using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using Rock.Serialization;

// ReSharper disable once CheckNamespace
namespace XmlDeserializationProxyTests
{
    public class XmlDeserializationProxyTests
    {
        private const string _typeSpecified =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo1, Rock.Core.UnitTests"" />
</FooContainer>";

        private const string _typeNotSpecified =
@"<FooContainer>
    <Foo />
</FooContainer>";

        private const string _caseSensitiveElementParameter =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests"">
        <Bar>abc</Bar>
    </Foo>
</FooContainer>";

        private const string _caseSensitiveAttributeParameter =
@"<FooContainer>
    <Foo Bar =""abc"" type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests""/>
</FooContainer>";

        private const string _caseInsensitiveElementParameter =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests"">
        <bar>abc</bar>
    </Foo>
</FooContainer>";

        private const string _caseInsensitiveAttributeParameter =
@"<FooContainer>
    <Foo bar =""abc"" type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests""/>
</FooContainer>";

        private const string _complexParameter =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo4, Rock.Core.UnitTests"">
        <Bar Baz=""abc"">
            <Qux>123</Qux>
        </Bar>
    </Foo>
</FooContainer>";

        private const string _complexParameterTypeSpecified =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo5, Rock.Core.UnitTests"">
        <Bar Baz=""abc"" type=""XmlDeserializationProxyTests.Bar5, Rock.Core.UnitTests"">
            <Qux>123</Qux>
        </Bar>
    </Foo>
</FooContainer>";

        [Test]
        public void SupportsSpecifyingType()
        {
            var fooContainer = Deserialize<FooContainer1>(_typeSpecified);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo1>>());
            Assert.That(fooContainer.Foo.CreateInstance(), Is.InstanceOf<Foo1>());
        }

        [Test]
        public void SupportsNotSpecifyingType()
        {
            var fooContainer = Deserialize<FooContainer2>(_typeNotSpecified);

            Assert.That(fooContainer.Foo, Is.InstanceOf<Foo2Proxy>());
            Assert.That(fooContainer.Foo.CreateInstance(), Is.InstanceOf<Foo2>());
        }

        [TestCase(_caseSensitiveElementParameter)]
        [TestCase(_caseSensitiveAttributeParameter)]
        [TestCase(_caseInsensitiveElementParameter)]
        [TestCase(_caseInsensitiveAttributeParameter)]
        public void SupportsCaseSensitiveAndCaseInsensitiveElementsAndAttributes(string xml)
        {
            var fooContainer = Deserialize<FooContainer3>(xml);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo3>>());
            Assert.That(fooContainer.Foo.CreateInstance(), Is.InstanceOf<Foo3>());
            Assert.That(fooContainer.Foo.CreateInstance().GetBar(), Is.EqualTo("abc"));
        }

        [Test]
        public void SupportsComplexParameters()
        {
            var fooContainer = Deserialize<FooContainer4>(_complexParameter);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo4>>());

            var foo = fooContainer.Foo.CreateInstance();

            Assert.That(foo, Is.InstanceOf<Foo4>());
            Assert.That(foo.GetBar().Baz, Is.EqualTo("abc"));
            Assert.That(foo.GetBar().Qux, Is.EqualTo(123));
        }

        [Test]
        public void SupportsComplexParametersWithSpecifiedType()
        {
            var fooContainer = Deserialize<FooContainer5>(_complexParameterTypeSpecified);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo5>>());

            var foo = fooContainer.Foo.CreateInstance();

            Assert.That(foo, Is.InstanceOf<Foo5>());
            Assert.That(foo.GetBar().Baz, Is.EqualTo("abc"));
            Assert.That(foo.GetBar().Qux, Is.EqualTo(123));
        }

        private static T Deserialize<T>(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    #region Type Specified

    [XmlRoot("FooContainer")]
    public class FooContainer1
    {
        public XmlDeserializationProxy<IFoo1> Foo { get; set; }
    }

    public interface IFoo1
    {
    }

    public class Foo1 : IFoo1
    {
    }

    #endregion

    #region Type Not Specified

    [XmlRoot("FooContainer")]
    public class FooContainer2
    {
        public Foo2Proxy Foo { get; set; }
    }

    public interface IFoo2
    {
    }

    public class Foo2 : IFoo2
    {
    }

    public class Foo2Proxy : XmlDeserializationProxy<IFoo2>
    {
        public Foo2Proxy()
            : base(typeof(Foo2))
        {
        }
    }

    #endregion

    #region Case Sensitive And Case Insensitive Elements And Attributes

    [XmlRoot("FooContainer")]
    public class FooContainer3
    {
        public XmlDeserializationProxy<IFoo3> Foo { get; set; }
    }

    public interface IFoo3
    {
        string GetBar();
    }

    public class Foo3 : IFoo3
    {
        private readonly string _bar;

        public Foo3(string bar)
        {
            _bar = bar;
        }

        public string GetBar()
        {
            return _bar;
        }
    }

    #endregion

    #region Complex Parameter

    [XmlRoot("FooContainer")]
    public class FooContainer4
    {
        public XmlDeserializationProxy<IFoo4> Foo { get; set; }
    }

    public interface IFoo4
    {
        Bar4 GetBar();
    }

    public class Foo4 : IFoo4
    {
        private readonly Bar4 _bar;

        public Foo4(Bar4 bar)
        {
            _bar = bar;
        }

        public Bar4 GetBar()
        {
            return _bar;
        }
    }

    public class Bar4
    {
        [XmlAttribute]
        public string Baz { get; set; }
        public int Qux { get; set; }
    }

    #endregion

    #region Complex Parameter With Type Specified

    [XmlRoot("FooContainer")]
    public class FooContainer5
    {
        public XmlDeserializationProxy<IFoo5> Foo { get; set; }
    }

    public interface IFoo5
    {
        IBar5 GetBar();
    }

    public class Foo5 : IFoo5
    {
        private readonly IBar5 _bar;

        public Foo5(IBar5 bar)
        {
            _bar = bar;
        }

        public IBar5 GetBar()
        {
            return _bar;
        }
    }

    public interface IBar5
    {
        string Baz { get; }
        int Qux { get; }
    }

    public class Bar5 : IBar5
    {
        [XmlAttribute]
        public string Baz { get; set; }
        public int Qux { get; set; }
    }

    #endregion
}