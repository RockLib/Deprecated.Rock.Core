using System;

namespace Rock.Extensions.DateTime
{
    public static class DateTimeSocialDateExtensions
    {
        #region ToSocialDate Methods

        /// <summary>
        /// Translates a given <see cref="DateTime"/> to a "social" date time string, e.g. "in 10 minutes" or "3 hours ago". 
        /// It compares the <paramref name="postedTime"/> parameter to <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="postedTime">A time to compare to <see cref="DateTime.Now"/>.</param>
        /// <returns>A "social" date time string.</returns>
        /// <remarks>
        /// Assuming the current date and time is June 6, 2009 1:00:00 PM (which is a Sunday),
        /// the following posted dates and times return the following results:
        /// <list type="bullet">
        /// <item>May 10, 1:30:00 PM returns "May 10 at 1:30 PM"</item>
        /// <item>June 2, 4:59:00 PM returns "Wednesday at 4:59 PM"</item>
        /// <item>June 5, 2:30:00 PM returns "Yesterday at 2:30 PM"</item>
        /// <item>June 6, 8:00:00 AM returns "5 hours ago"</item>
        /// <item>June 6, 12:00:00 PM returns "1 hour ago"</item>
        /// <item>June 6, 12:30:00 PM returns "30 minutes ago"</item>
        /// <item>June 6, 12:59:00 PM returns "1 minute ago"</item>
        /// <item>June 6, 12:59:35 PM returns "25 seconds ago"</item>
        /// <item>June 6, 12:59:59 PM returns "1 second ago ago"</item>
        /// <item>June 6, 1:00:01 PM returns "in 1 second"</item>
        /// <item>June 6, 1:00:08 PM returns "in 8 seconds"</item>
        /// <item>June 6, 1:01:00 PM returns "in 1 minute"</item>
        /// <item>June 6, 1:14:00 PM returns "in 14 minutes"</item>
        /// <item>June 6, 2:00:00 PM returns "in 1 hour"</item>
        /// <item>June 6, 4:00:00 PM returns "in 3 hours"</item>
        /// <item>June 7, 5:00:00 PM returns "Tomorrow at 5:00 PM"</item>
        /// <item>June 10, 3:30:00 PM returns "June 10 at 3:30 PM"</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// DateTime postedTime = DateTime.Now.AddSeconds(-1);
        /// string socialTime = postedTime.ToSocialDate();
        /// (Here, socialTime will be "1 second ago")
        /// </code>
        /// </example>
        public static string ToSocialDate(this System.DateTime postedTime)
        {
            System.DateTime currentTime = System.DateTime.Now;
            return ToSocialDate(postedTime, currentTime);
        }

        /// <summary>
        /// Translates a given <see cref="DateTime"/> to a "social" date time string, e.g. "in 10 minutes" or "3 hours ago". 
        /// It compares the <paramref name="postedTime"/> parameter to the <paramref name="currentTime"/> parameter.
        /// </summary>
        /// <param name="postedTime">A time to compare to the <paramref name="currentTime"/> parameter.</param>
        /// <param name="currentTime">A <see cref="DateTime"/> representing "now".</param>
        /// <returns>A "social" date time string.</returns>
        /// <remarks>
        /// Assuming the <paramref name="currentTime"/> is June 6, 2009 1:00:00 PM (which is a Sunday),
        /// the following posted dates and times return the following results:
        /// <list type="bullet">
        /// <item>May 10, 1:30:00 PM returns "May 10 at 1:30 PM"</item>
        /// <item>June 2, 4:59:00 PM returns "Wednesday at 4:59 PM"</item>
        /// <item>June 5, 2:30:00 PM returns "Yesterday at 2:30 PM"</item>
        /// <item>June 6, 8:00:00 AM returns "5 hours ago"</item>
        /// <item>June 6, 12:00:00 PM returns "1 hour ago"</item>
        /// <item>June 6, 12:30:00 PM returns "30 minutes ago"</item>
        /// <item>June 6, 12:59:00 PM returns "1 minute ago"</item>
        /// <item>June 6, 12:59:35 PM returns "25 seconds ago"</item>
        /// <item>June 6, 12:59:59 PM returns "1 second ago ago"</item>
        /// <item>June 6, 1:00:01 PM returns "in 1 second"</item>
        /// <item>June 6, 1:00:08 PM returns "in 8 seconds"</item>
        /// <item>June 6, 1:01:00 PM returns "in 1 minute"</item>
        /// <item>June 6, 1:14:00 PM returns "in 14 minutes"</item>
        /// <item>June 6, 2:00:00 PM returns "in 1 hour"</item>
        /// <item>June 6, 4:00:00 PM returns "in 3 hours"</item>
        /// <item>June 7, 5:00:00 PM returns "Tomorrow at 5:00 PM"</item>
        /// <item>June 10, 3:30:00 PM returns "June 10 at 3:30 PM"</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// DateTime now = new DateTime(2016, 6, 1, 15, 25, 31);
        /// DateTime postedTime = now.AddSeconds(-1);
        /// string socialTime = postedTime.ToSocialDate(now);
        /// (Here, socialTime will be "1 second ago")
        /// </code>
        /// </example>
        public static string ToSocialDate(this System.DateTime postedTime, System.DateTime currentTime)
        {
            TimeSpan duration = currentTime - postedTime;

            if (duration.TotalDays <= -2)
            {
                return postedTime.ToString("MMMM") + " " + postedTime.Day + " at " + postedTime.ToShortTimeString();
            }
            if (duration.TotalHours <= -24)
            {
                if (postedTime.DayOfWeek == currentTime.AddDays(1).DayOfWeek)
                {
                    return "Tomorrow at " + postedTime.ToShortTimeString();
                }

                return postedTime.ToString("MMMM") + " " + postedTime.Day + " at " + postedTime.ToShortTimeString();
            }
            if (duration.Hours < -1)
            {
                return "in " + (-1 * duration.Hours) + " hours";
            }
            if (duration.Hours == -1)
            {
                return "in 1 hour";
            }
            if (duration.Minutes < -1)
            {
                return "in " + (-1 * duration.Minutes) + " minutes";
            }
            if (duration.Minutes == -1)
            {
                return "in 1 minute";
            }
            if (duration.Seconds < -1)
            {
                return "in " + (-1 * duration.Seconds) + " seconds";
            }
            if (duration.Seconds == -1)
            {
                return "in 1 second";
            }
            if (duration.Days > 6)
            {
                return postedTime.ToString("MMMM") + " " + postedTime.Day + " at " + postedTime.ToShortTimeString();
            }
            if (duration.TotalDays >= 2)
            {
                return postedTime.DayOfWeek + " at " + postedTime.ToShortTimeString();
            }
            if (duration.TotalHours >= 24)
            {
                if (postedTime.DayOfWeek == currentTime.AddDays(-1).DayOfWeek)
                {
                    return "Yesterday at " + postedTime.ToShortTimeString();
                }

                return postedTime.DayOfWeek + " at " + postedTime.ToShortTimeString();
            }
            if (duration.Hours > 1)
            {
                return duration.Hours + " hours ago";
            }
            if (duration.Hours == 1)
            {
                return "1 hour ago";
            }
            if (duration.Minutes > 1)
            {
                return duration.Minutes + " minutes ago";
            }
            if (duration.Minutes == 1)
            {
                return "1 minute ago";
            }
            if (duration.Seconds > 1)
            {
                return duration.Seconds + " seconds ago";
            }
            if (duration.Seconds <= 1)
            {
                return "1 second ago";
            }
            return postedTime.ToString("MMMM") + " " + postedTime.Day + " at " + postedTime.ToShortTimeString();
        }

