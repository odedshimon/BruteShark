using System;
using System.Collections.Generic;
using System.Text;

namespace PcapProcessor
{
    public interface INetworkPacket
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
        public DateTime SentTime { get; set; }
    }

    public class NetworkPacket : INetworkPacket
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
        public DateTime SentTime { get; set; }
    }
}
