namespace BruteSharkDesktop
{
    public class NetworkMapEdge
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkMapEdge))
            {
                return false;
            }

            var networkCredential = obj as NetworkMapEdge;

            return this.Source == networkCredential.Source &&
                   this.Destination == networkCredential.Destination &&
                   this.Text == networkCredential.Text;
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^
                   this.Destination.GetHashCode() ^
                   this.Text.GetHashCode();
        }
    }
}
