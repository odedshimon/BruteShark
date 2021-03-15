using System;
using System.Collections.Generic;
using System.Text;
using PcapAnalyzer;


namespace CommonUi
{
    public class VoipCallPresentation
    {
        public byte[] RtpStream { get; set; }
        public Guid CallGuid { get; set; }
        public string To { get; set; }
        public string ToHost { get; set; }
        public string ToIP { get; set; }
        public string From { get; set; }
        public string FromHost { get; set; }
        public string FromIP { get; set; }
        public int RTPPort { get; set; }
        public string CallState { get; set; }
        public string RTPMediaType { get; set; }
        

        public VoipCallPresentation() { }

        public static VoipCallPresentation FromAnalyzerVoipCall(VoipCall call)
        {
            return new VoipCallPresentation
            {
                To = call.To,
                From = call.From,
                ToHost = call.ToHost,
                FromHost = call.FromHost,
                ToIP = call.FromIP,
                FromIP = call.FromIP,
                RTPPort = call.RTPPort,
                RTPMediaType = call.RTPMediaType,
                CallGuid = call.callGuid,
                CallState = call.CallState.ToString(),
                RtpStream = call.RTPStream()
            };
        }

        public override string ToString()
        {
            return $"{From}@{FromHost}({FromIP})->{To}@{ToHost}({ToIP}) - RTP port: {RTPPort}, Media type: {RTPMediaType} - state: {CallState}";
        }

        public string ToFilename()
        {
            var fileName = $"({From}-{FromHost}-{FromIP}) To ({To}-{ToHost}-{ToIP}) (Media Type {RTPMediaType})_{CallState}";
            return CommonUi.Exporting.ReplaceInvalidFileNameChars(fileName, '_');
        }

        public override bool Equals(object obj)
        {
            var other = obj as VoipCallPresentation;
            return this.CallGuid.Equals(other.CallGuid);
        }

        public override int GetHashCode()
        {
            return CallGuid.GetHashCode();
        }

        public byte[] GetRtpStream()
        {
            return this.RtpStream;
        }
    }
}
