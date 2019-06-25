using NT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using TMPro;
using UnityEngine.SceneManagement;

public class ExerciseManager : MonoBehaviour
{

	public VRTK_ControllerEvents leftController;
	public VRTK_ControllerEvents rightController;

    public GameObject menu;
    public GameObject endMenu;


    private void RecieveMessage(string msg)
    {
        if(msg.Contains("Fail Session /"))
        {
            endMenu.SetActive(true);
            menu.SetActive(true);

            float grade = float.Parse(msg.Split('/')[1]);

            endMenu.GetComponentInChildren<TextMeshProUGUI>().text = "The session has been failed with an accuracy of " + grade;
        }   
        
        if (msg.Contains("Pass Session /"))
        {

            endMenu.SetActive(true);
            menu.SetActive(true);

            float grade = float.Parse(msg.Split('/')[1]);

            endMenu.GetComponentInChildren<TextMeshProUGUI>().text = "The session has ended succesfully with an accuracy of " + grade; ;
        }
    }

    protected virtual void OnEnable()
	{
		if (leftController != null)
		{
			leftController.ButtonTwoPressed += TogglePause;
		}

		if (rightController != null)
		{
			rightController.ButtonTwoPressed += TogglePause;
		}
    }

	protected virtual void OnDisable()
	{
		if (leftController != null)
		{
			leftController.ButtonTwoPressed -= TogglePause;
		}

		if (rightController != null)
		{
			rightController.ButtonTwoPressed -= TogglePause;
		}
    }

    public void TogglePause(object sender, ControllerInteractionEventArgs e)
    {
        menu.gameObject.SetActive(!menu.activeInHierarchy);

        if (menu.gameObject.activeInHierarchy)
        {
            MessageSystem.SendMessage("Pause");
        }
        else
        {
            MessageSystem.SendMessage("Resume");
        }
    }


	public void StartExercise(){
        SessionManager.Instance.StartExecution();
        MessageSystem.onMessageSent += RecieveMessage;
    }

    public void EndExercise()
    {
        SessionManager.Instance.EndExercise();
    }

    public void Exit(){
        MessageSystem.onMessageSent -= RecieveMessage;
        SceneManager.LoadScene(0);
	}


}
