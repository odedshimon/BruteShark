using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class KerberosHash : NetworkHash, IDomainCredential
    {
        public string User { get; set; }
        public string Domain { get; set; }

        public string GetDoamin() => this.Domain;

        public string GetUsername() => this.User;

    }
}
