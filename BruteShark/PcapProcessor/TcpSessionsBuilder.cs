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
            this._sessions = new Dictionary<TcpSession, TcpRecon>();
            this.completedSessions = new List<TcpSession>();
        }

        public void HandlePacket(PacketDotNet.TcpPacket tcpPacket)
        {
            var session = new TcpSession()
            {
                SourceIp = ((PacketDotNet.IPPacket)tcpPacket.ParentPacket).SourceAddress.ToString(),
                SourcePort = tcpPacket.SourcePort,
                DestinationIp = ((PacketDotNet.IPPacket)tcpPacket.ParentPacket).DestinationAddress.ToString(),
                DestinationPort = tcpPacket.DestinationPort
            };

            if (!_sessions.ContainsKey(session))
            {
                TcpRecon recon = new TcpRecon();
                _sessions.Add(session, recon);
            }

            _sessions[session].ReassemblePacket(tcpPacket);
            // if the tcp packet contains FIN,ACK flags or just the FIN flag then we can determine 
            // that the session is terminated and that no more data is about to be sen  t
            if (tcpPacket.Flags == 17 || tcpPacket.Flags == 1)
            {
                session.Data = _sessions[session].Data;
                completedSessions.Add(session);
                _sessions.Remove(session);
            }
        }

        public void Clear()
        {
            this._sessions.Clear();
        }
    }
}
