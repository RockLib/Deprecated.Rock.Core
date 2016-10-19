using System;
using System.Xml;
using System.Xml.Serialization;
using Rock.DependencyInjection;
                                                                                                                                                    // ReSharper disable once CheckNamespace
namespace Rock.Serialization
{
    /// <summary>
    /// A class that creates instances of type <typeparamref name="TTarget"/>. Instances of
    /// <see cref="XmlDeserializationProxy{TTarget}"/> are intended to be created via
    /// standard deserialization.
    /// </summary>
    /// <typeparam name="TTarget">The type of object that an instance of
    /// <see cref="XmlDeserializationProxy{TTarget}"/></typeparam> creates.
    /// <remarks>
    /// The <see cref="XmlDeserializationProxy{TTarget}"/> class is flexible in the xml that
    /// it accepts.
    /// 
    /// For example, we want to obtain an instance of the FooContainer class:
    /// 
    /// <code>
    /// <![CDATA[
    /// public class FooContainer
    /// {
    ///     public XmlDeserializationProxy<IFoo> Foo { get; set; }
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
    /// created by the XmlDeserializationProxy&lt;IFoo&gt;.
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
    /// to create a subclass of <see cref="XmlDeserializationProxy{TTarget}"/>.
    /// 
    /// <code>
    /// <![CDATA[
    /// public class FooProxy : XmlDeserializationProxy<IFoo>
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
    public class XmlDeserializationProxy<TTarget>
    {
        private readonly XmlDeserializationProxyEngine<TTarget> _engine;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlDeserializationProxy{TTarget}"/>
        /// without specifying a default type. If no type is provided via the 
        /// <see cref="TypeAssemblyQualifiedName"/> property after this instance of
        /// <see cref="XmlDeserializationProxy{TTarget}"/> has been create, then subsequent
        /// calls to the <see cref="CreateInstance()"/> or <see cref="CreateInstance(IResolver)"/>
        /// methods will fail.
        /// </summary>
        public XmlDeserializationProxy()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="XmlDeserializationProxy{TTarget}"/>,
        /// specifying a default type. If <paramref name="defaultType"/> is null, and if no 
        /// type is provided via the <see cref="TypeAssemblyQualifiedName"/> property after
        /// this instance of <see cref="XmlDeserializationProxy{TTarget}"/> has been create, 
        /// then subsequent calls to the <see cref="CreateInstance()"/> or
        /// <see cref="CreateInstance(IResolver)"/> /// methods will fail.
        /// </summary>
        /// <param name="defaultType">
        /// The type of object to create if <see cref="TypeAssemblyQualifiedName"/> is not specified.
        /// </param>
        /// <remarks>
        /// If the inheritor of <see cref="XmlDeserializationProxy{TTarget}"/> can supply a
        /// default type, its default constructor should invoke this constructor, supplying
        /// the default type.
        /// <code>
        /// <![CDATA[
        /// public class FooProxy : XmlDeserializationProxy<IFoo>
        /// {
        ///     public FooProxy()
        ///         : base(typeof(Foo))
        ///     {
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </remarks>
        public XmlDeserializationProxy(Type defaultType)
        {
            _engine = new XmlDeserializationProxyEngine<TTarget>(this, defaultType, null);
        }

        /// <summary>
        /// Gets or sets the assembly qualified name of the type that this proxy serializes.
        /// NOTE: Do not use this property directly - it exists as an implementation detail
        /// for the internal use of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        [XmlAttribute("type")]
        public string TypeAssemblyQualifiedName
        {
            get { return _engine.TypeAssemblyQualifiedName; }
            set { _engine.TypeAssemblyQualifiedName = value; }
        }

        /// <summary>
        /// Gets or sets any xml attributes that exist in the xml document, but are not
        /// associated with a property of this class (whether this class is
        /// <see cref="XmlDeserializationProxy{TTarget}"/> or its inheritor).
        /// NOTE: Do not use this property directly - it exists as an implementation detail
        /// for the internal use of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        [XmlAnyAttribute]
        public XmlAttribute[] AdditionalAttributes
        {
            get { return _engine.AdditionalXmlAttributes; }
            set { _engine.AdditionalXmlAttributes = value; }
        }

        /// <summary>
        /// Gets or sets any xml elements that exist in the xml document, but are not
        /// associated with a property of this class (whether this class is
        /// <see cref="XmlDeserializationProxy{TTarget}"/> or its inheritor).
        /// NOTE: Do not use this property directly - it exists as an implementation detail
        /// for the internal use of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        [XmlAnyElement]
        public XmlElement[] AdditionalElements
        {
            get { return _engine.AdditionalXmlElements; }
            set { _engine.AdditionalXmlElements = value; }
        }

        /// <summary>
        /// Create a new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property, using values from the <see cref="AdditionalAttributes"/> and
        /// <see cref="AdditionalElements"/> properties, along with any properties specified by
        /// an inheritor of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
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
        /// property, using values from the <see cref="AdditionalAttributes"/> and
        /// <see cref="AdditionalElements"/> properties, along with any properties specified by
        /// an inheritor of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
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
            return _engine.CreateInstance(resolver);
        }
    }
}