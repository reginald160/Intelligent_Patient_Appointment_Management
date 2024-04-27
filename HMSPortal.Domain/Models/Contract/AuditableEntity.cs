using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSPortal.Domain.Models.Contract
{
    public class AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
        public string ? CreatedBy { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public string ? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }


    }
}
