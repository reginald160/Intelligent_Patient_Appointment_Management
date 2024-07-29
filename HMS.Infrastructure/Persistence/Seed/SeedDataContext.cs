using HMS.Infrastructure.Persistence.DataContext;
using HMSPortal.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
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

        

        public static async Task SeeData(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var dbContext = service.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                SeedEmailSettingData(dbContext);
                await SeedSuperAdminUser(dbContext, userManager, roleManager);
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

        public static async Task SeedSuperAdminUser(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string superAdmin = "SuperAdmin@gmail.com";
            var userRoleName = "SuperAdmin";
            if (!context.Users.Where(x=> x.Email == superAdmin).Any())
            {
                var user = new ApplicationUser
                {
                    UserName = superAdmin.ToUpper(),
                    Email = superAdmin,
                    EmailConfirmed = true,
                    PasswordConfirmed = true,

                };
                var result = await userManager.CreateAsync(user, "SuperAdmin160@");
                if (result.Succeeded)
                {
                   
                    var userRole = await roleManager.FindByNameAsync(userRoleName);
                    if (userRole == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = userRoleName });
                    }
                    user = userManager.Users.FirstOrDefault(x => x.Email == user.Email);
                    await userManager.AddToRoleAsync(user, userRoleName);

                    

                    var adminModel = new AdminModel
                    {
                        FirstName = "Moses",
                        LastName = "Smith",
                        Phone = "+447979643553",
                        DateOfBirth =  new DateTime(1993, 1, 1),
                        Gender = "Male",
                        Role = superAdmin,
                        Email = superAdmin,
                        ImageUrl = "https://res.cloudinary.com/dukd0jnep/image/upload/v1718523325/ehxwqremdpkwqvshlhhy.jpg",
                        UserId = user.Id
                    };
                    await context.AdminModels.AddAsync(adminModel);
                    await  context.SaveChangesAsync();

                }
                else
                {
                    throw new Exception("unable to create user");
                }
            }
        }

    }
}
