using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapAnalyzer
{
    class PasswordsModule : IModule
    {
        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        private List<IPasswordParser> _passwordParsers;
        public string Name => "Credentials Extractor (Passwords, Hashes)";


        public PasswordsModule()
        {
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

        public void Analyze(UdpPacket udpPacket) => AnalyzeGeneric(udpPacket);
        public void Analyze(UdpStream udpStream) => AnalyzeGeneric(udpStream);
        public void Analyze(TcpPacket tcpPacket) => AnalyzeGeneric(tcpPacket);
        public void Analyze(TcpSession tcpSession) => AnalyzeGeneric(tcpSession);

        public void AnalyzeGeneric(object item)
        {
            NetworkLayerObject credential = null;

            foreach (var parsrer in this._passwordParsers)
            {
                if (item is TcpPacket)
                {
                    credential = SafeRun(x => parsrer.Parse(x as TcpPacket), item as TcpPacket);
                }
                else if (item is TcpSession)
                {
                    credential = SafeRun(x => parsrer.Parse(x as TcpSession), item as TcpSession);
                }
                else if (item is UdpPacket)
                {
                    credential = SafeRun(x => parsrer.Parse(x as UdpPacket), item as UdpPacket);
                }
                else if (item is UdpStream)
                {
                    // Nothing to do.
                }
                else
                {
                    throw new Exception("Unsupported type for password module");
                }

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

        private NetworkLayerObject SafeRun(Func<object, NetworkLayerObject> func, object param)
        {
            try
            {
                return func(param);
            }
            catch (Exception ex) { }

            return null;
        }
    }
}
