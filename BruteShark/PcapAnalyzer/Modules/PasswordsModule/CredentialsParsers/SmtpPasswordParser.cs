using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    public class SmtpPasswordParser : IPasswordParser
    {
        // Some documentaion was found here: https://www.fehcom.de/qmail/smtpauth.html

        private const string AsciiNewLine = "\r\n";

        private Regex _smtpAuthPlainRegex = new Regex(string.Format(@"AUTH PLAIN (?<Credentials>.*)({0})235", AsciiNewLine));
        private Regex _smtpAuthLoginRegex = new Regex(string.Format(@"AUTH LOGIN{0}334 VXNlcm5hbWU6{0}(?<Username>.*){0}334 UGFzc3dvcmQ6{0}(?<Password>.*){0}235", AsciiNewLine));
        private Regex _smtpCramMd5Regex =   new Regex(string.Format(@"AUTH CRAM-MD5({0})334\s(?<Challenge>.*)({0})(?<Hash>.*)({0})235", AsciiNewLine));

        public NetworkCredential Parse(TcpPacket tcpPacket) => null;

        public NetworkCredential Parse(TcpSession tcpSession)
        {
            // TODO: determine the autentication direction
            NetworkCredential credential = null;
            var sessionData = Encoding.ASCII.GetString(tcpSession.Data);

            if((credential = SearchSmtpAuthLogin(tcpSession, sessionData)) != null)
            {
                return credential;
            }
            if ((credential = SearchSmtpAuthPlain(tcpSession, sessionData)) != null)
            {
                return credential;
            }
            if ((credential = SearchSmtpCramMd5(tcpSession, sessionData)) != null)
            {
                return credential;
            }

            return credential;
        }

        private NetworkPassword SearchSmtpAuthLogin(TcpSession tcpSession, string sessionData)
        {
            NetworkPassword credential = null;
            Match match = _smtpAuthLoginRegex.Match(sessionData);

            if (match.Success)
            {
                credential = new NetworkPassword()
                {
                    Protocol = "SMTP (Auth Login)",
                    Username = Utilities.DecodeAsciiBase64(match.Groups["Username"].Value),
                    Password = Utilities.DecodeAsciiBase64(match.Groups["Password"].Value),
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return credential;
        }

        private NetworkPassword SearchSmtpAuthPlain(TcpSession tcpSession, string sessionData)
        {
            NetworkPassword credential = null;
            Match match = _smtpAuthPlainRegex.Match(sessionData);

            if (match.Success)
            {
                // Decode the credentials Base64 string and split by Ascii Null character to get
                // the username and password (RFC 2595).
                // Note that we check if the 1'th cells of the split result is empty (regard to the RFC
                // the structure of the credentials is: 'authorization-id\0authentication-id\0passwd' 
                // and sometimes the client may leave the authorization identity empty to indicate that
                // it is the same as the authentication identity).
                string[] credentialsParts = Utilities.DecodeAsciiBase64(match.Groups["Credentials"].Value).Split('\0');

                return credential = new NetworkPassword()
                {
                    Protocol = "SMTP (Auth Plain)",
                    Username = credentialsParts[0] != string.Empty ? credentialsParts[0] : credentialsParts[1],
                    Password = credentialsParts[2],
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return credential;
        }

        private NetworkHash SearchSmtpCramMd5(TcpSession tcpSession, string sessionData)
        {
            NetworkHash credential = null;
            Match match = _smtpCramMd5Regex.Match(sessionData);

            if (match.Success)
            {
                credential = new CramMd5Hash()
                {
                    Protocol = "SMTP",
                    HashType = "CRAM-MD5",
                    Hash = match.Groups["Hash"].ToString(),
                    Challenge = match.Groups["Challenge"].ToString(),
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return credential;
        }

        
    }
}
