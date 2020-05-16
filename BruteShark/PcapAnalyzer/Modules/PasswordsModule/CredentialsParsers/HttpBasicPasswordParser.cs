using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    public class HttpBasicPasswordParser : IPasswordParser
    {
        private Regex _httpBasicAuthenticationRegex = new Regex(@"(.*)HTTP([\s\S]*)(Authorization: Basic )(?<Credentials>.*)");

        public NetworkLayerObject Parse(TcpPacket tcpPacket)
        {
            NetworkPassword credential = null;
            var packetData = Encoding.UTF8.GetString(tcpPacket.Data);

            Match match = this._httpBasicAuthenticationRegex.Match(packetData);

            if (match.Success)
            {
                try
                {
                    // Decode Base64 encoded credentials.
                    string credentials = Encoding.UTF8.GetString(
                                                Convert.FromBase64String(
                                                    match.Groups["Credentials"].Value));

                    var username = credentials.Split(':')[0];
                    var password = credentials.Split(':')[1];

                    credential = new NetworkPassword()
                    {
                        Protocol = "HTTP Basic Authentication",
                        Username = username,
                        Password = password,
                        Source = tcpPacket.SourceIp,
                        Destination = tcpPacket.DestinationIp
                    };
                }
                // Nothing to do, there always a chance it looks like Basic Authentication 
                // but its not.
                catch { }
            }

            return credential;
        }

        public NetworkLayerObject Parse(TcpSession tcpSession) => null;
    }
}
