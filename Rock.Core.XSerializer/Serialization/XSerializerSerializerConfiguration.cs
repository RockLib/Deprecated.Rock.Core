using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Rock.Serialization
{
    public class XSerializerSerializerConfiguration : IXSerializerSerializerConfiguration
    {
        private readonly Dictionary<Type, string> _rootElementNameMap = new Dictionary<Type, string>();

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
        public bool AlwaysEmitTypes { get; set; }
        public bool Redact { get; set; }
        public bool TreatEmptyElementAsString { get; set; }
        public bool EmitNil { get; set; }

        IReadOnlyDictionary<Type, string> IXSerializerSerializerConfiguration.RootElementNameMap
        {
            get { return _rootElementNameMap; }
        }

        public IDictionary<Type, string> RootElementNameMap
        {
            get { return _rootElementNameMap; }
            set
            {
                _rootElementNameMap.Clear();

                if (value != null)
                {
                    foreach (var kvp in value)
                    {
                        _rootElementNameMap.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }
    }
}