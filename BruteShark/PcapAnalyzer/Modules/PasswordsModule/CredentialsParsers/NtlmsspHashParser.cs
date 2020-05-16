using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapAnalyzer
{
    public static class ExtentionMethods
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    // Documention: http://davenport.sourceforge.net/ntlm.html#theNtlmMessageHeaderLayout
    // Algorithm inspired from https://github.com/lgandx/PCredz/blob/master/Pcredz
    public class NtlmsspHashParser : IPasswordParser
    {
        private enum NtlmServerState
        {
            WaitForChallenge,
            WaitForResponse
        }

        private const ushort _ntlmChallengeOffet = 24;
        private const ushort _ntlmChallengeLength = 8;

        // The signitures consists from the ASCII represntion of "NTLMSSP" + Message number (2 \ 3).
        private readonly byte[] _ntlmChallengeSigniture = new byte[] { 0x4e, 0x54, 0x4c, 0x4d, 0x53, 0x53, 0x50, 0x00, 0x02, 0x00, 0x00, 0x00 };
        private readonly byte[] _ntlmResponseSigniture =  new byte[] { 0x4e, 0x54, 0x4c, 0x4d, 0x53, 0x53, 0x50, 0x00, 0x03, 0x00, 0x00, 0x00 };

        public NetworkCredential Parse(TcpPacket tcpPacket)
        {
            return null;
        }

        public NetworkCredential Parse(TcpSession tcpSession)
        {
            NtlmHash ntlmHash = null;
            var serverState = NtlmServerState.WaitForChallenge;
            byte[] ntlmMessage = new byte[] { };

            foreach (TcpPacket packet in tcpSession.Packets)
            {
                // Look for server challenge.
                if (serverState == NtlmServerState.WaitForChallenge)
                {
                    int challengeSigniturePos = Utilities.SearchForSubarray(packet.Data, _ntlmChallengeSigniture);

                    if (challengeSigniturePos != -1)
                    {
                        // Get only the NTLM message part.
                        ntlmMessage = packet.Data.Skip(challengeSigniturePos).ToArray();
                        string challenge = ExtractNtlmChallenge(ntlmMessage);

                        ntlmHash = new NtlmHash()
                        {
                            Source = packet.DestinationIp,
                            Destination = packet.SourceIp,
                            Challenge = challenge,
                        };

                        serverState = NtlmServerState.WaitForResponse;
                    }
                }
                // Look for client response.
                else if (serverState == NtlmServerState.WaitForResponse && packet.SourceIp == ntlmHash.Source)
                {
                    int responseSigniturePos = Utilities.SearchForSubarray(packet.Data, _ntlmResponseSigniture);

                    if (responseSigniturePos != -1)
                    {
                        // Get only the NTLM message part.
                        ntlmMessage = packet.Data.Skip(responseSigniturePos).ToArray();
                        ntlmHash.LmHash = ExtractNtlmsspMessageItem(ntlmMessage, 14, false);
                        ntlmHash.NtHash = ExtractNtlmsspMessageItem(ntlmMessage, 22, false);
                        ntlmHash.Domain = ExtractNtlmsspMessageItem(ntlmMessage, 30);
                        ntlmHash.User = ExtractNtlmsspMessageItem(ntlmMessage, 38);
                        ntlmHash.Workstation = ExtractNtlmsspMessageItem(ntlmMessage, 46);
                        ntlmHash.Protocol = "NTLMSSP";

                        if (ntlmHash.NtHash.Length == 24)
                        {
                            ntlmHash.HashType = "NTLMv1";
                            ntlmHash.Hash = ntlmHash.LmHash;
                        }
                        else if (ntlmHash.NtHash.Length > 24)
                        {
                            ntlmHash.HashType = "NTLMv2";
                            ntlmHash.Hash = ntlmHash.NtHash;
                        }
                    }
                }
            }

            // Verify that we extracted a full hash.
            return ntlmHash?.Hash == null ? null : ntlmHash;
        }

        private string ExtractNtlmsspMessageItem(byte[] ntlmsspMessage, int itemIndex, bool decodeAsUnicode=true)
        {
            var itemLen = BitConverter.ToUInt16(ntlmsspMessage.SubArray(itemIndex, 2), 0);
            var itemOffset = BitConverter.ToUInt16(ntlmsspMessage.SubArray(itemIndex + 2, 2), 0);

            return decodeAsUnicode ? 
                Encoding.Unicode.GetString(ntlmsspMessage.SubArray(itemOffset, itemLen)).Replace("\0", string.Empty) : 
                ByteArrayToHexString(ntlmsspMessage.SubArray(itemOffset, itemLen));
        }

        private string ExtractNtlmChallenge(byte[] ntlmMessage)
        {
            return ByteArrayToHexString( 
                    ntlmMessage.Skip(_ntlmChallengeOffet)
                               .Take(_ntlmChallengeLength)
                               .ToArray());
        }

        public static string ByteArrayToHexString(byte[] input)
        {
            StringBuilder hex = new StringBuilder(input.Length * 2);

            foreach (byte b in input)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        
    }
}
