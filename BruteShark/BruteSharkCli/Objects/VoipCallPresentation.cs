using System;
using System.Collections.Generic;
using System.Text;
using PcapAnalyzer;

namespace BruteSharkCli
{
    internal class VoipCallPresentation
    {
        public string To { get; set; }
        public string ToHost { get; set; }
        public string ToIP { get; set; }
        public string From { get; set; }
        public string FromHost { get; set; }
        public string FromIP { get; set; }
        public int RTPPort { get; set; }
        internal string CallState { get; set; }

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
            _call.CallState = call.CallState.ToString();
            return _call;
        }

        public override string ToString()
        {
            return $"{From}@{FromHost}({FromIP}) -> {To}@{ToHost}({ToIP}) | RTP port: {RTPPort}| state: {CallState} ";
        }
    }
}
