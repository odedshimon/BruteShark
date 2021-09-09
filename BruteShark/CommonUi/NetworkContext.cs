using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUi
{
    public class NetworkContext
    {
        private Dictionary<string, NetworkNode> _networkNodes;
        private HashSet<PcapAnalyzer.DnsNameMapping> _dnsMappings;
        private HashSet<PcapAnalyzer.NetworkConnection> _connections;

        public NetworkContext()
        {
            _networkNodes = new Dictionary<string, NetworkNode>();
            _dnsMappings = new HashSet<PcapAnalyzer.DnsNameMapping>();
            _connections = new HashSet<PcapAnalyzer.NetworkConnection>();
        }

        public void HandleDnsNameMapping(PcapAnalyzer.DnsNameMapping dnsNameMapping)
        {
            _dnsMappings.Add(dnsNameMapping);
        }

        public void HandleNetworkConection(PcapAnalyzer.NetworkConnection networkConnection)
        {
            // Create network nodes if needed.
            if (_connections.Add(networkConnection))
            {
                if (!_networkNodes.ContainsKey(networkConnection.Source))
                {
                    _networkNodes[networkConnection.Source] = new NetworkNode(networkConnection.Source);
                }
                if (!_networkNodes.ContainsKey(networkConnection.Destination))
                {
                    _networkNodes[networkConnection.Destination] = new NetworkNode(networkConnection.Destination);
                }
            }

            // Update open ports.
            _networkNodes[networkConnection.Source].OpenPorts.Add(networkConnection.SrcPort);
            _networkNodes[networkConnection.Destination].OpenPorts.Add(networkConnection.DestPort);
        }

        public NetworkNode GetNode(string ipAddress) => _networkNodes[ipAddress];

    }
}
