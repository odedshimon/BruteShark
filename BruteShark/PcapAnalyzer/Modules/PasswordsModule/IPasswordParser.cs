using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    interface IPasswordParser
    {
        NetworkCredential Parse(TcpPacket tcpPacket);
        NetworkCredential Parse(TcpSession tcpSession);
    }
}
