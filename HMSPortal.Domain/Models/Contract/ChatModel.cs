using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models.Contract
{
	public class ChatModel : AuditableEntity
	{
		public string Message { get; set; }
		public DateTime SentAt { get; set; }
        public string MessageType { get; set; }
		public bool Flag { get; set; }

    }
}
