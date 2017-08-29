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

public class TestTODELETE : MonoBehaviour {

    //Global variables:
    public RetrieveRESTData.GeneSequenceAndChrosomome gene;
    public JToken arrayOfTranscripts;
    public List<JToken> allTranscripts = new List<JToken>();
    public List<int> lengthsOfTranscripts = new List<int>();
    public int numberOfTranscripts;
    public int indexOfTranscript;
    public List<JToken> allExons = new List<JToken>();
    public List<int> numberOfExonsPerTranscript = new List<int>();
    public int numberOfExonsInTranscript;
    public int indexOfExon;

    //Local variables:
    public JObject displayTranscript;
    Boolean readyToBuild;

    // Use this for initialization
    void Start () {
		
	}

    void OnMouseDown()
    {
        arrayOfTranscripts = GlobalControl.Instance.arrayOfTranscripts;
        allTranscripts = GlobalControl.Instance.allTranscripts;
        allExons = GlobalControl.Instance.allExons;
        lengthsOfTranscripts = GlobalControl.Instance.lengthsOfTranscripts;
        numberOfTranscripts = GlobalControl.Instance.numberOfTranscripts;
        numberOfExonsInTranscript = GlobalControl.Instance.numberOfExonsInTranscript;
        indexOfExon = GlobalControl.Instance.indexOfExon;
        indexOfTranscript = GlobalControl.Instance.indexOfTranscript;
        gene = GlobalControl.Instance.gene;

        string id = "ENST00000288602.10";
        StartCoroutine(makeRequestForTranscript(id));

        StartCoroutine("Wait");
    }

    // Update is called once per frame
    void Update () {
		
	}

    public IEnumerator makeRequestForTranscript(string id)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://rest.ensembl.org" + "/lookup/id/" + id + "?content-type=application/json;expand=1"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }
            else // Success
            {
                string transcript = request.downloadHandler.text;
                JObject theTranscriptAsObj = JObject.Parse(transcript);      //Converts string to JSON object.
                saveSingleTranscriptData(theTranscriptAsObj);
            }
        }
    }

    private void saveSingleTranscriptData(JObject theTranscriptAsObj)
    {
        GlobalControl.Instance.singleTranscript = theTranscriptAsObj;
        readyToBuild = true;
    }

    IEnumerator Wait()
    {
        while (!readyToBuild)
        {
            yield return null;
        }
        SceneManager.LoadScene("3_TranscriptBuilder");
    }
}
