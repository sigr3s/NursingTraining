using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class GraphTab : MonoBehaviour {
    [Header("Configuration")]
    public Color normalColor;
    public Color selectedColor;

    [Header("Prefab References")]
    public TextMeshProUGUI title;


    [Header("Debug")]
    public Button button;
    public Graphic graphic{
        get{
            return button.targetGraphic;
        }
    }

    [SerializeField] private bool _isSelected = false;
    public bool isSelected{
        get{
            return _isSelected;
        }

        set{
            _isSelected = value;

            if(_isSelected){
                graphic.color = selectedColor;
            }
            else
            {
                graphic.color = normalColor; 
            }
        }
    }

    public bool isScene;

    public string NTKey;
    
    private string displayName;
    public string DisplayName {
        get{
            return displayName;
        }

        set{
            displayName = value;
            title.text = value;
        }
    }



    private void Awake() {
        button = GetComponent<Button>();    
        button.onClick.AddListener(ChangeShowingGraph);
    }

    private void ChangeShowingGraph()
    {
        if(!isScene){
            SessionManager.Instance.OpenGraphFor(NTKey);
        }
        else
        {
            SessionManager.Instance.OpenSceneGraph();
        }
    }
}