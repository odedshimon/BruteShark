using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonUi
{
    public class NetworkContext
    {
        public Dictionary<string, HashSet<int>> OpenPorts { get; private set; }
        public HashSet<PcapAnalyzer.DnsNameMapping> DnsMappings { get; private set; }
        public HashSet<PcapAnalyzer.NetworkConnection> Connections { get; private set; }
        public HashSet<PcapProcessor.NetworkObject> NetworkSessions { get; private set; }

        public NetworkContext()
        {
            OpenPorts = new Dictionary<string, HashSet<int>>();
            DnsMappings = new HashSet<PcapAnalyzer.DnsNameMapping>();
            Connections = new HashSet<PcapAnalyzer.NetworkConnection>();
            NetworkSessions = new HashSet<PcapProcessor.NetworkObject>();
        }

        public bool HandleDnsNameMapping(PcapAnalyzer.DnsNameMapping dnsNameMapping)
        {
            return DnsMappings.Add(dnsNameMapping);
        }

        public void HandleNetworkConection(PcapAnalyzer.NetworkConnection networkConnection)
        {
            // Create network nodes if needed.
            if (Connections.Add(networkConnection))
            {
                if (!OpenPorts.ContainsKey(networkConnection.Source))
                {
                    OpenPorts[networkConnection.Source] = new HashSet<int>();
                }
                if (!OpenPorts.ContainsKey(networkConnection.Destination))
                {
                    OpenPorts[networkConnection.Destination] = new HashSet<int>();
                }
            }

            // Update open ports.
            OpenPorts[networkConnection.Source].Add(networkConnection.SrcPort);
            OpenPorts[networkConnection.Destination].Add(networkConnection.DestPort);
        }

        public string GetNodeDataJson(string ipAddress)
        {
            return JsonConvert.SerializeObject(new NetworkNode()
            {
                IpAddress = ipAddress,
                OpenPorts = this.OpenPorts[ipAddress],
                DnsMappings = this.DnsMappings.Where(d => d.Destination == ipAddress)
                                              .Select(d => d.Query)
                                              .ToHashSet()
            });
        }
    }

    public static class Extensions
    {
        public static HashSet<T> ToHashSet<T>(
            this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
    }

}
