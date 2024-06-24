using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public CacheUserModel GetCachedUser()
		{
			var cacheKey = CoreValiables.LoginUser;
			var cachedUser = _memoryCache.Get<CacheUserModel>(cacheKey);

			return cachedUser;
		}

	}
}
