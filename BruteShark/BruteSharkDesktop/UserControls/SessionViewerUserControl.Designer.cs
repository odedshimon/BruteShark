namespace BruteSharkDesktop
{
    partial class SessionViewerUserControl
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
            this.sessionDataRichTextBox = new System.Windows.Forms.RichTextBox();
            this.sessionDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.sourceIpLabel = new System.Windows.Forms.Label();
            this.destinationIpLabel = new System.Windows.Forms.Label();
            this.destinationPortLabel = new System.Windows.Forms.Label();
            this.sourcePortLabel = new System.Windows.Forms.Label();
            this.dataLengthLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.sessionDetailsGroupBox.SuspendLayout();
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
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.sessionDataRichTextBox);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.sessionDetailsGroupBox);
            this.mainSplitContainer.Size = new System.Drawing.Size(668, 348);
            this.mainSplitContainer.SplitterDistance = 258;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // sessionDataRichTextBox
            // 
            this.sessionDataRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionDataRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.sessionDataRichTextBox.Name = "sessionDataRichTextBox";
            this.sessionDataRichTextBox.Size = new System.Drawing.Size(668, 258);
            this.sessionDataRichTextBox.TabIndex = 1;
            this.sessionDataRichTextBox.Text = "";
            // 
            // sessionDetailsGroupBox
            // 
            this.sessionDetailsGroupBox.Controls.Add(this.dataLengthLabel);
            this.sessionDetailsGroupBox.Controls.Add(this.destinationPortLabel);
            this.sessionDetailsGroupBox.Controls.Add(this.sourcePortLabel);
            this.sessionDetailsGroupBox.Controls.Add(this.destinationIpLabel);
            this.sessionDetailsGroupBox.Controls.Add(this.sourceIpLabel);
            this.sessionDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.sessionDetailsGroupBox.Name = "sessionDetailsGroupBox";
            this.sessionDetailsGroupBox.Size = new System.Drawing.Size(668, 86);
            this.sessionDetailsGroupBox.TabIndex = 0;
            this.sessionDetailsGroupBox.TabStop = false;
            this.sessionDetailsGroupBox.Text = "Session Details";
            // 
            // sourceIpLabel
            // 
            this.sourceIpLabel.AutoSize = true;
            this.sourceIpLabel.Location = new System.Drawing.Point(24, 29);
            this.sourceIpLabel.Name = "sourceIpLabel";
            this.sourceIpLabel.Size = new System.Drawing.Size(60, 13);
            this.sourceIpLabel.TabIndex = 0;
            this.sourceIpLabel.Text = "Source IP: ";
            // 
            // destinationIpLabel
            // 
            this.destinationIpLabel.AutoSize = true;
            this.destinationIpLabel.Location = new System.Drawing.Point(25, 53);
            this.destinationIpLabel.Name = "destinationIpLabel";
            this.destinationIpLabel.Size = new System.Drawing.Size(79, 13);
            this.destinationIpLabel.TabIndex = 1;
            this.destinationIpLabel.Text = "Destination IP: ";
            // 
            // destinationPortLabel
            // 
            this.destinationPortLabel.AutoSize = true;
            this.destinationPortLabel.Location = new System.Drawing.Point(191, 53);
            this.destinationPortLabel.Name = "destinationPortLabel";
            this.destinationPortLabel.Size = new System.Drawing.Size(88, 13);
            this.destinationPortLabel.TabIndex = 3;
            this.destinationPortLabel.Text = "Destination Port: ";
            // 
            // sourcePortLabel
            // 
            this.sourcePortLabel.AutoSize = true;
            this.sourcePortLabel.Location = new System.Drawing.Point(190, 29);
            this.sourcePortLabel.Name = "sourcePortLabel";
            this.sourcePortLabel.Size = new System.Drawing.Size(69, 13);
            this.sourcePortLabel.TabIndex = 2;
            this.sourcePortLabel.Text = "Source Port: ";
            // 
            // dataLengthLabel
            // 
            this.dataLengthLabel.AutoSize = true;
            this.dataLengthLabel.Location = new System.Drawing.Point(362, 29);
            this.dataLengthLabel.Name = "dataLengthLabel";
            this.dataLengthLabel.Size = new System.Drawing.Size(110, 13);
            this.dataLengthLabel.TabIndex = 4;
            this.dataLengthLabel.Text = "Data Length (Bytes):  ";
            // 
            // SessionViewerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Name = "SessionViewerUserControl";
            this.Size = new System.Drawing.Size(668, 348);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.sessionDetailsGroupBox.ResumeLayout(false);
            this.sessionDetailsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.RichTextBox sessionDataRichTextBox;
        private System.Windows.Forms.GroupBox sessionDetailsGroupBox;
        private System.Windows.Forms.Label destinationPortLabel;
        private System.Windows.Forms.Label sourcePortLabel;
        private System.Windows.Forms.Label destinationIpLabel;
        private System.Windows.Forms.Label sourceIpLabel;
        private System.Windows.Forms.Label dataLengthLabel;
    }
}
