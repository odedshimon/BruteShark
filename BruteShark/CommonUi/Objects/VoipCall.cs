using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using PcapAnalyzer;


namespace CommonUi
{
    public class VoipCall
    {
        [Browsable(false)]
        public byte[] RTPStream { get; set; }
        [Browsable(false)]
        public Guid CallGuid { get; set; }

        public string From { get; set; }
        [DisplayName("From Host")]
        public string FromHost { get; set; }
        [DisplayName("From Ip")]
        public string FromIP { get; set; }
        public string To { get; set; }
        [DisplayName("To Host")]
        public string ToHost { get; set; }
        [DisplayName("To Ip")]
        public string ToIP { get; set; }
        [DisplayName("RTP Port")]
        public int RTPPort { get; set; }
        [DisplayName("Call State")]
        public string CallState { get; set; }
        [DisplayName("RTP Media Type")]
        public string RTPMediaType { get; set; }
        [DisplayName("Data Size (Bytes)")]
        public int DataSize => RTPStream.Length;


        public VoipCall() { }

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
            var other = obj as VoipCall;
            return this.CallGuid.Equals(other.CallGuid);
        }

        public override int GetHashCode()
        {
            return CallGuid.GetHashCode();
        }

    }
}
