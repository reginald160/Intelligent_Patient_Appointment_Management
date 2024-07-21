using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Message
{
    public class SystemContent
    {
        public static string GetSymptonFilterpath() {

            var relativeFilePath = "Core\\Chat\\SystemContent\\HealthConditonFilter.txt";
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, relativeFilePath);
            return filePath;
        }
    }
}
