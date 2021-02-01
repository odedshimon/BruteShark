namespace PcapAnalyzer.Modules.VoipCallsModule
{
    public class RTPHeader
    {
        public short Version;
        public bool Padding;
        public bool IsExtensionHeaderPresent;
        public short CSRCCount;
        public bool ImportanceMarker;
        public string PayloadType;
        public short SequenceNumber;
        public int TimeStamp;
        public int CSRCSource;
        public int CSRCContributingSource;
    }

    public RTPHeader(byte[] payload)
    {

    }
}