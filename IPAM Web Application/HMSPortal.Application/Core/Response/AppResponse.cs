using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Response
{
	public class AppResponse
	{
        public bool IsSuccessful { get; set; }
		public string Message { get; set; }
		public string ResponseCode { get; set; }
		public object Data { get; set; }
	}
}
