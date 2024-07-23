using HMSPortal.Application.ViewModels.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HMSPortal.Application.Core.Notification.Email
{
	public class EmailFormatter
	{
		public static string MailPath = "Statics\\EmailTemplates";
		public static string AppointmentPath = "newAppointment.html";
		public static string DoctorConfirmSignUpPath = "doctorSignUp.html";
        public static string PatientGenericEmailPath = "patientGeneric.html";

        public static string ConfirmationEmail = "EmailConfirmation.html";
		public static string EmailResponse = "EmailResponse.html";
		public static string TokenEmail = "Token.html";
		public static string genericEmail = "GenericEmail.html";
        private static object appointment;

        public static string FormatEmaiConfimation(string url, string rootPath)
		{
			string templateRootPath = CombinePath(rootPath, ConfirmationEmail);
			string content = string.Empty;
			using var sr = new StreamReader(templateRootPath);
			content = sr.ReadToEnd();
			content = content.Replace("{{Url}}", url);
			return content;
		}

		public static string FormatAppointment(string rootPath, AppointmentEmailModel appointment)
		{

			string templateRootPath = CombinePath(rootPath, AppointmentPath);
			string content = string.Empty;
			using var sr = new StreamReader(templateRootPath);
			string googleCalenderLink = GenerateGoogleCalendarLink("Medical Apppointment", appointment.Date, appointment.Date.AddMinutes(30), "Medical appointment", appointment.location);
			content = sr.ReadToEnd(); 
			content = content.Replace("{{bgImageUrl}}", appointment.BGImageUrl);
			content = content.Replace("{{LogoURL}}", appointment.LogoUrl);
			content = content.Replace("{{AppointmentDate}}", appointment.Date.ToString("dd/MMM/yyyy"));
			content = content.Replace("{{AppointmentTime}}", appointment.Time);
			content = content.Replace("{{PatientName}}", appointment.PatientName);
            content = content.Replace("{{RescheduleLink}}", appointment.RescheduleLink);
            content = content.Replace("{{ClinicLocation}}", appointment.location);
			content = content.Replace("{{GoogleCalendarLink}}", googleCalenderLink);
			return content;
		}

        public static string FormatUserGeneric(string rootPath, PatientGenericEmailModel message)
        {

            string templateRootPath = CombinePath(rootPath, EmailFormatter.PatientGenericEmailPath);
            string content = string.Empty;
            using var sr = new StreamReader(templateRootPath);
            content = sr.ReadToEnd();
            content = content.Replace("{{bgImageUrl}}", message.BGImageUrl);
            content = content.Replace("{{LogoURL}}", message.LogoUrl);
            content = content.Replace("{{Subject}}", message.Subject);
            content = content.Replace("{{Name}}", message.Name);
            content = content.Replace("{{Message}}", message.Message);
         
            return content;
        }

        public static string FormatDoctorAccountActivation(string rootPath, DoctorSignupEmailModel model)
        {

            string templateRootPath = CombinePath(rootPath, "DoctorAccountConfirmation.html");
            string content = string.Empty;
            using var sr = new StreamReader(templateRootPath);
            content = sr.ReadToEnd();
            content = content.Replace("{{bgImageUrl}}", model.BGImageUrl);
            content = content.Replace("{{LogoURL}}", model.LogoUrl);
            content = content.Replace("{{Link}}", model.SetPasswordLink);
            content = content.Replace("{{Name}}", model.DoctorName);
            return content;
        }

        public static  string GenerateEmailTemplate(string rootPath, string htmlPath,  Dictionary<string, string> replacements)
        {
            string templateRootPath = CombinePath(rootPath, htmlPath);
            string content = string.Empty;

            using (var sr = new StreamReader(templateRootPath))
            {
                content = sr.ReadToEnd();
            }

            // Perform replacements based on the dictionary
            foreach (var replacement in replacements)
            {
                content = content.Replace("{{"+replacement.Key +"}}", replacement.Value);
            }

            return content;
        }

        public static string FrorgotPassword(string rootPath, DoctorSignupEmailModel model)
        {

            string templateRootPath = CombinePath(rootPath, "ForgetPassword.html");
            string content = string.Empty;
            using var sr = new StreamReader(templateRootPath);
            content = sr.ReadToEnd();

            content = content.Replace("{{LogoURL}}", model.LogoUrl);
            content = content.Replace("{{Link}}", model.SetPasswordLink);
            content = content.Replace("{{Name}}", model.DoctorName);
            return content;
        }

        public static string FormatDoctorSignUpEmail(string rootPath, DoctorSignupEmailModel model)
        {

            string templateRootPath = CombinePath(rootPath, DoctorConfirmSignUpPath);
            string content = string.Empty;
            using var sr = new StreamReader(templateRootPath);
            content = sr.ReadToEnd();
            content = content.Replace("{{bgImageUrl}}", model.BGImageUrl);
            content = content.Replace("{{LogoURL}}", model.LogoUrl);
            content = content.Replace("{{link}}", model.SetPasswordLink);
            content = content.Replace("{{Specialization}}", model.Specialization);
            content = content.Replace("{{Name}}", model.DoctorName);
            return content;
        }


        public static string FormatGenericEmail(string message, string rootPath, string subject = "")
		{
			string templateRootPath = CombinePath(rootPath, genericEmail);
			string content = string.Empty;
			using var sr = new StreamReader(templateRootPath);
			content = sr.ReadToEnd();
			content = content.Replace("{{subject}}", subject);
			content = content.Replace("{{message}}", message);
			return content;
		}

		public static string FormatEmailResponse(string url, string rootPath)
		{
			string templateRootPath = CombinePath(rootPath, EmailResponse);
			string content = string.Empty;
			using var sr = new StreamReader(templateRootPath);
			content = sr.ReadToEnd();
			content = content.Replace("{{Url}}", url);
			return content;
		}

		private static string CombinePath(string rootpath, string name)
		{
			return Path.Combine(rootpath, MailPath, name);
		}

		public static string GenerateGoogleCalendarLink(string eventTitle, DateTime startDate,
			DateTime endDate, string description, string location)
		{
			// Format the dates to the required format
			string startDateFormatted = startDate.ToString("yyyyMMddTHHmmss");
			string endDateFormatted = endDate.ToString("yyyyMMddTHHmmss");

			// Encode the parameters to ensure they are URL-safe
			string eventTitleEncoded = HttpUtility.UrlEncode(eventTitle);
			string descriptionEncoded = HttpUtility.UrlEncode(description);
			string locationEncoded = HttpUtility.UrlEncode(location);

			// Construct the Google Calendar link
			string googleCalendarLink = $"https://calendar.google.com/calendar/r/eventedit?text={eventTitleEncoded}&dates={startDateFormatted}/{endDateFormatted}&details={descriptionEncoded}&location={locationEncoded}";

			return googleCalendarLink;
		}
	}
}
