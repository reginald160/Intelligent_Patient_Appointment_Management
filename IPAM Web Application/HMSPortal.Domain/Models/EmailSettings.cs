using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Domain.Models
{
    public class EmailSettings
    {
        [Key]
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string BackgroundImage { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderEmail { get; set; }
        public string DisplayName { get; set; }
    }
}
