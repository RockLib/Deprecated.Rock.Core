using System;
using System.Dynamic;
using Rock.Conversion;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConverter<ExpandoObject>> _expandoObjectConverter = new DefaultHelper<IConverter<ExpandoObject>>(() => new ExpandoObjectConverter());

        public static IConverter<ExpandoObject> ExpandoObjectConverter
        {
            get { return _expandoObjectConverter.Current; }
        }

        public static IConverter<ExpandoObject> DefaultExpandoObjectConverter
        {
            get { return _expandoObjectConverter.DefaultInstance; }
        }

        public static void SetExpandoObjectConverter(Func<IConverter<ExpandoObject>> getExpandoObjectConverterInstance)
        {
            _expandoObjectConverter.SetCurrent(getExpandoObjectConverterInstance);
        }

        public static void RestoreDefaultExpandoObjectConverter()
        {
            _expandoObjectConverter.RestoreDefault();
        }
    }
}