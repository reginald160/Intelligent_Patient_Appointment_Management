using HMSPortal.Application.ViewModels.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels
{
    public class SchedulerHandler
    {
        public Action<string, DateTime> action { get; set; }
        public Action<bool> gsgsgg { get; set; }

        public AddAppointmentViewModel model { get; set; }
        public Guid? ObjectId { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public string? Actuator { get; set; }
        public string JobId { get; set; }

        public DateTime JobDate { get; set; }
        public DateTime JobNotificationTime { get; set; }
        public bool Status { get; set; }
    }
}
