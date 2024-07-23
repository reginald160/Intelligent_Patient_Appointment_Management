// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.Core.Notification;

namespace HMSPortal.Areas.Identity.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<LoginModel> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IIdentityRespository _identityRespository;
		private readonly INotificatioServices _notificatioServices;

		public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger,
			UserManager<ApplicationUser> userManager, IIdentityRespository identityRespository,
			INotificatioServices notificatioServices)
		{
			_signInManager = signInManager;
			_logger = logger;
			_userManager=userManager;
			_identityRespository = identityRespository;
			_notificatioServices = notificatioServices;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }


		public List<IdentityError>? identityErrors { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string ErrorMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl ??= Url.Content("~/");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{

				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var user = _userManager.Users.FirstOrDefault(x => x.Email.ToLower() == Input.Email.ToLower());
				if (user == null)
				{
					ModelState.AddModelError(string.Empty, "User does not exist.");
					return Page();
				}
				if (!user.EmailConfirmed)
				{
					var patientEmailModel = new PatientEmailModel
					{
						Email = user.Email,
						LogoUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
						Link =  await _identityRespository.GenerateEmailConfirmationLinkAsync(user.Email),
						Name = user.Email,
					};
					Task.Run(async () =>
					{
						await _notificatioServices.SendPatientSignUpEmail(patientEmailModel);


					});
					ModelState.AddModelError(string.Empty, "To complete your registration, please confirm your email address by clicking the link sent to your email.");
					return Page();
				}

				if (user.IsRestrited)
				{
					ModelState.AddModelError(string.Empty, "your account has been restricted.");
					return Page();
				}
				var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				if (result.RequiresTwoFactor)
				{
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
				}
				if (result.Succeeded)
				{
					var roles = await _userManager.GetRolesAsync(user);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
						new Claim(ClaimTypes.Role, roles.FirstOrDefault())
					};

                    var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));

                    _logger.LogInformation($"User {User.Identity.Name} logged in.");

					return LocalRedirect(returnUrl);
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
