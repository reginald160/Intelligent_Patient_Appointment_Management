using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels.Patient;
using HMSPortal.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMSPortal.Application.ViewModels.Doctor;

namespace HMSPortal.Application.AppServices.IServices
{
	public interface IDoctorServices
	{
		bool CheckExistingDoctor(string email);
		Task<string> ClockInAsync(string doctorId);
		Task<string> ClockOutAsync(string doctorId);
		Task<AppResponse> CreateDoctor(AddDoctorViewModel viewModel);
		Task<AppResponse> DeleteDoctor(Guid id);
        List<GetDoctorViewModel> GetAllDoctors();
        Task<Dictionary<string, Guid>> GetAllDoctorsDroptDown(string department = null);
        GetDoctorViewModel GetDoctorById(Guid id);
		bool GetUserClockIn(string doctorId);
		Task UpdateDoctorAsync(EditDoctorViewModel model);
	}
}
