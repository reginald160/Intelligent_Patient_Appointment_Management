using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSPortal.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
        public Guid? CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public ApplicationUser? CreatedBy { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public Guid? UpdatedById { get; set; }

        [ForeignKey("UpdatedById")]
        public ApplicationUser? UpdatedBy { get; set; }

        public bool  IsDeleted { get; set; }

        public string Type { get; set; }

        public string TableName { get; set; }

        public DateTime DateTime { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }

        public string AffectedColumns { get; set; }

        public string? PrimaryKey { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }
    }
}
