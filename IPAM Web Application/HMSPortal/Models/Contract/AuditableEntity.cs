using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSPortal.Models.Contract
{
    public class AuditableEntity
    {
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.Now;
        public Guid? CreatedById { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public Guid? UpdatedById { get; set; }

   
    }
}
