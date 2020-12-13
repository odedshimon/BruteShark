using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    public class FtpPasswordParser : IPasswordParser
    {   
        private Regex ftpSuccessfullLoginRegex = new Regex(@"220(.*)"+Environment.NewLine+ @"USER\s(?<Username>.*)"+Environment.NewLine+@"331(.*)" + Environment.NewLine + @"PASS\s(?<Password>.*)" + Environment.NewLine);

        public NetworkLayerObject Parse(UdpPacket udpPacket) => null;

        public NetworkLayerObject Parse(TcpPacket tcpPacket) => null;

        public NetworkLayerObject Parse(TcpSession tcpSession)
        {
            NetworkPassword credential = null;
            var sessionData = Encoding.UTF8.GetString(tcpSession.Data);

            Match match = this.ftpSuccessfullLoginRegex.Match(sessionData);
            
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
