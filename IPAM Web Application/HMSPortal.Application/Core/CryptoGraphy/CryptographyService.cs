using HMSPortal.Application.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.CryptoGraphy
{
	public class CryptographyService : ICryptographyService
	{
		private readonly AppSetting _appSettings;
		internal char[] chars;
		private int _hashIterations;
		private int _hashSize;
		private string BVN_SECRET_KEY;

		public CryptographyService(AppSetting appSettings)
		{
			_appSettings=appSettings;
		}

		public string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);

		}

		public string Base64Encode(string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		public string ComputeHmac256(string key, string model)
		{
			var encoding = new ASCIIEncoding();
			byte[] keyBytes = encoding.GetBytes(key);
			byte[] modelBytes = encoding.GetBytes(model);

			var hmacsha256 = new HMACSHA256(key: keyBytes);
			byte[] hmacbBytesMessage = hmacsha256.ComputeHash(modelBytes);
			using (hmacsha256)
			{
				return ByteToHex(hmacbBytesMessage);
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				// String.Concat(Array.ConvertAll(hmacbBytesMessage, x => x.ToString("x2")));
				// return Convert.ToBase64String((hmacbBytesMessage));
			}
		}

		public string ByteToHex(byte[] hmacbBytesMessage)
		{
			char[] c = new char[hmacbBytesMessage.Length * 2];

			byte b;

			for (int bx = 0, cx = 0; bx < hmacbBytesMessage.Length; ++bx, ++cx)
			{
				b = ((byte)(hmacbBytesMessage[bx] >> 4));
				c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

				b = ((byte)(hmacbBytesMessage[bx] & 0x0F));
				c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
			}

			return new string(c);
		}

		
		public string DecryptApiKey(string clientId, string encryptKey)
		{
			string decryptedValue = null;
			encryptKey = encryptKey.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(encryptKey);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(clientId,
					new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				if (encryptor != null)
				{
					encryptor.Key = pdb.GetBytes(32);
					encryptor.IV = pdb.GetBytes(16);
					using (MemoryStream ms = new MemoryStream())
					{
						using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(),
							CryptoStreamMode.Write))
						{
							cs.Write(cipherBytes, 0, cipherBytes.Length);
							cs.Close();
						}

						decryptedValue = Encoding.UTF8.GetString(ms.ToArray());
					}
				}
			}

			return decryptedValue;
		}

		public string DecryptBvn(string encryptBvn)
		{
			string decryptedValue = null;
			// var key = Encoding.UTF8.(BvnSecretKey);
			byte[] cipherBytes = Convert.FromBase64String(encryptBvn);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(BVN_SECRET_KEY,
					new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				if (encryptor != null)
				{
					encryptor.Key = pdb.GetBytes(32);
					encryptor.IV = pdb.GetBytes(16);
					using (MemoryStream ms = new MemoryStream())
					{
						using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(),
							CryptoStreamMode.Write))
						{
							cs.Write(cipherBytes, 0, cipherBytes.Length);
							cs.Close();
						}

						decryptedValue = Encoding.UTF8.GetString(ms.ToArray());
					}
				}
			}

			return decryptedValue;
		}

		public string EncryptBvn(string bvn)
		{
			var clearBytes = Encoding.UTF8.GetBytes(bvn);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(BVN_SECRET_KEY,
					new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
				if (encryptor != null)
				{
					encryptor.Key = pdb.GetBytes(32);
					encryptor.IV = pdb.GetBytes(16);
					using (MemoryStream ms = new MemoryStream())
					{
						using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(),
							CryptoStreamMode.Write))
						{
							cs.Write(clearBytes, 0, clearBytes.Length);
							cs.Close();
						}

						return Convert.ToBase64String(ms.ToArray());
					}
				}
			}

			return "";
		}

		public bool ValidateHash(string input, string salt, string hashedValue)
		{
			if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(hashedValue))
				return false;
			byte[] saltByte = Convert.FromBase64String(salt);
			byte[] inputHash = CreateHash(input, saltByte);
			string hashedString = Convert.ToBase64String(inputHash);

			if (hashedString.Equals(hashedValue))
				return true;
			return false;
		}

		public byte[] CreateSalt()
		{
			byte[] salt;
			using (RNGCryptoServiceProvider rNgCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rNgCryptoServiceProvider.GetBytes(salt = new byte[_hashSize]);
			}

			return salt;
		}

		public byte[] CreateHash(string input, byte[] salt)
		{
			byte[] hash;
			using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(input, salt, _hashIterations))
			{
				hash = hashGenerator.GetBytes(_hashSize);
			}

			return hash;
		}

		public string GenerateClientKey(int size, string keyPrefix)
		{
			byte[] data = new byte[4 * size];
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetBytes(data);
			}

			StringBuilder result = new StringBuilder(size);
			for (int i = 0; i < size; i++)
			{
				var rnd = BitConverter.ToUInt32(data, i * 4);
				var idx = rnd % chars.Length;

				result.Append(chars[idx]);
			}

			return string.Concat(keyPrefix, result.ToString());
		}

		public string BinaryToString(string binary)
		{
			if (string.IsNullOrEmpty(binary))
				throw new ArgumentNullException("binary");

			if ((binary.Length % 8) != 0)
				throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < binary.Length; i += 8)
			{
				string section = binary.Substring(i, 8);
				int ascii = 0;
				try
				{
					ascii = Convert.ToInt32(section, 2);
				}
				catch
				{
					throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
				}
				builder.Append((char)ascii);
			}
			return builder.ToString();
		}
		public String Encrypt(String val)
		{
			MemoryStream ms = new MemoryStream();
			string rsp = "";
			try
			{
				string sharedkeyval = _appSettings.EncryprionKey;
				string sharedvectorval = _appSettings.EncryprionIV;


				sharedkeyval = BinaryToString(sharedkeyval);

				sharedvectorval = BinaryToString(sharedvectorval);
				byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
				byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

				TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
				byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

				CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
				cs.Write(toEncrypt, 0, toEncrypt.Length);
				cs.FlushFinalBlock();

			}
			catch
			{
				return "";
			}
			return Convert.ToBase64String(ms.ToArray());
		}
		public String Decrypt(String val)
		{
			MemoryStream ms = new MemoryStream();

			try
			{
				string sharedkeyval = _appSettings.EncryprionKey;
				string sharedvectorval = _appSettings.EncryprionIV;


				sharedkeyval = BinaryToString(sharedkeyval);
				sharedvectorval = BinaryToString(sharedvectorval);

				byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
				byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

				TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
				byte[] toDecrypt = Convert.FromBase64String(val);

				CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);


				cs.Write(toDecrypt, 0, toDecrypt.Length);
				cs.FlushFinalBlock();
			}
			catch
			{
				return "";
			}
			return Encoding.UTF8.GetString(ms.ToArray());
		}
	}
}
