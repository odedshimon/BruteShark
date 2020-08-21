using System;
using System.Collections.Generic;
using System.Text;

namespace PcapProcessor.Objects
{
    class UdpStreamBuilder
    {
        private Dictionary<UdpStream, UdpRecon> _sessions;

        public IEnumerable<UdpStream> Sessions
        {
            get
            {
                return this._sessions.Select(kvp => new UdpStream()
                {
                    SourceIp = kvp.Key.SourceIp,
                    DestinationIp = kvp.Key.DestinationIp,
                    SourcePort = kvp.Key.SourcePort,
                    DestinationPort = kvp.Key.DestinationPort,
                    Data = kvp.Value.Data,
                    Packets = kvp.Value.packets.Select(p => new PcapProcessor.UdpPacket()
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

        public UdpStreamBuilder()
        {
            this._sessions = new Dictionary<UdpStream, UdpRecon>();
        }

        public void HandlePacket(PacketDotNet.UdpPacket udpPacket)
        {
            var session = new UdpStream()
            {
                SourceIp = ((PacketDotNet.IPPacket)udpPacket.ParentPacket).SourceAddress.ToString(),
                SourcePort = udpPacket.SourcePort,
                DestinationIp = ((PacketDotNet.IPPacket)udpPacket.ParentPacket).DestinationAddress.ToString(),
                DestinationPort = udpPacket.DestinationPort
            };

            if (!_sessions.ContainsKey(session))
            {
                UdpRecon recon = new UdpRecon();
                _sessions.Add(session, recon);
            }

            _sessions[session].ReassemblePacket(udpPacket);
        }

        public void Clear()
        {
            this._sessions.Clear();
        }
    }
}
