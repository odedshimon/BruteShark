using System.Collections.Generic;

namespace PcapAnalyzer
{
    public class UdpStream
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
        public List<UdpPacket> Packets { get; set; }
        

        public UdpStream()
        {
            this.Packets = new List<UdpPacket>();
        }

    }
}