using System;
using System.Collections.Generic;
using System.Text;
using SIPSorcery.SIP;
using SIPSorcery.Net;
using System.Linq;

namespace PcapAnalyzer
{
    public enum CallState
    {
        Invited,
        InCall,
        Cancelled,
        Rejected,
        Completed
    }
    public class VoipCall
    {
        public string To { get; set; }
        public string ToHost { get; set; }
        public string ToIP { get; set; }
        public string From { get; set; }
        public string FromHost { get; set; }
        public string FromIP { get; set; }
        public int RTPPort { get; set; }
        public CallState CallState { get; set; }
        internal List<string> RTPMediaTypes { get; set; }
        private List<RTPPacket> _rtpPackets { get; set; }
        internal VoipCall()
        {
            _rtpPackets = new List<RTPPacket>();
            RTPMediaTypes = new List<string>();
        }

        public override bool Equals(object other)
        {
            var call = other as VoipCall;
            return (this.From == call.From || this.From == call.To) &&
                (this.To == call.To || this.To == call.From) &&
                (this.ToIP == call.ToIP || this.ToIP == call.FromIP) &&
                (this.FromIP == call.FromIP || this.FromIP == call.ToIP);
        }
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^
                this.To.GetHashCode() ^
                this.ToIP.GetHashCode() ^
                this.FromIP.GetHashCode() ^
                this.FromHost.GetHashCode() ^
                this.ToHost.GetHashCode(); 
        }

        public void addrtpPacket(RTPPacket packet)
        {
            _rtpPackets.Add(packet);
        }

        public byte[] RTPStream()
        {
            return _rtpPackets.SelectMany(p => p.Payload).ToArray();
        }
    }
}
