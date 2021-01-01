namespace BruteSharkDesktop
{
    partial class DnsResponseUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.queriesDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.queriesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // queriesDataGridView
            // 
            this.queriesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.queriesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queriesDataGridView.Location = new System.Drawing.Point(0, 0);
            this.queriesDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.queriesDataGridView.Name = "queriesDataGridView";
            this.queriesDataGridView.Size = new System.Drawing.Size(461, 407);
            this.queriesDataGridView.TabIndex = 0;
            // 
            // DnsResponseUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.queriesDataGridView);
            this.Name = "DnsResponseUserControl";
            this.Size = new System.Drawing.Size(461, 407);
            ((System.ComponentModel.ISupportInitialize)(this.queriesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView queriesDataGridView;
    }
}
