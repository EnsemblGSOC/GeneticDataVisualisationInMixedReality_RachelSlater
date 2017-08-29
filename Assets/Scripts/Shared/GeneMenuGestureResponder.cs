using UnityEngine;
using Newtonsoft.Json.Linq;
using HoloToolkit.Unity.InputModule;
using System;

//This class implements IInputClickHandler to handle the tap gesture.

public class GeneMenuGestureResponder : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (gameObject.name.Equals("ZoomIn"))
        {
            GameObject theGene = GameObject.Find("Gene");
            var currScale = theGene.transform.position;
            theGene.transform.position = new Vector3(currScale.x + 0.07f, currScale.y, (currScale.z - 0.5f));
        }
        else if (gameObject.name.Equals("ZoomOut"))
        {
            GameObject theGene = GameObject.Find("Gene");
            var currScale = theGene.transform.position;
            theGene.transform.position = new Vector3(currScale.x - 0.07f, currScale.y, (currScale.z + 0.5f));
        }
    }
}
