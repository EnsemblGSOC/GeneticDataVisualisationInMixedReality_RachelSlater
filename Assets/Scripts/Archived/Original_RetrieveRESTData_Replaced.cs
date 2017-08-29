//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using System.Net;
//using System.Web;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System;
//using UnityEngine.Networking;
//using Vuforia;
//using System.Threading;
//using UnityEngine.SceneManagement;

//public class Original_RetrieveRESTData_Replaced : MonoBehaviour
//{
//    public class GeneSequenceAndChrosomome
//    {
//        public GeneSequenceAndChrosomome(string desc, string id, string seq, string molecule)
//        {
//            Desc = desc;
//            Id = id;
//            Seq = seq;
//            Molecule = molecule;
//        }

//        public string Desc { get; set; }
//        public string Id { get; set; }
//        public string Seq { get; set; }
//        public string Molecule { get; set; }
//    }

//    private string baseURL = "http://rest.ensembl.org";

//    public JToken arrayOfTranscripts;
//    public List<JToken> allTranscripts = new List<JToken>();
//    public List<JToken> allExons = new List<JToken>();
//    public List<JToken> transcriptSeqStarts = new List<JToken>();
//    public int numberOfTranscripts;
//    public GeneSequenceAndChrosomome gene;
//    public int indexOfTranscript;
//    public int numberOfExonsInTranscript;
//    public int indexOfExon;
//    public int exonIndexCount;

//    Boolean readyToBuild;

//    // Default Unity script method #1: Initialization.

//    private void Start()
//    {
//        Debug.Log("__________IN MAIN SCENE!!!!!!!!____________");

//        Debug.Log("1. __________START____________");

//        //-------> Taking real user input will go here. 

//        string symbol = "";

//        if (symbol.Equals(""))
//        {
//            symbol = "brca2";
//        }

//        string species = "human";

//        Debug.Log("The symbol is: " + symbol);

//        StartCoroutine("Wait");

//        string theGeneId = getGeneID(species, symbol); //Calling Method 1.
//    }

//    // Default Unity script method #2: Updates (called once per frame).
//    void Update()
//    {
//    }

//    /* ______________
//    |               |
//    |  Method #1 :  |
//    |_______________| */

//    //Calling the 'Cross References' endpoint. http://rest.ensembl.org/documentation/info/xref_external 

//    string getGeneID(string species, string symbol)
//    {
//        Debug.Log("2. __________GETGENEID____________");

//        string endpoint = "/xrefs/symbol/" + species + "/" + symbol + "?object_type=gene";
//        string geneId = getContent(endpoint); //Calling Method 2. 
//        return geneId;
//    }


//    /* ______________
//    |               |
//    |  Method #2a:  |
//    |_______________| */
//    //Extract the corresponding gene id for the gene name that the user has entered. 

//    private string getContent(string endpoint)
//    {
//        Debug.Log("3. __________GETCONTENT____________");

//        HttpWebRequest url = (HttpWebRequest)WebRequest.Create(baseURL + endpoint);
//        url.ContentType = "application/json";
//        url.BeginGetResponse(new AsyncCallback(getContent_ReadWebRequestCallback), url);
//        return "removethereturn";
//    }

//    /* ____________________
//    |                      |
//    |  2 b) HTTP Response: |
//    |______________________| */

//    private void getContent_ReadWebRequestCallback(IAsyncResult callbackResult)
//    {
//        Debug.Log("4. __________GETCONTENT _ READ WEB REQUEST____________");

//        HttpWebRequest myRequest = default(HttpWebRequest);
//        HttpWebResponse myResponse = default(HttpWebResponse);
//        myRequest = (HttpWebRequest)callbackResult.AsyncState;

//        Debug.Log(">>>>>>>>>>> myRequest is --> " + myRequest);
//        myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
//        Debug.Log(">>>>>>>>>>> myResponse is --> " + myResponse);

//        using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
//        {
//            Debug.Log(">>>>>>>>>>> inside StreamReader");
//            string outputData = httpwebStreamReader.ReadToEnd();
//            Debug.Log(">>>>>>>>>>> outputData is --> " + outputData);
//            string[] results = outputData.Split(new[] { ",{" }, StringSplitOptions.None);

//            string firstResult = results[0];
//            Debug.Log(">>>>>>>>>>> firstResult is --> " + firstResult);
//            string object_firstResult = firstResult.Substring(1);
//            JObject theResultingGene = JObject.Parse(object_firstResult);      //Converts string to JSON object. 
//            Debug.Log(">>>>>>>>>>> theResultingGene is --> " + theResultingGene);

//            Debug.Log(object_firstResult + " <--- getContent - Method 2!");
//            string theGeneId = theResultingGene["id"].ToString();

//            getSequenceAndChromsome(theGeneId);
//        }
//    }


//    /* ______________
//    |               |
//    |  Method #3a:  |
//    |_______________| */

//    //Calling the 'Sequence' endpoint. --> '/sequence/id/:id' http://rest.ensembl.org/documentation/info/sequence_id

//    private void getSequenceAndChromsome(string endpoint)
//    {
//        Debug.Log("5. __________GETSEQANDCHROMOSME____________");

//        HttpWebRequest url = (HttpWebRequest)WebRequest.Create("http://rest.ensembl.org" + "/sequence/id/" + endpoint + "?content-type=text/plain;mask_feature=1");
//        //^^The ending params specify that the whole gene sequence is returned - introns and exons.

//        url.ContentType = "application/json";
//        url.BeginGetResponse(new AsyncCallback(getSequence_ReadWebRequestCallback), url);
//    }

//    /* ____________________
//    |                      |
//    |  3 b) HTTP Response: |
//    |______________________| */

