using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.Repository
{
	
	public class IdentityRespository : IIdentityRespository
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public IdentityRespository(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
		{
			_roleManager=roleManager;
			_userManager=userManager;
		}

		public bool ExistingUserEmail(string email)
		{
			return _userManager.Users.Any(u => u.Email == email);
		}
		
		public async Task DeleteUser(string email)
		{
			ApplicationUser user = await _userManager.FindByEmailAsync(email) ?? throw new ArgumentNullException(nameof(email));
		 await _userManager.DeleteAsync(user);
		}
		public async Task<string> CreateUser(string username, string password, Roles role)
		{

			var user = new ApplicationUser
			{
				UserName =username.ToUpper(),
				Email = username,
				EmailConfirmed = true,
			};
			var result = await _userManager.CreateAsync(user, password);
			if (result.Succeeded)
			{
				var userRoleName = role.ToString();
				var userRole = await _roleManager.FindByNameAsync(userRoleName);
				if (userRole == null)
				{
					await _roleManager.CreateAsync(new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = userRoleName });
				}
				user = _userManager.Users.FirstOrDefault(x => x.Email == user.Email);
				await _userManager.AddToRoleAsync(user, userRoleName);

				return user.Id;
			}
			else
			{
				throw new Exception("unable to create user");
			}
			

		}

	}
}
