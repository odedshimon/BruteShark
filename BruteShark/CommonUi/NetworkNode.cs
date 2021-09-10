using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommonUi
{
    public class NetworkNode
    {
        [DisplayName("IP Address")]
        public string IpAddress { get; set; }
        [DisplayName("Open Ports")]
        public HashSet<int> OpenPorts { get; set; }
        [DisplayName("DNS Mappings")]
        public HashSet<string> DnsMappings { get; set; }

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
