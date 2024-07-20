using Microsoft.AspNetCore.Identity;

namespace HMSPortal.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsRestrited { get; set; }
        public bool PasswordConfirmed { get; set; }

        public bool IsLocked { get; set; }

        public bool IsEnabled { get; set; }
    }
}
