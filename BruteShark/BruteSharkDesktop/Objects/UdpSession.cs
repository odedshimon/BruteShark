using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkDesktop
{
    public class UdpSession : TransportLayerSession
    {
        public List<UdpPacket> Packets { get; set; }

        public UdpSession()
        {
            this.Packets = new List<UdpPacket>();
        }
    }
}
