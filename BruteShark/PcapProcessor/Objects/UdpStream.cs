using System.Collections;
using System.Collections.Generic;

namespace PcapProcessor
{
    public class UdpStream
    {
        public string SourceIp { get; set; }
        public string DestinationIp { get; set; }

        //maybe unneccessary
        public int SourcePort { get; set; }
        public int DestinationPort { get; set; }
        public byte[] Data { get; set; }
        public List<UdpPacket> Packets { get; set; }


        public UdpStream()
        {
            this.Packets = new List<UdpPacket>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UdpStream))
            {
                return false;
            }

            var session = obj as UdpStream;

            // Note: we need to check both directions of the conversation to 
            // determine equality.
            return (this.SourceIp == session.SourceIp &&
                    this.DestinationIp == session.DestinationIp &&
                    this.SourcePort == session.SourcePort &&
                    this.DestinationPort == session.DestinationPort) ||
                    (this.SourceIp == session.DestinationIp &&
                    this.DestinationIp == session.SourceIp &&
                    this.SourcePort == session.DestinationPort &&
                    this.DestinationPort == session.SourcePort);
        }

        public override int GetHashCode()
        {
            return this.SourceIp.GetHashCode() ^
                   this.SourcePort.GetHashCode() ^
                   this.DestinationPort.GetHashCode() ^
                   this.DestinationIp.GetHashCode();
        }

    }
}