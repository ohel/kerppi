using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
