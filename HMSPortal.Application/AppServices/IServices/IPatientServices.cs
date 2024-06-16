using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels;
using HMSPortal.Application.ViewModels.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.AppServices.IServices
{
	public interface IPatientServices
	{
		bool CheckExistingPatient(string email);
		Task<AppResponse> CreatePatient(AddPatientViewModel viewModel);
        Task<AppResponse> DeletePatient(Guid id);
        Task<List<GetPatientViewModel>> GetAllPatient();
		GetPatientViewModel GetPatientById(Guid id);
		Task<AppResponse> UpdatePatient(GetPatientViewModel viewModel);
	}
}
