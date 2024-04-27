using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMSPortal.Domain.Models.Contract;
using HMSPortal.Domain.Models;
using HMSPortal.Domain.Enums;

namespace HMS.Infrastructure.Persistence.DataContext
{
    public abstract class BaseDbContext : IdentityDbContext<ApplicationUser>
    {
        private string _dbErrorMessage = "unable to persist to the database at the moment";

        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BaseEntity>? AuditLogs { get; set; }

        public DbSet<IdentityHistory>? IdentityHistories { get; set; }

        #region Actions

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            int result = await base.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return result;
            }
            else
            {
                throw new Exception();
            }

        }

        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            List<AuditEntry> auditEntries = OnBeforeSaveChanges(userId);
            int result = await base.SaveChangesAsync();
            if (result > 0)
            {
                await OnAfterSaveChanges(auditEntries);
                return result;
            }
            else
            {
                throw new Exception();
            }

        }

        private List<AuditEntry> OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            List<AuditEntry> list = new List<AuditEntry>();
            foreach (EntityEntry item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity || item.State == EntityState.Detached || item.State == EntityState.Unchanged)
                {
                    continue;
                }

                AuditEntry auditEntry = new AuditEntry(item);
                auditEntry.TableName = item.Entity.GetType().Name;
                auditEntry.UserId = userId;
                list.Add(auditEntry);
                foreach (PropertyEntry property in item.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string name = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[name] = property.CurrentValue;
                        continue;
                    }

                    switch (item.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[name] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[name] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(name);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[name] = property.OriginalValue;
                                auditEntry.NewValues[name] = property.CurrentValue;
                            }

                            break;
                    }
                }
            }

            foreach (AuditEntry item2 in list.Where((AuditEntry _) => !_.HasTemporaryProperties))
            {
                AuditLogs.Add(item2.ToAudit());
            }

            return list.Where((AuditEntry _) => _.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
            {
                return Task.CompletedTask;
            }

            foreach (AuditEntry auditEntry in auditEntries)
            {
                foreach (PropertyEntry temporaryProperty in auditEntry.TemporaryProperties)
                {
                    if (temporaryProperty.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[temporaryProperty.Metadata.Name] = temporaryProperty.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[temporaryProperty.Metadata.Name] = temporaryProperty.CurrentValue;
                    }
                }

                AuditLogs.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync();
        }



        protected EntityEntry<TEntity> ValidateConcurrency<TEntity>(TEntity entity) where TEntity : class
        {

            // Retrieve the original entity from the database
            var entityType = base.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey();
            var idProperty = primaryKey.Properties.First();
            var originalEntity = base.Set<TEntity>().Find(idProperty.GetGetter().GetClrValue(entity));

            // Compare the Inversion property values
            var inversionProperty = typeof(TEntity).GetProperty("ConcurrencyToken");

            if (!IsConcurrencyTokenEqual(inversionProperty.GetValue(originalEntity), inversionProperty.GetValue(entity)))
            {
                throw new DbUpdateConcurrencyException("Concurrency violation: the entity has been modified by another process.");
            }

            return base.Update(entity);
        }

        private bool IsConcurrencyTokenEqual(object originalValue, object newValue)
        {

            return StructuralComparisons.StructuralEqualityComparer.Equals(originalValue, newValue);
        }


        #endregion
    }
}
