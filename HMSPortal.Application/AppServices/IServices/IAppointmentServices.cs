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
        Task<AppResponse> CreateAppointmentByAdmin(AddAppointmentViewModel viewModel);
        Task<AppResponse> CreateAppointmentByPatient(AddAppointmentViewModel viewModel);
        Task<AppResponse> GetAllAppointment();
		List<string> GetAvailableSlotsForDateToString(DateTime date);
        Task<(List<SelectListItem> Patients, List<SelectListItem> Doctors)> GetPatientAndDoctor();
        Task<List<BotMessageViewModel>> GetRecentMessagesAsync(int numberOfMessages);
        Task<AppResponse> SaveChat(BotMessageViewModel viewModel);
    }
}
