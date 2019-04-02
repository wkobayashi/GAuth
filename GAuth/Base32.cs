using System;
using System.Collections.Generic;

namespace GAuth
{
    public static class Base32 // https://tools.ietf.org/html/rfc4648
    {
        public static string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static char[] Encode(byte[] data)
        {
            List<char> encoded = new List<char>();
            for (int i = 0; i < data.Length; i += 5)
            {
                byte[] b = new byte[] { 0, 0, 0, data[i + 4], data[i + 3], data[i + 2], data[i + 1], data[i + 0] };
                ulong ul = BitConverter.ToUInt64(b, 0);
                for (int j = 0; j < 8; j++)
                {
                    ulong k = (ul >> (59 - 5 * j)) & 0x1f;
                    char c = base32Chars[(int)k];
                    encoded.Add(c);
                }
            }
            return encoded.ToArray();
        }

        public static byte[] Decode(char[] encoded)
        {
            List<byte> data = new List<byte>();
            for (int i = 0; i < encoded.Length; i += 8)
            {
                ulong ul = 0;
                for (int j = 0; j < 8; j++)
                {
                    ul += (ulong)base32Chars.IndexOf(encoded[i + j]) << (59 - 5 * j);
                }
                byte[] bytes = BitConverter.GetBytes(ul);
                byte[] b = new byte[] { bytes[7], bytes[6], bytes[5], bytes[4], bytes[3] };
                data.AddRange(b);
            }
            return data.ToArray();
        }
    }
}
