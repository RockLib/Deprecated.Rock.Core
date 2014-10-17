using System;
using System.Runtime.Serialization;

namespace Rock.Configuration
{
    /// <summary>
    /// The exception that is thrown when an invalid configuration is encountered.
    /// </summary>
    [Serializable]
    public class InvalidConfigurationException : Exception
    {
        private const string _defaultMessage = "Error: invalid configuration.";

        private readonly string _documentFragment;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        public InvalidConfigurationException()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="message">A message describing why this <see cref="InvalidConfigurationException"/> exception was thrown.</param>
        /// <param name="documentFragment">The xml fragment string that caused this <see cref="InvalidConfigurationException"/> to be thrown.</param>
        public InvalidConfigurationException(string message, string documentFragment = null)
            : this(message, null, documentFragment)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="message">A message describing why this <see cref="InvalidConfigurationException"/> exception was thrown.</param>
        /// <param name="inner">The inner exception that caused this <see cref="InvalidConfigurationException"/> to be thrown, if any.</param>
        /// <param name="documentFragment">The xml fragment string that caused this <see cref="InvalidConfigurationException"/> to be thrown.</param>
        public InvalidConfigurationException(string message, Exception inner, string documentFragment = null)
            : base(string.IsNullOrEmpty(message) ? _defaultMessage : message, inner)
        {
            _documentFragment = documentFragment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the information to deserialize.</param>
        /// <param name="context">Contextual information about the source or destination.</param>
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _documentFragment = info.GetString("documentFragment");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("documentFragment", _documentFragment);
        }

        /// <summary>
        /// Gets the document fragment that makes up a configuration section.
        /// </summary>
        public string DocumentFragment
        {
            get { return _documentFragment; }
        }
    }
}
