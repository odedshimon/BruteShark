using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    class KerberosTicketHashParser : IPasswordParser
    {
        public NetworkLayerObject Parse(UdpPacket udpPacket) => 
            this.GetKerberosTicketsHash(udpPacket.SourceIp, udpPacket.DestinationIp, "UDP", udpPacket.Data);

        public NetworkLayerObject Parse(TcpPacket tcpPacket) => null;
            //this.GetKerberosTicketsHash(tcpPacket.SourceIp, tcpPacket.DestinationIp, "TCP", tcpPacket.Data);

        public NetworkLayerObject Parse(TcpSession tcpSession) => null;

        private NetworkLayerObject GetKerberosTicketsHash(string source, string destination, string protocol, byte[] data)
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
                        Protocol = protocol,
                        HashType = "Kerberos V5 TGS-REP etype 23"
                    };
                }
            }
            else if (kerberosPacket is KerberosAsRepPacket)
            {
                var kerberosAsRepPacket = kerberosPacket as KerberosAsRepPacket;

                if (kerberosAsRepPacket.Ticket.EncrytedPart.Etype == 23)
                {
                    return new KerberosTgsRepHash()
                    {
                        Source = source,
                        Destination = destination,
                        Realm = kerberosAsRepPacket.Ticket.Realm,
                        Etype = 23,
                        Username = kerberosAsRepPacket.Cname.Name,
                        ServiceName = kerberosAsRepPacket.Ticket.Sname.Name,
                        Hash = NtlmsspHashParser.ByteArrayToHexString(kerberosAsRepPacket.Ticket.EncrytedPart.Cipher),
                        Protocol = protocol,
                        HashType = "Kerberos V5 AS-REP etype 23"
                    };
                }
            }

            return null;
        }
    }
}
