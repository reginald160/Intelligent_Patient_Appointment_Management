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
		public string rootPath { get; set; }

		public NotificationRepository(IConfiguration configuration,
			ICryptographyService cryptographyService,
			SMTPSettings sMTPSettings,
			ApplicationDbContext dbContext,
			IWebHostEnvironment hostingEnvironment)
		{
			this.configuration=configuration;
			_cryptographyService=cryptographyService;
			_smtpConfiguration=sMTPSettings;
			_dbContext=dbContext;
			_hostingEnvironment=hostingEnvironment;

		}

		public void SendNotification(string message, DateTime appointmentDate)
		{
			// Logic to send notification
			Console.WriteLine($"{message} for appointment at {appointmentDate}");
		}

		public async Task<bool> SendMail(EmailRequest emailRequest)
		{
			try
			{
		
				//var key1 = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
				var sender = _cryptographyService.Base64Decode(_smtpConfiguration.sendgridSender);
				var key = _cryptographyService.Base64Decode(_smtpConfiguration.sendgridKey);
				using var message = new MailMessage();
				message.From = new MailAddress(sender, _smtpConfiguration.sendgridName);

				message.IsBodyHtml = true;
				message.To.Add(new MailAddress(emailRequest.To, "Easy Medicare"));
				message.Body = emailRequest.Body;

				message.Subject = emailRequest.Subject;
				if (emailRequest.Attachments.Count > 0)
				{
					foreach (var attachment in emailRequest.Attachments)
					{
						string fileName = Path.GetFileName(attachment.FileName);
						message.Attachments.Add(new System.Net.Mail.Attachment(attachment.OpenReadStream(), fileName));
					}
				}

				using var client = new SmtpClient(host: "smtp.sendgrid.net", port: 587);
				client.Credentials = new NetworkCredential(
					userName: "apikey",
					password: key
					);


				await client.SendMailAsync(message);
				return true;
			}
			catch (Exception ex)
			{

				//var model = $"{JsonConvert.SerializeObject(emailRequest)}";
				//var message = $"{"internal server occured while sending email"}{" - "}{ex}{" - "}{model}{DateTime.Now}";
				//_adminLogger.LogRequest(message, true);
				//var fialedMessage = new FailedEmailRequest
				//{
				//	To = emailRequest.To,
				//	Body = emailRequest.Body,
				//	Subject = emailRequest.Subject
				//};
				try
				{

					//await _bounceDbContext.AddAsync(fialedMessage);
					//await _bounceDbContext.SaveChangesAsync();


				}
				catch (Exception exp)
				{
					///var failedMessageModel = $"{JsonConvert.SerializeObject(fialedMessage)}";
					//var Failedmessage = $"{"internal server occured while Saving FailedEmailRequest data "}{" - "}{exp}{" - "}{model}{DateTime.Now}";
					//_adminLogger.LogRequest(Failedmessage, true);
				}

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
