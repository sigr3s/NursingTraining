using System;
using UnityEngine;
using XNode.InportExport;

namespace XNode.Examples {
    public class MathGraph : XNode.NodeGraph { 
        [ContextMenu("Export")]
        public void Export(){
            JSONImportExport jep = new JSONImportExport();
            jep.Export(this, Application.dataPath + "/" + name + ".json");
        }

        [ContextMenu("Import")]
        public void Import(){
            string path = Application.dataPath + "/" + name + ".json";
            JSONImportExport jimp = new JSONImportExport();
            MathGraph g = (MathGraph) jimp.Import(path);

            if(g == null) return;

            Clear();

            nodes = g.nodes;
            foreach(Node n in nodes){
                n.graph = this;
            }
        }
    }
}