using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels
{
	public class PartialViewModel
	{
		public string? Message { get; set; }
		public string? Role { get; set; }
		public IList<string>? Roles  { get; set; }
	}
}
