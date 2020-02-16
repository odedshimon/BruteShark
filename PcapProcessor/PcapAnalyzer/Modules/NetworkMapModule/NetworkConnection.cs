namespace PcapAnalyzer
{
    public class NetworkConnection
    {
        public string Source { get; set; }
        public string Destination { get; set; }


        public override bool Equals(object obj)
        {
            if (!(obj is NetworkConnection))
            {
                return false;
            }

            var networkConnection = obj as NetworkConnection;

            return this.Source == networkConnection.Source &&
                   this.Destination == networkConnection.Destination;

        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^
                   this.Destination.GetHashCode();
        }
    }
}