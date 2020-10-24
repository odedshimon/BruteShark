using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class KerberosAsRepHash : NetworkHash
    {
        public string Realm { get; set; }
        public string Username { get; set; }
        public string ServiceName { get; set; }
        public int Etype { get; set; }
    }
}
