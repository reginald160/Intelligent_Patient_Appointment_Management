using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels.Admin;
using HMSPortal.Application.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.AppServices.IServices
{
	public interface IAdminServices
	{
		Task<AppResponse> CreateAdmin(AddAdminViewModel viewModel);
        Task<AppResponse> DeleteAdmin(Guid id);
        GetAdminViewModel GetAdminById(Guid id);
        List<GetAdminViewModel> GetAllAdmin();
		Task<List<GetDoctorViewModel>> GetAvailableDoctors();
		Task<AppResponse> LockAdmin(Guid id);
        Task<AppResponse> UnLockAdmin(Guid id);
        Task<AppResponse> UpdateAdmin(GetAdminViewModel viewModel);
	}
}
