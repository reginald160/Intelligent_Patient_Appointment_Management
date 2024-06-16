using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Helpers
{
	public class RandomHelper
	{
		private static readonly Random random = new Random();

		public static string GeneratePassword()
		{
			const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
			const string digits = "0123456789";
			const string specialChars = "!@#$%^&*()_+[]{}|;:,.<>?";

			// Select one random character from each category
			char upper = upperCase[random.Next(upperCase.Length)];
			char lower = lowerCase[random.Next(lowerCase.Length)];
			char digit = digits[random.Next(digits.Length)];
			char special = specialChars[random.Next(specialChars.Length)];

			// Combine all characters into a single array
			char[] passwordChars = new char[8];

			// Fill the first four positions with the selected characters
			passwordChars[0] = upper;
			passwordChars[1] = lower;
			passwordChars[2] = digit;
			passwordChars[3] = special;

			// Fill the remaining positions with random characters from all categories
			string allChars = upperCase + lowerCase + digits + specialChars;
			for (int i = 4; i < passwordChars.Length; i++)
			{
				passwordChars[i] = allChars[random.Next(allChars.Length)];
			}

			// Shuffle the array to ensure randomness
			passwordChars = passwordChars.OrderBy(x => random.Next()).ToArray();

			// Convert the character array to a string and return
			return new string(passwordChars);
		}

		//public async Task<string> GeneratePatientIdAsync(string prrfix = "BNP")
		//{
		//	var serialNumber = _context.SerialNumbers.FirstOrDefault();
		//	if (serialNumber == null)
		//	{
		//		var model = new SerialNumber
		//		{
		//			LastModifiedBy = DateTime.Now.ToString(),
		//			DateCreated = DateTime.Now,
		//			ConsultationCount = 0,
		//			AdminCount = 0,
		//			PatientCount = 0,
		//			TherapistCount = 0,
		//		};
		//		await _context.AddAsync(model);
		//		await _context.SaveChangesAsync();


		//		return $"{prrfix}{1.ToString("D4")}";
		//	}
		//	long index = default;
		//	if (prrfix == "BNP")
		//	{

		//		serialNumber.PatientCount =  serialNumber.PatientCount + 1;
		//		index = serialNumber.PatientCount;
		//	}
		//	else
		//	{
		//		serialNumber.TherapistCount = serialNumber.TherapistCount + 1;
		//		index = serialNumber.TherapistCount;
		//	}

		//	_context.Update(serialNumber);
		//	await _context.SaveChangesAsync();

		//	return $"{prrfix}{index.ToString("D4")}";
		//}
	}
}
