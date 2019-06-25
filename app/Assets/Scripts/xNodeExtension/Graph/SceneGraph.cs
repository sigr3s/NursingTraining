using System;
using System.Collections;
using System.Collections.Generic;

using NT.Nodes;
using NT.Variables;
using NT.Nodes.Messages;

using UnityEngine;
using UnityEngine.EventSystems;
using XNode;
using XNode.InportExport;

namespace NT.Graph{

    public class SceneGraph : NTGraph
    {

        public override List<string> GetCallbacks(){
            return new List<string>(){"Application Start", "Application End", "Excercise Started", "Exercise End", "Pause", "Resume" };
        }

    }
}