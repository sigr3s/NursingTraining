using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuCard : MonoBehaviour {
    public TextMeshProUGUI tittle;

    private MainMenuManager manager;
    private SessionData sessionData;

    public void SetSessionData(SessionData data, MainMenuManager manager){
        tittle.text = string.IsNullOrEmpty(data.displayName) ? data.sessionID : data.displayName;
        this.manager = manager;
        this.sessionData = data;
    }

    public void Remove(){
        manager.RemoveSession(sessionData);
    }

    public void Edit(){
        manager.EditSession(sessionData);
    }

    public void Play(){
        manager.PlayeSession(sessionData);
    }


}