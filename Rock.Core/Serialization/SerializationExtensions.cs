namespace Rock.Serialization
{
    public static class SerializationExtensions
    {
        /// <summary>
        /// Deserializes an XML string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object represented by this string</typeparam>
        /// <param name="str">The XML string to deserialize</param>
        /// <returns>An object of type T</returns>
        public static T FromXml<T>(this string str)
        {
            return DefaultXmlSerializer.Current.DeserializeFromString<T>(str);
        }

        /// <summary>
        /// Deserializes a JSON string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object represented by this string</typeparam>
        /// <param name="str">The JSON string to deserialize</param>
        /// <returns>An object of type T</returns>
        public static T FromJson<T>(this string str)
        {
            return DefaultJsonSerializer.Current.DeserializeFromString<T>(str);
        }
    }
}