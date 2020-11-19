namespace PcapAnalyzer
{
    public class NetworkConnection
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Protocol { get; set; }
        public int SrcPort { get; set; }
        public int DestPort { get; set; }


        public override bool Equals(object obj)
        {
            if (!(obj is NetworkConnection))
            {
                return false;
            }

            var networkConnection = obj as NetworkConnection;

            return this.Source == networkConnection.Source &&
                   this.Destination == networkConnection.Destination &&
                   this.Protocol == networkConnection.Protocol &&
                   this.SrcPort == networkConnection.SrcPort &&
                   this.DestPort == networkConnection.DestPort;


        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^
                   this.Destination.GetHashCode() ^ 
                   this.Protocol.GetHashCode() ^ 
                   this.SrcPort.GetHashCode() ^ 
                   this.DestPort.GetHashCode();
        }
    }
}