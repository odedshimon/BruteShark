namespace BruteSharkDesktop
{
    partial class FilePreviewUserControl
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
            this.filePreviewSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filePreviewSplitContainer)).BeginInit();
            this.filePreviewSplitContainer.Panel1.SuspendLayout();
            this.filePreviewSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.filePreviewSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(509, 620);
            this.mainSplitContainer.SplitterDistance = 387;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // filePreviewSplitContainer
            // 
            this.filePreviewSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePreviewSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.filePreviewSplitContainer.Name = "filePreviewSplitContainer";
            this.filePreviewSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // filePreviewSplitContainer.Panel1
            // 
            this.filePreviewSplitContainer.Panel1.Controls.Add(this.headerLabel);
            this.filePreviewSplitContainer.Size = new System.Drawing.Size(509, 387);
            this.filePreviewSplitContainer.SplitterDistance = 35;
            this.filePreviewSplitContainer.TabIndex = 0;
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.Location = new System.Drawing.Point(3, 9);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(92, 20);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "File Preview";
            // 
            // FilePreviewUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Name = "FilePreviewUserControl";
            this.Size = new System.Drawing.Size(509, 620);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.filePreviewSplitContainer.Panel1.ResumeLayout(false);
            this.filePreviewSplitContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filePreviewSplitContainer)).EndInit();
            this.filePreviewSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer filePreviewSplitContainer;
        private System.Windows.Forms.Label headerLabel;
    }
}
