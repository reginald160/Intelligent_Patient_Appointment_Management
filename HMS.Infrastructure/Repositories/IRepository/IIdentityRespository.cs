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
        Task<string> GenerateEmailConfirmationLinkAsync(string email);
        Task<string> GenerateForgtPasswordLinkAsync(string email);
        string GenerateLink(string conroller, string action);
        bool IsValideToken(string token);
        Task LockUser(string email);
        void LogToken(string token, string userId);
        Task UnLockUser(string email);
    }
}
