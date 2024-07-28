using HMSPortal.Domain.Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models.Settings
{
    public class UserNotificationSettings : AuditableEntity
    {
        public string? IsEmailCheck { get; set; }
        public string? IsSMSCheck { get; set; }
        public string? IsWhatapp { get; set; }
        public string UserId { get; set; }
        public int NotificationInterval { get; set; }
        public int NotificationCount { get; set; }
    }
}
