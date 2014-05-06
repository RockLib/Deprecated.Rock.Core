using System;
using System.Dynamic;
using Rock.Conversion;

namespace Rock
{
    public static partial class Default
    {
        private static readonly Default<IConvertTo<ExpandoObject>> _defaultConvertToExpandoObject = new Default<IConvertTo<ExpandoObject>>(() => new ReflectinatorConvertToExpandoObject());

        public static IConvertTo<ExpandoObject> ConvertToExpandoObject
        {
            get { return _defaultConvertToExpandoObject.Current; }
        }

        public static void SetConvertToExpandoObject(Func<IConvertTo<ExpandoObject>> getConvertToExpandoObjectInstance)
        {
            _defaultConvertToExpandoObject.SetCurrent(getConvertToExpandoObjectInstance);
        }
    }
}