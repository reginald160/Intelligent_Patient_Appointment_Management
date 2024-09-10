using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels.Notification
{
	public class AllNotificationViewModel
	{
		public Guid Id { get; set; }
		public string? Message { get; set; }
		public string? ObjectId { get; set; }
		public string? Status { get; set; }
		public DateTime Date { get; set; }
		public DateTime NotificationTime { get; set; }
		public bool IsRead { get; set; }
		public string? UserId { get; set; }
	}
}
