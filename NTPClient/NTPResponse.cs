using System;

namespace NTPClient
{
    internal class NTPResponse
    {
        public DateTime CurrentServerTime { get; set; }
        public string ServerTimeZoneId { get; set; }
    }
}
