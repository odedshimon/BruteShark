namespace BruteSharkDesktop
{
    partial class SessionsExplorerUserControl
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
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.filterSessionsGroupBox = new System.Windows.Forms.GroupBox();
            this.clearFilterButton = new System.Windows.Forms.Button();
            this.filterButton = new System.Windows.Forms.Button();
            this.columnsComboBox = new System.Windows.Forms.ComboBox();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.bottomSplitContainer = new System.Windows.Forms.SplitContainer();
            this.sessionsDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.filterSessionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplitContainer)).BeginInit();
            this.bottomSplitContainer.Panel2.SuspendLayout();
            this.bottomSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sessionsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.filterSessionsGroupBox);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.bottomSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(744, 453);
            this.mainSplitContainer.SplitterDistance = 53;
            this.mainSplitContainer.SplitterWidth = 5;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // filterSessionsGroupBox
            // 
            this.filterSessionsGroupBox.Controls.Add(this.clearFilterButton);
            this.filterSessionsGroupBox.Controls.Add(this.filterButton);
            this.filterSessionsGroupBox.Controls.Add(this.columnsComboBox);
            this.filterSessionsGroupBox.Controls.Add(this.filterTextBox);
            this.filterSessionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterSessionsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.filterSessionsGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterSessionsGroupBox.Name = "filterSessionsGroupBox";
            this.filterSessionsGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterSessionsGroupBox.Size = new System.Drawing.Size(744, 53);
            this.filterSessionsGroupBox.TabIndex = 0;
            this.filterSessionsGroupBox.TabStop = false;
            this.filterSessionsGroupBox.Text = "Filter Sessions";
            // 
            // clearFilterButton
            // 
            this.clearFilterButton.Location = new System.Drawing.Point(349, 20);
            this.clearFilterButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(63, 27);
            this.clearFilterButton.TabIndex = 3;
            this.clearFilterButton.Text = "Clear";
            this.clearFilterButton.UseVisualStyleBackColor = true;
            this.clearFilterButton.Click += new System.EventHandler(this.clearFilterButton_Click);
            // 
            // filterButton
            // 
            this.filterButton.Location = new System.Drawing.Point(279, 20);
            this.filterButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(63, 27);
            this.filterButton.TabIndex = 2;
            this.filterButton.Text = "Filter";
            this.filterButton.UseVisualStyleBackColor = true;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // columnsComboBox
            // 
            this.columnsComboBox.FormattingEnabled = true;
            this.columnsComboBox.Location = new System.Drawing.Point(7, 22);
            this.columnsComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.columnsComboBox.Name = "columnsComboBox";
            this.columnsComboBox.Size = new System.Drawing.Size(140, 23);
            this.columnsComboBox.TabIndex = 1;
            // 
            // filterTextBox
            // 
            this.filterTextBox.Location = new System.Drawing.Point(155, 22);
            this.filterTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(116, 23);
            this.filterTextBox.TabIndex = 0;
            // 
            // bottomSplitContainer
            // 
            this.bottomSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.bottomSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.bottomSplitContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bottomSplitContainer.Name = "bottomSplitContainer";
            // 
            // bottomSplitContainer.Panel2
            // 
            this.bottomSplitContainer.Panel2.Controls.Add(this.sessionsDataGridView);
            this.bottomSplitContainer.Size = new System.Drawing.Size(744, 395);
            this.bottomSplitContainer.SplitterDistance = 414;
            this.bottomSplitContainer.SplitterWidth = 5;
            this.bottomSplitContainer.TabIndex = 0;
            // 
            // sessionsDataGridView
            // 
            this.sessionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sessionsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.sessionsDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sessionsDataGridView.Name = "sessionsDataGridView";
            this.sessionsDataGridView.Size = new System.Drawing.Size(325, 395);
            this.sessionsDataGridView.TabIndex = 0;
            // 
            // SessionsExplorerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SessionsExplorerUserControl";
            this.Size = new System.Drawing.Size(744, 453);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.filterSessionsGroupBox.ResumeLayout(false);
            this.filterSessionsGroupBox.PerformLayout();
            this.bottomSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplitContainer)).EndInit();
            this.bottomSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sessionsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.DataGridView sessionsDataGridView;
        private System.Windows.Forms.SplitContainer bottomSplitContainer;
        private System.Windows.Forms.GroupBox filterSessionsGroupBox;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.ComboBox columnsComboBox;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.Button clearFilterButton;
    }
}
