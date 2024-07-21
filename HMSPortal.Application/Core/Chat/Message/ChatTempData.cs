using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Message
{
    public class ChatTempData
    {
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public string ScheduleType { get; set; }
        public string Result { get; set; }
        public string Slot { get; set; }
        public DateTime Date { get; set; }
        public string HealthCondition { get; set; }
        public List<string> Questions { get; set; }
        public string QuestionsAndAnswers { get; set; }
    }
}
