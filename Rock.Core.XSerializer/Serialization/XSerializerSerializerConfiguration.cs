using System.Text;
using System.Xml.Serialization;

namespace Rock.Serialization
{
    public class XSerializerSerializerConfiguration : IXSerializerSerializerConfiguration
    {
        public XSerializerSerializerConfiguration()
        {
            Namespaces = new XmlSerializerNamespaces();
            Encoding = Encoding.UTF8;
            Redact = true;
        }

        public XmlSerializerNamespaces Namespaces { get; set; }
        public Encoding Encoding { get; set; }
        public string DefaultNamespace { get; set; }
        public bool Indent { get; set; }
        public string RootElementName { get; set; }
        public bool AlwaysEmitTypes { get; set; }
        public bool Redact { get; set; }
        public bool TreatEmptyElementAsString { get; set; }
        public bool EmitNil { get; set; }
    }
}