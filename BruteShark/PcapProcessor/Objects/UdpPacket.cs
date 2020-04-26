using System;
using System.Collections.Generic;
using System.Text;

namespace PcapProcessor
{
    // TODO: Think of creating better hierarchy for this class and TcpPacket (e.g TransporPacket class) 
    public class UdpPacket
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
    }
}
