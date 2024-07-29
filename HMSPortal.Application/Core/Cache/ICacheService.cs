using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Cache
{
    public interface ICacheService
    {
		string GetAppointmentCount();
		CacheUserModel GetCachedUser();
		string GetDoctorCount();
		string GetPatientCount();
    }
}
