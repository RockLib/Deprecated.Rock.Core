using System;
using System.Dynamic;
using Rock.Conversion;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConvertsTo<ExpandoObject>> _expandoObjectConverter = new DefaultHelper<IConvertsTo<ExpandoObject>>(() => new ExpandoObjectConverter());

        public static IConvertsTo<ExpandoObject> ExpandoObjectConverter
        {
            get { return _expandoObjectConverter.Current; }
        }

        public static IConvertsTo<ExpandoObject> DefaultExpandoObjectConverter
        {
            get { return _expandoObjectConverter.DefaultInstance; }
        }

        public static void SetExpandoObjectConverter(Func<IConvertsTo<ExpandoObject>> getExpandoObjectConverterInstance)
        {
            _expandoObjectConverter.SetCurrent(getExpandoObjectConverterInstance);
        }

        public static void RestoreDefaultExpandoObjectConverter()
        {
            _expandoObjectConverter.RestoreDefault();
        }
    }
}