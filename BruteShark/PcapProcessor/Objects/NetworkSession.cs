using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapProcessor
{
    public interface INetworkSession<out T> where T : INetworkPacket
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public string Protocol { get; set; }
        public int SentData { get; }
        public int ReceivedData { get; }
    }

    public abstract class NetworkSession<T> : INetworkSession<T> where T : NetworkPacket
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public string Protocol { get; set; }
        public abstract List<T> Packets { get; set; }
        public int SentData => Packets.Where(p => p.SourceIp == SourceIp).Sum(p => p.Data.Length);
        public int ReceivedData => Packets.Where(p => p.DestinationIp == SourceIp).Sum(p => p.Data.Length);
    }

}
