using UnityEngine;
using Newtonsoft.Json.Linq;
using HoloToolkit.Unity.InputModule;
using System;

//This class implements IInputClickHandler to handle the tap gesture.

public class TranscriptMenuGestureResponder : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (gameObject.name.Equals("ZoomIn"))
        {
            GameObject theTranscript = GameObject.Find("TheSelectedTranscript");
            var currScale = theTranscript.transform.position;
            theTranscript.transform.position = new Vector3(currScale.x + 0.07f, currScale.y, (currScale.z - 0.5f));
        }
        else if (gameObject.name.Equals("ZoomOut"))
        {
            GameObject theTranscript = GameObject.Find("TheSelectedTranscript");
            var currScale = theTranscript.transform.position;
            theTranscript.transform.position = new Vector3(currScale.x - 0.07f, currScale.y, (currScale.z + 0.5f));
        }
    }
}
