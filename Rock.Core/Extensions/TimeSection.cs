namespace Rock
{
    /// <summary>
    /// Defines time sections that can be truncated from a DateTime.
    /// </summary>
    public enum TimeSection
    {
        /// <summary>
        /// Indicates that the millisecond component of the DateTime will be truncated.
        /// </summary>
        Millisecond,
        /// <summary>
        /// Indicates that the second component of the DateTime will be truncated.
        /// </summary>
        Second,
        /// <summary>
        /// Indicates that the minute component of the DateTime will be truncated.
        /// </summary>
        Minute,
        /// <summary>
        /// Indicates that the hour component of the DateTime will be truncated.
        /// </summary>
        Hour
    }
}