using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PcapAnalyzer
{
    public class NetworkFile : NetworkLayerObject
    {
        // TODO: remove this (create pl file object)
        [Browsable(false)]
        public byte[] FileData { get; set; }
        public string Extention { get; set; }
        public string Algorithm { get; set; }
        public int FileSize
        {
            get
            {
                return this.FileData.Length;
            }
        }


        public override bool Equals(object obj)
        {
            if (!(obj is NetworkFile))
            {
                return false;
            }

            var networkFile = obj as NetworkFile;

            return base.Equals(networkFile) &&
                   this.FileData == networkFile.FileData &&
                   this.Extention == networkFile.Extention &&
                   this.Algorithm == networkFile.Algorithm;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   this.FileData.GetHashCode() ^
                   this.Extention.GetHashCode() ^
                   this.Algorithm.GetHashCode();
        }

        public override string ToString()
        {
            return $"File (File Extention: {Extention} carving aglorithm: {Algorithm})";
        }
    }
}
