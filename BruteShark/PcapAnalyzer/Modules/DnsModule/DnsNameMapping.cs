using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    public class DnsNameMapping
    {
        public DnsNameMapping()
        {
        }

        public string Query { get; set; }
        public string Destination { get; set; }
    }

    class DnsNameMappingComparer : IEqualityComparer<DnsNameMapping>
    {
        public bool Equals(DnsNameMapping x, DnsNameMapping y)
        {
            return x.Query == y.Query && x.Destination == y.Destination;
        }

        public int GetHashCode(DnsNameMapping obj)
        {
            return obj.Query.GetHashCode() ^ obj.Destination.GetHashCode();
        }
    }
}
