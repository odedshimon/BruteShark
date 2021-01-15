using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    class NetworkMapModule : IModule
    {
        public string Name => "Network Map";
        public string CliName => "NetworkMap";

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        private HashSet<NetworkConnection> _connections;

        public NetworkMapModule()
        {
            _connections = new HashSet<NetworkConnection>();
        }

        public void Analyze(UdpPacket udpPacket)
        {
            RaiseParsedItemDetected(udpPacket.SourceIp, udpPacket.DestinationIp, udpPacket.SourcePort, udpPacket.DestinationPort, "UDP");
        }

        public void Analyze(TcpPacket tcpPacket)
        {
            RaiseParsedItemDetected(tcpPacket.SourceIp, tcpPacket.DestinationIp, tcpPacket.SourcePort, tcpPacket.DestinationPort, "TCP");
        }

        private void RaiseParsedItemDetected(string source, string destination, int srcPort, int destPort, string protocol)
        {
            var connection = new NetworkConnection()
            {
                Source = source,
                Destination = destination,
                SrcPort = srcPort,
                DestPort = destPort,
                Protocol = protocol
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

        public void Analyze(UdpStream udpStream)
        {
            
        }
    }
}
