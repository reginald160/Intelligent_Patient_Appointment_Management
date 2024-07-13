using Confluent.Kafka;
using EchoBot.Model;
using HMS.Infrastructure.Repositories.IRepository;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Core.Notification.Email;
using HMSPortal.Application.ViewModels.Auth;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NuGet.Common;
using System;
using System.Numerics;
using System.Text;

namespace HMSPortal.Controllers
{
	public class AuthController : AdminBaseController
    {
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<AuthController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificatioServices _notificatioServices;
        private readonly IIdentityRespository _identityRespository;

        public AuthController(SignInManager<ApplicationUser> signInManager, ILogger<AuthController> logger, UserManager<ApplicationUser> userManager, INotificatioServices notificatioServices, IIdentityRespository identityRespository)
        {
            _signInManager=signInManager;
            _logger=logger;
            _userManager=userManager;
            _notificatioServices=notificatioServices;
            _identityRespository=identityRespository;
        }

        public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();

			_logger.LogInformation($"User {User.Identity.Name} logged out.");

			return RedirectToAction("Index", "Dashboard");
		}

        [HttpGet("Dashboard/AccessDenied")]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

		[HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string time)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(time))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            if (!DateTime.TryParse(time, out var emailSentTime))
            {
                return BadRequest("Invalid time format.");
            }

            // Check if the time has exceeded 10 minutes
            //var currentTime = DateTime.UtcNow;
            //var timeDifference = currentTime - emailSentTime;

            //if (timeDifference.TotalMinutes > 10)
            //{
            //    ViewBag.Message = "Expired";
            //    return BadRequest("The email confirmation link has expired.");
            //}


            var user = await _userManager.FindByIdAsync(userId);
         
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.EmailConfirmed)
            {
                if(user.PasswordConfirmed)
                {
                    ViewBag.Message = "Your account has been confirm";
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return RedirectToAction("SetPassword", new { id = user.Id.Base64Encode() }); // Redirect to a confirmation page or any other action

                }
            }
            byte[] decodedBytes = WebEncoders.Base64UrlDecode(token);
            token = Encoding.UTF8.GetString(decodedBytes);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("SetPassword", new {id = user.Id.Base64Encode()}); // Redirect to a confirmation page or any other action
            }
            else
            {
                // Handle errors (e.g., invalid token)
                return BadRequest("Error confirming your email.");
            }


        }


        public async Task<IActionResult> SetPassword(string id)
        {

           var user =  _userManager.Users.FirstOrDefault(x => x.Id == id.Base64Decode());
            if(user == null)
            {
                // Handle errors (e.g., invalid token)
                return BadRequest("Invalid User.");
            }
            var model = new SetPasswordViewModel
            {
                UserId = id,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                List<string> errorMessages = new List<string>();

                foreach (var error in ModelState)
                {
                    var sss = error.Value.Errors.Select(x => x.ErrorMessage).ToList();
                    errorMessages.AddRange(sss);
                }

                // Combine all error messages into a single message
                string allErrorMessages = string.Join("; ", errorMessages);

                ModelState.AddModelError("error-V", allErrorMessages);
                return View(viewModel);
            }
            viewModel.UserId = viewModel.UserId.Base64Decode();
            var user =  _userManager.Users.FirstOrDefault(x => x.Id == viewModel.UserId); 
            if(user.PasswordConfirmed)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, viewModel.Password);
            if (result.Succeeded)
            {
                // Optionally sign in the user after setting the password
                user.PasswordConfirmed = true;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation($"User:{user.Email} set  password");
                // Using Url.Action
                var loginUrl = _identityRespository.GenerateLink("Dashboard", "Index");
                var messageModel = new DoctorSignupEmailModel
                {
                    SetPasswordLink = loginUrl,
                    LogoUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",


                };
                Task.Run(async () =>
                {
                    await _notificatioServices.SendDoctorAcountConfirmation(messageModel);
                });


                // Redirect to dashboard or another page upon successful password set
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("error-V", "Erroro ccured while setting your password");
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(TokenViewModel viewModel)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Token);
            if(user == null)
            {
                ModelState.AddModelError("error-V", "User does not exist");
                return View(viewModel);
            }
            var emailModel = new DoctorSignupEmailModel
            {
                DoctorName = user.Email,
                To = user.Email,
                LogoUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
                SetPasswordLink =  await _identityRespository.GenerateForgtPasswordLinkAsync(user.Email)

            };
            Task.Run(async () =>
            {
                await _notificatioServices.SendForgetPasswordEmail(emailModel);

            });

            return RedirectToAction( actionName: "Message", controllerName: "Dashboard", new
            {
                title = "Password Reset",
                message = "A password reset link has been sent to the email provided.",
                controller = "Home",
                action ="Index",
                button = "Home"

            });
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token, string time)
        {
            var isValidToken = _identityRespository.IsValideToken(token.Base64Decode());
            if (isValidToken)
            {
                return BadRequest("Invalid token or token has been used");
            }

            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId.Base64Decode());
            if (user == null)
            {
                // Handle errors (e.g., invalid token)
                return BadRequest("Invalid User.");
            }
            var model = new SetPasswordViewModel
            {
                UserId = userId,
                Token = token,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(SetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                List<string> errorMessages = new List<string>();

                foreach (var error in ModelState)
                {
                    var sss = error.Value.Errors.Select(x => x.ErrorMessage).ToList();
                    errorMessages.AddRange(sss);
                }

                // Combine all error messages into a single message
                string allErrorMessages = string.Join("; ", errorMessages);

                ModelState.AddModelError("error-V", allErrorMessages);
                return View(viewModel);
            }
          
            viewModel.UserId = viewModel.UserId.Base64Decode();
            var user = _userManager.Users.FirstOrDefault(x => x.Id == viewModel.UserId);
            

 
            var result = await _userManager.ResetPasswordAsync(user, viewModel.Token.Base64Decode(), viewModel.Password);
            if (result.Succeeded)
            {
                _identityRespository.LogToken(viewModel.Token.Base64Decode(), user.Id);
                // Optionally sign in the user after setting the password
     
                _logger.LogInformation($"User:{user.Email} reset  password");
                // Using Url.Action
                // Redirect to dashboard or another page upon successful password set
                return RedirectToAction(actionName: "Message", controllerName: "Dashboard", new
                {
                    title = "Password Reset",
                    message = "Your password has been reset successfully",
                    controller = "Dashboard",
                    action = "Index",
                    button = "Login"

                });
            }
            else
            {
                ModelState.AddModelError("error-V", "Erroro ccured while reseting your password");
                return View(viewModel);
            }
        }




    }
}
