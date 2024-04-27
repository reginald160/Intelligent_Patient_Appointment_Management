using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSPortal.Domain.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        public string ? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public string Type { get; set; }

        public string TableName { get; set; }

        public DateTime DateTime { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }

        public string AffectedColumns { get; set; }

        public string? PrimaryKey { get; set; }

    }
}
