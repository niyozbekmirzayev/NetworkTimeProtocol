using System;

namespace NTPClient
{
    internal class NTPResponse
    {
        public DateTime CurrentServerTime { get; set; }
        public string ServerTimeZoneStandartName { get; set; }
    }
}
