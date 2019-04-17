using System;
using System.Collections;
using System.Collections.Generic;
using NT.Graph;
using UnityEngine;

public class GraphPanel : MonoBehaviour
{   
    public RuntimeGraph runtimeGraph;

    public GraphTab graphTab;
    public RectTransform content;

    [Header("Debug")]
    public List<GraphTab> tabs = new List<GraphTab>();

    private readonly string SceneGraphKey = "Scene Graph";

    void Start()
    {
        SessionManager.Instance.OnShowingGraphChanged.AddListener(ChangeGraph);
        SessionManager.Instance.OnGraphListChanged.AddListener(GrphListChanged);
        SessionManager.Instance.OnSessionLoaded.AddListener(SessionLoaded);

        runtimeGraph = GetComponentInChildren<RuntimeGraph>();
        runtimeGraph.SetGraph(SessionManager.Instance.sceneGraph);

        RefillTabs();
    }

    private void SessionLoaded()
    {
        RefillTabs();
    }


    private void CleanTabs(){
        foreach(GraphTab tab in tabs){
            Destroy(tab.gameObject);
        }

        tabs = new List<GraphTab>();
    }

    private void RefillTabs(){
        CleanTabs();        

        GraphTab sceneTab = Instantiate(graphTab, content);
        sceneTab.isSelected = true;
        sceneTab.isScene = true;
        sceneTab.NTKey = SceneGraphKey;

        tabs.Add(sceneTab);

        foreach(SceneObjectGraph sog in SessionManager.Instance.GetAllGraphs()){
            GraphTab sceneObjectTab = Instantiate(graphTab, content);

            sceneObjectTab.isSelected = false;
            sceneObjectTab.isScene = false;
            sceneObjectTab.NTKey = sog.linkedNTVariable;
            
            tabs.Add(sceneObjectTab);
        }

    }
    private void GrphListChanged()
    {
        RefillTabs();
    }

    private void ChangeGraph()
    {
        NTGraph graph = SessionManager.Instance.showingGraph;
        runtimeGraph.SetGraph(graph);

        string key = SceneGraphKey;

        if(graph.GetType() == typeof(SceneObjectGraph)){
            key = ((SceneObjectGraph) graph).linkedNTVariable;
        }

        foreach(GraphTab tab in tabs){
            if(tab.NTKey == key){
                tab.isSelected = true;
            }
            else
            {
                tab.isSelected = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