//    private void getSequence_ReadWebRequestCallback(IAsyncResult callbackResult)
//    {
//        Debug.Log("6. __________GETSEQ - READWEBREQUEST____________");

//        HttpWebRequest myRequest = default(HttpWebRequest);
//        HttpWebResponse myResponse = default(HttpWebResponse);
//        myRequest = (HttpWebRequest)callbackResult.AsyncState;

//        myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);


//        using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
//        {

//            string sequenceResult = httpwebStreamReader.ReadToEnd();


//            JObject object_sequenceResult = JObject.Parse(sequenceResult);      //Converts string to JSON object. 

//            //Extracting key values from the JSON object:
//            string desc = object_sequenceResult["desc"].ToString();
//            string id = object_sequenceResult["id"].ToString();
//            string seq = object_sequenceResult["seq"].ToString();
//            string molecule = object_sequenceResult["molecule"].ToString();

//            Debug.Log("Id: " + id + " Description: " + desc + " Sequence: " + seq);

//            getTranscripts(id);
//            gene = new GeneSequenceAndChrosomome(desc, id, seq, molecule);
//        }
//    }

//    /* ______________
//    |               |
//    |  Method #4:  |
//    |_______________| */

//    //Calling the 'LookUp' endpoint. --> '/lookup/id/:id' https://rest.ensembl.org/documentation/info/lookup

//    private void getTranscripts(string id)
//    {
//        Debug.Log("7. __________GETTRANSCRIPTS____________");

//        HttpWebRequest url = (HttpWebRequest)WebRequest.Create("http://rest.ensembl.org" + "/lookup/id/" + id + "?feature=transcript;content-type=application/json;expand=1");
//        //^^The params at the end specify that it will return all of the transcripts.
//        url.ContentType = "application/json";

//        url.BeginGetResponse(new AsyncCallback(getTranscripts_ReadWebRequestCallback), url);
//    }

//    /* ____________________
//    |                      |    
//    |  4 b) HTTP Response: |
//    |______________________| */

//    private void getTranscripts_ReadWebRequestCallback(IAsyncResult callbackResult)
//    {
//        Debug.Log("8. __________GETTRANSCRIPTS - READWEBREQUEST____________");

//        HttpWebRequest myRequest = default(HttpWebRequest);
//        HttpWebResponse myResponse = default(HttpWebResponse);
//        myRequest = (HttpWebRequest)callbackResult.AsyncState;
//        myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);

//        using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
//        {
//            string transcripts = httpwebStreamReader.ReadToEnd();

//            JObject theDetailedGene = JObject.Parse(transcripts);      //Converts string to JSON object.
//            arrayOfTranscripts = theDetailedGene.GetValue("Transcript");

//            getGeneInfoForHoloLens(arrayOfTranscripts);
//        }
//    }

//    /* ______________
//    |               |
//    |  Method #5:   |
//    |_______________| */

//    //Extracting info from the retrieved data.

//    private void getGeneInfoForHoloLens(JToken arrayOfTranscripts)
//    {
//        Debug.Log("9. __________GETGENEINFOFORHOLOLENS____________");

//        Debug.Log("A transcript object example: " + arrayOfTranscripts[0]);
//        foreach (JObject transcriptObj in arrayOfTranscripts)
//        {
//            JObject aTranscript = arrayOfTranscripts[numberOfTranscripts].Value<JObject>();
//            allTranscripts.Add(aTranscript);

//            JToken startValue = aTranscript.GetValue("start");  //Getting the value for sequence 'starting point' for each transcript.
//            transcriptSeqStarts.Add(startValue);

//            numberOfTranscripts++;
//        }

//        foreach (JToken transcriptStartingPoint in transcriptSeqStarts)
//        {
//            Debug.Log(transcriptStartingPoint + " <-- Seq starting point.");
//        }

//        Debug.Log(numberOfTranscripts + " <--- The number of transcripts in this gene");

//        SaveGeneToGlobalObject(arrayOfTranscripts, allTranscripts, transcriptSeqStarts, numberOfTranscripts);
//    }

//    /* ______________
//      |               |
//      |  Method #6:   |
//      |_______________| */

//    //Saving the gene data that we want to access in other scenes.

//    public void SaveGeneToGlobalObject(JToken arrayOfTranscripts, List<JToken> allTranscripts, List<JToken> transcriptSeqStarts, int numberOfTranscript)
//    {
//        Debug.Log("10. __________SAVE GENE TO GLOBAL OBJECT____________");

//        GlobalControl.Instance.arrayOfTranscripts = arrayOfTranscripts;
//        GlobalControl.Instance.allTranscripts = allTranscripts;
//        GlobalControl.Instance.transcriptSeqStarts = transcriptSeqStarts;
//        GlobalControl.Instance.numberOfTranscripts = numberOfTranscripts;
//        GlobalControl.Instance.gene = gene;
//        GlobalControl.Instance.indexOfTranscript = 0;
//        GlobalControl.Instance.numberOfExonsInTranscript = 0;
//        GlobalControl.Instance.indexOfExon = 0;
//        GlobalControl.Instance.exonIndexCount = 0;
//        readyToBuild = true;    //Triggers scene change & gene to build. 
//    }

//    /* ______________
//      |               |
//      |  Method #7:   |
//      |_______________| */

//    //Coroutine - initiated in Start() function, and changes scene to build gene.

//    IEnumerator Wait()
//    {
//        Debug.Log("11. _________TOP OF WAIT____________");

//        while (!readyToBuild)
//        {
//            yield return null;
//        }
//        Debug.Log("12. _________BOTTOM OF WAIT____________");
//        SceneManager.LoadScene("2_GeneBuilder");
//    }
//}