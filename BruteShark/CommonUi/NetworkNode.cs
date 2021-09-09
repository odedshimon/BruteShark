using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUi
{
    public class NetworkNode
    {
        public readonly string IpAddress;
        public HashSet<int> OpenPorts;

        public NetworkNode(string ipAddress)
        {
            this.IpAddress = ipAddress;
            this.OpenPorts = new HashSet<int>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkNode))
            {
                return false;
            }

            var networkNode = obj as NetworkNode;

            return base.Equals(networkNode) &&
                   this.IpAddress == networkNode.IpAddress;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   this.IpAddress.GetHashCode();
        }
    }
}
