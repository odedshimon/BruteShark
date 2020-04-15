using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{
    public class ImapPasswordParser : IPasswordParser
    {
        // IMAP enables two mechanisms for authentication using the following commands:
        //  1. Login - plain text user name and password.
        //  2. Authenticate - several authntication options (Plain, Kerberos, CRAM-MD5 and more).
        //
        // RFCs:
        //  https://tools.ietf.org/html/rfc4959 (PLAIN)
        //  https://tools.ietf.org/html/rfc1731 (Kerberos V4, GSSAPI, SKEY)
        //  https://tools.ietf.org/html/rfc2195 (CRAM-MD5)
        //  https://tools.ietf.org/html/rfc2831 (Digest-MD5)

        private const string AsciiNewLine = "\r\n";
        private const string ImapCommandTag = @"[A-Za-z0-9]{1,6} ";
        
        private Regex _imapPlaintextLoginRegex =    new Regex($@"({ImapCommandTag})?LOGIN (?<Username>.*) (?<Password>.*)\r\n({ImapCommandTag})?OK");
        private Regex _imapAuthenticateLoginRegex = new Regex($@"({ImapCommandTag})?((?i)AUTHENTICATE PLAIN)((\r\n(\+ )\r\n)| )(?<CredentialsBase64>.*)(\r\n)({ImapCommandTag})?OK");
        private Regex _imapCramMd5Regex =           new Regex($@"({ImapCommandTag})?((?i)AUTHENTICATE CRAM-MD5)\r\n\+ (?<Challenge>.*)\r\n(?<Response>.*)\r\n({ImapCommandTag})?OK");

        public NetworkCredential Parse(TcpPacket tcpPacket) => null;

        public NetworkCredential Parse(TcpSession tcpSession)
        {
            NetworkCredential credential = null;
            var sessionData = Encoding.ASCII.GetString(tcpSession.Data);

            if ((credential = SearchImapPlaintextLogin(tcpSession, sessionData)) != null)
            {
                return credential;
            }
            else if ((credential = SearchImapAuthenticateLogin(tcpSession, sessionData)) != null)
            {
                return credential;
            }
            else if ((credential = SearchImapCramMd5Hash(tcpSession, sessionData)) != null)
            {
                return credential;
            }

            return credential;
        }

        private NetworkCredential SearchImapCramMd5Hash(TcpSession tcpSession, string sessionData)
        {
            NetworkHash hash = null;
            Match match = _imapCramMd5Regex.Match(sessionData);

            if (match.Success)
            {
                // TODO: Handle the triming at the regex.
                hash = new CramMd5Hash()
                {
                    Protocol = "IMAP",
                    HashType = "CRAM-MD5",
                    Challenge = match.Groups["Challenge"].Value,
                    Hash = match.Groups["Response"].Value,
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return hash;
        }

        private NetworkPassword SearchImapPlaintextLogin(TcpSession tcpSession, string sessionData)
        {
            NetworkPassword password = null;
            Match match = _imapPlaintextLoginRegex.Match(sessionData);

            if (match.Success)
            {
                // TODO: Handle the triming at the regex.
                password = new NetworkPassword()
                {
                    Protocol = "IMAP Plaintext",
                    Username = match.Groups["Username"].Value.Trim(new char[] { '"' }),
                    Password = match.Groups["Password"].Value.Trim(new char[] { '"' }),
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return password;
        }

        private NetworkPassword SearchImapAuthenticateLogin(TcpSession tcpSession, string sessionData)
        {
            NetworkPassword password = null;
            Match match = _imapAuthenticateLoginRegex.Match(sessionData);

            if (match.Success)
            {
                // Decode the credentials Base64 string and split by Ascii Null character to get
                // the username and password (RFC 2595).
                // Note that we check if the 1'th cells of the split result is empty (regard to the RFC
                // the structure of the credentials is: 'authorization-id\0authentication-id\0passwd' 
                // and sometimes the client may leave the authorization identity empty to indicate that
                // it is the same as the authentication identity).
                // TODO: maybe extract to external class and use it also at the SMTP Parser.
                string[] credentialsParts = Utilities.DecodeAsciiBase64(match.Groups["CredentialsBase64"].Value).Split('\0');

                password = new NetworkPassword()
                {
                    Protocol = "IMAP LOGIN (Base64)",
                    Username = credentialsParts[0] != string.Empty ? credentialsParts[0] : credentialsParts[1],
                    Password = credentialsParts[2],
                    Source = tcpSession.SourceIp,
                    Destination = tcpSession.DestinationIp
                };
            }

            return password;
        }

    }
}
