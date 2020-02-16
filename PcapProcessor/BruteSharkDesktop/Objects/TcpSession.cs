using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkDesktop
{
    public class TcpSession
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
        [Browsable(false)]
        public List<TcpPacket> Packets { get; set; }

        public TcpSession()
        {
            this.Packets = new List<TcpPacket>();
        }
    }
}
