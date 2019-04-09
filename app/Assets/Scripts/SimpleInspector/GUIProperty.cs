using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class PropertyChangedEvent : UnityEvent<object, string>{

}

public class GUIProperty : MonoBehaviour {

    [Header("Scene Reference")]
    public TextMeshProUGUI fieldName;
    public InputField textInput;
    public Toggle boolInput;


    public PropertyChangedEvent OnValueChanged = new PropertyChangedEvent();

    
    [Header("Debug")]
    public string path;

    public enum PropertyType{
        String, 
        Number,
        Boolean
    }




    public void SetData(object data, string path, PropertyType propertyType){

        switch(propertyType){
            case PropertyType.String:
                boolInput.gameObject.SetActive(false);
                textInput.gameObject.SetActive(true);

                textInput.contentType = InputField.ContentType.Alphanumeric;
                textInput.text = (string) data;

                textInput.onEndEdit.RemoveAllListeners();
                textInput.onEndEdit.AddListener(ModifyPropertyString);
            break;
            case PropertyType.Number:
                boolInput.gameObject.SetActive(false);
                textInput.gameObject.SetActive(true);

                textInput.contentType = InputField.ContentType.DecimalNumber;
                textInput.text = (data).ToString();

                textInput.onEndEdit.RemoveAllListeners();

                if(data.GetType() == typeof(int)){
                    textInput.onEndEdit.AddListener(ModifyPropertyInt);
                }
                else if(data.GetType() == typeof(float))
                {
                    textInput.onEndEdit.AddListener(ModifyPropertyFloat);
                }
                else if(data.GetType() == typeof(double))
                {   
                    textInput.onEndEdit.AddListener(ModifyPropertyDouble);
                }

               
            break;
            case PropertyType.Boolean:
                boolInput.gameObject.SetActive(true);
                textInput.gameObject.SetActive(false);

                bool value = (bool) data;
                boolInput.isOn = value;

                boolInput.onValueChanged.RemoveAllListeners();
                boolInput.onValueChanged.AddListener(ModifyPropertyBool);
            break;
        }
        int start = path.LastIndexOf('/');

        start = start >= 0 ? start + 1 : 0;

        fieldName.text = path.Substring(start, path.Length - start);
        this.path = path;
    }

    private void ModifyPropertyFloat(string stringValue)
    {
        float value = 0;
        float.TryParse(stringValue, out value);
        OnValueChanged.Invoke(value, path);
    }

    private void ModifyPropertyInt(string stringValue)
    {
        int value = 0;
        int.TryParse(stringValue, out value);
        OnValueChanged.Invoke(value, path);
    }

    private void ModifyPropertyDouble(string stringValue)
    {
        double value = 0;
        double.TryParse(stringValue, out value);
        OnValueChanged.Invoke(value, path);
    }

    private void ModifyPropertyBool(bool value)
    {
        OnValueChanged.Invoke(value, path);
    }

    private void ModifyPropertyString(string value)
    {
        OnValueChanged.Invoke(value, path);
    }
}