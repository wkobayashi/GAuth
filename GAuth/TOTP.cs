using System;
using System.Security.Cryptography;

namespace GAuth
{
    public static class TOTP // https://tools.ietf.org/html/rfc6238
    {
        private static byte[] K = null;
        private static long ticks
        {
            get
            {
                return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).Ticks;
            }
        }
        private static uint T
        {
            get
            {
                return (uint)(ticks / 10000000);
            }
        }
        private const uint T0 = 0;
        private const ulong N = 6;
        private const ulong X = 30;
        private static ulong C = 0;
        private static byte[] HS = null;

        public static double GetExpirationTime()
        {
            return X - (ticks % ((long)X * 10000000)) / 10000000.0;
        }

        public static string Compute(string keyString)
        {
            K = Base32.Decode(keyString.ToCharArray());
            C = (T - T0) / X;
            HS = HmacSha1(K, C);
            return Truncate(HS);
        }

        private static byte[] HmacSha1(byte[] k, ulong c)
        {
            byte[] b = BitConverter.GetBytes(c);
            Array.Reverse(b);
            HMAC hmac = new HMACSHA1(k);
            return hmac.ComputeHash(b);
        }

        private static string Truncate(byte[] hs)
        {
            byte[] Sbits = DT(hs);
            Array.Reverse(Sbits);
            uint Snum = BitConverter.ToUInt32(Sbits, 0);
            uint D = Snum % (uint)Math.Pow(10, N);
            return D.ToString("D" + N.ToString());
        }

        private static byte[] DT(byte[] hs)
        {
            int offsetBits = hs[19] & 0xf;
            byte[] b = new byte[] { hs[offsetBits + 0], hs[offsetBits + 1], hs[offsetBits + 2], hs[offsetBits + 3] };
            b[0] &= 0x7f;
            return b;
        }
    }
}
