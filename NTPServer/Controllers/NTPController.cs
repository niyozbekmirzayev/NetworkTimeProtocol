using Microsoft.AspNetCore.Mvc;
using System;

namespace NTPServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NTPController : ControllerBase
    {
        [HttpGet]
        public NTPResponse Get()
        {
            // Value left for testing and debugging purposes
            var serverReceivedTime = DateTime.Now;

            return new NTPResponse
            {
                ServerReceivedTime = serverReceivedTime,
                ServerSentTime = DateTime.Now,
                ServerTimeZoneId = TimeZoneInfo.Local.Id.ToString(),
            };
        }
    }
}
