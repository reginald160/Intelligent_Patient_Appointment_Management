using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
    public class BrockerMessage
    {
        [Key]
        public Guid Id { get; set; }
        public string? Topic { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Delivered { get; set; }
    }
}
