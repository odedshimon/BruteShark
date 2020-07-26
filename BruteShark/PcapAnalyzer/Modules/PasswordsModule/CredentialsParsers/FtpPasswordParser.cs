using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    public class FtpPasswordParser : IPasswordParser
    {
        private Regex _ftpSuccessfullLoginRegex = new Regex(@"220(.*)\r\nUSER\s(?<Username>.*)\r\n331(.*)\r\nPASS\s(?<Password>.*)\r\n");

        public NetworkLayerObject Parse(UdpPacket udpPacket) => null;

        public NetworkLayerObject Parse(TcpPacket tcpPacket) => null;

        public NetworkLayerObject Parse(TcpSession tcpSession)
        {
            NetworkPassword credential = null;
            var sessionData = Encoding.UTF8.GetString(tcpSession.Data);

            Match match = this._ftpSuccessfullLoginRegex.Match(sessionData);
            
            if (match.Success)
            {
                credential = new NetworkPassword()
                {
                    Protocol = "FTP",
                    Username = match.Groups["Username"].Value,
                    Password = match.Groups["Password"].Value,
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return credential;
        }
    }
}
