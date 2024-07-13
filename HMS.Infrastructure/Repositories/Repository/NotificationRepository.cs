using HMS.Infrastructure.BackgroundJobs.Appointment;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Core.Notification.Email;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HMSPortal.Application.Core.CryptoGraphy;
using HMSPortal.Application.AppSettings;
using System.Web;
using HMSPortal.Application.ViewModels.Appointment;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using HMSPortal.Domain.Models;
using HMS.Infrastructure.Persistence.DataContext;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using DHTMLX.Scheduler.Settings;
using HMSPortal.Application.ViewModels;
using Microsoft.Extensions.Options;
using HMS.Infrastructure.Repositories.IRepository;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class NotificationRepository : INotificatioServices
	{
		private readonly IConfiguration configuration;
		private readonly ICryptographyService _cryptographyService;
		private readonly SMTPSettings _smtpConfiguration;
		public readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly  IWebHostEnvironment  _hostingEnvironment;
		private readonly IHttpContextAccessor contextAccessor;
		private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<NotificationRepository> _logger;
        private readonly IIdentityRespository _identityRespository;

        public string rootPath { get; set; }

        public NotificationRepository(IConfiguration configuration,
            ICryptographyService cryptographyService,
            IOptions<SMTPSettings> sMTPSettings,
            ApplicationDbContext dbContext,
            IWebHostEnvironment hostingEnvironment,
            ILogger<NotificationRepository> logger,
            IIdentityRespository identityRespository)
        {
            this.configuration=configuration;
            _cryptographyService=cryptographyService;
            _smtpConfiguration=sMTPSettings.Value;
            _dbContext=dbContext;
            _hostingEnvironment=hostingEnvironment;
            _logger=logger;
            _identityRespository=identityRespository;
        }



        public void SendNotification(string message, DateTime appointmentDate)
		{
			// Logic to send notification
			Console.WriteLine($"{message} for appointment at {appointmentDate}");
		}


        public  async Task SendgridEmail(EmailRequest emailRequest)
        {
            int SmtpPort = _smtpConfiguration.Port; // Using port 587 for TLS
            string Username = _smtpConfiguration.Username;
            string key = _cryptographyService.Base64Decode(_smtpConfiguration.Password);
            string SmtpServer = _smtpConfiguration.Server;
            using var message = new MailMessage();
            message.From = new MailAddress("reginald.ozougwu@yorksj.ac.uk", "MediSmart");

            message.IsBodyHtml = true;
            message.To.Add(new MailAddress("reginald1149@gmail.com", "MediSmart"));
            message.Body = emailRequest.Body;
            message.Subject = emailRequest.Subject;

            using var client = new SmtpClient(host: "smtp.sendgrid.net", port: SmtpPort);
            client.Credentials = new NetworkCredential(
                userName: Username,
                password: key
                );


            client.Send(message);

        }

        public async Task<bool> SendMail(EmailRequest emailRequest)
        {
            try
            {
                int SmtpPort = _smtpConfiguration.Port; // Using port 587 for TLS
                string Username = _smtpConfiguration.Username;
                string Password = _cryptographyService.Base64Decode(_smtpConfiguration.Password);
                string SmtpServer = _smtpConfiguration.Server;

                string from = "reginald.ozougwu@yorksj.ac.uk";
                emailRequest.To = "reginald1149@gmail.com";

                using (var client = new SmtpClient(SmtpServer, SmtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(Username, Password);
                    client.EnableSsl = true;


                    using (var message = new MailMessage(
						from,
						emailRequest.To,
						emailRequest.Subject,
                        emailRequest.Body))
                    {
                        message.IsBodyHtml = true;
                       await  client.SendMailAsync(message);
                    }
                }
                _logger.LogInformation($"Email with subject: {emailRequest.Subject} sent successfully to " + emailRequest.To);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error occured while sending email with Subject: {emailRequest.Subject} to {emailRequest.To}");

                return false;
            }
        }

        public async Task<bool> SendDoctorSignUpEmail(DoctorSignupEmailModel model)
        {
            try
            {
                rootPath = _hostingEnvironment.ContentRootPath;
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "bgImageUrl", model.BGImageUrl },
                    { "LogoURL", model.LogoUrl },
                    { "link", model.SetPasswordLink },
                    { "Specialization", model.DoctorName },
                    { "Name", model.DoctorName }
                };
                var emailRequest = new EmailRequest
                {
                    To = model.To,
                    Body = EmailFormatter.GenerateEmailTemplate(rootPath, "doctorSignUp.html", replacements),
                    Subject = "SignUp Confirmation"
                };

                await SendgridEmail(emailRequest);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> SendPatientSignUpEmail(PatientEmailModel model)
        {
            try
            {
                rootPath = _hostingEnvironment.ContentRootPath;
                Dictionary<string, string> replacements = new Dictionary<string, string>
                {

                    { "LogoURL", model.LogoUrl },
                    { "Link", model.Link },
                    { "Name", model.Name }
                };
                var emailRequest = new EmailRequest
                {
                    To = model.Email,
                    Body = EmailFormatter.GenerateEmailTemplate(rootPath, "patientWelcomeEmail.html", replacements),
                    Subject = "SignUp Confirmation"
                };

                await SendgridEmail(emailRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> SendForgetPasswordEmail(DoctorSignupEmailModel model)
        {
            try
            {
                rootPath = _hostingEnvironment.ContentRootPath;
               
                var emailRequest = new EmailRequest
                {
                    To = model.To,
                    Body = EmailFormatter.FrorgotPassword(rootPath, model),
                    Subject = "Password Reset"
                };

                await SendgridEmail(emailRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //public async Task<bool> PatientWelcomeMessage(PatientEmailModel model)
        //{
        //    try
        //    {
        //        rootPath = _hostingEnvironment.ContentRootPath;
        //        var emailRequest = new EmailRequest
        //        {
        //            To = model.Email,
        //            Body = EmailFormatter.FormatDoctorAccountActivation(rootPath, model),
        //            Subject = "Account Confirmation"
        //        };

        //        await SendgridEmail(emailRequest);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //}


        public async Task<bool> SendDoctorAcountConfirmation(DoctorSignupEmailModel model)
        {
            try
            {
                rootPath = _hostingEnvironment.ContentRootPath;
                var emailRequest = new EmailRequest
                {
                    To = model.To,
                    Body = EmailFormatter.FormatDoctorAccountActivation(rootPath, model),
                    Subject = "Account Confirmation"
                };

                await SendgridEmail(emailRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> SendAppointmentConfirmationEmail(AddAppointmentViewModel appointment)
		{
			try
			{
				rootPath = _hostingEnvironment.ContentRootPath;
				var patient = _dbContext.Patients.FirstOrDefault(x => x.Id.ToString() == appointment.PatientId);
				var template = new AppointmentEmailModel
				{
					PatientName = patient.FirstName,
					location = "30 seaton road, DA16 1DU",
					LogoUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
					BGImageUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
					Date = appointment.Date,
					Time = "2:30 PM"
				};
				var emailRequest = new EmailRequest
				{
					To = "ozougwuifeanyi160@gmail.com",
					Body = EmailFormatter.FormatAppointment(rootPath, template),
					Subject = "Appointment Confirmation"
				};

				await SendMail(emailRequest);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

   
    }
}
