using AutoMapper;
using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Schedulers.Appointment;
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static HMSPortal.Models.Enums;

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
        private readonly AppointmentScheduler _appointmentScheduler;
        public string rootPath { get; set; }
        public AppointmentRespository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration, IHostingEnvironment hostingEnvironment, INotificatioServices notificatioServices, ICryptographyService cryptographyService, IHttpContextAccessor contextAccessor, AppointmentScheduler appointmentScheduler)
        {
            _dbContext=dbContext;
            _mapper=mapper;
            _configuration=configuration;
            _hostingEnvironment=hostingEnvironment;
            _notificatioServices=notificatioServices;
            _cryptographyService=cryptographyService;
            this.contextAccessor=contextAccessor;
            rootPath = _hostingEnvironment.ContentRootPath;
            _appointmentScheduler=appointmentScheduler;
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
                DoctorId = !string.IsNullOrEmpty(viewModel.DoctorId) ? Guid.Parse(viewModel.DoctorId) : null,
                PatientId =  !string.IsNullOrEmpty(viewModel.PatientId) ? Guid.Parse(viewModel.PatientId) : null,
                ProblemDescrion = viewModel.ProblemDescrion,
                Department = viewModel.Department,
                TimeSlot = viewModel.TimeSlot,
                Status = AppointmentStatus.UpComming.ToString(),
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
        public async Task<AppResponse> CreateAppointmentByPatient(AddAppointmentViewModel viewModel)
        {
            var patient = _dbContext.Patients.FirstOrDefault(x => x.UserId ==  viewModel.PatientId);
            AppointmentModel appointment = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                Date = viewModel.Date,
                DoctorId = !string.IsNullOrEmpty(viewModel.DoctorId) ? Guid.Parse(viewModel.DoctorId) : null,
                PatientId =  patient.Id,
                ProblemDescrion = viewModel.ProblemDescrion,
                Department = viewModel.Department,
                TimeSlot = viewModel.TimeSlot,
                UserId = viewModel.PatientId,
                //Status = "Up coming",
                Status = AppointmentStatus.UpComming.ToString(),
                DateCreated = DateTimeOffset.Now,
                AppointmentType = AppointmentType.ByPatient.ToString(),
                ApointmentType =  viewModel.AppointmentType,
                ReferenceNumber = RandomHelper.GenerateAppointmentReferenceNumber(),


            };

            try
            {
                await _dbContext.Appointments.AddAsync(appointment);
                await  _dbContext.SaveChangesAsync();
                await Task.Run(() => _notificatioServices.SendAppointmentConfirmationEmail(viewModel));



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

        public async Task<AppResponse> GetRecentAppointmentByPatient(string userId)
        {
            //Up coming
            try
            {
               

                var appointments = await _dbContext.Appointments.Where(x => !x.IsDeleted && x.UserId == userId)
                .Select(x => new AllAppointmentViewModel
                {
                    Id = x.Id,
                    Date = x.Date,
                    DoctorComment = x.DoctorComment,
                    DoctorId = x.DoctorId,
                    PatientRef = x.Patient.PatientCode,
                    PatientId = x.PatientId,
                    PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
                    StartTime = x.StartTime,
                    Status = x.Status,
                    ReferenceNumber = x.ReferenceNumber,
                    ProblemDescrion = x.ProblemDescrion,
                    TimeSlot = x.TimeSlot,
                    AppointmentType = x.AppointmentType

                }).ToListAsync();



                return new AppResponse
                {
                    IsSuccessful = true,
                    Data = appointments
                };


            }
            catch(Exception ex)
            {
                return new AppResponse { IsSuccessful = false};
            }
        }

        public async Task<AppResponse> GetAllAppointment()
		{
           
			try
			{
				var appointments = await _dbContext.Appointments.Where(x => !x.IsDeleted)
				.Select(x => new AllAppointmentViewModel
				{
					Id = x.Id,
					Date = x.Date,
					DoctorComment = x.DoctorComment,
					DoctorId = x.DoctorId,
                    PatientRef = x.Patient.PatientCode,
					PatientId = x.PatientId,
					PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
					StartTime = x.StartTime,
					Status = x.Status,
					ReferenceNumber = x.ReferenceNumber,
					ProblemDescrion = x.ProblemDescrion,
					TimeSlot = x.TimeSlot,
					AppointmentType = x.AppointmentType

				}).ToListAsync();



				return new AppResponse
				{
					IsSuccessful = true,
                    Data = appointments
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
                    Content = x.Message,
                    Options = x.Options,
                    HasOptions = x.HasOptions

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
                    MessageType = viewModel.Type,
                    HasOptions = viewModel.HasOptions,
                    Options = string.IsNullOrEmpty(viewModel.Options) ? "" : viewModel.Options ,
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

        public List<string> GetAvailableSlotsForDateToString(DateTime date)
        {
            return _appointmentScheduler.GetAvailableSlotsForDateToString(date);
        }

        public Task<AppResponse> CreateAppointmentByPatient(string userId)
        {
            throw new NotImplementedException();
        }
      

    }
}
