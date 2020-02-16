using System;
using System.Collections.Generic;
using System.Text;

namespace BruteForce
{
    public class NtlmHash : Hash
    {
        public string Challenge { get; set; }
        public string User { get; set; }
        public string Domain { get; set; }
        public string LmHash { get; set; }
        public string NtHash { get; set; }
        public string Workstation { get; set; }
    }
}
