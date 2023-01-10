
using NTPClient;
using System;
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

            var response = await client.GetAsync($"http://localhost:5000/NTP?clientSentTime={DateTime.Now.ToString()}");
            var content = await response.Content.ReadAsStringAsync();

            var ntpData = Newtonsoft.Json.JsonConvert.DeserializeObject<NTPResponse>(content);

            // calculate delay and offset
            var clientReceivedTime = DateTime.Now;
            var delay = (clientReceivedTime - ntpData.ClientSentTime) - (ntpData.ServerSentTime - ntpData.ServerReceivedTime);
            var offset = ((ntpData.ServerReceivedTime - ntpData.ClientSentTime) + (ntpData.ServerSentTime - clientReceivedTime)) / 2;

            // set local time using offset 
            var localTime = clientReceivedTime + offset;

            Console.WriteLine("Before" + ": " + DateTime.Now);
            SetLocalTime(localTime);
            Console.WriteLine("After" +": "+ localTime);
        }

        static void SetLocalTime(DateTime newTime)
        {
            // create a new SYSTEMTIME struct
            SYSTEMTIME systemTime = new SYSTEMTIME();
            // fill the struct
            systemTime.wYear = (ushort)newTime.Year;
            systemTime.wMonth = (ushort)newTime.Month;
            systemTime.wDayOfWeek = (ushort)newTime.DayOfWeek;
            systemTime.wDay = (ushort)newTime.Day;
            systemTime.wHour = (ushort)newTime.Hour;
            systemTime.wMinute = (ushort)newTime.Minute;
            systemTime.wSecond = (ushort)newTime.Second;
            systemTime.wMilliseconds = (ushort)newTime.Millisecond;

            // call the SetLocalTime function using the SYSTEMTIME struct
            if (!SetLocalTime(ref systemTime))
            {
                // handle error
                Console.WriteLine("Error setting the local time.");
            }
        }

        // this struct is used for the SetLocalTime function
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
