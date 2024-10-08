﻿using AutoMapper;
using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Schedulers.Appointment;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.CryptoGraphy;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using HMSPortal.Application.ViewModels.Notification;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using HMSPortal.Domain.Models.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static HMSPortal.Models.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IJobScheduleService jobScheduleService;
		public string rootPath { get; set; }
		public AppointmentRespository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration, IHostingEnvironment hostingEnvironment, INotificatioServices notificatioServices, ICryptographyService cryptographyService, IHttpContextAccessor contextAccessor, AppointmentScheduler appointmentScheduler, IJobScheduleService jobScheduleService)
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
			this.jobScheduleService=jobScheduleService;
		
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
            var time = _appointmentScheduler.ConvertToDateTimeRange(viewModel.TimeSlot, viewModel.Date);
            AppointmentModel appointment = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                StartTime = time.StartTime,
                Endtime = time.StopTime,
                Date = time.StartTime,
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
        public async Task<AppResponse> CreateAppointmentByPatient(AddAppointmentViewModel viewModel)
        {
            var patient = _dbContext.Patients.FirstOrDefault(x => x.UserId ==  viewModel.PatientId);
            var time = _appointmentScheduler.ConvertToDateTimeRange(viewModel.TimeSlot, viewModel.Date);

            AppointmentModel appointment = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                StartTime = time.StartTime,
                Endtime = time.StopTime,
                Date = time.StartTime,
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

		public async Task<AppResponse> CancelAppointmentById( string appointmentId)
		{
			appointmentId = appointmentId.Replace(" ", "");
			var appointment = _dbContext.Appointments.Include("Patient").FirstOrDefault(x => x.Id == Guid.Parse(appointmentId));


			try
			{
				var patient = appointment.Patient;
				appointment.Status = AppointmentStatus.Cancelled.ToString();
				_dbContext.Appointments.Update(appointment);
				await _dbContext.SaveChangesAsync();
				var emailTemplate = new PatientGenericEmailModel
				{
					Name = patient.FirstName,
					Email = patient.Email,
					Subject = "Appointment Cancellation",
					Message = $"Kindly note that your upcoming appointment with reference {appointmentId}  scheduled for {appointment.Date.Date.ToString("d/MM/yyyy")} at {appointment.TimeSlot} has been cancelled"

				};
				await Task.Run(() => _notificatioServices.SendGenricMessage(emailTemplate));



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


		public async Task<AppResponse> CancelAppointment(string userId, string appointmentId)
        {
            appointmentId = appointmentId.Replace(" ", "");
            var appointment = _dbContext.Appointments.FirstOrDefault(x => x.ReferenceNumber.Contains(appointmentId));


            try
            {
                var patient =  _dbContext.Patients.FirstOrDefault(x=> x.UserId == userId);
                appointment.Status = AppointmentStatus.Cancelled.ToString();
                _dbContext.Appointments.Update(appointment);
                await _dbContext.SaveChangesAsync();
                var emailTemplate = new PatientGenericEmailModel
                {
                    Name = patient.FirstName,
                    Email = patient.Email,
                    Subject = "Appointment Cancellation",
                    Message = $"Kindly note that your upcoming appointment with reference {appointmentId}  scheduled for {appointment.Date.Date.ToString("d/MM/yyyy")} at {appointment.TimeSlot} has been cancelled"

                };
                await Task.Run(() => _notificatioServices.SendGenricMessage(emailTemplate));



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

        public async Task<AppResponse> RescheduleAppointmentByPatient(AddAppointmentViewModel viewModel, string appointmentId)
        {
            appointmentId = appointmentId.Replace(" ", "");
            var time = _appointmentScheduler.ConvertToDateTimeRange(viewModel.TimeSlot, viewModel.Date);
            var appointment = _dbContext.Appointments.FirstOrDefault(x => x.ReferenceNumber.Contains(appointmentId));
           
            try
            {
                appointment.TimeSlot = viewModel.TimeSlot;
                appointment.Date = time.StartTime;
                appointment.StartTime = time.StartTime;
                appointment.Endtime = time.StopTime;
                appointment.Date = time.StartTime;
                appointment.ApointmentType = AppointmentType.ByPatient.ToString();
                appointment.Status = AppointmentStatus.UpComming.ToString();
                 _dbContext.Appointments.Update(appointment);
                await _dbContext.SaveChangesAsync();
                viewModel.PatientId = appointment.PatientId.ToString();
                await Task.Run(() => _notificatioServices.SendAppointmentConfirmationEmail(viewModel));

                var schedular = new SchedulerHandler
                {
                    model= viewModel,
                    Actuator = viewModel.PatientId,
                    ObjectId = appointment.Id,
                    JobDate = appointment.Date,
                    JobNotificationTime = appointment.Date.AddMinutes(-5),
                    
                };
                await Task.Run(() => jobScheduleService.UpdateScheduledJob(schedular));

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
               var patient = _dbContext.Patients.Where(x=> x.UserId == userId).FirstOrDefault();

                var appointments = await _dbContext.Appointments.Where(x => !x.IsDeleted && x.PatientId == patient.Id)
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
                    UserId = x.UserId,
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

		public async Task<AppResponse> GetAllAppointmentByUser(string userId)
		{

			try
			{

				var appointments = await _dbContext.Appointments.Where(x => !x.IsDeleted && x.Patient.UserId == userId)
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
        
		public async Task<AppResponse> GetAllAppointmentByPatientUser(string userId)
		{

			try
			{
               
				var appointments = await _dbContext.Appointments.Where(x => !x.IsDeleted && x.Patient.UserId == userId)
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

        public async Task<AppResponse> GetAllAppointmentByDoctorUser(string userId)
        {

            try
            {

                var appointments = await _dbContext.Appointments.Include("Doctor").Where(x=> x.DoctorId != null)
                    .Where(x => !x.IsDeleted && x.Doctor.UserId == userId)
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

        public AllAppointmentViewModel GetappointmentById( Guid id)
        {
            var appointment =  _dbContext.Appointments.Where(x => !x.IsDeleted)
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

                }).FirstOrDefault(x => x.Id == id);
                return appointment; 
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
			var user =   contextAccessor.HttpContext.User;
			var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return await _dbContext.ChatModels.Where(x=> x.UserId == userId)
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

		public async Task<AppResponse> LogBotmessage(string userId, string message)
		{
			try
			{
				var model = new ChatModel
				{
					SentAt = DateTime.UtcNow,
					Message = message,
					UserId = userId,
					MessageType = CoreValiables.ChatSent,
					HasOptions = false,
					Options = "",
				};
				await _dbContext.ChatModels.AddAsync(model);
				await _dbContext.SaveChangesAsync();
				return new AppResponse
				{
					IsSuccessful = true,
				};
			}
			catch (Exception ex)
			{
				return new AppResponse();
			}

		}
		public async Task<AppResponse> LogUsermessage(string userId, string message)
		{
			try
			{
				var model = new ChatModel
				{
					SentAt = DateTime.UtcNow,
					Message = message,
					UserId = userId,
					MessageType = CoreValiables.ChatRecieved,
					HasOptions = false,
					Options = "",
				};
				await _dbContext.ChatModels.AddAsync(model);
				await _dbContext.SaveChangesAsync();
				return new AppResponse
				{
					IsSuccessful = true,
				};
			}
			catch (Exception ex)
			{
				return new AppResponse();
			}

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

		public async Task AssignAppointmentToDoctor(Guid doctorId, string appointmentId)
		{
           var appointment =  _dbContext.Appointments.FirstOrDefault(x => x.ReferenceNumber == appointmentId);
            if (appointment != null)
            {
				var doctor = _dbContext.Doctors.FirstOrDefault(x => x.Id == doctorId);
				appointment.DoctorId = doctorId;

                appointment.DoctorId = doctorId;
                _dbContext.Appointments.Update(appointment);
               await _dbContext.SaveChangesAsync();
                var time1 = appointment.Date;
				var notification = new CreateNotificationViewmodel
                {
                    NotificationTime = time1.AddMinutes(-10),
                    Date = appointment.Date,
					Name = "Dr. " + doctor.FirstName,
                    Email = doctor.Email,
                    Subject = "Appointment Assignment",
                    Message = $"Kindly note that  you have an upcomming appointment with Id {appointment.ReferenceNumber} " +
                    $"assigned to you.\n\n Date: {appointment.Date.ToString("dd/MM/yyyy")}\n TimeSlot: {appointment.TimeSlot}\n\n\n Regards"
                };
                await jobScheduleService.ScheduleNotification(notification);

				var emailTemplate = new PatientGenericEmailModel
				{
					Name =  "Dr." + doctor.FirstName,
					Email = doctor.Email,
					Subject = "Upcoming Appointment",
					Message = $"Kindly note that an appointment with Id {appointment.ReferenceNumber} has been assigned to you " +
					$"assigned to you.\n\n Date: {appointment.Date.ToString("dd/MM/yyyy")}\n TimeSlot: {appointment.TimeSlot}\n\n\n Regards"

				};
				await Task.Run(() => _notificatioServices.SendGenricMessage(emailTemplate));
				

			}

		}


	}
}
