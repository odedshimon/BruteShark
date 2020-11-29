using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PcapAnalyzer
{
    public class DnsModule : IModule
    {
        public string Name => "DNS";

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;

        private HashSet<DnsNameMapping> _mappings;

        public HashSet<DnsNameMapping> Mappings => _mappings;

        private DnsNameMappingComparer _comparer;

        public DnsModule()
        {
            _mappings = new HashSet<DnsNameMapping>();
            _comparer = new DnsNameMappingComparer();
        }

        public void Analyze(UdpPacket udpPacket)
        {
            //We are going to assume that all DNS traffic is on port 53
            if (udpPacket.DestinationPort != 53 && udpPacket.SourcePort != 53)
            {
                return;
            }

            var header = Header.FromArray(udpPacket.Data);
            if (header.Response == false)
            {
                //We don't care about non responses
            }
            else
            {
                var res = Response.FromArray(udpPacket.Data);

                var answers = GetStringsForAnswers(res.AnswerRecords);
                foreach (var answer in answers)
                {
                    //TODO: Probably should do this better...
                    RaiseParsedItemDetected(res.Questions[0].Name.ToString(), answer);
                }
            }
        }

        private IEnumerable<string> GetStringsForAnswers(IList<IResourceRecord> answerRecords)
        {
            var returnList = new List<string>();
            foreach (var ar in answerRecords)
            {
                if (ar is IPAddressResourceRecord)
                {
                    returnList.Add((ar as IPAddressResourceRecord).IPAddress.ToString());
                }
                else if (ar is CanonicalNameResourceRecord)
                {
                    returnList.Add((ar as CanonicalNameResourceRecord).CanonicalDomainName.ToString());
                }
                else if (ar is PointerResourceRecord)
                {
                    returnList.Add((ar as PointerResourceRecord).PointerDomainName.ToString());
                }
                else
                {
                    //Will show up obviously wrong in UX, need to decide which other queries are worthwhile to special case
                    returnList.Add(ar.ToString());
                }
            }

            return returnList;
        }
        
        private void RaiseParsedItemDetected(string query, string destination)
        {
            var response = new DnsNameMapping()
            {
                Query = query,
                Destination = destination,
            };

            if (_mappings.Contains(response, _comparer))
            {
                return;
            }

            if (_mappings.Add(response))
            {
                this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                {
                    ParsedItem = response
                });
            }
        }

        public void Analyze(TcpPacket tcpPacket) { }

        public void Analyze(TcpSession tcpSession) { }

        public void Analyze(UdpStream udpStream) { }
    }
}
