using System;
using System.Collections.Generic;
using System.Text;

namespace BruteForce
{
    public class KerberosAsRepHash : Hash
    {
        public string Realm { get; set; }
        public string Username { get; set; }
        public string ServiceName { get; set; }
    }
}