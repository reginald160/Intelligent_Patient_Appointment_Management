using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Application.Core.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HMSPortal.Controllers
{
	//[Authorize]
	public class DashboardController : Controller
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ApplicationDbContext _dbContext;
		private const string PatientCountCacheKey = "PatientCount";
		private readonly ICacheService _cacheService;


        public DashboardController(IMemoryCache memoryCache, ApplicationDbContext dbContext, ICacheService cacheService)
        {
            _memoryCache=memoryCache;
            _dbContext=dbContext;
            _cacheService=cacheService;
        }

        public IActionResult Index()
		{
			

			ViewBag.PatientCount = _cacheService.GetPatientCount();
			return View();
		}

		public IActionResult Add()
        {
			
			
	
			return View();
        }
        //[HttpPost]
        //public IActionResult Add()
        //{
        //    return View();
        //}

    }
}
