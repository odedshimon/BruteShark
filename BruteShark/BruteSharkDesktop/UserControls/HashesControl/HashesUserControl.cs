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
using System.IO;

namespace BruteSharkDesktop
{
    public partial class HashesUserControl : UserControl
    {
        private GenericTableUserControl _hashesTableUserControl; 

        public int HashesCount => _hashesTableUserControl.ItemsCount;

        public HashesUserControl()
        {
            InitializeComponent();

            this._hashesTableUserControl = new GenericTableUserControl();
            _hashesTableUserControl.Dock = DockStyle.Fill;
            _hashesTableUserControl.SetTableDataType(typeof(PcapAnalyzer.NetworkHash));
            _hashesTableUserControl.SelectionChanged += OnSelectionChanged;
            this.mainSplitContainer.Panel1.Controls.Clear();
            this.mainSplitContainer.Panel1.Controls.Add(_hashesTableUserControl);
        }

        // TODO: use PL object
        public void AddHash(PcapAnalyzer.NetworkHash networkHash)
        {
            _hashesTableUserControl.AddDataToTable(networkHash);

            if (!this.hashesComboBox.Items.Contains(networkHash.HashType))
            {
                this.hashesComboBox.Items.Add(networkHash.HashType);
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            var hash = _hashesTableUserControl.SelectedRowBoundItem;

            if (hash != null)
            {
                this.hashDataRichTextBox.Clear();

                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(hash))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(hash);
                    this.hashDataRichTextBox.Text += ($"{name} = {value}{Environment.NewLine}");
                }
            }
        }

        private void CreateHashcatFileButton_Click(object sender, EventArgs e)
        {
            var selectedHashType = this.hashesComboBox.SelectedItem;

            if (_hashesTableUserControl.ItemsCount == 0)
            {
                MessageBox.Show("No hashes found");
                return;
            }
            if (selectedHashType == null || selectedHashType.ToString() == string.Empty)
            {
                MessageBox.Show("No hash type selected");
                return;
            }

            try
            {
                var hashesToExport = _hashesTableUserControl.Items
                                        .Where(h => (h as PcapAnalyzer.NetworkHash).HashType == selectedHashType.ToString())
                                        .Select(h =>
                                            BruteForce.Utilities.ConvertToHashcatFormat(
                                                CommonUi.Casting.CastAnalyzerHashToBruteForceHash(h as PcapAnalyzer.NetworkHash)));

                var outputFilePath = CommonUi.Exporting.GetUniqueFilePath(Path.Combine(
                                        this.selectedFolderTextBox.Text, 
                                        $"Brute Shark - {selectedHashType} Hashcat Export.txt"));

                using (var streamWriter = new StreamWriter(outputFilePath, true))
                {
                    foreach (var hash in hashesToExport)
                    {
                        streamWriter.WriteLine(hash);
                    }
                }

                MessageBox.Show($"Hashes exported: {outputFilePath}");
            }
            catch (BruteForce.NotSupportedHashcatHash ex)
            {
                MessageBox.Show($"Hashcat does not support this hash type: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export hashes: {ex.Message}");
            }
        }

        private void ChoseDirectoryButton_Click(object sender, EventArgs e)
        {
            var selecetDirectoryDialog = new FolderBrowserDialog();

            if (selecetDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                this.selectedFolderTextBox.Text = selecetDirectoryDialog.SelectedPath;
            }
        }
        
    }
}
