using BruteForce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkDesktop
{
    public static class Casting
    {
        public static BruteSharkDesktop.TransportLayerPacket CastProcessorTcpPacketToBruteSharkDesktopTcpPacket(PcapProcessor.TcpPacket tcpPacket)
        {
            return new BruteSharkDesktop.TransportLayerPacket()
            {
                Protocol = "TCP",
                SourceIp = tcpPacket.SourceIp,
                DestinationIp = tcpPacket.DestinationIp,
                SourcePort = tcpPacket.SourcePort,
                DestinationPort = tcpPacket.DestinationPort,
                Data = tcpPacket.Data
            };
        }

        public static BruteSharkDesktop.TransportLayerPacket CastProcessorUdpPacketToBruteSharkDesktopUdpPacket(PcapProcessor.UdpPacket udpPacket)
        {
            return new BruteSharkDesktop.TransportLayerPacket()
            {
                Protocol = "UDP",
                SourceIp = udpPacket.SourceIp,
                DestinationIp = udpPacket.DestinationIp,
                SourcePort = udpPacket.SourcePort,
                DestinationPort = udpPacket.DestinationPort,
                Data = udpPacket.Data
            };
        }

        public static BruteSharkDesktop.TransportLayerSession CastProcessorTcpSessionToBruteSharkDesktopTcpSession(PcapProcessor.TcpSession tcpSession)
        {
            return new BruteSharkDesktop.TransportLayerSession()
            {
                Protocol = "TCP",
                SourceIp = tcpSession.SourceIp,
                DestinationIp = tcpSession.DestinationIp,
                SourcePort = tcpSession.SourcePort,
                DestinationPort = tcpSession.DestinationPort,
                Data = tcpSession.Data,
                Packets = tcpSession.Packets.Select(p => CastProcessorTcpPacketToBruteSharkDesktopTcpPacket(p)).ToList()
            };
        }

        public static BruteSharkDesktop.TransportLayerSession CastProcessorUdpSessionToBruteSharkDesktopUdpSession(PcapProcessor.UdpSession udpSession)
        {
            return new BruteSharkDesktop.TransportLayerSession()
            {
                Protocol = "UDP",
                SourceIp = udpSession.SourceIp,
                DestinationIp = udpSession.DestinationIp,
                SourcePort = udpSession.SourcePort,
                DestinationPort = udpSession.DestinationPort,
                Data = udpSession.Data,
                Packets = udpSession.Packets.Select(p => CastProcessorUdpPacketToBruteSharkDesktopUdpPacket(p)).ToList()
            };
        }
        
    }
}
