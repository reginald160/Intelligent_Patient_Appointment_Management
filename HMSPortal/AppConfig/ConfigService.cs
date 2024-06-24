using EchoBot;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using HMS.Infrastructure.BackgroundJobs.Appointment;
using HMS.Infrastructure.Repositories.IRepository;
using HMS.Infrastructure.Repositories.Repository;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.CryptoGraphy;
using HMSPortal.Application.Core.Notification;
using HMSPortal.Application.AppSettings;
using Microsoft.Extensions.Options;
using HMSPortal.Application.Core.Cache;

namespace HMSPortal.AppConfig
{
    public static class ConfigService
    {
        public static void ConfigureBotServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;
            });

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, EchoBot.Bots.EchoBot>();
        }

        public static void ConfigureDIServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityRespository, IdentityRespository>();
            services.AddScoped<IPatientServices, PatientRepository>();
            services.AddScoped<IDoctorServices, DoctorRepo>();
            services.AddScoped<IAppointmentServices, AppointmentRespository>();
            services.AddScoped<ISQLServices, ISQLRepository>();
            services.AddScoped<IJobScheduleService, JobSchedule>();
            services.AddTransient<INotificatioServices, NotificationRepository>();
            services.AddScoped<ICryptographyService, CryptographyService>();
			services.AddScoped<ICacheService, CacheRepository>();
			services.AddSignalR();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
			services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});


			services.Configure<SMTPSettings>(configuration.GetSection("SmtpConfiguration"));
            services.Configure<AppSetting>(configuration.GetSection("AppSettings"));
            // If needed, also register it as a singleton
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSetting>>().Value);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SMTPSettings>>().Value);
        }
    }
}
