using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class NtlmHash : NetworkHash, IDomainCredential
    {
        public string Challenge { get; set; }
        public string User { get; set; }
        public string Domain { get; set; }
        public string LmHash { get; set; }
        public string NtHash { get; set; }
        public string Workstation { get; set; }

        public string GetDoamin() => this.Domain;

        public string GetUsername() => this.User;
    }
}
