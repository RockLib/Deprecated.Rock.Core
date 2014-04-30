using System.Dynamic;

namespace Rock.Conversion
{
    public static class ToExpandoObjectExtension
    {
        public static ExpandoObject ToExpandoObject(this object obj)
        {
            return Default.ConvertToExpandoObject.Convert(obj);
        }
    }
}
