using HMSPortal.Domain;
using HMSPortal.Domain.Models;
using HMSPortal.Domain.Models.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Persistence.DataContext
{
    public class ApplicationDbContext : BaseDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //protected readonly ICurrentUser _currentUser;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options /*ICurrentUser currentUser = null*/, IHttpContextAccessor httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor=httpContextAccessor;
            //_currentUser = currentUser;

        }

    
        public DbSet<Patient>? Patients { get; set; }
		public DbSet<SequenceContract>? SequenceContract { get; set; }
		public DbSet<Doctor>? Doctors { get; set; }
        public DbSet<Payment>? Payments { get; set; }
        public DbSet<PaymentInvoice>? PaymentInvoices { get; set; }
       public DbSet<AppointmentModel>? Appointments { get; set; }
		public DbSet<AppointmentJobScheduleModel>? appointmentJobScheduleModels { get; set; }
		public DbSet<AppointmentEvents>? AppointmentEvents { get; set; }
		public DbSet<ChatModel>? ChatModels { get; set; }
        public DbSet<BrockerMessage>? BrockerMessages { get; set; }
        public DbSet<BrockerSubscription>? BrockerSubscriptions { get; set; }

        //public IDbConnection Connection => throw new NotImplementedException();


        public override EntityEntry Remove(object entity)
        {

            return base.Remove(entity);
        }

        #region Actions
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {

			return await base.SaveChangesAsync(cancellationToken);

			//      var time = DateTime.UtcNow;
			//      var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
			//      foreach (var entry in ChangeTracker.Entries<AuditableEntity>().ToList())
			//      {
			//          switch (entry.State)
			//          {
			//              case EntityState.Added:
			//                  entry.Entity.DateCreated = time;
			//                  entry.Entity.CreatedBy = "";
			//entry.Entity.LastUpdatedTime = time;

			//break;

			//              case EntityState.Modified:
			//                  entry.Entity.LastUpdatedTime = time;
			//                  entry.Entity.UpdatedBy = "";
			//                  //if (entry.Entity is IConcurrencyCheck)
			//                  //{
			//                  //    ValidateConcurrency(entry.Entity);
			//                  //    var concurrencyProperty = entry.Property("ConcurrencyToken");
			//                  //    concurrencyProperty.OriginalValue = concurrencyProperty.CurrentValue;
			//                  //    concurrencyProperty.IsModified = true;
			//                  //}
			//                  break;
			//          }
			//      }
			//      bool isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
			//      if (userId == Guid.Empty.ToString())
			//      {
			//          return await base.SaveChangesAsync(cancellationToken);
			//      }
			//      else
			//      {
			//          return await base.SaveChangesAsync(userId);
			//      }
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
            base.OnModelCreating(builder);


			builder.Entity<Payment>()
		        .HasOne(p => p.PaymentInvoice)
		        .WithMany()
		        .HasForeignKey(p => p.PaymentInvoiceId)
		        .OnDelete(DeleteBehavior.Restrict); 

			builder.Entity<Payment>()
				.HasOne(p => p.Patient)
				.WithMany()
				.HasForeignKey(p => p.PatientId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Payment>()
				.HasOne(p => p.Doctor)
				.WithMany()
				.HasForeignKey(p => p.DoctorId)
				.OnDelete(DeleteBehavior.Restrict);
		}


        #endregion


    }
}
