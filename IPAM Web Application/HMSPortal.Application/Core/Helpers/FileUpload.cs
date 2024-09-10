using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Helpers
{
	public  class FileUpload
	{
		public static string UploadFile(IFormFile file, string path = "files")
		{
			var folderName = "Resources";
			folderName = $"{folderName}\\{path}";
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
			if (file.Length > 0)
			{
				var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
				fileName = DateTime.UtcNow.ToString("yymmssfff") + Regex.Replace(fileName, @"\s", "");
				var fullPath = Path.Combine(pathToSave, fileName);
				var dbPath = Path.Combine(folderName, fileName);
				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					file.CopyTo(stream);
				}


				var uploadParams = new ImageUploadParams()
				{
					File = new FileDescription(@dbPath),
					//Folder = $"Resources/{path}/",
					//Folder = "footballbuzz/stories/", 
					Overwrite = false
				};

				Account account = new Account(
				"dukd0jnep",
				 "218328548349527",
				 "pHHXluwid33h4jA-82-7rzBCMJA");
				Cloudinary cloudinary = new Cloudinary(account);


				var uploadResult = cloudinary.Upload(uploadParams);
				var uploadedFilePath = uploadResult.SecureUrl.AbsoluteUri;
				File.Delete(fullPath);

				return uploadedFilePath;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
