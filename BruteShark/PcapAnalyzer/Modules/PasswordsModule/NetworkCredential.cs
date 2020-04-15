namespace PcapAnalyzer
{
    public class NetworkCredential
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Protocol { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkCredential))
            {
                return false;
            }

            var networkCredential = obj as NetworkCredential;

            return this.Source == networkCredential.Source &&
                   this.Destination == networkCredential.Destination &&
                   this.Protocol == networkCredential.Protocol;
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^
                   this.Destination.GetHashCode() ^
                   this.Protocol.GetHashCode();
        }
    }
}