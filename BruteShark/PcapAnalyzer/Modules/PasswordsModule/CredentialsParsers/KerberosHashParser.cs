using System;
using System.Collections.Generic;
using System.Text;


namespace PcapAnalyzer
{
    // Algorithm inspired from https://github.com/lgandx/PCredz/blob/master/Pcredz
    public class KerberosHashParser : IPasswordParser
    {
        // Kerberos message type, we look for 'krb-as-req' (decimal value 10) which means
        // pre-authentication request.
        private readonly byte asMsgType = 0x0a;

        // etype = eTYPE-ARCFOUR-HMAC-MD5 (23)
        private readonly byte rc4encType = 0x17;

        // PA-DATA means 'pre-authentication data', is used to augment the initial 
        // authentication with the KDC.
        private readonly byte[] pa_data_signiture = new byte[] { 0xa2, 0x36, 0x04, 0x34 };   // Hash length = 0x36 = 54
        private readonly byte[] pa_data_signiture2 = new byte[] { 0xa2, 0x35, 0x04, 0x33 };  // Hash length = 0x35 = 53


        public NetworkCredential Parse(UdpPacket udpPacket)
        {
            if (!isKerberos(udpPacket))
                return null;

            byte[] sig_part = udpPacket.Data.SubArray(40, 4);

            if (Utilities.SearchForSubarray(sig_part, this.pa_data_signiture) == 0 ||
                Utilities.SearchForSubarray(sig_part, this.pa_data_signiture2) == 0)
            {
                var paddingLen = 0;
                var hashOffset = 44;
                var userNameOffset = 144;
                var hashItemLen = (int)udpPacket.Data[41];

                if (hashItemLen == 53)
                    paddingLen = 1;
                if (hashItemLen != 54 && hashItemLen != 53)
                {
                    hashItemLen = (int)udpPacket.Data[48];
                    hashOffset = 49;
                    userNameOffset = hashItemLen + 97;
                }

                var hashLen = 52 - paddingLen;
                byte[] hash = udpPacket.Data.SubArray(hashOffset, hashLen);
                byte[] switchedHash = new byte[hashLen];
                hash.SubArray(16, 36).CopyTo(switchedHash, 0);
                hash.SubArray(0, 16).CopyTo(switchedHash, 36);
                string hashString = NtlmsspHashParser.ByteArrayToHexString(switchedHash);

                var userName =  ExtractKerberosMessageItem(udpPacket.Data, userNameOffset - paddingLen, out int userNameLength);
                string domain = ExtractKerberosMessageItem(udpPacket.Data, userNameOffset + userNameLength - paddingLen + 4, out int domainLength);

                return new KerberosHash()
                {
                    HashType = "Kerberos V5",
                    Protocol = "UDP",
                    Source = udpPacket.DestinationIp,
                    Destination = udpPacket.SourceIp,
                    User = userName,
                    Domain = domain,
                    Hash = hashString
                };
            }
            
            return null;
        }

        public NetworkCredential Parse(TcpPacket tcpPacket) => null;

        public NetworkCredential Parse(TcpSession tcpSession) => null;

        private bool isKerberos(UdpPacket udpPacket)
        {
            byte msgType = udpPacket.Data[17];
            byte encType = udpPacket.Data[39];

            return msgType == this.asMsgType || encType == this.rc4encType;
        }

        private string ExtractKerberosMessageItem(byte[] kerberosMessageData, int itemIndex, out int itemLength)
        {
            itemLength = (int)kerberosMessageData[itemIndex];
            var itemData = Encoding.ASCII.GetString(
                kerberosMessageData.SubArray(
                    itemIndex + 1,
                    itemLength));

            return itemData;
        }

    }
}

