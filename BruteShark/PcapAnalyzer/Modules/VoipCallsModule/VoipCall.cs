using System;
using System.Collections.Generic;
using System.Text;
using SIPSorcery.SIP;
using SIPSorcery.Net;
using System.Linq;

namespace PcapAnalyzer
{
    internal class VoipCall
    {
        public SIPEndPoint To { get; set; }
        public SIPEndPoint From { get; set; }
        public byte[] RTPStream
        {
            get => _rtpPackets.SelectMany(p => p.Payload).ToArray();
        }
        private List<RTPPacket> _rtpPackets { get; set; }
        internal VoipCall(SIPEndPoint from, SIPEndPoint to)
        {
            this.From = from;
            this.To = to;
            _rtpPackets = new List<RTPPacket>();
        }

        public override bool Equals(object other)
        {
            var call = other as VoipCall;
            return this.From == call.From && this.To == call.To;
        }
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^ this.To.GetHashCode();
        }

        public void addrtpPacket(RTPPacket packet)
        {
            _rtpPackets.Add(packet);
        }
    }
}
