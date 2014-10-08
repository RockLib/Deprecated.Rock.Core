using System.Dynamic;
using Rock.Defaults.Implementation;

namespace Rock.Conversion
{
    public static class ToExpandoObjectExtension
    {
        public static ExpandoObject ToExpandoObject(this object obj)
        {
            return Default.ObjectToExpandoObjectConverter.Convert(obj);
        }
    }
}
