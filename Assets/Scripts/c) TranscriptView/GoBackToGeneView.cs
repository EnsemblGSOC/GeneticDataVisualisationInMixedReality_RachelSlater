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

public class GoBackToGeneView : MonoBehaviour, IInputClickHandler
{
    //Internal class - a gene. 
    public class GeneSequenceAndChrosomome
    {
        public GeneSequenceAndChrosomome(string geneName, string desc, string id, string seq, string molecule)
        {
            GeneName = geneName;
            Desc = desc;
            Id = id;
            Seq = seq;
            Molecule = molecule;
        }

        public string GeneName { get; set; }
        public string Desc { get; set; }
        public string Id { get; set; }
        public string Seq { get; set; }
        public string Molecule { get; set; }
    }

    //All global variables. 
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

    //Local variables:
    private string baseURL = "http://rest.ensembl.org";
    public bool readyToBuild;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        parentGeneName = GlobalControl.Instance.parentGeneName;
        getGeneID("human", parentGeneName);
        StartCoroutine("Wait");     //See: https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html  --> Manages method sequence & no performance overhead.
    }

    //Delete thisvodi MouseDown function - just for Unity testing. 
        void OnMouseDown()
    {
        parentGeneName = GlobalControl.Instance.parentGeneName;
        getGeneID("human", parentGeneName);
        StartCoroutine("Wait");     //See: https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html  --> Manages method sequence & no performance overhead.
    }

    void getGeneID(string species, string symbol)
    {
        string endpoint = "/xrefs/symbol/" + species + "/" + symbol + "?object_type=gene";

        StartCoroutine(getContent(endpoint));
    }

    /* ______________
   |               |
   |  Method #2:   |
   |_______________| */
    //Extract the corresponding gene id for the gene name that the user has entered. 

    IEnumerator getContent(string endpoint)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseURL + endpoint))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }
            else // Success
            {
                string outputData = request.downloadHandler.text.ToString();

                string[] results = outputData.Split(new[] { ",{" }, StringSplitOptions.None);

                string firstResult = results[0];
                string object_firstResult = firstResult.Substring(1);
                JObject theResultingGene = JObject.Parse(object_firstResult);      //Converts string to JSON object. 

                string theGeneId = theResultingGene["id"].ToString();

                StartCoroutine(getSequenceAndChromsome(theGeneId));
            }
        }
    }

    /* ______________
    |               |
    |  Method #3:   |
    |_______________| */

    //Calling the 'Sequence' endpoint. --> '/sequence/id/:id' http://rest.ensembl.org/documentation/info/sequence_id

    public IEnumerator getSequenceAndChromsome(string endpoint)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://rest.ensembl.org" + "/sequence/id/" + endpoint + "?content-type=text/plain;mask_feature=1"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }
            else // Success
            {
                string sequenceResult = request.downloadHandler.text;
                JObject object_sequenceResult = JObject.Parse(sequenceResult);      //Converts string to JSON object. 

                //Extracting key values from the JSON object:
                string geneName = parentGeneName;

                string desc = object_sequenceResult["desc"].ToString();
                string id = object_sequenceResult["id"].ToString();
                string seq = object_sequenceResult["seq"].ToString();
                string molecule = object_sequenceResult["molecule"].ToString();

                Debug.Log("Id: " + id + " Description: " + desc + " Sequence: " + seq);

                originalGene = new GeneSequenceAndChrosomome(geneName, desc, id, seq, molecule);

                StartCoroutine(getTranscripts(id));
            }
        }
    }

    /* ______________
    |               |
    |  Method #4:   |
    |_______________| */

    IEnumerator getTranscripts(string id)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://rest.ensembl.org" + "/lookup/id/" + id + "?feature=transcript;content-type=application/json;expand=1"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }
            else // Success
            {
                string transcripts = request.downloadHandler.text;
                JObject theDetailedGene = JObject.Parse(transcripts);      //Converts string to JSON object.
                arrayOfTranscripts = theDetailedGene.GetValue("Transcript");
                JToken startChrom = theDetailedGene.GetValue("start");
                chromosomeStart = startChrom.ToObject<int>();
                JToken endChrom = theDetailedGene.GetValue("end");
                chromosomeEnd = endChrom.ToObject<int>();
                getGeneInfoForHoloLens(arrayOfTranscripts);
            }
        }
    }

    /* ______________
    |               |
    |  Method #5:   |
    |_______________| */

    //Extracting info from the retrieved data.

    private void getGeneInfoForHoloLens(JToken arrayOfTranscripts)
    {

        foreach (JObject transcriptObj in arrayOfTranscripts)
        {
            JObject aTranscript = arrayOfTranscripts[numberOfTranscripts].Value<JObject>();
            allTranscripts.Add(aTranscript);

            JToken startValue = aTranscript.GetValue("start");  //Getting the value for sequence 'starting point' for each transcript.
            JToken endValue = aTranscript.GetValue("end");
            int startOfTranscript = startValue.ToObject<int>();
            int endOfTranscript = endValue.ToObject<int>();

            int lengthOfTranscript = (endOfTranscript > startOfTranscript) ? endOfTranscript - startOfTranscript + 1 : startOfTranscript - endOfTranscript + 1;
            lengthsOfTranscripts.Add(lengthOfTranscript);

            numberOfTranscripts++;
        }

        SaveGeneToGlobalObject(arrayOfTranscripts, allTranscripts, lengthsOfTranscripts, numberOfTranscripts, originalGene);
    }

    /* ______________
     |               |
     |  Method #7:   |
     |_______________| */

    //Saving the gene data that we want to be able to access in other scenes.

    public void SaveGeneToGlobalObject(JToken arrayOfTranscripts, List<JToken> allTranscripts, List<int> lengthsOfTranscripts, int numberOfTranscript, GoBackToGeneView.GeneSequenceAndChrosomome originalGene)
    {
        GlobalControl.Instance.arrayOfTranscripts = arrayOfTranscripts;
        GlobalControl.Instance.allTranscripts = allTranscripts;
        GlobalControl.Instance.lengthsOfTranscripts = lengthsOfTranscripts;
        GlobalControl.Instance.numberOfTranscripts = numberOfTranscripts;
        GlobalControl.Instance.originalGene = originalGene;
        GlobalControl.Instance.indexOfTranscript = 0;
        GlobalControl.Instance.numberOfExonsInTranscript = 0;
        GlobalControl.Instance.indexOfExon = 0;
        GlobalControl.Instance.chromosomeStart = chromosomeStart;
        GlobalControl.Instance.chromosomeEnd = chromosomeEnd;
        GlobalControl.Instance.parentGeneName = originalGene.GeneName;
        readyToBuild = true;    //Triggers scene change & gene to build below. 
    }

    /* ______________
      |               |
      |  Method #8:   |
      |_______________| */

    //Coroutine - initiated in Start() function, and changes scene to build gene.

    IEnumerator Wait()
    {
        while (!readyToBuild)
        {
            yield return null;
        }
        SceneManager.LoadScene("2_GeneBuilder");
    }
}