namespace BruteSharkDesktop
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Passwords");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Hashes");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Credentials", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Network Map");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Tcp Sessions");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Network", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.secondaryUpperSplitContainer = new System.Windows.Forms.SplitContainer();
            this.runButton = new System.Windows.Forms.Button();
            this.removeFilesButton = new System.Windows.Forms.Button();
            this.addFilesButton = new System.Windows.Forms.Button();
            this.filesListView = new System.Windows.Forms.ListView();
            this.fileNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.secondaryLowerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.modulesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.modulesTreeView = new System.Windows.Forms.TreeView();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondaryUpperSplitContainer)).BeginInit();
            this.secondaryUpperSplitContainer.Panel1.SuspendLayout();
            this.secondaryUpperSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondaryLowerSplitContainer)).BeginInit();
            this.secondaryLowerSplitContainer.Panel1.SuspendLayout();
            this.secondaryLowerSplitContainer.Panel2.SuspendLayout();
            this.secondaryLowerSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modulesSplitContainer)).BeginInit();
            this.modulesSplitContainer.Panel1.SuspendLayout();
            this.modulesSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.secondaryUpperSplitContainer);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.secondaryLowerSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(800, 590);
            this.mainSplitContainer.SplitterDistance = 172;
            this.mainSplitContainer.SplitterWidth = 1;
            this.mainSplitContainer.TabIndex = 0;
            // 
            // secondaryUpperSplitContainer
            // 
            this.secondaryUpperSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.secondaryUpperSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.secondaryUpperSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.secondaryUpperSplitContainer.Name = "secondaryUpperSplitContainer";
            // 
            // secondaryUpperSplitContainer.Panel1
            // 
            this.secondaryUpperSplitContainer.Panel1.Controls.Add(this.runButton);
            this.secondaryUpperSplitContainer.Panel1.Controls.Add(this.removeFilesButton);
            this.secondaryUpperSplitContainer.Panel1.Controls.Add(this.addFilesButton);
            this.secondaryUpperSplitContainer.Panel1.Controls.Add(this.filesListView);
            this.secondaryUpperSplitContainer.Size = new System.Drawing.Size(796, 168);
            this.secondaryUpperSplitContainer.SplitterDistance = 416;
            this.secondaryUpperSplitContainer.SplitterWidth = 1;
            this.secondaryUpperSplitContainer.TabIndex = 0;
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Location = new System.Drawing.Point(176, 127);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(230, 34);
            this.runButton.TabIndex = 2;
            this.runButton.Text = "Run!";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // removeFilesButton
            // 
            this.removeFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeFilesButton.Location = new System.Drawing.Point(90, 127);
            this.removeFilesButton.Name = "removeFilesButton";
            this.removeFilesButton.Size = new System.Drawing.Size(80, 34);
            this.removeFilesButton.TabIndex = 1;
            this.removeFilesButton.Text = "-";
            this.removeFilesButton.UseVisualStyleBackColor = true;
            this.removeFilesButton.Click += new System.EventHandler(this.removeFilesButton_Click);
            // 
            // addFilesButton
            // 
            this.addFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addFilesButton.Location = new System.Drawing.Point(7, 127);
            this.addFilesButton.Margin = new System.Windows.Forms.Padding(0);
            this.addFilesButton.Name = "addFilesButton";
            this.addFilesButton.Size = new System.Drawing.Size(80, 34);
            this.addFilesButton.TabIndex = 0;
            this.addFilesButton.Text = "+";
            this.addFilesButton.UseVisualStyleBackColor = true;
            this.addFilesButton.Click += new System.EventHandler(this.addFilesButton_Click);
            // 
            // filesListView
            // 
            this.filesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileNameColumnHeader,
            this.sizeColumnHeader,
            this.statusColumnHeader});
            this.filesListView.FullRowSelect = true;
            this.filesListView.Location = new System.Drawing.Point(7, 7);
            this.filesListView.Margin = new System.Windows.Forms.Padding(0);
            this.filesListView.Name = "filesListView";
            this.filesListView.Size = new System.Drawing.Size(399, 114);
            this.filesListView.TabIndex = 0;
            this.filesListView.UseCompatibleStateImageBehavior = false;
            this.filesListView.View = System.Windows.Forms.View.Details;
            // 
            // fileNameColumnHeader
            // 
            this.fileNameColumnHeader.Text = "File Name";
            this.fileNameColumnHeader.Width = 194;
            // 
            // sizeColumnHeader
            // 
            this.sizeColumnHeader.Text = "Size";
            this.sizeColumnHeader.Width = 93;
            // 
            // statusColumnHeader
            // 
            this.statusColumnHeader.Text = "Status";
            this.statusColumnHeader.Width = 104;
            // 
            // secondaryLowerSplitContainer
            // 
            this.secondaryLowerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.secondaryLowerSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.secondaryLowerSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.secondaryLowerSplitContainer.Name = "secondaryLowerSplitContainer";
            this.secondaryLowerSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // secondaryLowerSplitContainer.Panel1
            // 
            this.secondaryLowerSplitContainer.Panel1.Controls.Add(this.modulesSplitContainer);
            // 
            // secondaryLowerSplitContainer.Panel2
            // 
            this.secondaryLowerSplitContainer.Panel2.Controls.Add(this.progressBar);
            this.secondaryLowerSplitContainer.Size = new System.Drawing.Size(796, 413);
            this.secondaryLowerSplitContainer.SplitterDistance = 384;
            this.secondaryLowerSplitContainer.TabIndex = 0;
            // 
            // modulesSplitContainer
            // 
            this.modulesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modulesSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.modulesSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.modulesSplitContainer.Name = "modulesSplitContainer";
            // 
            // modulesSplitContainer.Panel1
            // 
            this.modulesSplitContainer.Panel1.Controls.Add(this.modulesTreeView);
            this.modulesSplitContainer.Size = new System.Drawing.Size(796, 384);
            this.modulesSplitContainer.SplitterDistance = 229;
            this.modulesSplitContainer.TabIndex = 0;
            // 
            // modulesTreeView
            // 
            this.modulesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modulesTreeView.Location = new System.Drawing.Point(0, 0);
            this.modulesTreeView.Name = "modulesTreeView";
            treeNode1.Name = "PasswordsNode";
            treeNode1.Text = "Passwords";
            treeNode2.Name = "HashesNode";
            treeNode2.Text = "Hashes";
            treeNode3.Name = "CredentialsNode";
            treeNode3.Text = "Credentials";
            treeNode4.Name = "NetworkMapNode";
            treeNode4.Text = "Network Map";
            treeNode5.Name = "SessionsNode";
            treeNode5.Text = "Tcp Sessions";
            treeNode6.Name = "NetworkNode";
            treeNode6.Text = "Network";
            this.modulesTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode6});
            this.modulesTreeView.Size = new System.Drawing.Size(229, 384);
            this.modulesTreeView.TabIndex = 0;
            this.modulesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.modulesTreeView_AfterSelect);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(796, 25);
            this.progressBar.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 590);
            this.Controls.Add(this.mainSplitContainer);
            this.Name = "MainForm";
            this.Text = "Brute Shark";
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.secondaryUpperSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.secondaryUpperSplitContainer)).EndInit();
            this.secondaryUpperSplitContainer.ResumeLayout(false);
            this.secondaryLowerSplitContainer.Panel1.ResumeLayout(false);
            this.secondaryLowerSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.secondaryLowerSplitContainer)).EndInit();
            this.secondaryLowerSplitContainer.ResumeLayout(false);
            this.modulesSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.modulesSplitContainer)).EndInit();
            this.modulesSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer secondaryUpperSplitContainer;
        private System.Windows.Forms.ListView filesListView;
        private System.Windows.Forms.Button addFilesButton;
        private System.Windows.Forms.ColumnHeader fileNameColumnHeader;
        private System.Windows.Forms.ColumnHeader statusColumnHeader;
        private System.Windows.Forms.ColumnHeader sizeColumnHeader;
        private System.Windows.Forms.SplitContainer secondaryLowerSplitContainer;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button removeFilesButton;
        private System.Windows.Forms.SplitContainer modulesSplitContainer;
        private System.Windows.Forms.TreeView modulesTreeView;
        private System.Windows.Forms.Button runButton;
    }
}

