namespace BruteSharkDesktop
{
    partial class NetworkMapUserControl
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("bbbb");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("hhh", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.nodeDetailsTreeView = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.nodeDetailsTreeView);
            this.mainSplitContainer.Size = new System.Drawing.Size(800, 383);
            this.mainSplitContainer.SplitterDistance = 525;
            this.mainSplitContainer.TabIndex = 0;
            this.mainSplitContainer.Text = "splitContainer1";
            // 
            // nodeDetailsTreeView
            // 
            this.nodeDetailsTreeView.Location = new System.Drawing.Point(30, 65);
            this.nodeDetailsTreeView.Name = "nodeDetailsTreeView";
            treeNode1.Name = "Node0";
            treeNode1.Text = "bbbb";
            treeNode2.Name = "Node0";
            treeNode2.Text = "hhh";
            this.nodeDetailsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.nodeDetailsTreeView.Size = new System.Drawing.Size(200, 218);
            this.nodeDetailsTreeView.TabIndex = 0;
            // 
            // NetworkMapUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "NetworkMapUserControl";
            this.Size = new System.Drawing.Size(800, 383);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.TreeView nodeDetailsTreeView;
    }
}
