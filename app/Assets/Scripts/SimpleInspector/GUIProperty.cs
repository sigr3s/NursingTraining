using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GUIProperty : MonoBehaviour {

    [Header("Scene Reference")]
    public TextMeshProUGUI fieldName;
    public InputField textInput;
    public Toggle boolInput;

    
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

            break;
            case PropertyType.Number:
                boolInput.gameObject.SetActive(false);
                textInput.gameObject.SetActive(true);

                textInput.contentType = InputField.ContentType.DecimalNumber;

                textInput.text = (data).ToString();
            break;
            case PropertyType.Boolean:
                boolInput.gameObject.SetActive(true);
                textInput.gameObject.SetActive(false);

                bool value = (bool) data;

                boolInput.isOn = value;
            break;
        }
        int start = path.LastIndexOf('/');

        start = start >= 0 ? start + 1 : 0;

        fieldName.text = path.Substring(start, path.Length - start);
    }
}