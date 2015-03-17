namespace Rock
{
    /// <summary>
    /// Represents information about an application.
    /// </summary>
    public interface IApplicationInfo
    {
        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        string ApplicationId { get; }
    }
}