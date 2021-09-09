using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BruteSharkDesktop
{
    public static class Utilities
    {
        public static void ShowInfoMessageBox(string text)
        {
            // NOTE: Info message box is also set up at front of the form, it solves the 
            // problem of message box that is hidden under the form.
            MessageBox.Show(
                text: text,
                caption: "Info",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Information,
                defaultButton: MessageBoxDefaultButton.Button1,
                options: MessageBoxOptions.DefaultDesktopOnly);
        }

        public static IEnumerable<TreeNode> IterateAllNodes(TreeNodeCollection nodes)
        {
            // Recursively iterate over all nodes and sub nodes.
            foreach (TreeNode node in nodes)
            {
                yield return node;

                foreach (var child in IterateAllNodes(node.Nodes))
                    yield return child;
            }
        }

    }
}
