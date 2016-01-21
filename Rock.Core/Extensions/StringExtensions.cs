using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rock.Cryptography;
using Rock.Serialization;

namespace Rock.Extensions
{
    /// <summary>
    /// string extensions
    /// </summary>
    public static class StringExtensions
    {
		private const string SsnRegexPattern = "<SSN>[0-9-]+</SSN>";
        private static readonly Regex SsnRegex = new Regex(SsnRegexPattern, RegexOptions.IgnoreCase);
        /// <summary>
        /// Determines whether a string [is not null or empty].
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>
        /// 	<c>true</c> if [is not null or empty] [the specified STR]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

		/// <summary>
		/// Determines whether a string [is null or empty].  Short for string.IsNulOrEmpty(str)
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>
		/// 	<c>true</c> if [is null or empty] [the specified STR]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Checks if a string is null before doing an equals comparison. The comparison is case sensitive.
		/// </summary>
		/// <param name="originalString">string that is checked for null before comparison</param>
		/// <param name="compareTo">string to compare str to</param>
		/// <exception cref="ArgumentNullException">comparison cannot be null</exception>
		/// <returns>returns true if and only if str is not null and str and comparison are equal</returns>
		public static bool IsNotNullAndEquals(this string originalString, string compareTo)
		{
			if (originalString == null)
			{
				return false;
			}
			else
			{
				bool result = originalString.Equals(compareTo);
				return result;
			}
		}

        /// <summary>
        /// Deserializes a JSON string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object represented by this string</typeparam>
        /// <param name="str">The JSON string to deserialize</param>
        /// <returns>An object of type T</returns>
        public static T FromJson<T>(this string str)
        {
	        return DefaultJsonSerializer.Current.DeserializeFromString<T>(str);
        }

        /// <summary>
        /// Deserializes an XML string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object represented by this string</typeparam>
        /// <param name="str">The XML string to deserialize</param>
        /// <returns>An object of type T</returns>
		public static T FromXml<T>(this string str)
        {
	        return DefaultXmlSerializer.Current.DeserializeFromString<T>(str);
		}

		/// <summary>
        /// Converts a string to a Hash of the specified type.
        /// </summary>
        /// <param name="value">The string to be hashed.</param>
        /// <param name="hashType">Type of the hash.</param>
        /// <returns>The hash value</returns>
        public static string ToHash(this string value, HashType hashType)
        {
            return hashType.ComputeHash(value);
        }

        /// <summary>
        /// Takes in xml notation that may or may not contain an SSN element, and removes it from the string.
        /// </summary>
        /// <param name="message">Original message that may contain an XML formatted SSN</param>
        /// <returns>Original message with SSN ommited</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>The following examples that will result in the SSN being omitted.</listheader>
        /// <item><SSN>123-45-6789</SSN></item>
        /// <item><SSN>123456789</SSN></item>
        /// <item><SSN>123-456789</SSN></item>
        /// <item><ssn>123-45-6789</ssn></item>
        /// <item><ssn>123456789</ssn></item>
        /// <item><ssn>123-456789</ssn></item>
        /// </list>
        /// </remarks>
        public static string OmitXmlSsn(this string message)
        {
            return SsnRegex.Replace(message, "<SSN>*** SSN Omitted ***</SSN>");
        }

		/// <summary>
		/// Truncate a string to no more than <paramref name="maxLength"/> characters.
		/// </summary>
		/// <param name="text">The string to be truncated</param>
		/// <param name="maxLength"></param>
		/// <example>
		/// <code>
		/// const int maxVendorNameKeywordLength = 30;
		/// 
		/// var validVendorKeyword = sourceText.Truncate(maxVendorNameKeywordLength);
		/// </code></example>
		/// <returns>The truncated text, if originally longer than <paramref name="maxLength"/>, <paramref name="text"/>, or null if <paramref name="text"/> is null</returns>
		public static string Truncate(this string text, int maxLength)
		{
			if (text == null) return null;
			return text.Substring(0, Math.Min(text.Length, maxLength));
		}

		/// <summary>
		/// An alternative to <seealso cref="string.Contains"/> that offers the ability to do case-insensitive comparison
		/// </summary>
		/// <param name="source">Text to check for <paramref name="toCheck"/> text.</param>
		/// <param name="toCheck">The text to search for in <paramref name="source"/></param>
		/// <param name="comp">The <see cref="StringComparison"/> option to use for textual comparison</param>
		/// <returns>true if <paramref name="source"/> contains <paramref name="toCheck"/>, false otherwise.</returns>
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			if (source == null) return false;
			if (toCheck == null) throw new ArgumentNullException("toCheck");
			if (!Enum.IsDefined(typeof(StringComparison), comp)) throw new ArgumentOutOfRangeException("comp");

			return source.IndexOf(toCheck, comp) >= 0;
		}

		/// <summary>
		/// Get the last word from <paramref name="text"/> that contains space-delimited text, not including the leading space.
		/// </summary>
		/// <param name="text">The string to extract the last word from.</param>
		/// <returns>The original text if it contains no spaces, null if <paramref name="text"/> is null, or the last word of <paramref name="text"/></returns>
		public static string LastWord(this string text)
		{
			if (text == null) return null;
			var x = text.LastIndexOf(" ", StringComparison.Ordinal);
			return x != -1 ? text.Substring(x + 1) : text;
		}

		/// <summary>
		/// Get the first word from <paramref name="text"/> that contains space-delimited text, not including the trailing space.
		/// </summary>
		/// <param name="text">The string to extract the first word from.</param>
		/// <returns>The original text if it contains no spaces, null if <paramref name="text"/> is null, or the first word of <paramref name="text"/></returns>
		public static string FirstWord(this string text)
		{
			if (text == null) return null;
			var x = text.IndexOf(" ", StringComparison.Ordinal);
			return x != -1 ? text.Substring(0, x) : text;
		}

		/// <summary>
		/// Convert space-delimited text into initials
		/// </summary>
		/// <param name="value"></param>
		/// <example>
		/// <code>
		/// var docAlias = string.Format("{0} {1:MM/dd/yy} {2:MM/dd/yy}", legalName.Initials(), startDate, endDate);
		/// </code>
		/// </example>
		/// <returns>initials of text</returns>
		public static string Initials(this string value)
		{
			if (value == null) return null;
			var items = value.Split(' ');
			var sb = new StringBuilder();
			foreach (var e in items.Where(e => e.Length >= 1))
			{
				sb.Append(e[0]);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Convert "first last" text into "last, first" format.
		/// </summary>
		/// <param name="value"></param>
		/// <example>
		/// <code>
		/// var clientNameForConversion = legalName.ToLastFirst();
		/// </code>
		/// </example>
		/// <returns>Converted text or null if <paramref name="value"/> is null</returns>
		public static string ToLastFirst(this string value)
		{
			if (value == null) return null;
			var items = value.Split(' ');
			if (items.Length == 1) return value;
			return string.Concat(string.Concat(items.Last(), ", "), items.First());
		}

		public static bool IsNotNullOrWhiteSpace(this string value)
		{
			return !string.IsNullOrWhiteSpace(value);
		}

	    public static bool IsNullOrWhiteSpace(this string value)
	    {
		    return string.IsNullOrWhiteSpace(value);
	    }
    }
}
