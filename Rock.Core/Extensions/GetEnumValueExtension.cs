using System;
using System.Collections.Concurrent;

namespace Rock.Extensions
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

            static Get()
            {
                if (!typeof(T).IsEnum)
                {
                    throw new InvalidOperationException("Unable to get enum value for type {0} - not an enum.");
                }
            }

            public static T EnumValue(string value)
            {
                return _values.GetOrAdd(value, x => (T)Enum.Parse(typeof(T), x, true));
            }
        }
    }
}