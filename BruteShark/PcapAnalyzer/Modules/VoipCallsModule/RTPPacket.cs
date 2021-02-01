using SIPSorcery.Net;


namespace PcapAnalyzer
{
    public class RTPPacket : UdpPacket
    {

        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
        public PcapRTPHeader Header;
    }

    public RTPPacket(UdpPacket udpPacket)
    {
        
    } 
}