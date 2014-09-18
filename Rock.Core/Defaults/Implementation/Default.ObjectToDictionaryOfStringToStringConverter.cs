using System;
using System.Collections.Generic;
using Rock.Conversion;

namespace Rock.Defaults.Implementation
{
    public static partial class Default
    {
        private static readonly DefaultHelper<IConvertsTo<IDictionary<string, string>>> _objectToDictionaryOfStringToStringConverter = new DefaultHelper<IConvertsTo<IDictionary<string, string>>>(() => new ConvertsToDictionaryOfStringTo<string>());

        public static IConvertsTo<IDictionary<string, string>> ObjectToDictionaryOfStringToStringConverter
        {
            get { return _objectToDictionaryOfStringToStringConverter.Current; }
        }

        public static IConvertsTo<IDictionary<string, string>> DefaultObjectToDictionaryOfStringToStringConverter
        {
            get { return _objectToDictionaryOfStringToStringConverter.DefaultInstance; }
        }

        public static void SetObjectToDictionaryOfStringToStringConverter(Func<IConvertsTo<IDictionary<string, string>>> getObjectToDictionaryOfStringToStringConverterInstance)
        {
            _objectToDictionaryOfStringToStringConverter.SetCurrent(getObjectToDictionaryOfStringToStringConverterInstance);
        }

        public static void RestoreDefaultObjectToDictionaryOfStringToStringConverter()
        {
            _objectToDictionaryOfStringToStringConverter.RestoreDefault();
        }
    }
}