using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Helpers
{
    public class FileHelper
    {
        public static  string ReadFileContent(string filePath)
        {
            
            // Ensure the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file at path {filePath} does not exist.");
            }

            // Read the file content
            using (var reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

