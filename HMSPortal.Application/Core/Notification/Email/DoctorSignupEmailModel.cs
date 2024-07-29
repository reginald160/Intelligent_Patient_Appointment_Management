using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Notification.Email
{
    public class DoctorSignupEmailModel
    {
        public string To { get; set; }
        public string LogoURL { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string SetPasswordLink { get; set; }
        public string BGImageUrl { get; set; }
        public string LogoUrl { get; set; }
    }

    public class PatientEmailModel
    {
        public string Email { get; set; }
        public string LogoURL { get; set; }
        public string Name { get; set; }
  
        public string Link { get; set; }
        public string BGImageUrl { get; set; }
        public string LogoUrl { get; set; }
    }

}
