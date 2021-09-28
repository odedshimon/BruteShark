using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapProcessor
{
    // This is a generic interface with a covariant parameter T (using the "out" keyword).
    // The covariant is necessary because we want to return a specific derived type later (e.g TcpPacket)
    // but still to treat to INetworkSession as base, e.g. create a list of types INetworkSession
    // and add to that list different derived types like TcpSessions and UdpSessions on the fly.
    // Another example at: 
    // https://stackoverflow.com/questions/13280108/collection-of-derived-classes-that-have-generic-base-class
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
