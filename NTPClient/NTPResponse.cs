using System;

namespace NTPClient
{
    internal class NTPResponse
    {
        public DateTime ServerReceivedTime { get; set; }
        public DateTime ServerSentTime { get; set; }
        public string ServerTimeZoneId { get; set; }
    }
}
