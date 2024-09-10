using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class DisplayControllerNameAttribute : Attribute
	{
		public string DisplayName { get; }

		public DisplayControllerNameAttribute(string displayName) => DisplayName = displayName;
	}
}
