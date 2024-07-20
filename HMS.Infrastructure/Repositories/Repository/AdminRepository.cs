using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.Core.Response;
using HMSPortal.Application.ViewModels;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMSPortal.Application.ViewModels.Admin;
using Microsoft.Extensions.Logging;
using HMSPortal.Application.ViewModels.Patient;
using Microsoft.AspNetCore.Identity;

namespace HMS.Infrastructure.Repositories.Repository
{
	public class AdminRepository : IAdminServices
	{
		private readonly ApplicationDbContext _db;
		private readonly IIdentityRespository _identityRespository;
		private readonly IMemoryCache _memoryCache;
		private readonly INotificatioServices _notificatioServices;
		private ILogger<AdminRepository> _logger;

		public AdminRepository(ApplicationDbContext db, IIdentityRespository identityRespository,
			IMemoryCache memoryCache, INotificatioServices notificatioServices, ILogger<AdminRepository> logger)
		{
			_db=db;
			_identityRespository=identityRespository;
			_memoryCache=memoryCache;
			_notificatioServices=notificatioServices;
			_logger=logger;
		}

		public async Task<AppResponse> CreateAdmin(AddAdminViewModel viewModel)
		{

			var userId = await _identityRespository.CreateUser(viewModel.Email ?? "Admin@gmail.com", "adminUser160@", viewModel.Role == "Admin" ? Roles.Admin : Roles.SuperAdmin);

			var adminModel = new AdminModel
			{
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				Phone = viewModel.Phone,
				DateOfBirth = viewModel.DateOfBirth,
				Gender = viewModel.Gender,
				Role = viewModel.Role,
				Email = viewModel.Email,
				ImageUrl = viewModel.ImageUrl,
				UserId = userId,

			};
			try
			{
				 var admin = await _db.AdminModels.AddAsync(adminModel);
				await _db.SaveChangesAsync();
				
				var adminEmailModel = new PatientEmailModel
				{
					Email = adminModel.Email,
					LogoUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
					Link =  await _identityRespository.GenerateEmailConfirmationLinkAsync(adminModel.Email),
					Name = viewModel.FirstName,
				};
				Task.Run(async () =>
				{
					await _notificatioServices.SendAdminSignUpEmail(adminEmailModel);


				});
				_logger.LogInformation($"Admin user : {adminModel.Email} has been created");
				return new AppResponse
				{
					IsSuccessful = true,
					Data = admin.Entity.Id
				};


			}
			catch (Exception ex)
			{
				_logger.LogError($"Errro occured while,creatining admin: {ex}");
				await _identityRespository.DeleteUser(viewModel.Email);
				return new AppResponse { IsSuccessful = false };
			}
		}

        public GetAdminViewModel GetAdminById(Guid id)
        {
			var viewModel = _db.AdminModels.FirstOrDefault(x => x.Id == id);

			return new GetAdminViewModel
			{
				Id = viewModel.Id,
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				Phone = viewModel.Phone,
				DateOfBirth = viewModel.DateOfBirth,
				Gender = viewModel.Gender,
				Email = viewModel.Email,
				Role = viewModel.Role,
				ImageUrl = viewModel.ImageUrl,
				UserId = viewModel.UserId,
				IsLocked = viewModel.IsLocked,
			};

          
        }

        public List<GetAdminViewModel> GetAllAdmin()
		{

            var admins = _db.AdminModels.Where(x=> !x.IsDeleted).Select(viewModel =>
			new GetAdminViewModel
			{
				Id = viewModel.Id,
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				Phone = viewModel.Phone,
				DateOfBirth = viewModel.DateOfBirth,
				Gender = viewModel.Gender,
				Email = viewModel.Email,
				Role = viewModel.Role,
				ImageUrl = viewModel.ImageUrl,
				UserId = viewModel.UserId,
				IsLocked = viewModel.IsLocked,
			}).ToList();

			return admins;
		}

		public async Task<AppResponse> UpdateAdmin(GetAdminViewModel viewModel)
		{
			var admin = _db.AdminModels.FirstOrDefault(x => x.Id == viewModel.Id);
			admin.Gender = viewModel.Gender;
			admin.FirstName = viewModel.FirstName;
			admin.LastName = viewModel.LastName;
			admin.Role = viewModel.Role;
			admin.Phone = viewModel.Phone;
			admin.DateOfBirth = viewModel.DateOfBirth;
			if (viewModel.Image != null)
				admin.ImageUrl = viewModel.ImageUrl;

			try
			{
				_db.AdminModels.Update(admin);
				await _db.SaveChangesAsync();
				return new AppResponse { IsSuccessful = true };
			}
			catch (Exception ex)
			{
				return new AppResponse { IsSuccessful = false };
			}



		}

        public async Task<AppResponse> DeleteAdmin(Guid id)
        {
            try
            {

                var admin = _db.AdminModels.FirstOrDefault(x => x.Id ==id);
                admin.IsDeleted = true;
                _db.AdminModels.Update(admin);
                await _db.SaveChangesAsync();
				_identityRespository.LockUser(admin.Email);
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
        public async Task<AppResponse> LockAdmin(Guid id)
        {
            try
            {

                var admin = _db.AdminModels.FirstOrDefault(x => x.Id ==id);
                admin.IsLocked = true;
                _db.AdminModels.Update(admin);
                await _db.SaveChangesAsync();
               await  _identityRespository.LockUser(admin.Email);
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

        public async Task<AppResponse> UnLockAdmin(Guid id)
        {
            try
            {

                var admin = _db.AdminModels.FirstOrDefault(x => x.Id ==id);
                admin.IsLocked = false;
                _db.AdminModels.Update(admin);
                await _db.SaveChangesAsync();
               await  _identityRespository.UnLockUser(admin.Email);
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
