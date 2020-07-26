using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    interface IPasswordParser
    {
        NetworkLayerObject Parse(UdpPacket udpPacket);
        NetworkLayerObject Parse(TcpPacket tcpPacket);
        NetworkLayerObject Parse(TcpSession tcpSession);
    }
}
