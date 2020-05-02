using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BruteSharkDesktop
{
    public partial class NetworkMapUserControl : UserControl
    {
        private HashSet<NetworkMapEdge> _edges;
        Microsoft.Msagl.GraphViewerGdi.GViewer _viewer;
        Microsoft.Msagl.Drawing.Graph _graph;

        public int NodesCount => _graph.Nodes.Count();

        public NetworkMapUserControl()
        {
            InitializeComponent();

            // Add MSAGL Graph control.
            _edges = new HashSet<NetworkMapEdge>();
            _viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            _graph = new Microsoft.Msagl.Drawing.Graph("graph");
            _viewer.Graph = _graph;
            _viewer.Dock = DockStyle.Fill;
            this.Controls.Add(_viewer);
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

            _graph.FindNode(source).Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
            _graph.FindNode(destination).Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;

            _viewer.Graph = _graph;
            this.ResumeLayout();
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


        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

    }
}
