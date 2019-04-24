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


    [CreateAssetMenu(fileName = "New Scene Graph", menuName = "NT/Scene Graph")]
    public class SceneGraph : NTGraph
    {

        public override List<string> GetCallbacks(){
            return new List<string>(){"onApplicationStart", "onApplicationEnd", "exerciceStarted", "exerciceEnd", "pauseButton", "resumeButton", "completedButton", "quitButton" };
        }

        public string message;

        [ContextMenu("Start Execution")]
        public void StartExecution(){
            MessageSystem.onMessageSent -= MessageRecieved;
            MessageSystem.onMessageSent += MessageRecieved;

            GenerateCallbackDict();
            sceneVariables.variableRepository.ResetToDefault();
            MessageSystem.SendMessage("start");

        }

         [ContextMenu("Send message")]
        public void SendMessage(){
            MessageSystem.SendMessage(message);
        }

    }
}