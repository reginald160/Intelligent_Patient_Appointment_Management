using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Helpers
{
	public class ConfigurationHelper
	{
		private static readonly Lazy<IConfiguration> configuration = new Lazy<IConfiguration>(() =>
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			return builder.Build();
		});
		public static string GetConnectionString(string name)
		{
			return configuration.Value.GetConnectionString(name) ?? "***" ;
		}
	}
}
