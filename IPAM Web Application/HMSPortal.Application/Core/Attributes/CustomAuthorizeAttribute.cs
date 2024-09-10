using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Claims;

namespace HMSPortal.Application.Core.Attributes
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		public List<string> RequiredRoles { get; }
		public string RequiredRole { get; }
		public string RedirectUrl = "/Dashboard/AccessDenied";

		public CustomAuthorizeAttribute(string requiredRole)
		{
			RequiredRoles = requiredRole.Split(',').ToList();
			//RedirectUrl = redirectUrl;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var user = context.HttpContext.User;

			// Check if the user is authenticated
			if (!user.Identity.IsAuthenticated)
			{
				context.Result = new RedirectResult(RedirectUrl);
				return;
			}


			var userRoles = user.Claims
			.Where(c => c.Type == ClaimTypes.Role)
			.Select(c => c.Value)
			.ToList();

			// Check if the user has the required role
			// Check if the user has any of the required roles
			if (!RequiredRoles.Any(role => user.IsInRole(role)))
			{
				context.Result = new RedirectResult(RedirectUrl);
				return;
			}
		}
	}
}
