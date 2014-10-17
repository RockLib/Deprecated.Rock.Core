using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Rock.Serialization
{
    /// <summary>
    /// A read/write implementation of <see cref="IXSerializerSerializerConfiguration"/>.
    /// Provides a mechanism for configuring XSerializer.
    /// </summary>
    public class XSerializerSerializerConfiguration : IXSerializerSerializerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XSerializerSerializerConfiguration"/> class.
        /// </summary>
        public XSerializerSerializerConfiguration()
        {
            RootElementNameMap = new Dictionary<Type, string>();
            Redact = true;
        }

        /// <summary>
        /// Gets or sets the XML namespaces and prefixes that is used to generate qualified names in an XML-document instance.
        /// </summary>
        public XmlSerializerNamespaces Namespaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Text.Encoding"/> to emit in the xml declaration when serializing.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the default namespace of all XML elements in an XML document.
        /// </summary>
        public string DefaultNamespace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the XML document should be indented when serializing.
        /// </summary>
        public bool Indent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to always emit type information when
        /// serializing the value of a property of type <see cref="object"/> or
        /// <see cref="System.Dynamic.ExpandoObject"/>.
        /// </summary>
        public bool AlwaysEmitTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to redact values returned from properties
        /// marked with the <see cref="XSerializer.RedactAttribute"/>.
        /// </summary>
        public bool Redact { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to deserialize an empty element (e.g. &gt;foo/&lt;)
        /// as an empty string when deserializing as type <see cref="object"/> or
        /// <see cref="System.Dynamic.ExpandoObject"/>.
        /// </summary>
        public bool TreatEmptyElementAsString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to emit 'xsi:nil="true"' every time a property
        /// with a null value is encountered.
        /// </summary>
        public bool EmitNil { get; set; }

        IReadOnlyDictionary<Type, string> IXSerializerSerializerConfiguration.RootElementNameMap
        {
            get { return RootElementNameMap; }
        }

        /// <summary>
        /// Gets or sets a dictionary that contains the names of root elements that the serializer
        /// should use when serializing/deserializing objects of a given type. The keys in
        /// the dictionary are the types that need a custom root element name, and the
        /// values are those root element names.
        /// </summary>
        public Dictionary<Type, string> RootElementNameMap { get; set; }
    }
}