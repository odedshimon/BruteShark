using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BruteSharkDesktop
{
    public class TransportLayerObject
    {
        [DisplayName("Source Ip")]
        public string SourceIp { get; set; }

        [DisplayName("Destination Ip")]
        public string DestinationIp { get; set; }

        [DisplayName("Source Port")]
        public int SourcePort { get; set; }

        [DisplayName("Destination Port")]
        public int DestinationPort { get; set; }

        [Browsable(false)]
        public byte[] Data { get; set; }
    }
}
