namespace Rock.Extensions.DateTime
{
    public static class ToUnixTimestampExtension
    {
        private static readonly System.DateTime _unixEpoch = new System.DateTime(1970, 1, 1);

        /// <summary>
        /// Creates a unix timestamp from the given DateTime
        /// </summary>
        /// <param name="value">The DateTime to convert.</param>
        /// <returns>A Unix Timestamp representation of the given DateTime</returns>
        public static long ToUnixTimestamp(this System.DateTime value)
        {
            System.TimeSpan span = value - _unixEpoch;
            return (long)span.TotalSeconds;
        }
    }
}
