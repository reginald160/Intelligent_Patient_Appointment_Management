﻿using HMSPortal.Application.Core.Response;
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
	
        Task AssignAppointmentToDoctor(Guid doctorId, string appointmentId);
        Task<AppResponse> CancelAppointment(string userId, string appointmentId);
		Task<AppResponse> CancelAppointmentById(string appointmentId);
		Task<AppResponse> CreateAppointmentByAdmin(AddAppointmentViewModel viewModel);
        Task<AppResponse> CreateAppointmentByPatient(AddAppointmentViewModel viewModel);
        Task<AppResponse> CreateAppointmentByPatient(string userId);
        Task<AppResponse> GetAllAppointment();
        Task<AppResponse> GetAllAppointmentByDoctorUser(string userId);
        Task<AppResponse> GetAllAppointmentByPatientUser(string userId);
		Task<AppResponse> GetAllAppointmentByUser(string userId);
        AllAppointmentViewModel GetappointmentById(Guid id);
        List<string> GetAvailableSlotsForDateToString(DateTime date);
        Task<(List<SelectListItem> Patients, List<SelectListItem> Doctors)> GetPatientAndDoctor();
        Task<AppResponse> GetRecentAppointmentByPatient(string userId);
        Task<List<BotMessageViewModel>> GetRecentMessagesAsync(int numberOfMessages);
		Task<AppResponse> LogBotmessage(string userId, string message);
		Task<AppResponse> LogUsermessage(string userId, string message);
		Task<AppResponse> RescheduleAppointmentByPatient(AddAppointmentViewModel viewModel, string appointmentId);
        Task<AppResponse> SaveChat(BotMessageViewModel viewModel);
    }
}
