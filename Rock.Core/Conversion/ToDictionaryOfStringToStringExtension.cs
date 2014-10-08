using System.Collections.Generic;
using Rock.Defaults.Implementation;

namespace Rock.Conversion
{
    public static class ToDictionaryOfStringToStringExtension
    {
        public static IDictionary<string, string> ToDictionaryOfStringToString(this object obj)
        {
            return Default.ObjectToDictionaryOfStringToStringConverter.Convert(obj);
        }
    }
}
