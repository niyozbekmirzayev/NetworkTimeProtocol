using System;

namespace NTPClient
{
    internal class NTPResponse
    {
        public DateTime ClientSentTime { get; set; }

        public DateTime ServerReceivedTime { get; set; }

        public DateTime ServerSentTime { get; set; }
    }
}
