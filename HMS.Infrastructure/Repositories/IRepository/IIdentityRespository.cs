using HMSPortal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories.IRepository
{
	public interface IIdentityRespository
	{
		Task<string> CreateUser(string username, string password, Roles role);
		Task DeleteUser(string email);
		bool ExistingUserEmail(string email);
	}
}
