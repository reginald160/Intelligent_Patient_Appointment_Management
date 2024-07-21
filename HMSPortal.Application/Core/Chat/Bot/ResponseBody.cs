using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Bot
{
    public class ChatResponse
    {
        public ChatResponse()
        {
            Endpoint = CoreValiables.ChatTextEndpoint;
        }
        public string Message { get; set; }
        public string NextEndpoint { get; set; }
        public string Endpoint { get; set; }
        public string Body { get; set; }
        public string NextAction { get; set; }
        public bool IsAdditionA { get; set; }
        public ResponseType ResponseType { get; set; }
        public List<string> Messages { get; set; }
    }

    public enum ResponseType
    {
        Text, DropDown, Options
    }
}
