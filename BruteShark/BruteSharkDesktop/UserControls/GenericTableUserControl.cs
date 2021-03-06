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
    public partial class GenericTableUserControl : UserControl
    {
        public event EventHandler SelectionChanged
        {
            add { this.mainDataGridView.SelectionChanged += value; }
            remove { this.mainDataGridView.SelectionChanged += value; }
        }

        private HashSet<object> _dataHashSet;
        private BindingSource _dataGridViewBindingSource;

        public HashSet<object> ItemsHashSet
        {
            get
            {
                return _dataHashSet;
            }
            private set { }
        }

        public IEnumerable<object> Items
        {
            get
            {
                return _dataHashSet;
            }
            private set { }
        }

        public object SelectedRowBoundItem
        {
            get
            {
                return this.mainDataGridView.SelectedRows.Count > 0 ? this.mainDataGridView.SelectedRows[0].DataBoundItem : null;
            }
        }

        public int ItemsCount => _dataHashSet.Count;


        public GenericTableUserControl()
        {
            InitializeComponent();
            _dataHashSet = new HashSet<object>();
            _dataGridViewBindingSource = new BindingSource();
            this.mainDataGridView.DataSource = _dataGridViewBindingSource;
            this.mainDataGridView.AutoGenerateColumns = true;
            this.mainDataGridView.AllowUserToAddRows = false;
        }

        public GenericTableUserControl(IEnumerable<object> data) : this()
        {
            FillDataGridView(data);
        }

        public void FillDataGridView(IEnumerable<object> data)
        {
            // NOTE: BindingSource is usefull for cases the data is collection of derived types.
            _dataGridViewBindingSource.DataSource = data;

            // Resize the DataGridView columns to fit the newly loaded content.
            this.mainDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        public void SetTableDataType(Type objectDesiredType)
        {
            _dataGridViewBindingSource.DataSource = objectDesiredType;
        }

        public void AddDataToTable(object row)
        {
            if (_dataHashSet.Add(row))
            {
                this.SuspendLayout();

                _dataGridViewBindingSource.Add(row);

                this.ResumeLayout();
            }
        }
    }
}
