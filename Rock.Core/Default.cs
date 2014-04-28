using System.Dynamic;
using Rock.Serialization;

namespace Rock.Framework
{
    internal static class Default
    {
        public static readonly IJsonSerializer JsonSerializer = new NewtonsoftJsonSerializer();
        public static readonly IConvertObjectsTo<ExpandoObject> ObjectToExpandoObjectConverter = new ReflectinatorObjectToExpandoObjectConverter();
    }
}
