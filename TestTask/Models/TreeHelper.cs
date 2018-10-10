using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTask.Models
{
    public static class TaskTreeHelper
    {
        public static void MakeTreeValidForJsonFormat(Tree<Models.TaskWithSubtreePath> tree)
        {
            if (tree.HeadNode.SubNodes == null) return;

            foreach(var node in tree.HeadNode.SubNodes)
            {
                deleteParentNodes(node);
            }
        }

        private static void deleteParentNodes(Node<Models.TaskWithSubtreePath> node)
        {
            node.ParentNode = null;

            if (node.SubNodes == null) return;

            foreach(var n in node.SubNodes)
            {
                deleteParentNodes(n);
            }
        }
    }
}
