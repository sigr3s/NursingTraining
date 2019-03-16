
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace NT.Graph
{
    public class VariableTreeViewItem : TreeViewItem{
		public Type variableType;		
        public Type dataType;
        public enum VariableNodeType
        {
            GET,
            SET
        }

        public VariableNodeType variableNodeType;
        public string vairbaleKey;

        public string variablePath = "";
	}

    public class VariableTreeView : NTTree<VariableTreeViewItem>
    {
        public VariableTreeView(TreeViewState state, List<TreeViewItem> items) : base(state, items)
        {
        }
    }
}