namespace PcapAnalyzer
{
    public class NetworkPassword : NetworkLayerObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public override bool Equals(object obj)
        {
            if (!(obj is NetworkPassword))
            {
                return false;
            }

            var networkCredential = obj as NetworkPassword;

            return base.Equals(networkCredential) &&
                   this.Username == networkCredential.Username &&
                   this.Password == networkCredential.Password;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   this.Username.GetHashCode() ^
                   this.Password.GetHashCode();
        }
    }
}