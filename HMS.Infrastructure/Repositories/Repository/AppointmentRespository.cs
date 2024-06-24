using AutoMapper;
using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.CryptoGraphy;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using HMSPortal.Domain.Models.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class AppointmentRespository : IAppointmentServices
    {
        public readonly ApplicationDbContext _dbContext;
        public readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly INotificatioServices _notificatioServices;
		private readonly ICryptographyService _cryptographyService;
		private readonly IHttpContextAccessor contextAccessor;
		public string rootPath { get; set; }
		public AppointmentRespository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration, IHostingEnvironment hostingEnvironment, INotificatioServices notificatioServices, ICryptographyService cryptographyService, IHttpContextAccessor contextAccessor)
		{
			_dbContext=dbContext;
			_mapper=mapper;
			_configuration=configuration;
			_hostingEnvironment=hostingEnvironment;
			_notificatioServices=notificatioServices;
			_cryptographyService=cryptographyService;
			this.contextAccessor=contextAccessor;
			rootPath = _hostingEnvironment.ContentRootPath;
		}
		public async Task<(List<SelectListItem> Patients, List<SelectListItem> Doctors)> GetPatientAndDoctor()
        {
            var patients = await _dbContext.Patients
                                           .Where(p => !p.IsDeleted)
                                           .Select(p => new SelectListItem
                                           {
                                               Value = p.Id.ToString(),
                                               Text = p.PatientCode + "-" + p.LastName,
                                           })
                                           .ToListAsync();

            var doctors = await _dbContext.Doctors
                                          .Where(d => !d.IsDeleted)
                                          .Select(d => new SelectListItem
                                          {
                                              Value = d.Id.ToString(),
                                              Text = d.DoctorCode + "-" + d.LastName,
                                          })
                                          .ToListAsync();

            return (patients, doctors);
        }

        public async Task<AppResponse> CreateAppointmentByAdmin(AddAppointmentViewModel viewModel)
        {
           
            AppointmentModel appointment = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                Date = viewModel.Date,
                DoctorId = Guid.Parse(viewModel.DoctorId),
                PatientId = Guid.Parse(viewModel.PatientId),
                ProblemDescrion = viewModel.ProblemDescrion,
                Department = viewModel.Department,
                TimeSlot = viewModel.TimeSlot,
                DateCreated = DateTimeOffset.Now,
                AppointmentType = AppointmentType.ByAdmin.ToString(),
                ReferenceNumber = RandomHelper.GenerateAppointmentReferenceNumber(),
                

            };
            var patient = _dbContext.Patients.FirstOrDefault(x => x.Id == appointment.PatientId);

			try
            {
               await _dbContext.Appointments.AddAsync(appointment);
               //await  _dbContext.SaveChangesAsync();

                await _notificatioServices.SendAppointmentConfirmationEmail(viewModel);



				return new AppResponse
                {
                    IsSuccessful = true,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new AppResponse();
            }

        }

        public async Task<List<BotMessageViewModel>> GetRecentMessagesAsync(int numberOfMessages)
        {
            return await _dbContext.ChatModels
                .OrderBy(cm => cm.SentAt)
                .Take(numberOfMessages).Select(x=> new BotMessageViewModel
                {
                    UserId = x.UserId,
                    SentAt = x.SentAt,
                    Type = x.MessageType,
                    Content = x.Message
                })
                .ToListAsync();
        }
        public async Task<AppResponse> SaveChat( BotMessageViewModel viewModel)
        {
            try
            {
                var model = new ChatModel
                {
                    SentAt = DateTime.UtcNow,
                    Message = viewModel.Content,
                    UserId = viewModel.UserId,
                    MessageType = viewModel.Type
                };
               await  _dbContext.ChatModels.AddAsync(model);
               await _dbContext.SaveChangesAsync();
                return new AppResponse
                {
                    IsSuccessful = true,
                };
            }
            catch(Exception ex)
            {
                return new AppResponse();
            }

        }

    }
}
