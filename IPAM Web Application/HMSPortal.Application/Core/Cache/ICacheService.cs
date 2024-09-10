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
        string GetCancelledAppointmentCounByUserId(string userid);
        string GetCompletedAppointmentCounByDoctorId(string userid);
        string GetCompletedAppointmentCounByUserId(string userid);
        string GetDoctorCount();
		string GetPatientCount();
        string GetPatientNotification(string userid);
        string GetUpcomingAppointmentCounByDoctorId(string userid);
        string GetUpcomingAppointmentCounByUserId(string userid);
    }
}
