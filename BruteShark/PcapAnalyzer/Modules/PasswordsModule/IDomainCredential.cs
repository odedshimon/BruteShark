using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public interface IDomainCredential
    {
        public string GetDoamin();
        public string GetUsername();
    }
}
