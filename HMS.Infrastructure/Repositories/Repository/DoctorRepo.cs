using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Application.ViewModels.Doctor;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories.Repository
{
	public class DoctorRepo : IDoctorServices
	{
		private readonly ApplicationDbContext _db;
		private readonly IIdentityRespository _identityRespository;

		public DoctorRepo(ApplicationDbContext db, IIdentityRespository identityRespository)
		{
			_db=db;
			_identityRespository=identityRespository;
		}

		public bool CheckExistingDoctor ( string email)
		{
			return _identityRespository.ExistingUserEmail(email);
		}
        public List <GetDoctorViewModel> GetAllDoctors()
        {
            var doctors = _db.Doctors.Where(x => !x.IsDeleted).Select (viewModel => new GetDoctorViewModel
            {
                Id = viewModel.Id,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DoctorCode = viewModel.DoctorCode,
                Specialty = viewModel.Specialty,
                Age = viewModel.Age,
                BackgroundHistory = viewModel.BackgroundHistory,
                Phone = viewModel.Phone,
                DateOfBirth = viewModel.DateOfBirth,
                Address = viewModel.Address,
                Gender = viewModel.Gender,
                Email = viewModel.Email,
                PostalCode = viewModel.PostalCode,
                HouseNumber = viewModel.HouseNumber,
                ImageUrl = viewModel.ImageUrl,
                UserId = viewModel.UserId,
            }).ToList();

			return doctors;
        }
		public async Task UpdateDoctorAsync(EditDoctorViewModel model)
		{
			var doctor = await _db.Doctors.FindAsync(model.Id);

			if (doctor == null)
			{
				throw new Exception("Doctor not found");
			}


			var viewModelProperties = typeof(EditDoctorViewModel).GetProperties();
			var entityProperties = typeof(Doctor).GetProperties().Select(p => p.Name).ToHashSet();

			foreach (var property in viewModelProperties)
			{
				if (entityProperties.Contains(property.Name))
				{
					var newValue = property.GetValue(model);
					var currentValue = typeof(Doctor).GetProperty(property.Name)?.GetValue(doctor);

					if (newValue != null && !newValue.Equals(currentValue))
					{
						typeof(Doctor).GetProperty(property.Name)?.SetValue(doctor, newValue);
						_db.Entry(doctor).Property(property.Name).IsModified = true;
					}
				}
			}

			await _db.SaveChangesAsync();
		}
	
	public GetDoctorViewModel GetDoctorById(Guid id)
		{
			var viewModel = _db.Doctors.FirstOrDefault(x => x.Id == id);

			return new GetDoctorViewModel
			{
				Id = viewModel.Id,
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				DoctorCode = viewModel.DoctorCode,
				Specialty = viewModel.Specialty,
				YearsOfExperience = viewModel.YearsOfExperience,
				Age = viewModel.Age,
				BackgroundHistory = viewModel.BackgroundHistory,
				Phone = viewModel.Phone,
				DateOfBirth = viewModel.DateOfBirth,
				Address = viewModel.Address,
				Gender = viewModel.Gender,
				Email = viewModel.Email,
				PostalCode = viewModel.PostalCode,
				HouseNumber = viewModel.HouseNumber,
				ImageUrl = viewModel.ImageUrl,
				UserId = viewModel.UserId,
			};
		}
		public async Task<AppResponse> CreateDoctor(AddDoctorViewModel viewModel)
		{
			bool existingUser = _db.Users.Any(x=> x.Email == viewModel.Email);
			if(existingUser)
			{
				return new AppResponse
				{
					Message = "User with email " + viewModel.Email+" already exist",
					ResponseCode = "01",
					IsSuccessful = false,


				};
			}
			var userId = await _identityRespository.CreateUser(viewModel.Email ?? "Admin@gmail.com", viewModel.Password, Roles.Doctor);
			var seqNumber = await new SequenceContractHelper().GenerateNextPatientNumberAsync(3);

			var doctorModel = new Doctor
			{
				FirstName = viewModel.FirstName,
				Age = viewModel.Age,
				YearsOfExperience = viewModel.YearsOfExperience,
				Specialty = viewModel.Specialty,
				LastName = viewModel.FirstName,
				DoctorCode = "DT/"+ seqNumber.ToString(CoreValiables.SequenceNumberFormat),
				Phone = viewModel.Phone,
				DateOfBirth = viewModel.DateOfBirth,
				Address = viewModel.Address,
				Gender = viewModel.Gender,
				Email = viewModel.Email,
				DoctorDetails = viewModel.DoctorDetails,
				PostalCode = "",
				HouseNumber = "",
				ImageUrl = viewModel.ImageUrl,
				SerialNumber = "PD/"+ seqNumber.ToString(CoreValiables.SequenceNumberFormat),
				UserId = userId,

			};
			try
			{
				var response = await _db.Doctors.AddAsync(doctorModel);
				await _db.SaveChangesAsync();
				await new SequenceContractHelper().UpdateSequence(seqNumber, 3);
				return new AppResponse
				{
					IsSuccessful = true,
					Data  = response.Entity.Id
				};


			}
			catch (Exception ex)
			{
				await _identityRespository.DeleteUser(viewModel.Email);
				return new AppResponse { IsSuccessful = false };
			}
		}

        public async Task<AppResponse> DeleteDoctor(Guid id)
        {
            try
            {
                var doctor = _db.Doctors.FirstOrDefault(x => x.Id ==id);
                doctor.IsDeleted = true;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
                return new AppResponse { IsSuccessful = true };
            }
            catch (Exception ex)
            {
                return new AppResponse
                {
                    Message = ex.Message,
                };
            }
        }
		
	}
}
