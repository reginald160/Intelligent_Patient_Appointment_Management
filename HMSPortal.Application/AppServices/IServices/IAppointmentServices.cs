using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.AppServices.IServices
{
    public interface IAppointmentServices
    {
		Task AssignAppointmentToDoctor(Guid doctorId, Guid appointmentId);
		Task<AppResponse> CancelAppointment(string userId, string appointmentId);
		Task<AppResponse> CancelAppointmentById(string appointmentId);
		Task<AppResponse> CreateAppointmentByAdmin(AddAppointmentViewModel viewModel);
        Task<AppResponse> CreateAppointmentByPatient(AddAppointmentViewModel viewModel);
        Task<AppResponse> CreateAppointmentByPatient(string userId);
        Task<AppResponse> GetAllAppointment();
		Task<AppResponse> GetAllAppointmentByUser(string userId);
		List<string> GetAvailableSlotsForDateToString(DateTime date);
        Task<(List<SelectListItem> Patients, List<SelectListItem> Doctors)> GetPatientAndDoctor();
        Task<AppResponse> GetRecentAppointmentByPatient(string userId);
        Task<List<BotMessageViewModel>> GetRecentMessagesAsync(int numberOfMessages);
        Task<AppResponse> RescheduleAppointmentByPatient(AddAppointmentViewModel viewModel, string appointmentId);
        Task<AppResponse> SaveChat(BotMessageViewModel viewModel);
    }
}
