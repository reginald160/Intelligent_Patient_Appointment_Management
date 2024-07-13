using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text
{
    public  static class StringExtensions
    { 
        public static string Base64Decode(this string base64EncodedData)
        {
            if (base64EncodedData == null)
                throw new ArgumentNullException(nameof(base64EncodedData));

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(this string plainText)
        {
            if (plainText == null)
                throw new ArgumentNullException(nameof(plainText));

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        

        
    }
}
