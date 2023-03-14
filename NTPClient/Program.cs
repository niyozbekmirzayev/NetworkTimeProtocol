using NTPClient;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NtpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new HttpClient();

            var clientSentDate = DateTime.Now;
            var response = await client.GetAsync($"http://localhost:5000/NTP");
            var content = await response.Content.ReadAsStringAsync();

            var ntpResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<NTPResponse>(content);

            // Calculate delay and offset
            var clientReceivedTime = DateTime.Now;
            var delay = (clientReceivedTime - clientSentDate) - (ntpResponse.ServerSentTime - ntpResponse.ServerReceivedTime);
            var offset = ((ntpResponse.ServerReceivedTime - clientSentDate) + (ntpResponse.ServerSentTime - clientReceivedTime)) / 2;

            // Set local time using offset and delay 
            var serverTime = DateTime.Now + offset + delay;

            SetLocalTime(serverTime);
            SetSystemTimeZone(ntpResponse.ServerTimeZoneId);
        }

        public static void SetSystemTimeZone(string timeZoneId)
        {
            if (TimeZoneInfo.FindSystemTimeZoneById(timeZoneId) == null)
            {
                Console.WriteLine("Invalid time zone: " + timeZoneId);
                return;
            }

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "tzutil.exe",
                Arguments = "/s \"" + timeZoneId + "\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process != null)
            {
                process.WaitForExit();
                TimeZoneInfo.ClearCachedData();
            }
            else
            {
                Console.WriteLine("No process created");
            }
        }

        static void SetLocalTime(DateTime newTime)
        {
            // create a new SYSTEMTIME struct
            var systemTime = new SYSTEMTIME();
            systemTime.wYear = (ushort)newTime.Year;
            systemTime.wMonth = (ushort)newTime.Month;
            systemTime.wDayOfWeek = (ushort)newTime.DayOfWeek;
            systemTime.wDay = (ushort)newTime.Day;
            systemTime.wHour = (ushort)newTime.Hour;
            systemTime.wMinute = (ushort)newTime.Minute;
            systemTime.wSecond = (ushort)newTime.Second;
            systemTime.wMilliseconds = (ushort)newTime.Millisecond;

            if (!SetLocalTime(ref systemTime))
            {
                Console.WriteLine("Error setting the local time.");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        // this method is used to change the local time
        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref SYSTEMTIME systemTime);
    }
}
