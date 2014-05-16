using System;
using System.IO;
using System.Text;

namespace Rock.IO
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing information to a string. The information is stored in an 
    /// underlying <see cref="StringBuilder"/>. This class overrides the <see cref="EncodedStringWriter.Encoding"/>
    /// property of the base class, <see cref="StringWriter"/>, which always returns
    /// <see cref="System.Text.Encoding.Unicode"/>.
    /// </summary>
    public class EncodedStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedStringWriter"/> class using the specified
        /// encoding. If no encoding is provided, <see cref="System.Text.Encoding.UTF8"/> is used.
        /// </summary>
        /// <param name="encoding">The encoding in which the output is written.</param>
        public EncodedStringWriter(Encoding encoding = null)
        {
            _encoding = GetEncoding(encoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedStringWriter"/> class that writes to the specified
        /// <see cref="StringBuilder"/> using the specified encoding. If no encoding is provided, 
        /// <see cref="System.Text.Encoding.UTF8"/> is used.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to write to.</param>
        /// <param name="encoding">The encoding in which the output is written.</param>
        public EncodedStringWriter(StringBuilder sb, Encoding encoding = null)
            : base(sb)
        {
            _encoding = GetEncoding(encoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedStringWriter"/> class with the specified format 
        /// control using the specified encoding. If no encoding is provided, 
        /// <see cref="System.Text.Encoding.UTF8"/> is used.
        /// </summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
        /// <param name="encoding">The encoding in which the output is written.</param>
        public EncodedStringWriter(IFormatProvider formatProvider, Encoding encoding = null)
            : base(formatProvider)
        {
            _encoding = GetEncoding(encoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedStringWriter"/> class that writes to the specified
        /// <see cref="StringBuilder"/>, has the specified format provider, and uses the specified encoding. If no 
        /// encoding is provided, <see cref="System.Text.Encoding.UTF8"/> is used.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to write to.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> object that controls formatting.</param>
        /// <param name="encoding">The encoding in which the output is written.</param>
        public EncodedStringWriter(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding = null)
            : base(sb, formatProvider)
        {
            _encoding = GetEncoding(encoding);
        }

        /// <summary>
        /// Gets the System.Text.Encoding in which the output is written.
        /// </summary>
        /// <returns>The Encoding in which the output is written.</returns>
        public override Encoding Encoding
        {
            get { return _encoding; }
        }

        private static Encoding GetEncoding(Encoding encoding)
        {
            return encoding ?? Encoding.UTF8;
        }
    }
}
