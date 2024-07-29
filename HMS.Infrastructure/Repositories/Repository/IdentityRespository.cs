using Azure.Core;
using CloudinaryDotNet;
using Confluent.Kafka;
using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.Core.Chat.SignalR;
using HMSPortal.Domain.Enums;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.Repository
{
	
	public class IdentityRespository : IIdentityRespository
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly UrlEncoder _urlEncoder;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _applicationDbContext;
		private readonly IHubContext<UserHub> _hubContext;

		public IdentityRespository(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, UrlEncoder urlEncoder, IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext, IHubContext<UserHub> hubContext)
		{
			_roleManager=roleManager;
			_userManager=userManager;
			_urlHelperFactory=urlHelperFactory;
			_actionContextAccessor=actionContextAccessor;
			_urlEncoder=urlEncoder;
			_httpContextAccessor=httpContextAccessor;
			_applicationDbContext=applicationDbContext;
			_hubContext=hubContext;
		}

		public string GenerateLink(string conroller, string action)
		{
          

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var callbackUrl = urlHelper.Action(
                action: action,
                controller: conroller,
                values: null,
                protocol: _httpContextAccessor.HttpContext.Request.Scheme);

            Console.WriteLine(callbackUrl);
            return callbackUrl;
        }

        public async Task<string> GenerateForgtPasswordLinkAsync(string email)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            //token = _urlEncoder.Encode(token);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var callbackUrl = urlHelper.Action(
                action: "ResetPassword",
                controller: "Auth",
                values: new { userId = user.Id.Base64Encode(), token = token, time = DateTime.Now.ToString() },
                protocol: _httpContextAccessor.HttpContext.Request.Scheme);

            Console.WriteLine(callbackUrl);
            return callbackUrl;
        }
        public async Task<string> GenerateEmailConfirmationLinkAsync(string email)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == email);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            //token = _urlEncoder.Encode(token);

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var callbackUrl = urlHelper.Action(
                action: "ConfirmEmail",
                controller: "Auth",
                values: new { userId = user.Id, token = token, time = DateTime.Now.ToString() },
                protocol: _httpContextAccessor.HttpContext.Request.Scheme);

			Console.WriteLine(callbackUrl);
            return callbackUrl;
        }
		public bool ExistingUserEmail(string email)
		{
			return _userManager.Users.Any(u => u.Email == email);
		}
        public bool IsValideToken (string token)
        {
           return  _applicationDbContext.AuthenticationTokens.Any(x=> x.Token == token);
        }
        public async void LogToken(string token, string userId)
        {
            var Authtoken = new AuthenticationToken
            {
                UserId = userId,
                Token = token,
                ExpiryDate = DateTime.Now,
                IsUsed = true

            };
           await  _applicationDbContext.AuthenticationTokens.AddAsync(Authtoken); 
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task UnLockUser(string email)
        {
			ApplicationUser user = await _userManager.FindByEmailAsync(email) ?? throw new ArgumentNullException(nameof(email));

			await _userManager.SetLockoutEnabledAsync(user, false);

			// Reset access failed count
			await _userManager.ResetAccessFailedCountAsync(user);
			user.IsRestrited = false;
            user.LockoutEnabled = false;
			// Disable lockout for the user

			await _userManager.UpdateAsync(user);
            //await _applicationDbContext.SaveChangesAsync();
        }
        public async Task LockUser(string email)
        {
			using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

			ApplicationUser user = await _userManager.FindByEmailAsync(email) ?? throw new ArgumentNullException(nameof(email));

			await _userManager.SetLockoutEnabledAsync(user, true);
			// Set access failed count to trigger lockout
			await _userManager.AccessFailedAsync(user);
			user.IsRestrited = true;
            user.LockoutEnabled = true;
	
			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				await transaction.CommitAsync();
				await _hubContext.Clients.All.SendAsync("ForceLogout", user.Id);
			}
			
			//await _applicationDbContext.SaveChangesAsync();
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
				EmailConfirmed = false,
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
