using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonUi
{
    public class NetworkContext
    {
        private enum SessionsType
        {
            TCP,
            UDP
        };

        public Dictionary<string, HashSet<int>> OpenPorts { get; private set; }
        public HashSet<PcapAnalyzer.DnsNameMapping> DnsMappings { get; private set; }
        public HashSet<PcapAnalyzer.NetworkConnection> Connections { get; private set; }
        public HashSet<PcapProcessor.INetworkSession<PcapProcessor.NetworkPacket>> NetworkSessions { get; private set; }

        public NetworkContext()
        {
            OpenPorts = new Dictionary<string, HashSet<int>>();
            DnsMappings = new HashSet<PcapAnalyzer.DnsNameMapping>();
            Connections = new HashSet<PcapAnalyzer.NetworkConnection>();
            NetworkSessions = new HashSet<PcapProcessor.INetworkSession<PcapProcessor.NetworkPacket>>();
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

        private NetworkNode GetNode(string ipAddress)
        {
            var tcpSessionsCount = 0;
            var udpSessionsCount = 0;
            var sentData = 0;
            var receivedData = 0;

            // We iterate all the session once and calculate various things at 
            // once (sessions count, data sent etc..)
            foreach (var session in this.NetworkSessions
                .Where(s => s.SourceIp == ipAddress || s.DestinationIp == ipAddress))
            {
                if (session.Protocol == "TCP")
                    tcpSessionsCount++;
                else if (session.Protocol == "UDP")
                    udpSessionsCount++;

                if (session.SourceIp == ipAddress)
                {
                    sentData += session.SentData;
                    receivedData += session.ReceivedData;
                }
                else
                {
                    sentData += session.ReceivedData;
                    receivedData += session.SentData;
                }
            }

            return new NetworkNode()
            {
                IpAddress = ipAddress,
                OpenPorts = this.OpenPorts[ipAddress],
                TcpSessionsCount = tcpSessionsCount,
                UdpStreamsCount = udpSessionsCount,
                DnsMappings = GetNodeDnsMappings(ipAddress),
                SentData = sentData,
                ReceiveData = receivedData
            };
        }

        public string GetNodeDataJson(string ipAddress)
        {
            return JsonConvert.SerializeObject(GetNode(ipAddress));
        }

        private HashSet<string> GetNodeDnsMappings(string ipAddress)
        {
            return this.DnsMappings
                       .Where(d => d.Destination == ipAddress)
                       .Select(d => d.Query)
                       .ToHashSet();
        }

        public List<NetworkNode> GetAllNodes()
        {
            return OpenPorts.Keys.Select(n => GetNode(n)).ToList();
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
