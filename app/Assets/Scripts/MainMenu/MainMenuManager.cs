using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    public Transform content;
    public GameObject emptyList;
    public GameObject sessionPrototype;

    public TMP_InputField sessionID;

    private Dictionary<string, GameObject> sessions = new Dictionary<string, GameObject>();
    private void Awake() {
        DirectoryInfo sessionsDir = new DirectoryInfo(SessionManager.GetSavePath());

        if(!sessionsDir.Exists) {
            emptyList.SetActive(true);
            return;
        }

        FileInfo[] files = sessionsDir.GetFiles("config.nt", SearchOption.AllDirectories);
        
        if(files.Length == 0){
            emptyList.SetActive(true);
            
        }
        else
        {
            emptyList.SetActive(false);

            foreach(FileInfo fi in files){
                GameObject itemCard = Instantiate(sessionPrototype, content);
                itemCard.SetActive(true);
                SessionData sd = SessionManager.GetSession(fi.Directory.Name);
                itemCard.GetComponent<MenuCard>().SetSessionData(sd, this);
                sessions.Add(sd.sessionID, itemCard);
            }
        }

    }

    public void PlayeSession(SessionData sessionData)
    {
         SessionManager.sessionToLoad = sessionData;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void EditSession(SessionData sessionData)
    {
        SessionManager.sessionToLoad = sessionData;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void RemoveSession(SessionData sessionData)
    {
        if(sessions.ContainsKey(sessionData.sessionID)){
            GameObject go = sessions[sessionData.sessionID];
            Destroy(go);
            sessions.Remove(sessionData.sessionID);
        }

        SessionManager.DeleteSession(sessionData.sessionID);
    }

    public void CreateFromWindow(){
        SessionData data = new SessionData();

        data.sessionID = Guid.NewGuid().ToString();
        data.displayName = sessionID.text;

        EditSession(data);
    }
}