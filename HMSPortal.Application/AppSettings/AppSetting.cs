using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.AppSettings
{
	public class AppSetting
	{
		public string EncryprionKey { get; set; }
		public string EncryprionIV { get; set; }
		public string BVN_SECRET_KEY { get; set; }
		public string Chars { get; set; }
		public int HashSize { get; set; }
		public int HashIterations { get; set; }
	}
}

