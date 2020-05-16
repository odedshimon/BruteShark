using System;
using System.Linq;
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

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] GetDataBetweenHeaderAndFooter(byte[] data, byte[] header, byte[] footer)
        {
            int header_position = Utilities.SearchForSubarray(data, header);

            if (header_position > 0)
            {
                // TODO: check if this skip is memory inefficient, if not refactor SearchForSubarray to get optional
                // parameter of start index
                int footer_position = Utilities.SearchForSubarray(data.Skip(header_position).ToArray(), footer);

                if (footer_position > 0)
                {
                    return data.SubArray(index: header_position, length: footer_position);
                }
            }

            return null;
        }

    }
}