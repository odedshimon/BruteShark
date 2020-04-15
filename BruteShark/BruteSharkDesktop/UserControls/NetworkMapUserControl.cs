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
        Microsoft.Msagl.GraphViewerGdi.GViewer _viewer;
        Microsoft.Msagl.Drawing.Graph _graph;

        public int NodesCount => _graph.Nodes.Count();

        public NetworkMapUserControl()
        {
            InitializeComponent();

            // Add MSAGL Graph control.
            _viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            _graph = new Microsoft.Msagl.Drawing.Graph("graph");
            _viewer.Graph = _graph;
            _viewer.Dock = DockStyle.Fill;
            this.Controls.Add(_viewer);
        }

        public void AddEdge(string source, string destination, string edgeText = "")
        {
            this.SuspendLayout();
            _graph.AddEdge(source, edgeText, destination);
            _viewer.Graph = _graph;
            this.ResumeLayout();
        }
    }
}
