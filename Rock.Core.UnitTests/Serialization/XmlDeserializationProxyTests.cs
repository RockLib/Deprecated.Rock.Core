using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using Rock.DependencyInjection;
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

        private const string _invalidType =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo2, Rock.Core.UnitTests"" />
</FooContainer>";

        private const string _typeNotSpecified =
@"<FooContainer>
    <Foo />
</FooContainer>";

        private const string _caseSensitiveElementParameter =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests"">
        <Bar>123</Bar>
    </Foo>
</FooContainer>";

        private const string _caseSensitiveAttributeParameter =
@"<FooContainer>
    <Foo Bar =""123"" type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests""/>
</FooContainer>";

        private const string _caseInsensitiveElementParameter =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests"">
        <bar>123</bar>
    </Foo>
</FooContainer>";

        private const string _caseInsensitiveAttributeParameter =
@"<FooContainer>
    <Foo bar =""123"" type=""XmlDeserializationProxyTests.Foo3, Rock.Core.UnitTests""/>
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

        private const string _complexParameterTypeRequiredButNotSpecified =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo5, Rock.Core.UnitTests"">
        <Bar Baz=""abc"">
            <Qux>123</Qux>
        </Bar>
    </Foo>
</FooContainer>";

        private const string _externalDependency =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo6, Rock.Core.UnitTests"">
        <Bar type=""XmlDeserializationProxyTests.Bar6, Rock.Core.UnitTests""/>
    </Foo>
</FooContainer>";

        private const string _defaultParameterValue =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo7, Rock.Core.UnitTests"">
        <Bar type=""XmlDeserializationProxyTests.Bar7, Rock.Core.UnitTests""/>
    </Foo>
</FooContainer>";

        private const string _specifiedProxyProperty =
@"<FooContainer>
    <Foo Bar=""abc"" type=""XmlDeserializationProxyTests.Foo8, Rock.Core.UnitTests""/>
</FooContainer>";

        private const string _unspecifiedProxyProperty =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo8, Rock.Core.UnitTests"">
        <Bar>abc</Bar>
    </Foo>
</FooContainer>";

        private const string _readWritePropertiesOnTargetClass =
