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

        public List<string> filenames {get; set;}

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
            this.filenames = new List<string>();
            this._sessions = new Dictionary<TcpSession, TcpRecon>();
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
        }
        
        public void Clear()
        {
            this._sessions.Clear();
        }

        public void ClearSession(TcpSession session)
        {
            this._sessions.Remove(session);
        }
    }
}

