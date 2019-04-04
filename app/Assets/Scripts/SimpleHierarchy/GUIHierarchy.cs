using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIHierarchy : MonoBehaviour {
    public GameObject prefab;


    [Header("Scene References")]
    public GameObject content;


    [Header("Debug")]
    public List<HierarchyModel> root;
    public NTPool pool;
    public List<GameObject> activeItems = new List<GameObject>();

    void Awake() {
        GameObject p = new GameObject("Pool_" + this.name);
        pool = p.AddComponent<NTPool>();
        pool.Initialize(prefab);
        
        DontDestroyOnLoad(pool);
    }


    public virtual List<HierarchyModel> GetRoot(){
        List<HierarchyModel> root = new List<HierarchyModel>();

        HierarchyModel mroot = new HierarchyModel(new HierarchyData{ name = "Root"});
        mroot.AddChild(new HierarchyModel(new HierarchyData{ name = "Node 1"}));
        mroot.AddChild(new HierarchyModel(new HierarchyData{ name = "Node 2"}));

        root.Add(mroot);

        return root;
    }
    
    [ContextMenu("Rebuild")]
    public virtual void Rebuild(){        
        root = GetRoot();
        BuildUI();
    }

    private void Start() {
       Rebuild();
    }

    public void BuildUI(){
        foreach (var item in activeItems)
        {
            pool.PoolItem(item);
        }

        activeItems = new List<GameObject>();

        foreach (var rootItem in root)
        {
            UIFor(rootItem, content.transform);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
    }

    public void UIFor(HierarchyModel model, Transform parent){
        if(model == null) return;

        if(model.uiObject != null){
            model.uiObject.transform.parent = parent;
        } 
        else
        {
            GameObject hierarchyUIElement = pool.GetItem();
            hierarchyUIElement.transform.SetParent(parent);
            activeItems.Add(hierarchyUIElement);
            
            GUIHierarchyItem guihi = hierarchyUIElement.GetComponent<GUIHierarchyItem>();
            guihi.data = model.data;

            foreach (var child in model.GetChildren())
            {  
                UIFor(child, guihi.childTransfrom);                
            }
        }       
    }
}