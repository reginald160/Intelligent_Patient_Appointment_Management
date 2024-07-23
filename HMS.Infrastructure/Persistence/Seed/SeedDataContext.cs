using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace HMS.Infrastructure.Persistence.Seed
{
    public class SeedDataContext
    {

        

        public static void SeeData(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var dbContext = service.GetRequiredService<ApplicationDbContext>();
                SeedEmailSettingData(dbContext);
            }

        
        }
        public static void SeedEmailSettingData(ApplicationDbContext context)
        {
  
            if (!context.EmailSettings.Any())
            {
                context.EmailSettings.AddRange(
                    new EmailSettings
                    {
                        Id = Guid.NewGuid(),
                        Logo = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
                        BackgroundImage = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
                        Host = "smtp.sendgrid.net",
                        Port = "587",
                        Username = "apikey",
                        Password = "U0cuRXVFMGNyY2dScml2Q2lUTnJFeEg5US4wUVpSZTBIcVI5M19YN205Mm5Fakl6RmlucXlXRVVEci12OEJLWkpUZ3k0",
                        SenderEmail = "reginald.ozougwu@yorksj.ac.uk",
                        DisplayName = "MediSmart"
                    }
                // Add more initial data if necessary
                );

                context.SaveChanges();
            }
        }
    }
}
