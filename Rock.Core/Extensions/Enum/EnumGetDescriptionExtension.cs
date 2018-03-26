using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Rock.Extensions.Enum
{
    public static class EnumGetDescriptionExtension
    {
        private static readonly ConcurrentDictionary<System.Enum, string> _descriptionCache = new ConcurrentDictionary<System.Enum, string>();

        /// <summary>
        /// Gets the description of the enumeration value.
        /// </summary>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>The description of the enumeration value, if provided. Else, the result of calling <see cref="object.ToString"/> on the value.</returns>
        public static string GetDescription(this System.Enum enumValue)
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
