using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Rock.Serialization
{
    /// <summary>
    /// Defines various settings used to configure XSerializer.
    /// </summary>
    public interface IXSerializerSerializerConfiguration
    {
        /// <summary>
        /// Gets the XML namespaces and prefixes that is used to generate qualified names in an XML-document instance.
        /// </summary>
        XmlSerializerNamespaces Namespaces { get; }

        /// <summary>
        /// Gets the <see cref="System.Text.Encoding"/> to emit in the xml declaration when serializing.
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// Gets the default namespace of all XML elements in an XML document.
        /// </summary>
        string DefaultNamespace { get; }

        /// <summary>
        /// Gets a value indicating whether the XML document should be indented when serializing.
        /// </summary>
        bool Indent { get; }

        /// <summary>
        /// Gets a value indicating whether to always emit type information when
        /// serializing the value of a property of type <see cref="object"/> or
        /// <see cref="System.Dynamic.ExpandoObject"/>.
        /// </summary>
        bool AlwaysEmitTypes { get; }

        /// <summary>
        /// Gets a value indicating whether to redact values returned from properties
        /// marked with the <see cref="XSerializer.RedactAttribute"/>.
        /// </summary>
        bool Redact { get; }

        /// <summary>
        /// Gets a value indicating whether to deserialize an empty element (e.g. &gt;foo/&lt;)
        /// as an empty string when deserializing as type <see cref="object"/> or
        /// <see cref="System.Dynamic.ExpandoObject"/>.
        /// </summary>
        bool TreatEmptyElementAsString { get; }

        /// <summary>
        /// Gets a value indicating whether to emit 'xsi:nil="true"' every time a property
        /// with a null value is encountered.
        /// </summary>
        bool EmitNil { get; }

        /// <summary>
        /// Gets a dictionary that contains the names of root elements that the serializer
        /// should use when serializing/deserializing objects of a given type. The keys in
        /// the dictionary are the types that need a custom root element name, and the
        /// values are those root element names.
        /// </summary>
        IReadOnlyDictionary<Type, string> RootElementNameMap { get; }
    }
}