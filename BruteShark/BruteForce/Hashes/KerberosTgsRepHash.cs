using System;
using System.Collections.Generic;
using System.Text;

namespace BruteForce
{
    public class KerberosTgsRepHash : Hash
    {
        // TODO: use enum
        public int Etype { get; set; }
        public string Realm { get; set; }
        public string Username { get; set; }
        public string ServiceName { get; set; }
    }
}
