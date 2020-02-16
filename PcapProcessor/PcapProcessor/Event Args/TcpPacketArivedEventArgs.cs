using System;

namespace PcapProcessor
{
    public class TcpPacketArivedEventArgs : EventArgs
    {
        public TcpPacket Packet { get; set; }
    }
}