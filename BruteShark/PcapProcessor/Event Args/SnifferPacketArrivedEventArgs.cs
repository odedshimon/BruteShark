using System;
using System.Collections.Generic;
using System.Text;
using SharpPcap;

namespace PcapProcessor
{
    class SnifferPacketArrivedEventArgs
    {
        public PacketDotNet.Packet packet {get; set;}
    }
}
