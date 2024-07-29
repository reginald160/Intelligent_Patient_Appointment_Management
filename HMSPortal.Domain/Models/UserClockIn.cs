using HMSPortal.Domain.Models.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
    public class UserClockIn : AuditableEntity
    {
        public string UserId { get; set; }
        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
    }
}
