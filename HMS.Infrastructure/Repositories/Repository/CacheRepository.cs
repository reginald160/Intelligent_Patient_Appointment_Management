using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HMSPortal.Models.Enums;

namespace HMS.Infrastructure.Repositories.Repository
{
    public class CacheRepository : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CacheRepository(IMemoryCache memoryCache, ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _memoryCache=memoryCache;
            _context=context;
            _userManager=userManager;
            _signInManager=signInManager;
        }


        public string GetUpcomingAppointmentCounByDoctorId(string userid)
        {
            const string appointmentCountCacheKey = CoreValiables.DoctorUpcomingAppointmentCountCacheKey;

            // Try to get the patient count from the cache
            if (!_memoryCache.TryGetValue(appointmentCountCacheKey, out string patientCount))
            {
                //var patient = _context.Doctors.FirstOrDefault(x => x.UserId == userid);
                // If not found in cache, query the database and set the cache
                patientCount = _context.Appointments.Include("Doctor").Where(x=> x.DoctorId != null)
                    .Where(x => x.Doctor.UserId == userid && x.Status == "UpComming").Count(x => !x.IsDeleted).ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

                _memoryCache.Set(appointmentCountCacheKey, patientCount, cacheEntryOptions);
            }

            return patientCount;
        }
        public string GetCompletedAppointmentCounByDoctorId(string userid)
        {
            const string appointmentCountCacheKey = CoreValiables.DoctorCompletedAppointmentCountCacheKey;

            // Try to get the patient count from the cache
            if (!_memoryCache.TryGetValue(appointmentCountCacheKey, out string patientCount))
            {
                var patient = _context.Doctors.FirstOrDefault(x => x.UserId == userid);
                // If not found in cache, query the database and set the cache
                patientCount = _context.Appointments.Include("Doctor").Where(x=> x.DoctorId != null)
                    .Where(x => x.Doctor.UserId == userid && x.Status == "Completed").Count(x => !x.IsDeleted).ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

                _memoryCache.Set(appointmentCountCacheKey, patientCount, cacheEntryOptions);
            }

            return patientCount;
        }
        public string GetCompletedAppointmentCounByUserId(string userid)
        {
            const string appointmentCountCacheKey = CoreValiables.CompletedAppointmentCountCacheKey;

            // Try to get the patient count from the cache
            if (!_memoryCache.TryGetValue(appointmentCountCacheKey, out string patientCount))
            {
                var patient = _context.Patients.FirstOrDefault(x=> x.UserId == userid);
                // If not found in cache, query the database and set the cache
                patientCount = _context.Appointments.Where(x=> x.PatientId == patient.Id && x.Status == "Completeted").Count(x => !x.IsDeleted).ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

                _memoryCache.Set(appointmentCountCacheKey, patientCount, cacheEntryOptions);
            }

            return patientCount;
        }

        public string GetUpcomingAppointmentCounByUserId(string userid)
        {
            const string appointmentCountCacheKey = CoreValiables.UpcomingAppointmentCountCacheKey;

            // Try to get the patient count from the cache
            if (!_memoryCache.TryGetValue(appointmentCountCacheKey, out string patientCount))
            {
                var patient = _context.Patients.FirstOrDefault(x => x.UserId == userid);
                // If not found in cache, query the database and set the cache
                patientCount = _context.Appointments.Where(x => x.PatientId == patient.Id && x.Status == "UpComming").Count(x => !x.IsDeleted).ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

                _memoryCache.Set(appointmentCountCacheKey, patientCount, cacheEntryOptions);
            }

            return patientCount;
        }

        public string GetPatientNotification(string userid)
        {
            const string appointmentCountCacheKey = CoreValiables.PatientNotificationCountCacheKey;

            // Try to get the patient count from the cache
            if (!_memoryCache.TryGetValue(appointmentCountCacheKey, out string patientCount))
            {
                var patient = _context.Patients.FirstOrDefault(x => x.UserId == userid);
                // If not found in cache, query the database and set the cache
                patientCount = _context.Notifications.Where(x => x.UserId == userid).Count().ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

                _memoryCache.Set(appointmentCountCacheKey, patientCount, cacheEntryOptions);
            }

            return patientCount;
        }

        public string GetPatientCount()
        {
            const string PatientCountCacheKey = CoreValiables.PatientCountCacheKey;
     
            // Try to get the patient count from the cache
            if (!_memoryCache.TryGetValue(PatientCountCacheKey, out string patientCount))
            {
                // If not found in cache, query the database and set the cache
                patientCount = _context.Patients.Count(x => !x.IsDeleted).ToString();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

                _memoryCache.Set(PatientCountCacheKey, patientCount, cacheEntryOptions);
            }

            return patientCount;
        }
		public string GetDoctorCount()
		{
			const string DoctorCountCacheKey = CoreValiables.DoctorCountCacheKey;

			// Try to get the patient count from the cache
			if (!_memoryCache.TryGetValue(DoctorCountCacheKey, out string patientCount))
			{
				// If not found in cache, query the database and set the cache
				patientCount = _context.Doctors.Count(x => !x.IsDeleted).ToString();
				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

				_memoryCache.Set(DoctorCountCacheKey, patientCount, cacheEntryOptions);
			}

			return patientCount;
		}
		public string GetAppointmentCount()
		{
			const string appointmentCountCacheKey = CoreValiables.AppointmentCountCacheKey;

			// Try to get the patient count from the cache
			if (!_memoryCache.TryGetValue(appointmentCountCacheKey, out string patientCount))
			{
				// If not found in cache, query the database and set the cache
				patientCount = _context.Appointments.Count(x => !x.IsDeleted).ToString();
				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Set the cache expiration time

				_memoryCache.Set(appointmentCountCacheKey, patientCount, cacheEntryOptions);
			}

			return patientCount;
		}

		public CacheUserModel GetCachedUser()
		{
			var cacheKey = CoreValiables.LoginUser;
			var cachedUser = _memoryCache.Get<CacheUserModel>(cacheKey);

			return cachedUser;
		}

        public string GetCancelledAppointmentCounByUserId(string userid)
        {
            throw new NotImplementedException();
        }
    }
}
