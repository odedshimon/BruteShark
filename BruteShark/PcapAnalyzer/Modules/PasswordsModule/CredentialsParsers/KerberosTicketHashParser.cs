using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    class KerberosTicketHashParser : IPasswordParser
    {
        public NetworkLayerObject Parse(UdpPacket udpPacket)
        {
            var kerberosPacket = KerberosPacketParser.GetKerberosPacket(udpPacket.Data);

            if (kerberosPacket is null)
            {
                return null;
            }

            if (kerberosPacket is KerberosTgsRepPacket)
            {
                var kerberosTgsRepPacket = kerberosPacket as KerberosTgsRepPacket;

                if (kerberosTgsRepPacket.Ticket.EncrytedPart.Etype == 23)
                {
                    return new KerberosTgsRepHash()
                    {
                        Source = udpPacket.SourceIp,
                        Destination = udpPacket.DestinationIp,
                        Realm = kerberosTgsRepPacket.Ticket.Realm,
                        Etype = 23,
                        Username = kerberosTgsRepPacket.Cname.Name,
                        ServiceName = kerberosTgsRepPacket.Ticket.Sname.Name,
                        Hash = NtlmsspHashParser.ByteArrayToHexString(kerberosTgsRepPacket.Ticket.EncrytedPart.Cipher),
                        Protocol = "UDP",
                        HashType = "Kerberos TGS Rep Etype 23"
                    };
                }
            }

            return null;
        }

        public NetworkLayerObject Parse(TcpPacket tcpPacket) => null;

        public NetworkLayerObject Parse(TcpSession tcpSession) => null;
    }
}
