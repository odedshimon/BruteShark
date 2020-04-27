using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapAnalyzer
{
    class PasswordsModule : IModule
    {

        private KerberosHashParser _kerberosParser;
        private List<IPasswordParser> _passwordParsers;

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        public string Name => "Passwords Extractor";


        public PasswordsModule()
        {
            this._kerberosParser = new KerberosHashParser();
            _initilyzePasswordParsersList();
        }

        private void _initilyzePasswordParsersList()
        {
            // Create an instance for any available parser by looking for every class that 
            // implements IPasswordParser.
            this._passwordParsers = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(s => s.GetTypes())
                                    .Where(p => typeof(IPasswordParser).IsAssignableFrom(p) && !p.IsInterface)
                                    .Select(t => (IPasswordParser)Activator.CreateInstance(t))
                                    .ToList();
        }

        public void Analyze(UdpPacket udpPacket)
        {
            NetworkCredential credential = this._kerberosParser.Parse(udpPacket);

            if (credential != null)
            {
                // Raise event.
                this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                {
                    ParsedItem = credential
                });
            }
        }

        public void Analyze(TcpPacket tcpPacket)
        {
            foreach (var parsrer in this._passwordParsers)
            {
                NetworkCredential credential = parsrer.Parse(tcpPacket);

                if (credential != null)
                {
                    // Raise event.
                    this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                    {
                        ParsedItem = credential
                    });

                }
            }
        }

        public void Analyze(TcpSession tcpSession)
        {
            foreach (var parsrer in this._passwordParsers)
            {
                NetworkCredential credential = parsrer.Parse(tcpSession);

                if (credential != null)
                {
                    // Raise event.
                    this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                    {
                        ParsedItem = credential
                    });
                }
            }
        }
    }
}
