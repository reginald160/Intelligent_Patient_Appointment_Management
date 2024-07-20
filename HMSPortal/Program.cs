using Hangfire;
using Hangfire.SqlServer;
using HMS.Infrastructure.BackgroundJobs.Appointment;
using HMS.Infrastructure.Persistence.DataContext;
using HMS.Infrastructure.Repositories.IRepository;
using HMS.Infrastructure.Repositories.Repository;
using HMSPortal.AppConfig;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.AppSettings;
using HMSPortal.Application.Core.Chat.SignalR;
using HMSPortal.Application.Core.CryptoGraphy;
using HMSPortal.Application.Core.Mapper.Automaper;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.Middlewares;
using HMSPortal.Domain.Models;
using HMSPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
	options.Lockout.MaxFailedAccessAttempts = 5;
}).AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Identity/Account/Login";
	options.LogoutPath = "/Identity/Account/Logout";
	options.AccessDeniedPath = "/Dashboard/AccessDenied";
	options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
	options.SlidingExpiration = true;
});

builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(20);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();
// Register the memory cache
ConfigService.ConfigureDIServices(builder.Services, configuration);

ConfigService.ConfigureBotServices(builder.Services, configuration);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHangfire(configuration => configuration
			.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
			.UseSimpleAssemblyNameTypeSerializer()
			.UseDefaultTypeSerializer()
			.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
			{
				CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
				SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
				QueuePollInterval = TimeSpan.Zero,
				UseRecommendedIsolationLevel = true,
				DisableGlobalLocks = true
			}));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    //.MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Clear default logging providers
builder.Logging.ClearProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHangfireDashboard();
app.UseHangfireServer();


app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Home}/{action=Index}/{id?}"
//	);


//app.UseMiddleware<UserLoginCacheMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Register SignalR hubs
    endpoints.MapHub<ChatHub>("/chatHub");
	endpoints.MapHub<UserHub>("/userHub");
});
app.MapRazorPages();

app.Run();
