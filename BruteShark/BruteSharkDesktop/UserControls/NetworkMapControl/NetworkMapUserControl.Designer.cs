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
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("hhhhh", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.nodeTreeView = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nodeTreeView
            // 
            this.nodeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeTreeView.Location = new System.Drawing.Point(0, 0);
            this.nodeTreeView.Name = "nodeTreeView";
            treeNode1.Name = "Node0";
            treeNode1.Text = "bbbb";
            treeNode2.Name = "Node0";
            treeNode2.Text = "hhhhh";
            this.nodeTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.nodeTreeView.Size = new System.Drawing.Size(220, 383);
            this.nodeTreeView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nodeTreeView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(580, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 383);
            this.panel1.TabIndex = 1;
            // 
            // NetworkMapUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "NetworkMapUserControl";
            this.Size = new System.Drawing.Size(800, 383);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView nodeTreeView;
        private System.Windows.Forms.Panel panel1;
    }
}
