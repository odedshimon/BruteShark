using System.Collections.Generic;

namespace PcapAnalyzer
{
    public class TcpSession
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
        public List<TcpPacket> Packets { get; set; }

        public TcpSession()
        {
            this.Packets = new List<TcpPacket>();
        }

    }
}