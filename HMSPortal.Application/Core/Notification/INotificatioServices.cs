using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.ViewModels.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Notification
{
    public interface INotificatioServices
    {
		Task<bool> SendAppointmentConfirmationEmail(AddAppointmentViewModel appointment);
		Task<bool> SendMail(EmailRequest emailRequest);
		void SendNotification(string message, DateTime appointmentDate);
    }
}
