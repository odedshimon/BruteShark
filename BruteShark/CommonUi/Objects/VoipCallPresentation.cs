using System;
using System.Collections.Generic;
using System.Text;
using PcapAnalyzer;


namespace CommonUi
{
    public class VoipCallPresentation
    {
        public string To { get; set; }
        public string ToHost { get; set; }
        public string ToIP { get; set; }
        public string From { get; set; }
        public string FromHost { get; set; }
        public string FromIP { get; set; }
        public int RTPPort { get; set; }
        public string CallState { get; set; }
        public string RTPMediaType { get; set; }
        internal Guid callGuid;
        private byte[] _rtpPackets { get; set; }

        public VoipCallPresentation() {}
        public static VoipCallPresentation FromAnalyzerVoipCall(VoipCall call)
        {
            VoipCallPresentation _call = new VoipCallPresentation();
            _call.To = call.To;
            _call.From = call.From;
            _call.ToHost = call.ToHost;
            _call.FromHost = call.FromHost;
            _call.ToIP = call.FromIP;
            _call.FromIP = call.FromIP;
            _call.RTPPort = call.RTPPort;
            _call.RTPMediaType = call.RTPMediaType;
            _call.callGuid = call.callGuid;
            _call.CallState = call.CallState.ToString();
            _call._rtpPackets = call.RTPStream();
            return _call;
        }

        public override string ToString()
        {
            return $"{From}@{FromHost}({FromIP})->{To}@{ToHost}({ToIP}) - RTP port: {RTPPort}, Media type: {RTPMediaType} - state: {CallState}";
        }

        public string ToFilename()
        {
            return $"from_{From}_{FromHost}_{FromIP}_to_{To}_{ToHost}_{ToIP}_RTP_port_{RTPPort}_{RTPMediaType}_state_{CallState}".Replace(':', '_');
        }
        public override bool Equals(object obj)
        {
            var other = obj as VoipCallPresentation;
            return this.callGuid.Equals(other.callGuid);
        }
        public override int GetHashCode()
        {
            return callGuid.GetHashCode();
        }
        public byte[] GetRTPStream()
        {
            return this._rtpPackets;
        }
    }
}
