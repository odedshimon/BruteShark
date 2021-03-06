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
    public partial class FilesUserControl : UserControl
    {
        private GenericTableUserControl _filesTableUserControl;
        private FilePreviewUserControl _filePreviewUserControl;
        public int FilesCount => _filesTableUserControl.ItemsCount;
        public HashSet<PcapAnalyzer.NetworkFile> Files => _filesTableUserControl.ItemsHashSet.Cast<PcapAnalyzer.NetworkFile>().ToHashSet();

        public FilesUserControl()
        {
            InitializeComponent();

            this._filesTableUserControl = new GenericTableUserControl();
            _filesTableUserControl.Dock = DockStyle.Fill;
            _filesTableUserControl.SetTableDataType(typeof(PcapAnalyzer.NetworkFile));
            _filesTableUserControl.SelectionChanged += OnSelectionChanged;
            this.mainSplitContainer.Panel1.Controls.Clear();
            this.mainSplitContainer.Panel1.Controls.Add(_filesTableUserControl);
            _filePreviewUserControl = new FilePreviewUserControl();
            _filePreviewUserControl.Dock = DockStyle.Fill;
            this.mainSplitContainer.Panel2.Controls.Add(_filePreviewUserControl);
        }

        // TODO: use PL object
        public void AddFile(PcapAnalyzer.NetworkFile networkFile)
        {
            _filesTableUserControl.AddDataToTable(networkFile);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            var selectedFile = _filesTableUserControl.SelectedRowBoundItem as PcapAnalyzer.NetworkFile;

            if (selectedFile != null)
            {
                _filePreviewUserControl.PreviewFile(selectedFile.FileData, selectedFile.Extention);
            }
        }

    }
}
