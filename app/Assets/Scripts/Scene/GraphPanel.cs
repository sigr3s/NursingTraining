using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPanel : MonoBehaviour
{

    void Start()
    {
        SessionManager.Instance.OnShowingGraphChanged.AddListener(ChangeGraph);
    }

    private void ChangeGraph()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
