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

        public void Analyze(UdpPacket udpPacket)
        {
            RaiseParsedItemDetected(udpPacket.SourceIp, udpPacket.DestinationIp);
        }

        public void Analyze(TcpPacket tcpPacket)
        {
            RaiseParsedItemDetected(tcpPacket.SourceIp, tcpPacket.DestinationIp);
        }

        private void RaiseParsedItemDetected(string source, string destination)
        {
            var connection = new NetworkConnection()
            {
                Source = source,
                Destination = destination
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
