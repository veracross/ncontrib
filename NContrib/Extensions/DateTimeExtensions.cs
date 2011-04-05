using System;

namespace NContrib.Extensions {

    public static class DateTimeExtensions {

        public static DateTime UnixEpoch =  new DateTime(1970, 1, 1);

        /// <summary>Converts a UNIX timestamp to a DateTime object</summary>
        /// <param name="unixTimestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this double unixTimestamp) {
            return UnixEpoch.AddSeconds(unixTimestamp);
        }

        /// <summary>Converts a UTC DateTime to a DateTime in the specified timeZoneId</summary>
        /// <param name="dt"></param>
        /// <param name="timeZoneId">Time zone to convert to</param>
        /// <remarks>
        /// Use TimeZoneInfo.GetSystemTimeZones() to view available TimeZoneIds
        /// Or look in HKLM\Software\Microsoft\Windows NT\CurrentVersion\Time Zones\
        /// </remarks>
        /// <returns></returns>
        public static DateTime ToTimeZone(this DateTime dt, string timeZoneId) {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
        }

        /// <summary>Converts a DateTime from one time zone to another</summary>
        /// <param name="dt"></param>
        /// <param name="fromTimeZoneId">Time zone to convert from</param>
        /// <param name="toTimeZoneId"></param>
        /// <remarks>
        /// Use TimeZoneInfo.GetSystemTimeZones() to view available TimeZoneIds
        /// Or look in HKLM\Software\Microsoft\Windows NT\CurrentVersion\Time Zones\
        /// </remarks>
        /// <returns></returns>
        public static DateTime ToTimeZone(this DateTime dt, string fromTimeZoneId, string toTimeZoneId) {
            var fromTzi = TimeZoneInfo.FindSystemTimeZoneById(fromTimeZoneId);
            var toTzi = TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneId);

            return TimeZoneInfo.ConvertTime(dt, fromTzi, toTzi);
        }

        /// <summary>Converts an offset DateTime in the specified timeZoneId to a UTC DateTime</summary>
        /// <param name="dt"></param>
        /// <param name="timeZoneId">Time zone to convert from</param>
        /// <remarks>
        /// Use TimeZoneInfo.GetSystemTimeZones() to view available TimeZoneIds
        /// Or look in HKLM\Software\Microsoft\Windows NT\CurrentVersion\Time Zones\
        /// </remarks>
        /// <returns></returns>
        public static DateTime ToUniversalTime(this DateTime dt, string timeZoneId) {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
        }

        /// <summary>Converts the given DateTime to a UNIX timestamp</summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(this DateTime dt) {
            return (long)(dt - UnixEpoch).TotalSeconds;
        }
    }
}
