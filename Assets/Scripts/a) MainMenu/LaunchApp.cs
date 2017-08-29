using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This first script is attached to an empty game object in the 'Preload' scene. It triggers the app to start. 

public class LaunchApp : MonoBehaviour {

	void Start () {
        GameObject introPanel = GameObject.Find("IntroPanelCanvas");

        var buttons = introPanel.GetComponentsInChildren<UnityEngine.UI.Button>();
        UnityEngine.UI.Button startAppButton = buttons[0];
        startAppButton.onClick.AddListener(launchApp);
    }

    private void launchApp()
    {
        SceneManager.LoadScene("1_MainMenu");
    }

    void Update () {	
	}
}
