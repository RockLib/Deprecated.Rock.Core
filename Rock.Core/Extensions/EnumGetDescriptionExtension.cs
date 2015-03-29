using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Rock
{
    public static class EnumGetDescriptionExtension
    {
        private static readonly ConcurrentDictionary<Enum, string> _descriptionCache = new ConcurrentDictionary<Enum, string>();

        /// <summary>
        /// Gets the description of the enumeration value.
        /// </summary>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>The description of the enumeration value, if provided. Else, the result of calling <see cref="object.ToString"/> on the value.</returns>
        public static string GetDescription(this Enum enumValue)
        {
            return
                _descriptionCache.GetOrAdd(
                    enumValue,
                    value =>
                    {
                        var field = enumValue.GetType().GetField(enumValue.ToString());

                        if (field != null)
                        {
                            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

                            if (attribute != null)
                            {
                                return attribute.Description;
                            }
                        }

                        return enumValue.ToString();
                    });
        }
    }
}
