using System;
using System.IO;
using System.Text;

namespace Rock.Serialization
{
    public static class SerializerExtensions
    {
        public static void Serialize<T>(this ISerializer serializer, Stream stream, T value)
        {
            serializer.Serialize(stream, value, typeof(T));
        }

        public static T Deserialize<T>(this ISerializer serializer, Stream stream)
        {
            return (T)serializer.Deserialize(stream, typeof(T));
        }

        public static byte[] SerializeToByteArray(this ISerializer serializer, object item, Type type)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, item, type);

                memoryStream.Flush();
                return memoryStream.ToArray();
            }
        }

        public static object DeserializeFromByteArray(this ISerializer serializer, byte[] data, Type type)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return serializer.Deserialize(memoryStream, type);
            }
        }

        public static byte[] SerializeToByteArray<T>(this ISerializer serializer, T item)
        {
            return serializer.SerializeToByteArray(item, typeof(T));
        }

        public static T DeserializeFromByteArray<T>(this ISerializer serializer, byte[] data)
        {
            return (T)serializer.DeserializeFromByteArray(data, typeof(T));
        }

        public static string SerializeToString(this ISerializer serializer, object item, Type type, Encoding encoding = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, item, type);

                memoryStream.Flush();
                return (encoding ?? Encoding.UTF8).GetString(memoryStream.ToArray());
            }
        }

        public static object DeserializeFromString(this ISerializer serializer, string data, Type type, Encoding encoding = null)
        {
            using (var memoryStream = new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(data)))
            {
                return serializer.Deserialize(memoryStream, type);
            }
        }

        public static string SerializeToString<T>(this ISerializer serializer, T item, Encoding encoding = null)
        {
            return serializer.SerializeToString(item, typeof(T), encoding);
        }

        public static T DeserializeFromString<T>(this ISerializer serializer, string data, Encoding encoding = null)
        {
            return (T)serializer.DeserializeFromString(data, typeof(T), encoding);
        }
    }
}