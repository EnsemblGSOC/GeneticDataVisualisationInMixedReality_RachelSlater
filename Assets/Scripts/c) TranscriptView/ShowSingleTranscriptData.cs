using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ShowSingleTranscriptData : MonoBehaviour {

    //Global variables:
    public JObject singleTranscript;

    //Local variables:



    // Use this for initialization
    void Start () {
        singleTranscript = GlobalControl.Instance.singleTranscript;
        displayInDataPanel();
    }

    private void displayInDataPanel()
    {
        GameObject dataPanel = GameObject.Find("DataValues");
        var allDataValues = dataPanel.GetComponentsInChildren<Text>();

        for (int i = 0; i < allDataValues.Length; i++)
        {
            string eachValue = allDataValues[i].name;
            Text theValueDisplayed;

            switch (eachValue)
            {
                case "DisplayNameData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = singleTranscript.GetValue("display_name").ToString();
                    break;
                case "IdData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = singleTranscript.GetValue("id").ToString();
                    break;
                case "IsCanonicalData":
                    theValueDisplayed = allDataValues[i];
                    JToken valueIsCanonical = singleTranscript.GetValue("is_canonical");
                    if (valueIsCanonical.ToString().Equals("0"))
                    {
                        theValueDisplayed.text = "false";
                    }
                    else
                    {
                        theValueDisplayed.text = "true";
                    }
                    break;
                case "StrandData":
                    theValueDisplayed = allDataValues[i];
                    JToken strandProperty = singleTranscript.GetValue("strand");
                    if (strandProperty.ToString().Equals("-1"))
                    {
                        theValueDisplayed.text = "reverse";
                    }
                    else
                    {
                        theValueDisplayed.text = "forward";
                    }
                    break;
                case "SpeciesData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = singleTranscript.GetValue("species").ToString();
                    break;
                case "BiotypeData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = singleTranscript.GetValue("biotype").ToString();
                    break;
                case "StartCoordsData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = singleTranscript.GetValue("start").ToString();
                    break;
                case "EndCoordsData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = singleTranscript.GetValue("end").ToString();
                    break;
                case "ProteinData":
                    theValueDisplayed = allDataValues[i];
                    JObject theProtein = singleTranscript.GetValue("Translation").ToObject<JObject>();
                    theValueDisplayed.text = theProtein.GetValue("id").ToString();
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
