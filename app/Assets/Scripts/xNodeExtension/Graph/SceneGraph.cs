using System;
using System.Collections;
using System.Collections.Generic;

using NT.Nodes;
using NT.Variables;
using NT.Nodes.Messages;

using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace NT.Graph{


    [CreateAssetMenu(fileName = "New Scene Graph", menuName = "NT/Scene Graph")]
    public class SceneGraph : NTGraph
    {
       
        [ContextMenu("Start Execution")]
        public void StartExecution(){
            MessageSystem.onMessageSent -= MessageRecieved;
            MessageSystem.onMessageSent += MessageRecieved;

            if(coroutineRunner == null){
                GameObject go = new GameObject("Coroutine Executor");
                coroutineRunner =  go.AddComponent<CoroutineRunner>();
            }

            GenerateCallbackDict();
            sceneVariables.variableRepository.ResetToDefault();
            
            MessageSystem.SendMessage("start");

        }

       
    }
}