        #endregion

        /// <summary>
        /// Calculates the number of times a certain day of the week falls between
        /// the two specified dates.
        /// </summary>
        /// <param name="date1">The first date</param>
        /// <param name="date2">The second date</param>
        /// <param name="dow">The day of week to look for</param>
        /// <returns>number of occurences represented as an integer</returns>
        /// <remarks>
        /// Assuming current date and time is June 6, 2009 1:00:00 PM and this is a Sunday
        /// The following posted dates and times return the following results:
        /// <list type="bullet">
        /// <item>date1 = July 23 2010(Friday); date2 = July 23 2010(Friday); dow = Saturday; returns '0'</item>
        /// <item>date1 = July 23 2010(Friday); date2 = July 24 2010(Saturday); dow = Saturday; returns '1'</item>
        /// <item>date1 = July 23 2010(Friday); date2 = July 31 2010(Saturday); dow = Saturday; returns '2'</item>
        /// <item>date1 = July 23 2010(Friday); date2 = August 7 2010(Saturday); dow = Saturday; returns '3'</item>
        /// <item>date1 = July 23 2010(Friday); date2 = July 16 2010(Friday); dow = Saturday; returns '1'</item>
        /// <item>date1 = July 19 2010(Monday); date2 = July 23 2010(Friday); dow = Saturday; returns '0'</item>
        /// <item>date1 = July 19 2010(Monday); date2 = July 23 2010(Friday); dow = Wednesday; returns '1'</item>
        /// <item>date1 = June 27 2010(Friday); date2 = August 6 2010(Friday); dow = Monday; returns '6'</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// date1 = DateTime.UtcNow;
        /// date2 = DateTime.UtcNow.AddDays(7);
        /// numDays = NumberOfWeekdayBetweenDates(date1, date2, DayOfWeek.Friday);
        /// (Here, numDays = 1)
        /// </code>
        /// </example>
        public static int NumberOfWeekdayBetweenDates(this System.DateTime date1, System.DateTime date2, DayOfWeek dow)
        {
            System.DateTime d1 = date1;
            System.DateTime d2 = date2;

            if (date2 < date1)
            {
                d1 = date2;
                d2 = date1;
            }

            d1 = dow >= d1.DayOfWeek ? d1.AddDays(dow - d1.DayOfWeek) : d1.AddDays(7 - (int)d1.DayOfWeek + (int)dow);

            d2 = dow >= d2.DayOfWeek ? d2.AddDays(dow - d2.DayOfWeek) : d2.AddDays(7 - (int)d2.DayOfWeek + (int)dow);

            int tS = d2.Subtract(d1).Days;
            int answr = tS / 7;

            if (date1.DayOfWeek == dow)
            {
                answr -= 1;
            }

            if (date2.DayOfWeek == dow)
            {
                answr += 1;
            }

            return answr;
        }
    }
}
