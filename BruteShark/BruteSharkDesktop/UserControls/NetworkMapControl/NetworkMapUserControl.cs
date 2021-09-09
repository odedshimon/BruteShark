using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PcapAnalyzer;
using System.Net;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.IO;

namespace BruteSharkDesktop
{
    public partial class NetworkMapUserControl : UserControl
    {
        private CommonUi.NetworkContext _networkContext;
        private Dictionary<string, HashSet<string>> _dnsMappings;
        private HashSet<NetworkMapEdge> _edges;
        Microsoft.Msagl.GraphViewerGdi.GViewer _viewer;
        Microsoft.Msagl.Drawing.Graph _graph;

        public int NodesCount => _graph.Nodes.Count();

        public NetworkMapUserControl(CommonUi.NetworkContext networkContext)
        {
            InitializeComponent();
            _networkContext = networkContext;

            // Add MSAGL Graph control.
            _dnsMappings = new Dictionary<string, HashSet<string>>();
            _edges = new HashSet<NetworkMapEdge>();
            _viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            _graph = new Microsoft.Msagl.Drawing.Graph("graph");
            _viewer.Graph = _graph;
            _viewer.Dock = DockStyle.Fill;
            this.Controls.Add(_viewer);

            _viewer.MouseClick += OnGraphMouseClick;
        }

        private void OnGraphMouseClick(object sender, MouseEventArgs e)
        {
            foreach (var en in _viewer.Entities)
            {
                if (en.MarkedForDragging && en is IViewerNode)
                {
                    var ipAddress = new StringReader((en as DNode).Node.LabelText).ReadLine();
                    Utilities.ShowInfoMessageBox(ipAddress);
                }
            }
        }

        public void AddEdge(string source, string destination, string edgeText = "")
        {
            this.SuspendLayout();

            // We creaete an edge object and save it in a HashTable to avoid inserting
            // double edges.
            var newEdge = new NetworkMapEdge()
            {
                Source = source,
                Destination = destination,
                Text = edgeText
            };

            if (!_edges.Contains(newEdge))
            {
                _graph.AddEdge(source, edgeText, destination);
                _edges.Add(newEdge);
            }

            var sourceNode = _graph.FindNode(source);
            var destinationNode = _graph.FindNode(destination);
            sourceNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
            sourceNode.LabelText = GetNodeText(source);
            destinationNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
            destinationNode.LabelText = GetNodeText(destination);

            _viewer.Graph = _graph;
            this.ResumeLayout();
        }

        private string GetNodeText(string ipAddress)
        {
            var res = ipAddress;

            if (_dnsMappings.ContainsKey(ipAddress))
            {
                res += Environment.NewLine + "DNS: " + _dnsMappings[ipAddress].First();

                if (_dnsMappings[ipAddress].Count > 1)
                {
                    res += $" ({_dnsMappings[ipAddress].Count} more)";
                }
            }

            return res;
        }

        public void HandleHash(PcapAnalyzer.NetworkHash hash)
        {
            // Usually the hashes username is named "User" \ "Username".
            var user = GetPropValue(hash, "User");
            var username = GetPropValue(hash, "Username");
            var displayUserName = user != null ? user : username;

            if (displayUserName != null)
            {
                var domain = GetPropValue(hash, "Domain");
                if (domain != null)
                {
                    if (domain.ToString().Length > 0)
                    {
                        displayUserName = domain.ToString() + @"\" + displayUserName;
                    }
                }

                var edgeText = $"{hash.HashType} Hash";

                AddEdge(displayUserName.ToString(), hash.Destination, edgeText);
                _graph.FindNode(displayUserName.ToString()).Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGreen;
            }
        }

        public void HandlePassword(PcapAnalyzer.NetworkPassword password)
        {
            var edgeText = $"{password.Protocol} Password";
            AddEdge(password.Username, password.Destination, edgeText);
            _graph.FindNode(password.Username).Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGreen;
        }

        // Normally DNS mappings arriving before real data, but we can't count on it therfore we 
        // are saving the mappings for future hosts.
        public void HandleDnsNameMapping(DnsNameMapping dnsNameMapping)
        {
            if (!IsIpAddress(dnsNameMapping.Query) && IsIpAddress(dnsNameMapping.Destination))
            {
                if (_dnsMappings.ContainsKey(dnsNameMapping.Destination))
                {
                    if (_dnsMappings[dnsNameMapping.Destination].Add(dnsNameMapping.Query))
                    {
                        UpdateNodeLabel(dnsNameMapping.Destination);
                    }
                }
                else
                {
                    _dnsMappings[dnsNameMapping.Destination] = new HashSet<string>();
                    _dnsMappings[dnsNameMapping.Destination].Add(dnsNameMapping.Query);
                    UpdateNodeLabel(dnsNameMapping.Destination);
                }
            }
        }

        private void UpdateNodeLabel(string ipAddress)
        {
            var node = _graph.FindNode(ipAddress);

            if (node != null)
            {
                node.LabelText = GetNodeText(ipAddress);
            }
        }

        private bool IsIpAddress(string ip)
        {
            return IPAddress.TryParse(ip, out IPAddress ipAddress);
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

    }
}
