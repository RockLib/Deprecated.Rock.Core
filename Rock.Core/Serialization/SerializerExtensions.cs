using System;
using System.IO;

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

        public static byte[] Serialize(this ISerializer serializer, object item, Type type)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, item, type);

                memoryStream.Flush();
                return memoryStream.ToArray();
            }
        }

        public static object Deserialize(this ISerializer serializer, byte[] data, Type type)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return serializer.Deserialize(memoryStream, type);
            }
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T item)
        {
            return serializer.Serialize(item, typeof(T));
        }

        public static T Deserialize<T>(this ISerializer serializer, byte[] data)
        {
            return (T)serializer.Deserialize(data, typeof(T));
        }
    }
}