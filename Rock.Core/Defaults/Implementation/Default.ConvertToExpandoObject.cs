using System;
using System.Dynamic;
using Rock.Conversion;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConvertTo<ExpandoObject>> _convertToExpandoObject = new DefaultHelper<IConvertTo<ExpandoObject>>(() => new ReflectinatorConvertToExpandoObject());

        public static IConvertTo<ExpandoObject> ConvertToExpandoObject
        {
            get { return _convertToExpandoObject.Current; }
        }

        public static void SetConvertToExpandoObject(Func<IConvertTo<ExpandoObject>> getConvertToExpandoObjectInstance)
        {
            _convertToExpandoObject.SetCurrent(getConvertToExpandoObjectInstance);
        }
    }
}