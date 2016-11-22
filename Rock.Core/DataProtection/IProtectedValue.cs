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
        /// <returns></returns>
        byte[] GetValue();
    }
}