using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIHierarchyItem : MonoBehaviour
{
    [Header("Scene References")]
    public TextMeshProUGUI title;
    public Transform childTransfrom;


    private HierarchyData _data;
    public HierarchyData data{
        get{
            return _data;
        }

        set{
            _data = value;
            title.text = _data.name;
            title.color = Color.white;

            if(_data.selected){
                title.color = Color.red;
            }

            hierarchy = GetComponentInParent<GUIHierarchy>();

            if(toggle != null){
                toggle.interactable = data.hasChildren;
                toggle.SetIsOnWithoutNotify(true);
            }
            
            childTransfrom.gameObject.SetActive(true);
    
            OnDataSetted();
        }
    }

    protected GUIHierarchy hierarchy;

    protected Toggle toggle;

    private void OnEnable() {
        toggle =  GetComponentInChildren<Toggle>();

        toggle?.onValueChanged.AddListener( (b) => {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) hierarchy.content.transform);
        });
    }

    public virtual void OnDataSetted(){

    }
}
