using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    class KerberosTicketHashParser : IPasswordParser
    {
        public NetworkLayerObject Parse(UdpPacket udpPacket) => 
            this.GetKerberosTicketsHash(udpPacket.SourceIp, udpPacket.DestinationIp, udpPacket.Data);

        public NetworkLayerObject Parse(TcpPacket tcpPacket) => null; 
            // this.GetKerberosTicketsHash(tcpPacket.SourceIp, tcpPacket.DestinationIp, tcpPacket.Data);

        public NetworkLayerObject Parse(TcpSession tcpSession) => null;

        private NetworkLayerObject GetKerberosTicketsHash(string source, string destination, byte[] data)
        {
            var kerberosPacket = KerberosPacketParser.GetKerberosPacket(data);

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
                        Source = source,
                        Destination = destination,
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
    }
}
