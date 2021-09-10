using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace BruteSharkDesktop
{
    public static class JsonTreeViewLoader
    {
		public static void LoadJsonToTreeView(this TreeView treeView, string json, string rootNodeText)
		{
            var root = JToken.Parse(json);
            //DisplayTreeView(treeView, root, ((JObject)root).Properties().Select(p => p.Name).FirstOrDefault());
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
            catch (Exception ex)
            {

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
            else if (token is JObject)
            {
                var obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    childNode.Tag = property;
                    AddNode(property.Value, childNode);
                }
            }
            else if (token is JArray)
            {
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    childNode.Tag = array[i];
                    AddNode(array[i], childNode);
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
