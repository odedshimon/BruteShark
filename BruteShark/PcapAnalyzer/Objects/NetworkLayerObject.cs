namespace PcapAnalyzer
{
    public class NetworkLayerObject : InternetLayerObject
    {
        public string Protocol { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkLayerObject))
            {
                return false;
            }

            var networkObject = obj as NetworkLayerObject;

            return base.Equals(networkObject) &&
                   this.Protocol == networkObject.Protocol;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   this.Protocol.GetHashCode();
        }
    }
}