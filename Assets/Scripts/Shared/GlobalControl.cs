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

//Singelton Design pattern: https://www.sitepoint.com/saving-data-between-scenes-in-unity/ 
//This script initialising all of the global variables in the application.
//It is possible to access any of these in all scenes. 

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;                   //This global instance object can then be used in any scene. 

    public RetrieveRESTData.GeneSequenceAndChrosomome gene;
    public GoBackToGeneView.GeneSequenceAndChrosomome originalGene;
    public int chromosomeStart;
    public int chromosomeEnd;
    public JToken arrayOfTranscripts;
    public List<JToken> allTranscripts = new List<JToken>();
    public List<int> lengthsOfTranscripts = new List<int>();
    public int numberOfTranscripts;
    public int indexOfTranscript;
    public List<JToken> allExons = new List<JToken>();
    public int numberOfExonsInTranscript;
    public int indexOfExon;
    public JObject singleTranscript;
    public string parentGeneName;
    public Dictionary<string, string> transcriptBioTypeDict = new Dictionary<string, string>();
    public Dictionary<string, Color> transcriptColorDict = new Dictionary<string, Color>();

    void Awake()    //Cross-scene persistence.
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}

