using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Rock.Serialization
{
    public class XSerializerSerializerConfiguration : IXSerializerSerializerConfiguration
    {
        public XSerializerSerializerConfiguration()
        {
            RootElementNameMap = new Dictionary<Type, string>();
            Redact = true;
        }

        public XmlSerializerNamespaces Namespaces { get; set; }
        public Encoding Encoding { get; set; }
        public string DefaultNamespace { get; set; }
        public bool Indent { get; set; }
        public bool AlwaysEmitTypes { get; set; }
        public bool Redact { get; set; }
        public bool TreatEmptyElementAsString { get; set; }
        public bool EmitNil { get; set; }

        IReadOnlyDictionary<Type, string> IXSerializerSerializerConfiguration.RootElementNameMap
        {
            get { return RootElementNameMap; }
        }

        public Dictionary<Type, string> RootElementNameMap { get; set; }
    }
}