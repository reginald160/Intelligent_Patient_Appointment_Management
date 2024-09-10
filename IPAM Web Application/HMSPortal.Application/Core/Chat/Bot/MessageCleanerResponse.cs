using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Bot
{
    public class MessageCleanerResponse
    {
        public string? Intent { get; set; }
        public string? cleanedMessage { get; set; }
    }
    public class MessageDescriptionResponse
    {
        public string? Department { get; set; }
        public string? Sympton { get; set; }
        public string? NotEnough { get; set; }
        public string? Endpoint { get; set; }
    }
}
