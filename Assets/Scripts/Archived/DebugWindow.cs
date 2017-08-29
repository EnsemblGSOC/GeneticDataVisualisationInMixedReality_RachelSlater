using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DebugWindow : MonoBehaviour {

    TextMesh textMesh;

    // Default Unity script method #1: Initialization.
    void Start () {
        textMesh = gameObject.GetComponentInChildren<TextMesh>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        if (textMesh.text.Length > 300)
        {
            textMesh.text = message + "\n";
        }
        else
        {
            textMesh.text += message + "\n";
        }
    }

    // Default Unity script method #2: Updates (called once per frame).
    void Update () {
		
	}
}
