using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Helpers
{
    public class Logics
    {
        public static Tuple<DateTime, string> ExtractDate(string response)
        {
            var rep1 = response.Split('@');
            var date = rep1[1];
            var time = rep1[0].Split('-')[0];
            var fdfd = date + " " + time;
            var dt = DateTime.Parse(fdfd);

            return new Tuple<DateTime, string>(dt, rep1[0]);
        }
    }
}
