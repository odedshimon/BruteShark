using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    class NetworkMapModule : IModule
    {
        public string Name => "Network Map";

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        private HashSet<NetworkConnection> _connections;

        public NetworkMapModule()
        {
            _connections = new HashSet<NetworkConnection>();
        }

        public void Analyze(UdpPacket tcpPacket) { }

        public void Analyze(TcpPacket tcpPacket)
        {
            var connection = new NetworkConnection()
            {
                Source = tcpPacket.SourceIp,
                Destination = tcpPacket.DestinationIp
            };

            if (_connections.Add(connection))
            {
                this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                {
                    ParsedItem = connection
                });
            }
        }

        public void Analyze(TcpSession tcpSession) { }

    }
}
