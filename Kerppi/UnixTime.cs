/*
    Copyright 2015, 2017 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;

namespace Kerppi
{
    class UnixTime
    {
        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        /// <summary>
        /// Returns the DateTime corresponding to given unix time.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(long? unixTime)
        {
            if (unixTime == null) return null;
            long unixTimeStampInTicks = ((long)unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixEpoch.Ticks + unixTimeStampInTicks);
        }

        public static DateTime ToDateTime(long unixTime)
        {
            long unixTimeStampInTicks = (unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixEpoch.Ticks + unixTimeStampInTicks);
        }

        /// <summary>
        /// Returns the unix time corresponding to given DateTime. DateTime may be local or UTC, it is not converted.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long? FromDateTime(DateTime? dateTime)
        {
            if (dateTime == null) return null;
            long unixTimeStampInTicks = ((DateTime)dateTime - unixEpoch).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public static long FromDateTime(DateTime dateTime)
        {
            long unixTimeStampInTicks = (dateTime - unixEpoch).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }
    }
}
