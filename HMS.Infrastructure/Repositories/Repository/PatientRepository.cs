using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class PatientRepository : IPatientServices
	{
        private readonly ApplicationDbContext _db;
        private readonly IIdentityRespository _identityRespository;


		public PatientRepository(ApplicationDbContext db, IIdentityRespository identityRespository) : base()
		{
			_db = db;
			_identityRespository=identityRespository;
		}
		public GetPatientViewModel GetPatientById(Guid id)
		{
			var viewModel = _db.Patients.FirstOrDefault(x => x.Id == id) ;
			if(viewModel == null)
			{
				return null;
			}

			return new GetPatientViewModel
			{
				Id = viewModel.Id,
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				Height = viewModel.Height,
				Weight = viewModel.Weight,
				PatientCode = viewModel.PatientCode,
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

		public async Task<List<GetPatientViewModel>> GetAllPatient()
		{
			return await _db.Patients.Where(x => !x.IsDeleted).Select(viewModel => new GetPatientViewModel
			{
				Id = viewModel.Id,
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				PatientCode = viewModel.PatientCode,
				Phone = viewModel.Phone,
				DateOfBirth = viewModel.DateOfBirth,
				Address = viewModel.Address,
				Gender = viewModel.Gender,
				Email = viewModel.Email,
				PostalCode = viewModel.PostalCode,
				HouseNumber = viewModel.HouseNumber,
				ImageUrl = viewModel.ImageUrl,
				UserId = viewModel.UserId,
			}).ToListAsync();
		}
        public async Task<AppResponse> DeletePatient(Guid id)
		{
			try
			{
				var patient = _db.Patients.FirstOrDefault(x => x.Id ==id);
				patient.IsDeleted = true;
				_db.Patients.Update(patient);
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
		public async Task<AppResponse> UpdatePatient(GetPatientViewModel viewModel)
		{
			var patient =  _db.Patients.FirstOrDefault(x => x.Id == viewModel.Id);
			patient.Height = viewModel.Height;
			patient.Weight = viewModel.Weight;
			patient.Gender = viewModel.Gender;
			patient.FirstName = viewModel.FirstName;
			patient.LastName = viewModel.LastName;
			patient.HouseNumber = viewModel.HouseNumber;
			patient.Address = viewModel.Address;
			patient.DateOfBirth = viewModel.DateOfBirth;
			if (viewModel.Image != null)
				patient.ImageUrl = viewModel.ImageUrl;

			try
			{
				_db.Patients.Update(patient);
				await _db.SaveChangesAsync();
				return new AppResponse { IsSuccessful = true };
			}
			catch (Exception ex)
			{
				return new AppResponse { IsSuccessful = false };
			}


		
		}

		public async Task<AppResponse> CreatePatient(AddPatientViewModel viewModel)
		{
		
			var userId = await _identityRespository.CreateUser(viewModel.Email ?? "Admin@gmail.com", viewModel.Password, Roles.Patient);
			var seqNumber = await new SequenceContractHelper().GenerateNextPatientNumberAsync(1);

			var patientModel = new Patient
			{
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				PatientCode = "PT/"+ seqNumber.ToString(CoreValiables.SequenceNumberFormat),
				Phone = viewModel.Phone,
				Weight = viewModel.Weight,
				Height = viewModel.Height,
				DateOfBirth = viewModel.DateOfBirth,
				Address = viewModel.Address,
				Gender = viewModel.Gender,
				Email = viewModel.Email,
				PostalCode = viewModel.PostalCode,
				HouseNumber = viewModel.HouseNumber,
				ImageUrl = viewModel.ImageUrl,
				SerialNumber = "PT/"+ seqNumber.ToString(CoreValiables.SequenceNumberFormat),
				UserId = userId,

			};
			try
			{
				var patient = await _db.Patients.AddAsync(patientModel);
				await _db.SaveChangesAsync();
				await new SequenceContractHelper().UpdateSequence(seqNumber, 1);
				return new AppResponse
				{
					IsSuccessful = true,
					Data = patient.Entity.Id
				};
	

			}
			catch (Exception ex)
			{
				await _identityRespository.DeleteUser(viewModel.Email);
				return new AppResponse { IsSuccessful = false };
			}
		}

		public void Update(Patient obj)
        {
            _db.Patients.Update(obj);
        }
		public bool CheckExistingPatient(string email)
		{
			return _identityRespository.ExistingUserEmail(email);
		}
	}
}
