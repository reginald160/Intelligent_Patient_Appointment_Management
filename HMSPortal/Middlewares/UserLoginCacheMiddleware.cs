using HMSPortal.Application.Core;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Middlewares
{

    public class UserLoginCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserLoginCacheMiddleware(RequestDelegate next, IMemoryCache memoryCache, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var cacheKey = CoreValiables.LoginUser;
            var existingUser = _memoryCache.Get<CacheUserModel>(cacheKey);
            var userCache = new CacheUserModel();

            if (existingUser == null)
            {
             
                var user = _httpContextAccessor.HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    var currentUser = await _userManager.GetUserAsync(user);
                    if (currentUser != null)
                    {
                        userCache = new CacheUserModel
                        {
                            Id = currentUser.Id,
                            Email = currentUser.Email
                        };

                    }
                }
                else
                {
                    var currentUser =  _userManager.Users.FirstOrDefault(x => x.Email == "ozougwuifeanyi160@gmail.com");
                    if (currentUser != null)
                    {
                        userCache = new CacheUserModel
                        {
                            Id = currentUser.Id,
                            Email = currentUser.Email
                        };

                    }
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _memoryCache.Set(cacheKey, userCache, cacheEntryOptions);
            }
           
            await _next(context);
        }

    }
}
