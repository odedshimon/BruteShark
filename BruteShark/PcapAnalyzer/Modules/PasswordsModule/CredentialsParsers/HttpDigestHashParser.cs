using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    class HttpDigestHashParser : IPasswordParser
    {
        // Some knwoledge was taken from: https://httpwg.org/specs/rfc7616.html

        private const string AsciiNewLine = "\r\n";
        private const string _httpDigestClientHeader = "Authorization: Digest";
        private Regex _clientHeaderRegex = new Regex($@"{_httpDigestClientHeader} (?<HeaderData>.*)\r\n");


        public NetworkLayerObject Parse(UdpPacket udpPacket) => null;

        public NetworkLayerObject Parse(TcpPacket tcpPacket)
        {
            HttpDigestHash hash = null;

            var packetData = Encoding.ASCII.GetString(tcpPacket.Data);
            Match match = _clientHeaderRegex.Match(packetData);

            if (match.Success)
            {
                // Parse the Client header data to a dictionary.
                var headerData = match.Groups["HeaderData"].ToString();
                Dictionary<string, string> headerParts = ParseHttpDigestHeader(headerData);

                hash = new HttpDigestHash();
                hash.Source = tcpPacket.SourceIp;
                hash.Destination = tcpPacket.DestinationIp;
                hash.Protocol = "HTTP";
                hash.HashType = "HTTP-Digest";
                hash.Method = packetData.Substring(0, packetData.IndexOf(' '));
                hash.Username = headerParts["username"];
                hash.Nonce = headerParts["nonce"];
                hash.Qop = headerParts["qop"];
                hash.Realm = headerParts["realm"];
                hash.Uri = headerParts["uri"];
                hash.Nc = headerParts["nc"];
                hash.Cnonce = headerParts["cnonce"];
                hash.Response = headerParts["response"];
                hash.Hash = headerParts["response"];
            }

            return hash;
        }

        public NetworkLayerObject Parse(TcpSession tcpSession) => null;

        // Parse the Server \ Client header data to a dictionary.
        // The header elements are separated with comma, a value can be wrapped with quotation mark or not.
        // Example of headerData: 
        // qop="auth",algorithm=MD5-sess,nonce="27f6f774a965",charset=utf-8,realm="INS.COM"
        private Dictionary<string, string> ParseHttpDigestHeader(string headerData)
        {
            return headerData.Split(',')
                   .ToDictionary(i => i.Substring(0, i.IndexOf('=')).Trim(' '),
                                 i => i.Substring(i.IndexOf('=') + 1).Trim('"'));
        }

    }
}