@"<FooContainer>
    <Foo type=""XmlDeserializationProxyTests.Foo9, Rock.Core.UnitTests"">
        <Bar>abc</Bar>
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
        public void RequiresSpecifyingTypeWhenNoDefaultTypeExists()
        {
            var fooContainer = Deserialize<FooContainer1>(_typeNotSpecified);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo1>>());

            Assert.That(() => fooContainer.Foo.CreateInstance(), Throws.Exception);
        }

        [Test]
        public void ThrowsAnExceptionIfTheTypeSpecifiedIsNotCompatibleWithTheTargetType()
        {
            var fooContainer = Deserialize<FooContainer1>(_invalidType);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo1>>());

            Assert.That(() => fooContainer.Foo.CreateInstance(), Throws.Exception);
        }

        [Test]
        public void ACustomProxyWithAnInvalidDefaultTypeThrowsExceptionInItsConstructor()
        {
            Assert.That(() => new InvalidProxy(), Throws.Exception);
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
            Assert.That(fooContainer.Foo.CreateInstance().GetBar(), Is.EqualTo(123));
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

        [Test]
        public void RequiresSpecifiedTypeWhenPropertyTypeIsAbstract()
        {
            var fooContainer = Deserialize<FooContainer5>(_complexParameterTypeRequiredButNotSpecified);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo5>>());

            var foo = fooContainer.Foo.CreateInstance();

            Assert.That(foo, Is.InstanceOf<Foo5>());
            Assert.That(foo.GetBar(), Is.Null);
        }

        [Test]
        public void UsesValueFromIResolverIfNoMatchForParameterIsFound()
        {
            var fooContainer = Deserialize<FooContainer6>(_externalDependency);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo6>>());

            var autoContainer = new AutoContainer(new Baz6());
            var foo = fooContainer.Foo.CreateInstance(autoContainer);

            Assert.That(foo, Is.InstanceOf<Foo6>());

            Assert.That(foo.GetBar(), Is.InstanceOf<Bar6>());
            Assert.That(foo.GetBaz(), Is.InstanceOf<Baz6>());
        }

        [Test]
        public void UsesParameterDefaultValueIfNoMatchForParameterIsFoundAndIResolverCannotResolve()
        {
            var fooContainer = Deserialize<FooContainer7>(_defaultParameterValue);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo7>>());

            var foo = fooContainer.Foo.CreateInstance();

            Assert.That(foo, Is.InstanceOf<Foo7>());

            Assert.That(foo.GetBar(), Is.InstanceOf<Bar7>());
            Assert.That(foo.GetBaz(), Is.EqualTo(-1));
        }

        [TestCase(_specifiedProxyProperty)]
        [TestCase(_unspecifiedProxyProperty)]
        public void AttemptsToRetrieveValuesFromProxyProperties(string xml)
        {
            var fooContainer = Deserialize<FooContainer8>(xml);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo8>>());

            var foo = fooContainer.Foo.CreateInstance();

            Assert.That(foo, Is.InstanceOf<Foo8>());

            Assert.That(foo.GetBar(), Is.EqualTo("abc"));
        }

        [Test]
        public void SupportsReadWritePropertiesOnTargetClass()
        {
            var fooContainer = Deserialize<FooContainer9>(_readWritePropertiesOnTargetClass);

            Assert.That(fooContainer.Foo, Is.InstanceOf<XmlDeserializationProxy<IFoo9>>());

            var foo = fooContainer.Foo.CreateInstance();

            Assert.That(foo, Is.InstanceOf<Foo9>());

            Assert.That(foo.Bar, Is.EqualTo("abc"));
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

    #region Invalid Proxy

    public class InvalidProxy : XmlDeserializationProxy<IFoo1>
    {
        public InvalidProxy()
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
        int GetBar();
    }

    public class Foo3 : IFoo3
    {
        private readonly int _bar;

        public Foo3(int bar)
        {
            _bar = bar;
        }

        public int GetBar()
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

    #region External Dependency 

    [XmlRoot("FooContainer")]
    public class FooContainer6
    {
        public XmlDeserializationProxy<IFoo6> Foo { get; set; }
    }

    public interface IFoo6
    {
        IBar6 GetBar();
        IBaz6 GetBaz();
    }

    public interface IBar6
    {
    }

    public class Bar6 : IBar6
    {
    }

    public interface IBaz6
    {
    }

    public class Baz6 : IBaz6
    {
    }

    public class Foo6 : IFoo6
    {
        private readonly IBar6 _bar;
        private readonly IBaz6 _baz;

        public Foo6(IBar6 bar, IBaz6 baz)
        {
            _bar = bar;
            _baz = baz;
        }

        public IBar6 GetBar()
        {
            return _bar;
        }

        public IBaz6 GetBaz()
        {
            return _baz;
        }
    }

    #endregion

    #region Default Parameter Value

    [XmlRoot("FooContainer")]
    public class FooContainer7
    {
        public XmlDeserializationProxy<IFoo7> Foo { get; set; }
    }

    public interface IFoo7
    {
        IBar7 GetBar();
        int GetBaz();
    }

    public interface IBar7
    {
    }

    public class Bar7 : IBar7
    {
    }

    public class Foo7 : IFoo7
    {
        private readonly IBar7 _bar;
        private readonly int _baz;

        public Foo7(IBar7 bar, int baz = -1)
        {
            _bar = bar;
            _baz = baz;
        }

        public IBar7 GetBar()
        {
            return _bar;
        }

        public int GetBaz()
        {
            return _baz;
        }
    }

    #endregion

    #region Attempts To Retrieve Values From Proxy Properties

    [XmlRoot("FooContainer")]
    public class FooContainer8
    {
        public Foo8Proxy Foo { get; set; }
    }

    public interface IFoo8
    {
        string GetBar();
    }

    public class Foo8: IFoo8
    {
        private readonly string _bar;

        public Foo8(string bar)
        {
            _bar = bar;
        }

        public string GetBar()
        {
            return _bar;
        }
    }

    public class Foo8Proxy : XmlDeserializationProxy<IFoo8>
    {
        public Foo8Proxy()
            : base(typeof(Foo8))
        {
        }

        [XmlAttribute]
        public string Bar { get; set; }
    }

    #endregion

    #region Read/Write Properties On TargetClass

    [XmlRoot("FooContainer")]
    public class FooContainer9
    {
        public XmlDeserializationProxy<IFoo9> Foo { get; set; }
    }

    public interface IFoo9
    {
        string Bar { get; }
    }

    public class Foo9 : IFoo9
    {
        private readonly int _baz;

        public Foo9(int baz)
        {
            _baz = baz;
        }

        public string Bar { get; set; }

        public int Baz
        {
            get { return _baz; }
        }
    }

    #endregion
}