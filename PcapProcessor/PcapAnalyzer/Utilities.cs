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
    }
}