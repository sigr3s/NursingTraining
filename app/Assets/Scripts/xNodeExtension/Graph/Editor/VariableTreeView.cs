
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace NT.Graph
{
    public class VariableTreeViewItem : TreeViewItem{
		public Type variableType;
        public enum VariableNodeType
        {
            GET,
            SET
        }

        public VariableNodeType variableNodeType;
        public string vairbaleKey;
	}

    public class VariableTreeView : NTTree<VariableTreeViewItem>
    {
        public VariableTreeView(TreeViewState state, List<TreeViewItem> items) : base(state, items)
        {
        }
    }
}