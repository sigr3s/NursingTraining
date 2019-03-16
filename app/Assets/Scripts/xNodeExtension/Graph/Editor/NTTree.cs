using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NT.Graph
{
    public class NTTree<T> : TreeView where T : TreeViewItem
    {
        List<TreeViewItem> items;

        public NTTree(TreeViewState state, List<TreeViewItem> items) : base(state)
        {
            this.items = items;
			Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new NodeTreeViewItem {id = 0, depth = -1, displayName = "Root"};
			SetupParentsAndChildrenFromDepths (root, items);
			return root;
        }

		protected override bool CanStartDrag(CanStartDragArgs args){
			return true;
		}


        protected override void SetupDragAndDrop(SetupDragAndDropArgs args){
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			DragAndDrop.PrepareStartDrag();

            // Set up what we want to drag
			DragAndDrop.objectReferences = new Object[0];

            // Start the actual drag
            DragAndDrop.StartDrag("Dragging title");

            // Make sure no one uses the event after us
            Event.current.Use();
		}
        public TreeViewItem selectedItem = null;
		private TreeViewItem clickedItem = null;

        protected override void RowGUI (RowGUIArgs args)
		{
			Rect toggleRect = args.rowRect;
			toggleRect.x += GetContentIndent(args.item);

			base.RowGUI(args);


			Event e = Event.current;
			EventType eventType = e.rawType;
			
			if(e.isMouse && eventType != EventType.MouseDrag){
				clickedItem = null;
				selectedItem = null;
			}

			switch(eventType){
				case EventType.MouseDown:
					if(args.rowRect.Contains(e.mousePosition)){
						clickedItem = args.item as T;
						selectedItem = null;
					}
				break;
				case EventType.MouseDrag:
					selectedItem = clickedItem;
				break;
			}
		}
    }
}