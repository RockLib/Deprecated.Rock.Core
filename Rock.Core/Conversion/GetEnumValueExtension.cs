using System;
using System.Collections.Concurrent;

namespace Rock.Conversion
{
    public static class GetEnumValueExtension
    {
        public static T GetEnumValue<T>(this string value)
            where T : struct
        {
            return Get<T>.EnumValue(value);
        }

        private static class Get<T>
            where T : struct
        {
            private static readonly ConcurrentDictionary<string, T> _values = new ConcurrentDictionary<string, T>();

            public static T EnumValue(string value)
            {
                return _values.GetOrAdd(value, x =>
                {
                    if (!typeof(T).IsEnum)
                    {
                        throw new InvalidOperationException("Unable to get enum value. Type '{0}' is not an enum.");
                    }

                    return (T)Enum.Parse(typeof(T), x, true);
                });
            }
        }
    }
}