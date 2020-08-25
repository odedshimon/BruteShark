using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkDesktop
{
    public class TcpSession : TransportLayerSession
    {
        public List<TcpPacket> Packets { get; set; }

        public TcpSession()
        {
            this.Packets = new List<TcpPacket>();
        }
    }
}
