using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.CryptoGraphy
{
	public interface ICryptographyService
	{
		string Base64Decode(string base64EncodedData);
		string Base64Encode(string plainText);
		string BinaryToString(string binary);
		string ByteToHex(byte[] hmacbBytesMessage);
		string ComputeHmac256(string key, string model);
		byte[] CreateHash(string input, byte[] salt);
		byte[] CreateSalt();
		string Decrypt(string val);
		string DecryptApiKey(string clientId, string encryptKey);
		string DecryptBvn(string encryptBvn);
		string Encrypt(string val);
		string EncryptBvn(string bvn);
		string GenerateClientKey(int size, string keyPrefix);
		bool ValidateHash(string input, string salt, string hashedValue);
	}
}
