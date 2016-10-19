using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using Rock.DependencyInjection;
using Rock.Serialization;

namespace Rock.Configuration
{
    // TODO: Update this documentation!
    /// <summary>
    /// An inheritor of <see cref="ConfigurationElement"/> that creates instances of type
    /// <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type of object that an instance of
    /// <see cref="LateBoundConfigurationElement{TTarget}"/></typeparam> creates.
    /// <remarks>
    /// The <see cref="LateBoundConfigurationElement{TTarget}"/> class is flexible in the xml that
    /// it accepts.
    /// 
    /// For example, we want to obtain an instance of the FooContainer class:
    /// 
    /// <code>
    /// <![CDATA[
    /// public class FooContainer
    /// {
    ///     public LateBoundConfigurationElement<IFoo> Foo { get; set; }
    /// }
    /// 
    /// public interface IFoo
    /// {
    ///     void FooBar();
    /// }
    /// 
    /// public class Foo : IFoo
    /// {
    ///     private readonly string _bar;
    /// 
    ///     public Foo(string bar)
    ///     {
    ///         _bar = bar;
    ///     }
    /// 
    ///     public void FooBar()
    ///     {
    ///         Console.WriteLine("Foo: {0}", _bar);
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// In order to obtain an instance of the 'FooContainer' class, use standard xml
    /// serialization.
    /// 
    /// <code>
    /// string xml = GetXml();
    /// 
    /// FooContainer fooContainer;
    /// 
    /// using (var stringReader = new StringReader(xml))
    /// {
    ///     var serializer = new XmlSerializer(typeof(FooContainer));
    ///     fooContainer = (FooContainer)serializer.Deserialize(stringReader);
    /// }
    /// </code>
    /// 
    /// Now we can create an instance of the 'IFoo' interface, using the 'Foo' property.
    /// 
    /// <code>
    /// IFoo foo = fooContainer.Foo.CreateInstance();
    /// </code>
    /// 
    /// We can use this xml to deserialize an instance of FooContainer. Note that the
    /// 'Foo' element has a 'type' attribute that describes the type that should be
    /// created by the LateBoundConfigurationElement&lt;IFoo&gt;.
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly">
    ///     <Bar>abc</Bar>
    ///   </Foo>
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// 
    /// Note that the 'Bar' property of the 'Foo' class is specified by an element. We
    /// can also specify the 'Bar' property with an xml attribute.
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly" Bar="abc" />
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// 
    /// These dynamic properties are also case-insensitive.
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly">
    ///     <bar>abc</bar>
    ///   </Foo>
    /// </FooContainer>
    /// 
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly" bar="abc" />
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// 
    /// If we want to supply a default type (and omit the 'type' xml attribute), we need
    /// to create a subclass of <see cref="LateBoundConfigurationElement{TTarget}"/>.
    /// 
    /// <code>
    /// <![CDATA[
    /// public class FooProxy : LateBoundConfigurationElement<IFoo>
    /// {
    ///     public FooProxy()
    ///         : base(typeof(Foo))
    ///     {
    ///     }
    /// }
    /// 
    /// public class FooContainer
    /// {
    ///     public FooProxy Foo { get; set; }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// Now our xml doesn't need to specify the 'type' xml attribute (but it still can if 
    /// it needs a type other than the default type).
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo Bar="abc" />
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// </remarks>
    public class LateBoundConfigurationElement<TTarget> : ConfigurationElement
    {
        private readonly XmlDeserializationProxyEngine<TTarget> _engine;

        /// <summary>
        /// Initializes a new instance of <see cref="LateBoundConfigurationElement{TTarget}"/>
        /// without specifying a default type. If no type is provided via the 
        /// <see cref="TypeAssemblyQualifiedName"/> property after this instance of
        /// <see cref="LateBoundConfigurationElement{TTarget}"/> has been created, then this call
        /// and subsequent calls to the <see cref="CreateInstance(IResolver)"/> method will throw an
        /// exception.
        /// </summary>
        public LateBoundConfigurationElement()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LateBoundConfigurationElement{TTarget}"/>,
        /// specifying a default type. If <paramref name="defaultType"/> is null, and if no 
        /// type is provided via the <see cref="TypeAssemblyQualifiedName"/> property after
        /// this instance of <see cref="LateBoundConfigurationElement{TTarget}"/> has been created, 
        /// then this call and subsequent calls to the <see cref="CreateInstance(IResolver)"/> method
        /// will throw an exception.
        /// </summary>
        /// <param name="defaultType">
        /// The type of object to create if <see cref="TypeAssemblyQualifiedName"/> is not specified.
        /// </param>
        /// <remarks>
        /// If the inheritor of <see cref="LateBoundConfigurationElement{TTarget}"/> can supply a
        /// default type, its default constructor should invoke this constructor, supplying
        /// the default type.
        /// <code>
        /// <![CDATA[
        /// public class FooProxy : LateBoundConfigurationElement<IFoo>
        /// {
        ///     public FooProxy()
        ///         : base(typeof(Foo))
        ///     {
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </remarks>
        public LateBoundConfigurationElement(Type defaultType)
        {
            _engine = new XmlDeserializationProxyEngine<TTarget>(this, defaultType, typeof(ConfigurationElement));
            TypeAssemblyQualifiedName = _engine.TypeAssemblyQualifiedName;
        }

        /// <summary>
        /// Gets or sets the assembly qualified name of the concrete type that is
        /// created by this instance of <see cref="LateBoundConfigurationElement{TTarget}"/>.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeAssemblyQualifiedName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Create a new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property, using values specified from any unrecognized xml attributes or elements,
        /// along with any properties specified by an inheritor of the
        /// <see cref="LateBoundConfigurationElement{TTarget}"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/> property.
        /// </returns>
        public TTarget CreateInstance()
        {
            return CreateInstance(null);
        }

        /// <summary>
        /// Create a new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property, using values specified from any unrecognized xml attributes or elements,
        /// along with any properties specified by an inheritor of the
        /// <see cref="LateBoundConfigurationElement{TTarget}"/> class.
        /// </summary>
        /// <param name="resolver">
        /// An optional <see cref="IResolver"/> that can supply any missing values required by a
        /// constructor of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property.
        /// </param>
        /// <returns>
        /// A new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/> property.
        /// </returns>
        public virtual TTarget CreateInstance(IResolver resolver)
        {
            if (TypeAssemblyQualifiedName != null)
            {
                _engine.TypeAssemblyQualifiedName = TypeAssemblyQualifiedName;
            }

            return _engine.CreateInstance(resolver);
        }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown attribute is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="name">The name of the unrecognized attribute.</param><param name="value">The value of the unrecognized attribute.</param>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            var attribute = new XAttribute(name, value);
            _engine.AdditionalXAttributes.Add(attribute);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <param name="elementName">The name of the unknown subelement.</param><param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.</param><exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.- or -One or more of the element's attributes is locked.- or -<paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.- or -The element has a Boolean attribute with an invalid value.- or -An attempt was made to deserialize a property more than once.- or -An attempt was made to deserialize a property that is not a valid member of the element.- or -The element cannot contain a CDATA or text element.</exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            var element = XElement.Load(reader.ReadSubtree());
            _engine.AdditionalXElements.Add(element);
            return true;
        }
    }
}