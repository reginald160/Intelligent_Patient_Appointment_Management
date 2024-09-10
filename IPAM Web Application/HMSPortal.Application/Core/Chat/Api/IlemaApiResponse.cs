using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Api
{
    public class IlemaApiResponse
    {

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("off_topic")]
        public bool OffTopic { get; set; }

        [JsonProperty("is_greeting")]
        public bool IsGreeting { get; set; }

        [JsonProperty("is_farewell")]
        public bool IsFarewell { get; set; }

    }
}
