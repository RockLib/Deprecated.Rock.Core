using System.Collections.Generic;
using Rock.Immutable;

namespace Rock.Conversion
{
    public static class ToDictionaryOfStringToStringExtension
    {
        private static readonly Semimutable<IConvertsTo<IDictionary<string, string>>> _converter = new Semimutable<IConvertsTo<IDictionary<string, string>>>(GetDefaultConverter);

        public static IDictionary<string, string> ToDictionaryOfStringToString(this object obj)
        {
            return Converter.Convert(obj);
        }

        public static IConvertsTo<IDictionary<string, string>> Converter
        {
            get { return _converter.Value; }
        }

        public static void SetConverter(IConvertsTo<IDictionary<string, string>> converter)
        {
            _converter.Value = converter;
        }

        internal static void ResetConverter()
        {
            UnlockConverter();
            _converter.ResetValue();
        }

        internal static void UnlockConverter()
        {
            _converter.GetUnlockValueMethod().Invoke(_converter, null);
        }

        private static IConvertsTo<IDictionary<string, string>> GetDefaultConverter()
        {
            return new ConvertsToDictionaryOfStringTo<string>();
        }
    }
}
