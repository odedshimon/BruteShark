

namespace PcapAnalyzer

{
    public class VoipCallsModule : IModule
    {
        public string Name => "Voip Calls";
        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        public void Analyze(UdpPacket udpPacket) { }

        public void Analyze(TcpPacket tcpPacket) { }

        public void Analyze(TcpSession tcpSession) {}
        public void Analyze(UdpStream udpStream) {}
    }
}