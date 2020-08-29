using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkDesktop
{
    public class TransportLayerSession : TransportLayerObject 
    {
        
        public List<TransportLayerPacket> Packets { get; set; }

        public TransportLayerSession()
        {
            this.Packets = new List<TransportLayerPacket>();
        }

    }
}
