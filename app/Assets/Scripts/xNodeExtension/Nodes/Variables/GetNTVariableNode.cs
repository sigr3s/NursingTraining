using XNode;
using NT.Atributes;
using NT.Variables;
using NT.Graph;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using NT.SceneObjects;

namespace NT.Nodes.Variables
{
    [System.Serializable]
    public class GetNTVariableNode : NTNode, IVariableNode //<T,K> : NTNode, IVariableNode where K: INTVaribale
    {
        [SerializeField] private Type variableType;
        [SerializeField] private string variablePath;
        [SerializeField] private Type dataType;
        [SerializeField] private string dataKey;

        public override object GetValue(NodePort port) {
            if(graph is NTGraph){
                NTGraph g = (NTGraph) graph;
                object variableValue = g.variableDelegate.GetValue(dataKey);

                if(variableValue != null){
                    if(!string.IsNullOrEmpty(variablePath)){
                        object value = ReflectionUtilities.GetValueOf(new List<string>(variablePath.Split('/')), variableValue);
                        
                        if(value != null && value is  SceneGameObjectReference){
                            value = ( (SceneGameObjectReference) value).reference;                            
                        }
                        
                        return value;
                    }
                    else
                    {
                        return g.variableDelegate.GetSceneGameObject(dataKey);
                    }
                }
                else if(string.IsNullOrEmpty(variablePath))
                {
                    return g.variableDelegate.GetSceneGameObject(dataKey);;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }


        public void SetVariableKey(string dataKey,Type dataType, string variablePath, Type variableType){

            this.dataType = dataType;
            this.dataKey = dataKey;

            this.variablePath = variablePath;
            this.variableType = variableType;

            InitializeNodeTypes();
        }


        private void InitializeNodeTypes(){
            if(!string.IsNullOrEmpty(variablePath)){
                if(!HasPort(variablePath)){
                    AddInstanceOutput(variableType, ConnectionType.Override, TypeConstraint.Strict, variablePath);
                }
            }
            else
            {
                if(!HasPort(variablePath)){
                    AddInstanceOutput(variableType, ConnectionType.Override, TypeConstraint.Strict, variablePath);
                }
            }
        }

    }
}