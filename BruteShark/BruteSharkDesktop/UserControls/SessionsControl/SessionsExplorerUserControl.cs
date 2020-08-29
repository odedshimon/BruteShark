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
    public partial class SessionsExplorerUserControl : UserControl
    {
        private SessionViewerUserControl _sessionViewerUserControl;
        private BindingSource _sessionsBindingSource;

        public int SessionsCount => this.sessionsDataGridView.RowCount;

        public SessionsExplorerUserControl()
        {
            InitializeComponent();

            // Initialize the sessions gridview.
            _sessionsBindingSource = new BindingSource();
            this.sessionsDataGridView.DataSource = _sessionsBindingSource;
            this.sessionsDataGridView.SelectionChanged += OnSessionsDataGridViewSelectionChanged;
            this.sessionsDataGridView.AllowUserToAddRows = false;

            // Add the Session Viewer control.
            _sessionViewerUserControl = new SessionViewerUserControl();
            _sessionViewerUserControl.Dock = DockStyle.Fill;
            this.bottomSplitContainer.Panel1.Controls.Add(_sessionViewerUserControl);

            // Initialize the filter columns Combo Box.
            InitializeColumnsToComboBox();
        }

        private void InitializeColumnsToComboBox()
        {
            // TODO: use reflection to get only the properties with a Browsable attribute, than 
            // extract their DisplayName attribute value.
            string[] columns = {"Source Ip", "Destination Ip", "Source Port", "Destination Port" };

            foreach (var column in columns)
            {
                this.columnsComboBox.Items.Add(column);
            }
        }

        private void OnSessionsDataGridViewSelectionChanged(object sender, EventArgs e)
        {
            var session = (this.sessionsDataGridView.SelectedRows.Count > 0 ? 
                              this.sessionsDataGridView.SelectedRows[0].DataBoundItem : null)
                              as TransportLayerSession;

            if (session != null)
            {
                _sessionViewerUserControl.SetSessionView(session);
            }
        }

        public void AddSession(TransportLayerSession session)
        {
            this.SuspendLayout();

            _sessionsBindingSource.Add(session);

            this.ResumeLayout();
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            try
            {
                var columnName = this.columnsComboBox.SelectedItem.ToString().Replace(" ", "");
                var wantedValue = this.filterTextBox.Text;

                // Get all the rows that do not match the user's filter.
                DataGridViewRow[] rows = this.sessionsDataGridView.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => !(r.Cells[columnName].Value != null && r.Cells[columnName].Value.ToString().Equals(wantedValue)))
                    .ToArray();

                // Since the sessions gridview is binded to data source we have to suspend the binding on all the rows 
                // in the currency manager before setting the irelvant rows invisible.
                CurrencyManager currencyManager = (CurrencyManager)BindingContext[this.sessionsDataGridView.DataSource];
                currencyManager.SuspendBinding();

                foreach (var row in rows)
                {
                    row.Visible = false;
                }

                currencyManager.ResumeBinding();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to filter the sessions. Message {ex.Message}");
            }
        }

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.sessionsDataGridView.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
