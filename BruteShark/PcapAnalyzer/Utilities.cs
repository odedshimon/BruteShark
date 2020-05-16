using System;
using System.Text;

namespace PcapAnalyzer
{
    internal static class Utilities
    {
        internal static string DecodeAsciiBase64(string input)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(input));
        }

        public static int SearchForSubarray(byte[] input, byte[] subarray)
        {
            var len = subarray.Length;
            var limit = input.Length - len;

            for (var i = 0; i <= limit; i++)
            {
                var k = 0;

                for (; k < len; k++)
                {
                    if (subarray[k] != input[i + k]) break;
                }

                if (k == len) return i;
            }

            return -1;
        }

    }
}