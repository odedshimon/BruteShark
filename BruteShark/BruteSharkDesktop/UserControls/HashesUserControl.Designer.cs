namespace BruteSharkDesktop
{
    partial class HashesUserControl
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
            this.hashViewSplitContainer = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.hashDataRichTextBox = new System.Windows.Forms.RichTextBox();
            this.hashesComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.createHashcatFileButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.choseDirectoryButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.selectedFolderTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hashViewSplitContainer)).BeginInit();
            this.hashViewSplitContainer.Panel1.SuspendLayout();
            this.hashViewSplitContainer.Panel2.SuspendLayout();
            this.hashViewSplitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.hashViewSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(595, 444);
            this.mainSplitContainer.SplitterDistance = 257;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // hashViewSplitContainer
            // 
            this.hashViewSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hashViewSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.hashViewSplitContainer.Name = "hashViewSplitContainer";
            // 
            // hashViewSplitContainer.Panel1
            // 
            this.hashViewSplitContainer.Panel1.Controls.Add(this.label1);
            this.hashViewSplitContainer.Panel1.Controls.Add(this.hashDataRichTextBox);
            // 
            // hashViewSplitContainer.Panel2
            // 
            this.hashViewSplitContainer.Panel2.Controls.Add(this.panel1);
            this.hashViewSplitContainer.Panel2.Controls.Add(this.label2);
            this.hashViewSplitContainer.Size = new System.Drawing.Size(595, 183);
            this.hashViewSplitContainer.SplitterDistance = 347;
            this.hashViewSplitContainer.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Full Hash Data";
            // 
            // hashDataRichTextBox
            // 
            this.hashDataRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashDataRichTextBox.Location = new System.Drawing.Point(3, 16);
            this.hashDataRichTextBox.Name = "hashDataRichTextBox";
            this.hashDataRichTextBox.Size = new System.Drawing.Size(341, 164);
            this.hashDataRichTextBox.TabIndex = 0;
            this.hashDataRichTextBox.Text = "";
            this.hashDataRichTextBox.WordWrap = false;
            // 
            // hashesComboBox
            // 
            this.hashesComboBox.FormattingEnabled = true;
            this.hashesComboBox.Location = new System.Drawing.Point(104, 11);
            this.hashesComboBox.Name = "hashesComboBox";
            this.hashesComboBox.Size = new System.Drawing.Size(101, 21);
            this.hashesComboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Brute Force";
            // 
            // createHashcatFileButton
            // 
            this.createHashcatFileButton.Location = new System.Drawing.Point(16, 72);
            this.createHashcatFileButton.Name = "createHashcatFileButton";
            this.createHashcatFileButton.Size = new System.Drawing.Size(189, 23);
            this.createHashcatFileButton.TabIndex = 0;
            this.createHashcatFileButton.Text = "Create Hashcat Hashes file";
            this.createHashcatFileButton.UseVisualStyleBackColor = true;
            this.createHashcatFileButton.Click += new System.EventHandler(this.createHashcatFileButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.selectedFolderTextBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.choseDirectoryButton);
            this.panel1.Controls.Add(this.createHashcatFileButton);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.hashesComboBox);
            this.panel1.Location = new System.Drawing.Point(6, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 107);
            this.panel1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Select hash type:";
            // 
            // choseDirectoryButton
            // 
            this.choseDirectoryButton.Location = new System.Drawing.Point(104, 45);
            this.choseDirectoryButton.Name = "choseDirectoryButton";
            this.choseDirectoryButton.Size = new System.Drawing.Size(27, 20);
            this.choseDirectoryButton.TabIndex = 5;
            this.choseDirectoryButton.Text = "...";
            this.choseDirectoryButton.UseVisualStyleBackColor = true;
            this.choseDirectoryButton.Click += new System.EventHandler(this.choseDirectoryButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Select directory:";
            // 
            // selectedFolderTextBox
            // 
            this.selectedFolderTextBox.Location = new System.Drawing.Point(137, 45);
            this.selectedFolderTextBox.Name = "selectedFolderTextBox";
            this.selectedFolderTextBox.ReadOnly = true;
            this.selectedFolderTextBox.Size = new System.Drawing.Size(68, 20);
            this.selectedFolderTextBox.TabIndex = 7;
            // 
            // HashesUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Name = "HashesUserControl";
            this.Size = new System.Drawing.Size(595, 444);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.hashViewSplitContainer.Panel1.ResumeLayout(false);
            this.hashViewSplitContainer.Panel1.PerformLayout();
            this.hashViewSplitContainer.Panel2.ResumeLayout(false);
            this.hashViewSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hashViewSplitContainer)).EndInit();
            this.hashViewSplitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer hashViewSplitContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox hashDataRichTextBox;
        private System.Windows.Forms.ComboBox hashesComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button createHashcatFileButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button choseDirectoryButton;
        private System.Windows.Forms.TextBox selectedFolderTextBox;
    }
}
