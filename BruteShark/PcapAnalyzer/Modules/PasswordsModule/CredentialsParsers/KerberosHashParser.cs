using System;
using System.Collections.Generic;
using System.Text;


namespace PcapAnalyzer
{
    public class KerberosHashParser : IPasswordParser
    {
        private readonly byte msgType = 0x0a;
        private readonly byte encType = 0x17;
        private readonly byte MessageType = 0x02; 
        private readonly byte[] sig = new byte[] { 0xa2, 0x36, 0x04, 0x34 };   // Hash length = 0x36 = 54
        private readonly byte[] sig2 = new byte[] { 0xa2, 0x35, 0x04, 0x33 };  // Hash length = 0x35 = 53


        public NetworkCredential Parse(UdpPacket udpPacket)
        {
            byte msgType = udpPacket.Data[17];
            byte encType = udpPacket.Data[39];

            if (msgType == this.msgType || encType == this.encType)
            {
                byte[] sig_part = udpPacket.Data.SubArray(40, 4);

                if (NtlmsspHashParser.SearchForSubarray(sig_part, this.sig) == 0 ||
                    NtlmsspHashParser.SearchForSubarray(sig_part, this.sig2) == 0)
                {
                    int hashLen = (int)udpPacket.Data[41];

                    if (hashLen == 54)
                    {
                        byte[] hash = udpPacket.Data.SubArray(44, 52);
                        byte[] switchedHash = new byte[52];
                        hash.SubArray(16, 36).CopyTo(switchedHash, 0);
                        hash.SubArray(0, 16).CopyTo(switchedHash, 36);
                        string hashString = NtlmsspHashParser.ByteArrayToHexString(switchedHash);

                        int userNameLen = (int)udpPacket.Data[144];
                        string userName = Encoding.ASCII.GetString(udpPacket.Data.SubArray(145, userNameLen));

                        int domainLen = (int)udpPacket.Data[145 + userNameLen + 3];
                        string domain = Encoding.ASCII.GetString(udpPacket.Data.SubArray(145 + userNameLen + 4 , domainLen));

                        var kerberosHash = new KerberosHash()
                        {
                            HashType = "Kerberos V5",
                            Protocol = "UDP",
                            Source = udpPacket.DestinationIp,
                            Destination = udpPacket.SourceIp,
                            User = userName,
                            Domain = domain,
                            Hash = hashString
                        };

                        return kerberosHash;
                    }
                }
            }

            return null;
        }


        public NetworkCredential Parse(TcpPacket tcpPacket) => null;

        public NetworkCredential Parse(TcpSession tcpSession) => null;

    }
}
