using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.ViewModels.Dashboard
{
    public class MessageViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string ReturnUrl { get; set; }
        public string ButtonTitle { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
