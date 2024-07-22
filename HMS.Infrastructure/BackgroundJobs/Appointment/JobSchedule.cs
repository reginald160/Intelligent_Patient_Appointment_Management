using Hangfire;
using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.Repository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.ViewModels.Appointment;
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
    public class JobSchedule : IJobScheduleService
	{
		private readonly INotificatioServices _notificatioServices;
		private readonly ApplicationDbContext _dbContext;

		public JobSchedule(INotificatioServices notificatioServices, ApplicationDbContext dbContext)
		{
			_notificatioServices=notificatioServices;
			_dbContext=dbContext;
		}


        public string ScheduleNotification(Action<string, DateTime> sendNotification, string message, DateTime appointmentDate)
        {
            // Schedule the notification one day before the appointment
            var oneDayBeforeId = BackgroundJob.Schedule(
                () => sendNotification(message, appointmentDate),
                appointmentDate.AddDays(-1));

            // Optionally, store the job ID in your database
            // SaveJobIdToDatabase(oneDayBeforeId, appointmentDate);

            return oneDayBeforeId;
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

        public void ScheduleAppointment(AddAppointmentViewModel appointmentViewmodel)
		{
		
			var appointmentDate = appointmentViewmodel.Date;
			// Schedule the notification one day before the appointment
			var oneDayBeforeId = BackgroundJob.Schedule(
				() => _notificatioServices.SendNotification("One day reminder", appointmentDate),
				appointmentDate.AddDays(-1));

			// Schedule the notification twenty minutes before the appointment
			var twentyMinutesBeforeId = BackgroundJob.Schedule(
				() => _notificatioServices.SendNotification("Twenty minutes reminder", appointmentDate),
				appointmentDate.AddMinutes(-20));

			

			// Store job ids in the database or elsewhere to reference later
			//var appointment = new Appointment
			//{
			//	AppointmentDate = appointmentDate,
			//	OneDayBeforeJobId = oneDayBeforeId,
			//	TwentyMinutesBeforeJobId = twentyMinutesBeforeId
			//};
			//_dbContext.Appointments.Add(appointment);
			//_dbContext.SaveChanges();

		}

	

		//public void CancelAppointment(int appointmentId)
		//{
		//	var appointment = _dbContext.Appointments.Find(appointmentId);
		//	if (appointment == null)
		//	{
		//		return NotFound();
		//	}

		//	// Cancel the Hangfire jobs
		//	BackgroundJob.Delete(appointment.OneDayBeforeJobId);
		//	BackgroundJob.Delete(appointment.TwentyMinutesBeforeJobId);

		//	// Remove appointment from the database
		//	_dbContext.Appointments.Remove(appointment);
		//	_dbContext.SaveChanges();

		//	return Ok("Appointment canceled successfully");
		//}

	}

}
