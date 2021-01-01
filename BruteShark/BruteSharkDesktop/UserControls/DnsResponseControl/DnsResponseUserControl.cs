using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using PcapAnalyzer;

namespace BruteSharkDesktop
{
    public partial class DnsResponseUserControl : UserControl
    {
        private BindingSource _queriesBindingSource;
        public int AnswerCount => this.queriesDataGridView.RowCount;

        public DnsResponseUserControl()
        {
            InitializeComponent();

            // Initialize the answers gridview.
            _queriesBindingSource = new BindingSource();
            this.queriesDataGridView.DataSource = _queriesBindingSource;
            this.queriesDataGridView.AllowUserToAddRows = false;
        }

        internal void AddNameMapping(DnsNameMapping mapping)
        {
            this.SuspendLayout();

            _queriesBindingSource.Add(mapping);

            this.ResumeLayout();
        }
    }
}