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
using UnityEngine.UI;

public class TranscriptFunctionality : MonoBehaviour
{
    //Global variables:
    public RetrieveRESTData.GeneSequenceAndChrosomome gene;
    public JToken arrayOfTranscripts;
    public List<JToken> allTranscripts = new List<JToken>();
    public List<int> lengthsOfTranscripts = new List<int>();
    public int numberOfTranscripts;
    public int indexOfTranscript;
    public List<JToken> allExons = new List<JToken>();
    public int numberOfExonsInTranscript;
    public int indexOfExon;
    public Dictionary<string, string> transcriptBioTypeDict = new Dictionary<string, string>();
    public Dictionary<string, Color> transcriptColorDict = new Dictionary<string, Color>();

    //Local variables:
    public JObject theTranscript;
    public Text transcriptText;
    public JArray exons = new JArray();
    public JArray sortedArrayOfExons = new JArray();
    public Boolean readyToBuildExon = false;
    public int[] exonNumberPerTranscript;
    public int lastTranscriptIndex;
    public Boolean strandNumIsPositive = true;

    void Start()
    {
        arrayOfTranscripts = GlobalControl.Instance.arrayOfTranscripts;
        allTranscripts = GlobalControl.Instance.allTranscripts;
        allExons = GlobalControl.Instance.allExons;
        lengthsOfTranscripts = GlobalControl.Instance.lengthsOfTranscripts;
        numberOfTranscripts = GlobalControl.Instance.numberOfTranscripts;
        numberOfExonsInTranscript = GlobalControl.Instance.numberOfExonsInTranscript;
        gene = GlobalControl.Instance.gene;
        indexOfExon = GlobalControl.Instance.indexOfExon;
        transcriptBioTypeDict = GlobalControl.Instance.transcriptBioTypeDict;
        transcriptColorDict = GlobalControl.Instance.transcriptColorDict;

        exonNumberPerTranscript = new int[numberOfTranscripts];

        attachDataToGameObject();

        StartCoroutine("WaitToBuildExon");
    }

    //This method attaches each instantiated transcript game object with it's corresponding data retrieved from the REST API. 

    public void attachDataToGameObject()
    {
        theTranscript = allTranscripts[indexOfTranscript].ToObject<JObject>();  //The 'theTranscript' variable can now be accessed through the app.

        //Transcript values to be displayed (rendered in the next method):
        var id = theTranscript.GetValue("id");
        int transcriptStart = (int)theTranscript.GetValue("start");

        //List of Exons for each transcript:
        JArray allTheExons = new JArray(theTranscript.GetValue("Exon"));
        string arrayOfExons = theTranscript.GetValue("Exon").ToString();
        exons = JsonConvert.DeserializeObject<JArray>(arrayOfExons);
        numberOfExonsInTranscript = exons.Count();

        //Adding the exons as token objects to a list.
        exonNumberPerTranscript[indexOfTranscript] = numberOfExonsInTranscript;
        for (int k = 0; k < exons.Count(); k++)
        {
            JToken anExon = exons[k].Value<JToken>();
            allExons.Add(anExon);                           //Initialising the global variable - all of the exon objects. 
        }

        sortedArrayOfExons = new JArray(exons.OrderBy(JToken => JToken["start"]));

        //Getting the 'lastTranscriptIndex' variable so it can be used in the check for whether all transcripts have been looped through. 
        //When they have - ready to build exons.

        var anyTranscriptObject = GameObject.FindObjectOfType<TranscriptFunctionality>();
        int theLastIndexVariable = anyTranscriptObject.lastTranscriptIndex;

        if (indexOfTranscript == theLastIndexVariable && exonNumberPerTranscript.Count() == numberOfTranscripts)
        {
            readyToBuildExon = true;
        }

        colorTranscripts();
    }

    private void colorTranscripts()
    {
        JObject eachTranscript = allTranscripts[indexOfTranscript].ToObject<JObject>();
        string biotype = eachTranscript.GetValue("biotype").ToString();

        var theParent = GameObject.Find("ParentObject" + indexOfTranscript.ToString());
        var transcriptRender = theParent.GetComponentInChildren<Renderer>();

        Color colorMatch = Color.white;

        if (transcriptBioTypeDict.ContainsKey(biotype))
        {
            string bioTypeLookUp = transcriptBioTypeDict[biotype];
            colorMatch = transcriptColorDict[bioTypeLookUp];
        }
        transcriptRender.material.color = colorMatch;

        //colorTranscriptLegend();
    }

    //private void colorTranscriptLegend()
    //{
    //    GameObject ss = GameObject.Find("What");
    //    MeshRenderer a = ss.GetComponentInChildren<MeshRenderer>();
    //    a.material.color = Color.cyan;


    //    GameObject colorSquares = GameObject.Find("ColorSquares");
    //    UnityEngine.UI.Image[] colors = colorSquares.GetComponentsInChildren<Image>();

    //    for (int i = 0; i < colors.Count(); i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                var d = colors[i].GetComponent<MeshRenderer>();
    //                d.material.color = Color.cyan;
    //                //colors[i].color = Color.cyan;
    //                break;
    //            case 1:
    //                colors[i].color = Color.magenta;
    //                break;
    //            case 2:
    //                colors[i].color = Color.yellow;
    //                break;
    //            case 3:
    //                colors[i].color = Color.grey;
    //                break;
    //            case 4:
    //                colors[i].color = Color.green;
    //                break;
    //            case 5:
    //                colors[i].color = Color.blue;
    //                break;
    //            case 6:
    //                colors[i].color = Color.red;
    //                break;
    //        }
    //    }
    //}

    //Coroutine - initiated in Start() function, and changes scene to build exon.

    IEnumerator WaitToBuildExon()
    {
        while (!readyToBuildExon)
        {
            yield return new WaitForSeconds(10);
        }
        attachedBuildExonScript();  //Ensures that exon data has been accessed for all transcripts. 
    }

    public void attachedBuildExonScript()
    {
        var emptyExon = GameObject.Find("Empty_ExonBuildingBlock");
        var emptyExonScript = emptyExon.GetComponent<ExonGenerator>();
        emptyExonScript.readyToBuildExon();
    }

    void Update()
    {
    }
}