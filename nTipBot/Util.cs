using System.Security.Cryptography;
using System.Text;

namespace nTipBot
{
	public class Util
	{
		/// <summary>
		/// Will take care of underscore characters in usernames
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public static string SafeEscape(string msg)
		{
			return msg.Replace("_", "\\_");
		}

		/// <summary>
		/// Return SHA256 hash from string (utf8)
		/// </summary>
		/// <param name="rawData">string to be hashed</param>
		/// <returns></returns>
		public static string ComputeSha256Hash(string rawData)
		{
			// Create a SHA256   
			using (SHA256 sha256Hash = SHA256.Create())
			{
				// ComputeHash - returns byte array  
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

				// Convert byte array to a string   
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
	}
}
