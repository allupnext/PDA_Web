using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Infrastructure_Shared.Services
{
    public class CommonService
    {
    }

    public static class GenerateOTP
    {
        private static readonly ThreadLocal<System.Security.Cryptography.RandomNumberGenerator> crng = new ThreadLocal<System.Security.Cryptography.RandomNumberGenerator>(System.Security.Cryptography.RandomNumberGenerator.Create);
        private static readonly ThreadLocal<byte[]> bytes = new ThreadLocal<byte[]>(() => new byte[sizeof(int)]);
        public static int NextInt()
        {
            crng.Value.GetBytes(bytes.Value);
            return BitConverter.ToInt32(bytes.Value, 0) & int.MaxValue;
        }
        public static double NextDouble()
        {
            while (true)
            {
                long x = NextInt() & 0x001FFFFF;
                x <<= 31;
                x |= (long)NextInt();
                double n = x;
                const double d = 1L << 52;
                double q = n / d;
                if (q != 1.0)
                    return q;
            }
        }
    }

    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates a random password based on the rules passed in the parameters
        /// </summary>
        /// <param name="includeLowercase">Bool to say if lowercase are required</param>
        /// <param name="includeUppercase">Bool to say if uppercase are required</param>
        /// <param name="includeNumeric">Bool to say if numerics are required</param>
        /// <param name="includeSpecial">Bool to say if special characters are required</param>
        /// <param name="includeSpaces">Bool to say if spaces are required</param>
        /// <param name="lengthOfPassword">Length of password required. Should be between 8 and 128</param>
        /// <returns></returns>
        public static string GeneratePassword(int length = 6)
        {
             string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
             Random random = new Random();
             string password = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }

    }
}
