using Hangfire;
using Hangfire.Common;
using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.Repository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Notification;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.BackgroundJobs.Appointment
{
    [Authorize]
    public class JobSchedule : IJobScheduleService
	{
		private readonly INotificatioServices _notificatioServices;
		private readonly ApplicationDbContext _dbContext;

		public JobSchedule(INotificatioServices notificatioServices, ApplicationDbContext dbContext)
		{
			_notificatioServices=notificatioServices;
			_dbContext=dbContext;
		}



        public async Task<string> ScheduleNotification(CreateNotificationViewmodel notification)
        {
        
            var id = Guid.NewGuid();
            var emailTemplate = new PatientGenericEmailModel
            {
                Name = notification.Name,
                Email = notification.Email,
                Subject = "Notification",
                Message = notification.Message,
                NotificationId = id.ToString(),

            };


            // Schedule the notification one day before the appointment
            var jobId = BackgroundJob.Schedule(
                () => _notificatioServices.SendNotificationMessage(emailTemplate),
                notification.NotificationTime);

            var model = new Notification
            {
                Id = id,
                DateCreated = notification.Date,
                Date = notification.Date,
                Message = notification.Message,
                UserId = notification.UserId,
                ObjectId = jobId,
                NotificationTime = notification.NotificationTime,
                Status = "UpComing",
                

            };

            _dbContext.Notifications.Add(model);
            await _dbContext.SaveChangesAsync();


            return jobId;
        }

        public async Task<string> ScheduleAppointmentNotification(SchedulerHandler scheduler)
        {

            // Schedule the notification one day before the appointment
            var oneDayBeforeId = BackgroundJob.Schedule(
                () => _notificatioServices.SendAppointmentConfirmationEmail(scheduler.model),
                DateTime.Now.AddMinutes(1));

            var jobEvents = new AppointmentEvents
            {
                Comment = scheduler.Comment,
                DateCreated = DateTime.Now,
                JobDate = scheduler.JobDate,
                JobNotificationTime = scheduler.JobNotificationTime,
                ObjectId = scheduler.ObjectId,
                JobId = scheduler.JobId,
			};

			_dbContext.AppointmentEvents.Add(jobEvents);
			await _dbContext.SaveChangesAsync();


			return oneDayBeforeId;
        }
        public async Task<string> UpdateScheduledJob(SchedulerHandler scheduler)
        {
			var job = _dbContext.AppointmentEvents.FirstOrDefault(x => x.ObjectId == scheduler.ObjectId);
			// Schedule the notification one day before the appointment

			// Delete the old job
			try
			{
				if (job != null)
				{
					BackgroundJob.Delete(job.JobId);


					var oneDayBeforeId = BackgroundJob.Schedule(
						() => _notificatioServices.SendAppointmentConfirmationEmail(scheduler.model),
						scheduler.JobNotificationTime);

					job.JobId = oneDayBeforeId;
					job.JobDate = scheduler.JobDate;
					job.JobNotificationTime = scheduler.JobNotificationTime;
					_dbContext.AppointmentEvents.Update(job);
					await _dbContext.SaveChangesAsync();

					return oneDayBeforeId;
				}
				else
				{
					// Schedule the notification one day before the appointment
					var oneDayBeforeId = BackgroundJob.Schedule(
						() => _notificatioServices.SendAppointmentConfirmationEmail(scheduler.model),
						scheduler.JobNotificationTime);

					var jobEvents = new AppointmentEvents
					{
						Comment = scheduler.Comment,
						DateCreated = DateTime.Now,
						JobDate = scheduler.JobDate,
						JobNotificationTime = scheduler.JobNotificationTime,
						ObjectId = scheduler.ObjectId,
						JobId = oneDayBeforeId,
					};

					_dbContext.AppointmentEvents.Add(jobEvents);
					await _dbContext.SaveChangesAsync();


					return oneDayBeforeId;
				}

			}
			catch (Exception ex)
			{
				return "";
			}
        }

        // Method to update a scheduled job
        public string UpdateScheduledJob(string oldJobId, Action<string, DateTime> sendNotification, string newMessage, DateTime newAppointmentDate)
        {
            // Delete the old job
            BackgroundJob.Delete(oldJobId);

            // Schedule a new job with updated parameters
            var newJobId = BackgroundJob.Schedule(
                () => sendNotification(newMessage, newAppointmentDate),
                newAppointmentDate.AddDays(-1));

            // Optionally, update the job ID in your database
            // UpdateJobIdInDatabase(newJobId, newAppointmentDate);

            return newJobId;
        }

        // Method to delete a scheduled job
        public void DeleteScheduledJob(string jobId)
        {
            BackgroundJob.Delete(jobId);
        }

      
		public Task<string> ScheduleNotification(SchedulerHandler scheduler)
		{
			throw new NotImplementedException();
		}



		
	}

	

}
