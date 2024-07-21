using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string GetGreeting()
        {
            var currentTime = DateTime.UtcNow;
            if (currentTime.Hour < 12)
            {
                return "Good Morning";
            }
            else if (currentTime.Hour < 18)
            {
                return "Good Afternoon";
            }
            else
            {
                return "Good Evening";
            }
        }
        public static List<string> ExtractQuestions(string input)
        {
            List<string> questions = new List<string>();
            List<string> questionsg = new List<string>();
            // Regular expression to match questions starting with Qn: and ending with ?
            string pattern = @"Q\d+:.*?\?";

            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                questions.Add(match.Value.Trim());
            }

            return questions;
        }

    }
}
