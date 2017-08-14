using System.Dynamic;
using Rock.Immutable;

namespace Rock.Conversion
{
    public static class ToExpandoObjectExtension
    {
        private static readonly Semimutable<IConvertsTo<ExpandoObject>> _converter = new Semimutable<IConvertsTo<ExpandoObject>>(GetDefaultConverter);

        public static ExpandoObject ToExpandoObject(this object obj)
        {
            return _converter.Value.Convert(obj);
        }

        public static IConvertsTo<ExpandoObject> Converter
        {
            get { return _converter.Value; }
        }

        public static void SetConverter(IConvertsTo<ExpandoObject> converter)
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
            _converter.UnlockValue();
        }

        private static IConvertsTo<ExpandoObject> GetDefaultConverter()
        {
            return new ConvertsToExpandoObject();
        }
    }
}
