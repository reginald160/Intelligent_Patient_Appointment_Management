using HMSPortal.Domain.Models.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
    public class IdentityHistory : AuditableEntity
    {
        public IdentityHistory()
        {
            DateCreated =  DateTimeOffset.Now;
            LastUpdatedTime =  DateTimeOffset.Now;
        }

        [Key]
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
        public string? Key { get; set; }
    }
}
