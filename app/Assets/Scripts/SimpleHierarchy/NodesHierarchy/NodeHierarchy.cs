using System;
using System.Collections.Generic;
using NT;
using NT.Nodes.Variables;
using UnityEngine;
using XNode;

public class NodeHierarchy : GUIHierarchy {

    private void Start() {
        Rebuild();
    }

    public override List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();

        Dictionary<string, HierarchyModel> parents = new Dictionary<string, HierarchyModel>();

        foreach(Type t in ReflectionUtilities.nodeTypes){
            
            if(t.IsAbstract || t.IsGenericType) continue;
            if( typeof(IVariableNode).IsAssignableFrom(t) ) continue;

            if(t == typeof(FlowNode) ||  t == typeof(NTNode)) continue;

            //FIXME: Freindly names

            var node =  (NTNode) Activator.CreateInstance(t);

            List<string> pathParts = node.GetPath();

            HierarchyModel parent = null;

            if(pathParts.Count > 1){
                string fullPath = "";
                for(int i = 0; i < pathParts.Count - 1; i++){
                    fullPath += "/" + pathParts[i]; 
                    if(parents.ContainsKey(fullPath)){
                        parent = parents[fullPath];
                    }
                    else
                    {
                        if(parent == null){
                            parent = new HierarchyModel(new NodeHierarchyData{name =  pathParts[i]});
                            root.Add(parent);
                            parents.Add(fullPath, parent);
                        }
                        else
                        {
                            HierarchyModel hm = new HierarchyModel(new NodeHierarchyData{name = pathParts[i]});
                            parent.AddChild(hm);
                            parent = hm;
                        }
                    }
                }
            }
            
            if(parent != null){
                parent.AddChild(new HierarchyModel(new NodeHierarchyData{name = pathParts[pathParts.Count -1], nodeType = t}));
            }
            else
            {
                root.Add(new HierarchyModel(new NodeHierarchyData{name = pathParts[pathParts.Count -1], nodeType = t}));   
            }
        }
        
        return root;
    }
}

public class NodeHierarchyData : HierarchyData{
    public Type nodeType;
    public Action<Node> onNodeCreated;
}