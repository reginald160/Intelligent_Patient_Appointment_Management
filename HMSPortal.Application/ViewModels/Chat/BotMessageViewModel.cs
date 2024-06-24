using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels.Chat
{
    public class BotMessageViewModel
    {
        public string  UserId { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public string Response { get; set; }
        public DateTime SentAt { get; set; }


    }

    public class BotMessage
    {
        public List<BotMessageViewModel> Messages { get; set; }
        public string? UserId { get; set; }
    }


}
