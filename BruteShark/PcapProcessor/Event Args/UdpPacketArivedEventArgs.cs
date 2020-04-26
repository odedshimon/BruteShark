using System;

namespace PcapProcessor
{
    public class UdpPacketArivedEventArgs : EventArgs
    {
        public UdpPacket Packet { get; set; }
    }
}
