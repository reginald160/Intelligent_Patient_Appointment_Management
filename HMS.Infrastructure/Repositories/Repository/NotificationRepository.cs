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
using HMSPortal.Application.ViewModels.Notification;
using Microsoft.EntityFrameworkCore;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using HMS.Infrastructure.Migrations;
using Microsoft.CodeAnalysis;

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
		public async Task<bool> SendAdminSignUpEmail(PatientEmailModel model)
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
					Body = EmailFormatter.GenerateEmailTemplate(rootPath, "adminAccountConfirmation.html", replacements),
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
        public async Task<bool> SendGenricMessage(PatientGenericEmailModel template)
        {
            try
            {
                var emailSetting = _dbContext.EmailSettings.FirstOrDefault();
                template.LogoUrl = emailSetting.Logo;
                template.BGImageUrl = emailSetting.BackgroundImage;

                rootPath = _hostingEnvironment.ContentRootPath;
                var emailRequest = new EmailRequest
                {
                    To = template.Email,
                    Body = EmailFormatter.FormatUserGeneric(rootPath, template),
                    Subject = template.Subject
                };

                await SendgridEmail(emailRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendNotificationMessage(PatientGenericEmailModel template)
        {
            try
            {
                var emailSetting = _dbContext.EmailSettings.FirstOrDefault();
                template.LogoUrl = emailSetting.Logo;
                template.BGImageUrl = emailSetting.BackgroundImage;

                rootPath = _hostingEnvironment.ContentRootPath;
                var emailRequest = new EmailRequest
                {
                    To = template.Email,
                    Body = EmailFormatter.FormatUserGeneric(rootPath, template),
                    Subject = template.Subject
                };

                await SendgridEmail(emailRequest);

               var notification = _dbContext.Notifications.FirstOrDefault(x => x.Id == Guid.Parse(template.NotificationId));
                notification.Status = "Completed";
                _dbContext.Notifications.Update(notification);
               await _dbContext.SaveChangesAsync();


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
				var patient = _dbContext.Patients.FirstOrDefault(x => x.Id == Guid.Parse(appointment.PatientId));
				var template = new AppointmentEmailModel
				{
					PatientName = patient.FirstName,
					location = "30 seaton road, DA16 1DU",
					LogoUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
					BGImageUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
					Date = appointment.Date,
					Time = appointment.TimeSlot
				};
				var emailRequest = new EmailRequest
				{
					To = patient.Email,
					Body = EmailFormatter.FormatAppointment(rootPath, template),
					Subject = "Appointment Confirmation"
				};

				await SendgridEmail(emailRequest);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

        public async Task<bool> CreateNotification(CreateNotificationViewmodel notification)
        {

           
            // Create a new notification
            var model = new Notification
            {
                DateCreated = notification.Date,
                Date = notification.Date,
                Message = notification.Message,
                UserId = notification.UserId,
               
            };

            _dbContext.Notifications.Add(model);
           await  _dbContext.SaveChangesAsync();
            

            return true;
        }
        public  async Task<bool>SendSMS(string to, string messageBody)
        {

            var sid = _configuration["TwilioSetting:Sid"];
            var token = _configuration["TwilioSetting:Token"];
            var systemNumber = _configuration["TwilioSetting:SystemNumber"];
            TwilioClient.Init(sid, token);

            var message = await MessageResource.CreateAsync(
                from: new Twilio.Types.PhoneNumber(systemNumber),
                body: messageBody,
                to: new Twilio.Types.PhoneNumber(to));

            if (message == null || message.Status.ToString() == "Failed")
                return false;
            return true;
        }
        public async Task CreateDefaultNotificationSetting(string userId)
        {
            var model = new HMSPortal.Domain.Models.Settings.UserNotificationSettings
            {
                UserId = userId,
                NotificationInterval = 5,
                IsEmailCheck = "true",
                IsSMSCheck = "true",
            };

            await _dbContext.UserNotificationSettings.AddAsync(model);
            await _dbContext.SaveChangesAsync();
         }

    }
}
