using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.ViewModels;
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
		Task<bool> SendAdminSignUpEmail(PatientEmailModel model);
		Task<bool> SendAppointmentConfirmationEmail(AddAppointmentViewModel appointment);
        Task<bool> SendDoctorAcountConfirmation(DoctorSignupEmailModel model);
        Task<bool> SendDoctorSignUpEmail(DoctorSignupEmailModel model);
        Task<bool> SendForgetPasswordEmail(DoctorSignupEmailModel model);
        Task<bool> SendMail(EmailRequest emailRequest);
		void SendNotification(string message, DateTime appointmentDate);
        Task<bool> SendPatientSignUpEmail(PatientEmailModel model);
    }
}
