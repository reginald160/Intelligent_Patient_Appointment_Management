using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
    public class BrockerSubscription
    {
        [Key]
        public Guid Id { get; set; }
        public string? Topic { get; set; }
        public string? Subscriber { get; set; }
    }
}
