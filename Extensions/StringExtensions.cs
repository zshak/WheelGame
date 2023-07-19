using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Wheel.Extensions
{
    public static class StringExtensions
    {
        public static string HashString(this string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
