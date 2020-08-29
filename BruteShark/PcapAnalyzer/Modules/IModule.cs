using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public interface IModule
    {
        string Name { get; }
        event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        void Analyze(UdpPacket udpPacket);
        void Analyze(TcpPacket tcpPacket);
        void Analyze(TcpSession tcpSession);
        void Analyze(UdpStream udpStream);
    }
}
