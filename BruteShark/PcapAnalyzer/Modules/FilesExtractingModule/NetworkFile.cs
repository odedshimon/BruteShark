using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class NetworkFile : NetworkLayerObject
    {
        public byte[] FileData { get; set; }
        public string Extention { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkFile))
            {
                return false;
            }

            var networkFile = obj as NetworkFile;

            return base.Equals(networkFile) &&
                   this.FileData == networkFile.FileData &&
                   this.Extention == networkFile.Extention;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   this.FileData.GetHashCode() ^
                   this.Extention.GetHashCode();
        }
    }
}
