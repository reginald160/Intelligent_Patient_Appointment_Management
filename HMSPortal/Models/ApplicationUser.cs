using Microsoft.AspNetCore.Identity;

namespace HMSPortal.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public bool IsRestrited { get; set; }
    }
}
