using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace BruteSharkDesktop
{
    // Originally taken from https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
    // Did some customizations for BruteShark needs.
    public static class JsonTreeViewLoader
    {
		public static void LoadJsonToTreeView(this TreeView treeView, string json, string rootNodeText)
		{
            var root = JToken.Parse(json);
            DisplayTreeView(treeView, root, rootNodeText);
        }

        private static void DisplayTreeView(TreeView treeView, JToken root, string rootName)
        {
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                var tNode = treeView.Nodes[treeView.Nodes.Add(new TreeNode(rootName))];
                tNode.Tag = root;

                AddNode(root, tNode);

                treeView.ExpandAll();
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        private static void AddNode(JToken token, TreeNode inTreeNode)
        {
            if (token == null)
                return;
            if (token is JValue)
            {
                var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                childNode.Tag = token;
            }
            else if (token is JObject jObject)
            {
                foreach (var property in jObject.Properties())
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    childNode.Tag = property;
                    AddNode(property.Value, childNode);
                }
            }
            else if (token is JArray jArray)
            {
                foreach (JValue jv in jArray)
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(jv.ToString()))];
                }
            }
            else
            {
                // TODO: log
                // Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }


    }
}
