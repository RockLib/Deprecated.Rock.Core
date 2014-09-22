using System;
using System.Dynamic;
using Rock.Conversion;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConvertsTo<ExpandoObject>> _objectToExpandoObjectConverter = new DefaultHelper<IConvertsTo<ExpandoObject>>(() => new ConvertsToExpandoObject());

        public static IConvertsTo<ExpandoObject> ObjectToExpandoObjectConverter
        {
            get { return _objectToExpandoObjectConverter.Current; }
        }

        public static IConvertsTo<ExpandoObject> DefaultObjectToExpandoObjectConverter
        {
            get { return _objectToExpandoObjectConverter.DefaultInstance; }
        }

        public static void SetObjectToExpandoObjectConverter(Func<IConvertsTo<ExpandoObject>> getObjectToExpandoObjectConverterInstance)
        {
            _objectToExpandoObjectConverter.SetCurrent(getObjectToExpandoObjectConverterInstance);
        }

        public static void RestoreDefaultObjectToExpandoObjectConverter()
        {
            _objectToExpandoObjectConverter.RestoreDefault();
        }
    }
}