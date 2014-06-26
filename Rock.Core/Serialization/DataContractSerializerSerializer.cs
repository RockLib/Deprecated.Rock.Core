using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Rock.IO;

namespace Rock.Serialization
{
    public class DataContractSerializerSerializer : ISerializer
    {
        public void SerializeToStream(Stream stream, object item, Type type)
        {
            var serializer = new DataContractSerializer(type);
            serializer.WriteObject(stream, item);
        }

        public object DeserializeFromStream(Stream stream, Type type)
        {
            var serializer = new DataContractSerializer(type);
            return serializer.ReadObject(stream);
        }

        public string SerializeToString(object item, Type type, Encoding encoding = null)
        {
            var sb = new StringBuilder();

            using (var stringWriter = new EncodedStringWriter(sb, encoding))
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings {Encoding = encoding}))
                {
                    var serializer = new DataContractSerializer(type);
                    serializer.WriteObject(xmlWriter, item);
                }
            }

            return sb.ToString();
        }

        public object DeserializeFromString(string data, Type type)
        {
            using (var stringReader = new StringReader(data))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    var serializer = new DataContractSerializer(type);
                    return serializer.ReadObject(xmlReader);
                }
            }
        }
    }
}