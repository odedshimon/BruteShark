using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    public class FtpPasswordParser : IPasswordParser
    {
        private Regex ftpSuccessfullLoginRegex = new Regex(@"220(.*)[\r\n]+USER\s(?<Username>.*)[\r\n]+331(.*)[\r\n]+PASS\s(?<Password>.*)[\r\n]+");
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
                
                if(credential.Password.EndsWith("\r"))
                {
                    credential.Password = credential.Password.Replace("\r", "");
                }

                if (credential.Username.EndsWith("\r"))
                {
                    credential.Username = credential.Username.Replace("\r", "");
                }
            }

            return credential;
        }
    }
}
