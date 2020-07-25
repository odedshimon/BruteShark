using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PcapAnalyzer
{

    public class TelnetPasswordParser : IPasswordParser
    {
        private const byte _nvtAsciiMinValue = 0x20;
        private const byte _nvtAsciiMaxValue = 0x7E;
        private Regex _telnetPossibleLoginRegex = new Regex(@"login:([\s\S]*)Password:(.*)");

        private enum TelnetServerState
        {
            None,
            WaitForUsername,
            WaitForPassword,
        }

        public NetworkLayerObject Parse(UdpPacket udpPacket) => null;

        public NetworkLayerObject Parse(TcpPacket tcpPacket)
        {
            return null;
        }

        // This function extracts Telnet username and password from a Tcp session.
        // Algorithm: 
        //  - First check if the session data matches a Regular expression. 
        //  - If match, iterate over session packets and manage a state machine 
        //    depends on the authentication phase.
        //
        // The algorithm should work on both Line-Mode (RFC 1116) and Character-Mode (the default 
        // Telnet mode). 
        public NetworkLayerObject Parse(TcpSession tcpSession)
        {
            int serverPort;
            int clientPort;
            var serverIp = string.Empty;
            var clientIp = string.Empty;
            var password = string.Empty;
            var username = string.Empty;
            var state = TelnetServerState.None;
            NetworkPassword credential = null;

            var sessionData = Encoding.UTF8.GetString(tcpSession.Data);
            Match match = _telnetPossibleLoginRegex.Match(sessionData);

            if (match.Success)
            {
                foreach (var packet in tcpSession.Packets)
                {
                    // Telnet works with ASCII characters set (known as NVT ASCII).
                    var packetData = Encoding.ASCII.GetString(packet.Data);

                    if (packetData.Contains("login:") && state == TelnetServerState.None)
                    {
                        // Set the direction of the authentication.
                        state = TelnetServerState.WaitForUsername;
                        serverIp = packet.SourceIp;
                        clientIp = packet.DestinationIp;
                        serverPort = packet.SourcePort;
                        clientPort = packet.DestinationPort;
                    }
                    else if (state == TelnetServerState.WaitForUsername && packet.SourceIp == clientIp)
                    {
                        // Buffer the username (relevant for char mode).
                        // NOTE: Telnet username can be passed a char by char (Character-Mode) or 
                        // at once (Line-Mode).
                        username += getNvtAsciiDataString(packet.Data);
                    }
                    else if (packetData.Contains("Password:") && state == TelnetServerState.WaitForUsername)
                    {
                        state = TelnetServerState.WaitForPassword;
                    }
                    else if (state == TelnetServerState.WaitForPassword && packet.SourceIp == clientIp)
                    {
                        // Buffer the password (relevant for char mode).
                        password += getNvtAsciiDataString(packet.Data);
                    }
                    else if (packet.SourceIp == serverIp && packet.Data.Length > 0 && state == TelnetServerState.WaitForPassword)
                    {
                        // Final stage is when the server sends any data after the client sends 
                        // the password.
                        credential = new NetworkPassword()
                        {
                            Protocol = "Telnet",
                            Username = username,
                            Password = password,
                            Source = clientIp,
                            Destination = serverIp
                        };

                        break;
                    }
                }
            }

            return credential;
        }

        // This function returns a string contains only NVT ASCII Data charcters,
        // which is the legit Telnet user data characters (read more at RFC 854).
        private string getNvtAsciiDataString(byte[] data)
        {
            return String.Concat(
                data.Where(b =>  
                        b > _nvtAsciiMinValue && 
                        b < _nvtAsciiMaxValue)
                    .Select(b => Convert.ToChar(b)));
        }
    }
}
