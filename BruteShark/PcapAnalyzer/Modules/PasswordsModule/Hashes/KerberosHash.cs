using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class KerberosHash : NetworkHash
    {
        public string User { get; set; }
        public string Domain { get; set; }
    }
}
