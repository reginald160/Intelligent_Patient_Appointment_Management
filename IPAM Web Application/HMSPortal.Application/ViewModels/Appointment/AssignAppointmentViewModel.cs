using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels.Appointment
{
    public class AssignAppointmentViewModel
    {
        public string AppointRef { get; set; }
        public Dictionary<string, Guid> DoctorIds { get; set; }
        public Guid DoctorId { get; set; }
    }
}
