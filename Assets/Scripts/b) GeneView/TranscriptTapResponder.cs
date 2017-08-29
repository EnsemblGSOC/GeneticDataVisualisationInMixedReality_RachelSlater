using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Linq;
using System.Text;
using System.IO;
using System;
using UnityEngine.Networking;
using Vuforia;
using System.Threading;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity.InputModule;

//This class implements IInputClickHandler to handle the tap gesture.

public class TranscriptTapResponder : MonoBehaviour, IInputClickHandler
{
    //Global variables:
    public List<JToken> allTranscripts = new List<JToken>();

    void Start()
    {
        allTranscripts = GlobalControl.Instance.allTranscripts;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (gameObject.name.Contains("ParentObject"))                                           //If a transcript is clicked on.
        {
            var scriptOnClickedObj = gameObject.GetComponentInChildren<TranscriptFunctionality>();
            int theIndex = scriptOnClickedObj.indexOfTranscript;
            var theData = gameObject.AddComponent<OnTranscriptClick>();
            theData.displayTranscript = allTranscripts[theIndex].ToObject<JObject>();  //The 'theTranscript' variable can now be accessed through the app.
        }
    }
}
