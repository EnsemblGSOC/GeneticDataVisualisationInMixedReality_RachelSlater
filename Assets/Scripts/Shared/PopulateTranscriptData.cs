using HoloToolkit.Unity.InputModule;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

// This class implements IFocusable to respond to gaze changes.
// It highlights the object being gazed at.

public class PopulateTranscriptData : MonoBehaviour, IFocusable
{
    //Global variables:
    public JToken arrayOfTranscripts;

    //Local variables;
    private Material[] defaultMaterials;
    private int focussedTranscriptIndex;

    private void Start()
    {
        defaultMaterials = GetComponent<Renderer>().materials;                          //Identifying focus event based on the 'Render' component.
        arrayOfTranscripts = GlobalControl.Instance.arrayOfTranscripts;
    }

    public void OnFocusEnter()
    {
        for (int i = 0; i < defaultMaterials.Length; i++)
        {
            // Highlight the material when gaze enters using the shader property.
            defaultMaterials[i].SetFloat("_Highlight", .5f);
        }

        if (gameObject.name.Equals("Transcript"))
        {
            var transcriptScript = gameObject.GetComponent<TranscriptFunctionality>();
            focussedTranscriptIndex = transcriptScript.indexOfTranscript;
        }
        else if (gameObject.name.Contains("Exon") || gameObject.name.Equals("Intron"))
        {
            var exonScript = gameObject.GetComponent<ExonFunctionality>();
            focussedTranscriptIndex = exonScript.sameIndexAsParentTranscript;
        }

        JObject theTranscriptsData = arrayOfTranscripts[focussedTranscriptIndex].Value<JObject>();
        JToken tStart = theTranscriptsData.GetValue("start");

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
                    theValueDisplayed.text = theTranscriptsData.GetValue("display_name").ToString();
                    break;
                case "IdData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = theTranscriptsData.GetValue("id").ToString();
                    break;
                case "IsCanonicalData":
                    theValueDisplayed = allDataValues[i];
                    JToken valueIsCanonical = theTranscriptsData.GetValue("is_canonical");
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
                    JToken strandProperty = theTranscriptsData.GetValue("strand");
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
                    theValueDisplayed.text = theTranscriptsData.GetValue("species").ToString();
                    break;
                case "BiotypeData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = theTranscriptsData.GetValue("biotype").ToString();
                    break;
                case "StartCoordsData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = theTranscriptsData.GetValue("start").ToString();
                    break;
                case "EndCoordsData":
                    theValueDisplayed = allDataValues[i];
                    theValueDisplayed.text = theTranscriptsData.GetValue("end").ToString();
                    break;
                case "ProteinData":
                    theValueDisplayed = allDataValues[i];
                    JObject theProtein = theTranscriptsData.GetValue("Translation").ToObject<JObject>();
                    theValueDisplayed.text = theProtein.GetValue("id").ToString();
                    break;
            }
        }
    }


    public void OnFocusExit()
    {
        for (int i = 0; i < defaultMaterials.Length; i++)
        {
            // Remove highlight on material when gaze exits.
            defaultMaterials[i].SetFloat("_Highlight", 0f);
        }
    }
}
