using System;

namespace Rock.StringFormatting
{
    public static class ToHexStringExtensions
    {
        private static readonly char[] _lookup = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        /// <summary>
        /// Converts the given byte array into its equivalent hexidecimal string representation.
        /// </summary>
        /// <param name="buffer">The byte array to convert.</param>
        /// <param name="includePrefix">Whether to include the "0x" prefix at the beginning of the return value.</param>
        /// <returns>The hexadecimal string reprentation of <paramref name="buffer"/>.</returns>
        public static string ToHexString(this byte[] buffer, bool includePrefix = true)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            int i = 0, p, length = buffer.Length;
            char[] c;

            if (includePrefix)
            {
                c = new char[(buffer.Length * 2) + 2];
                c[0] = '0';
                c[1] = 'x';
                p = 2;
            }
            else
            {
                c = new char[buffer.Length * 2];
                p = 0;
            }

            while (i < length)
            {
                byte b = buffer[i++];
                c[p++] = _lookup[b / 0x10];
                c[p++] = _lookup[b % 0x10];
            }

            return new string(c, 0, c.Length);
        }
    }
}
