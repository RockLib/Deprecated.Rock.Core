using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Rock.Collections
{
    /// <summary>
    /// Provides a serializable dictionary that supports native XML serialization since by default,
    /// anything implementing IDictionary cannot be serialized.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private static readonly XmlSerializer _defaultKeySerializer = new XmlSerializer(typeof(TKey));
        private static readonly XmlSerializer _defaultValueSerializer = new XmlSerializer(typeof(TValue));

        private readonly XmlSerializer _keySerializer;
        private readonly XmlSerializer _valueSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="keySerializer">The serializer to use when serializing .</param>
        /// <param name="valueSerializer">The value serializer.</param>
        public SerializableDictionary(XmlSerializer keySerializer = null, XmlSerializer valueSerializer = null)
        {
            _keySerializer = keySerializer ?? _defaultKeySerializer;
            _valueSerializer = valueSerializer ?? _defaultValueSerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        public SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            while (reader.Read() && reader.MoveToContent() != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Item");

                var key = (TKey)_keySerializer.Deserialize(reader);
                var value = (TValue)_valueSerializer.Deserialize(reader);

                Add(key, value);
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var item in this)
            {
                writer.WriteStartElement("Item");

                // We'll never have a null key, so always write it.
                _keySerializer.Serialize(writer, item.Key);

                // But is is possible to have a null value, so only write it if it exists.
                if (item.Value != null)
                {
                    _valueSerializer.Serialize(writer, item.Value);
                }

                writer.WriteEndElement();
            }
        }
    }
}
