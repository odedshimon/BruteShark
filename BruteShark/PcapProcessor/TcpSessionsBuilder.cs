using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PcapProcessor
{
    internal class TcpSessionsBuilder
    {
        private Dictionary<TcpSession, TcpRecon> _sessions;
        internal List<TcpSession> completedSessions { get; }
        public bool IsLiveCapture { get; set; }

        public IEnumerable<TcpSession> Sessions
        {
            get
            {
                return this._sessions.Select(kvp => new TcpSession()
                {
                    SourceIp = kvp.Key.SourceIp,
                    DestinationIp = kvp.Key.DestinationIp,
                    SourcePort = kvp.Key.SourcePort,
                    DestinationPort = kvp.Key.DestinationPort,
                    Data = kvp.Value.Data,
                    Packets = kvp.Value.packets.Select(p => new PcapProcessor.TcpPacket()
                    {
                        SourceIp = ((PacketDotNet.IPPacket)p.ParentPacket).SourceAddress.ToString(),
                        DestinationIp = ((PacketDotNet.IPPacket)p.ParentPacket).DestinationAddress.ToString(),
                        SourcePort = p.SourcePort,
                        DestinationPort = p.DestinationPort,
                        Data = p.PayloadData
                    }).ToList()
                });
            }
            private set { }
        }

        public TcpSessionsBuilder()
        {
            IsLiveCapture = false;
            this._sessions = new Dictionary<TcpSession, TcpRecon>();
            this.completedSessions = new List<TcpSession>();
        }

        public void HandlePacket(PacketDotNet.TcpPacket tcpPacket)
        {
            var ipPacket = (PacketDotNet.IPPacket)tcpPacket.ParentPacket;

            var session = new TcpSession()
            {
                SourceIp = ipPacket.SourceAddress.ToString(),
                SourcePort = tcpPacket.SourcePort,
                DestinationIp = ipPacket.DestinationAddress.ToString(),
                DestinationPort = tcpPacket.DestinationPort
            };

            if (!_sessions.ContainsKey(session))
            {
                TcpRecon recon = new TcpRecon();
                _sessions.Add(session, recon);
            }

            _sessions[session].ReassemblePacket(tcpPacket);

            // If the tcp packet contains FIN or ACK flags we can determine that the session 
            // is terminated by means that no more data is about to be sent.
            if (IsLiveCapture)
            { 
                if (tcpPacket.Flags == 17 || tcpPacket.Flags == 1)
                {
                    session.Data = _sessions[session].Data;

                    foreach (var currentTcpPacket in _sessions[session].packets)
                    {
                        var currentIpPacket = (PacketDotNet.IPPacket)currentTcpPacket.ParentPacket;

                        session.Packets.Add(new TcpPacket()
                        {
                            SourceIp = currentIpPacket.SourceAddress.ToString(),
                            SourcePort = currentTcpPacket.SourcePort,
                            DestinationIp = currentIpPacket.DestinationAddress.ToString(),
                            DestinationPort = currentTcpPacket.DestinationPort,
                            Data = currentTcpPacket.PayloadData
                        });
                    }

                    completedSessions.Add(session);
                    _sessions.Remove(session);
                }
            }
        }

        public void Clear()
        {
            this._sessions.Clear();
        }
    }
}
