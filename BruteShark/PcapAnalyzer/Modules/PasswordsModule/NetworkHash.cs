using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class NetworkHash : NetworkLayerObject
    {
        public string Hash { get; set; }
        public string HashType { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkHash))
            {
                return false;
            }

            var networkHash = obj as NetworkHash;

            return base.Equals(networkHash) &&
                   this.Hash == networkHash.Hash &&
                   this.HashType == networkHash.HashType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   this.Hash.GetHashCode() ^
                   this.HashType.GetHashCode();
        }
    }
}
