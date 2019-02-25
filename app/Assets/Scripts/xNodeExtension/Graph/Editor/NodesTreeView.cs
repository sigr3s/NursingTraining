using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NT.Graph
{
    public class NodeTreeViewItem : TreeViewItem{
		public Type nodeType;
	}

    public class NodesTreeView : NTTree<NodeTreeViewItem>
	{
		public NodesTreeView(TreeViewState treeViewState, List<TreeViewItem> items)
			: base(treeViewState, items)
		{
		}
	}
}