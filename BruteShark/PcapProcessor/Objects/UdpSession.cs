using System.Collections;
using System.Collections.Generic;

namespace PcapProcessor
{
    public class UdpSession : NetworkObject
    {
        public byte[] Data { get; set; }
        public List<UdpPacket> Packets { get; set; }

        public UdpSession()
        {
            this.Packets = new List<UdpPacket>();
            this.Protocol = "UDP";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UdpSession))
            {
                return false;
            }

            var session = obj as UdpSession;

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