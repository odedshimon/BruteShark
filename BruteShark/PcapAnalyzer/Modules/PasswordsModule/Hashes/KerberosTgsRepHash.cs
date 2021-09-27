using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class KerberosTgsRepHash : NetworkHash, IDomainCredential
    {
        public string Realm { get; set; }
        public string Username { get; set; }
        public string ServiceName { get; set; }
        public int Etype { get; set; }

        public string GetDoamin() => this.Realm;

        public string GetUsername() => this.Username;

    }
}
