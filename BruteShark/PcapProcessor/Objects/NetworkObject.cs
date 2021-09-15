using System;
using System.Collections.Generic;
using System.Text;

namespace PcapProcessor
{
    public class NetworkObject
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public string Protocol { get; set; }
    }
}
