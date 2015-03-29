namespace Rock
{
    /// <summary>
    /// Represents information about an application.
    /// </summary>
    public interface IApplicationIdProvider
    {
        /// <summary>
        /// Gets the ID of the current application.
        /// </summary>
        string GetApplicationId();
    }
}