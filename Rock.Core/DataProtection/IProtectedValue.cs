namespace Rock.DataProtection
{
    /// <summary>
    /// Defines an interface for encapsulating sensitive data.
    /// </summary>
    public interface IProtectedValue
    {
        /// <summary>
        /// Gets the plain text value.
        /// </summary>
        /// <returns>The plain text value</returns>
        /// <exception cref="DataProtectionException">
        /// Implementors of <see cref="IProtectedValue"/> should throw a <see cref="DataProtectionException"/>
        /// when they unable to retrieve a plain text value from its source.
        /// </exception>
        byte[] GetValue();
    }
}