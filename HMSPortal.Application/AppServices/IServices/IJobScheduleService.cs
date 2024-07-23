using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.AppServices.IServices
{
    public interface IJobScheduleService
    {
        Task<string> ScheduleAppointmentNotification(SchedulerHandler scheduler);
        Task<string> ScheduleNotification(SchedulerHandler scheduler);
        Task<string> ScheduleNotification(CreateNotificationViewmodel notification);
        Task<string> UpdateScheduledJob(SchedulerHandler scheduler);
	}
}
