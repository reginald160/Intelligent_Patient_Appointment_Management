
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static string GetControllerDisplayName(this IHtmlHelper htmlHelper)
		{
			var controllerActionDescriptor = htmlHelper.ViewContext.ActionDescriptor as ControllerActionDescriptor;
			if (controllerActionDescriptor != null)
			{
				var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
				var displayNameAttribute = controllerTypeInfo.GetCustomAttributes(typeof(DisplayControllerNameAttribute), false)
															 .FirstOrDefault() as DisplayControllerNameAttribute;
				return displayNameAttribute?.DisplayName ?? controllerActionDescriptor.ControllerName;
			}
			return string.Empty;
		}
	}
}
