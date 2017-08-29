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
using HoloToolkit.UI.Keyboard;

public class RetrieveRESTData : MonoBehaviour
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
    public RetrieveRESTData.GeneSequenceAndChrosomome gene;
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

    //Local variables.
    public bool readyToBuild;
    public Keyboard keyboard;
    public String input;
    public GameObject theKeyboard;
    public UnityEngine.UI.InputField geneName_keyboardInput;
    public UnityEngine.UI.InputField transcriptID_keyboardInput;
    public Boolean fetchGene = false;               //Two x Booleans to control which scene is loaded. 
    public Boolean fetchTranscript = false;
    public Boolean transcriptDictation = false;
    public GameObject wholeCanvas;
    public GameObject inputPanel;
    public Vector3 cursorPosition;
    public Vector3 originalCanvasPosition;

    private string baseURL = "http://rest.ensembl.org";

    // Default Unity script method #1: Initialization.

    private void Start()
    {
        theKeyboard = (GameObject)Instantiate(Resources.Load("Keyboard"));
        //theKeyboard.SetActive(true);
   
        input = "";

        inputPanel = GameObject.Find("Panel");

        wholeCanvas = GameObject.Find("KeyboardInputPanel");
        originalCanvasPosition = wholeCanvas.transform.position;

        var buttons = wholeCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
        UnityEngine.UI.Button searchGeneButton = buttons[0];
        searchGeneButton.onClick.AddListener(readyToFetchGeneData);
        UnityEngine.UI.Button searchTranscriptButton = buttons[1];
        searchTranscriptButton.onClick.AddListener(readyToFetchTranscriptData);
        UnityEngine.UI.Button dictateTranscript = buttons[2];
        dictateTranscript.onClick.AddListener(transcriptDictationActivated);

        var inputFields = wholeCanvas.GetComponentsInChildren<UnityEngine.UI.InputField>();
        geneName_keyboardInput = inputFields[0];
        transcriptID_keyboardInput = inputFields[1];

        //____________________________________________________________________Testing in Unity!(Remove this for emulator). 
        readyToFetchGeneData();
        //readyToFetchTranscriptData();

        StartCoroutine("Wait");     //See: https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html  --> Manages method sequence & no performance overhead.
    }

    // Default Unity script method #2: Updates (called once per frame).
    void Update()
    {            
        if (theKeyboard.activeInHierarchy && !(theKeyboard.GetComponentInChildren<UnityEngine.UI.InputField>()).Equals(input))
        {
            theKeyboard.transform.localScale = new Vector3(0.00065f, 0.00065f, 0.00065f);
            theKeyboard.transform.position = new Vector3(0, -0.05f, 2);

            wholeCanvas.transform.position = new Vector3(originalCanvasPosition.x, originalCanvasPosition.y + 0.15f, originalCanvasPosition.z);

            if (geneName_keyboardInput.isActiveAndEnabled)
            {
                input = geneName_keyboardInput.text;
            }
            else if (transcriptID_keyboardInput.isActiveAndEnabled)
            {
                input = transcriptID_keyboardInput.text;
            }
        }

        var theCursor = GameObject.Find("DefaultCursor");
        cursorPosition = theCursor.transform.position;

        var buttonsOnKeyboard = theKeyboard.GetComponentsInChildren<UnityEngine.UI.Button>();
        UnityEngine.UI.Button theCloseButton = buttonsOnKeyboard[0];
        theCloseButton.onClick.AddListener(recentreInputPanel);
    }

    private void recentreInputPanel()
    {
        wholeCanvas.transform.position = new Vector3(originalCanvasPosition.x + 0.4f, originalCanvasPosition.y + 0.1f, originalCanvasPosition.z);
        Debug.Log(cursorPosition + " cursor psoition");
    }


    public void readyToFetchGeneData()
    {
        input = geneName_keyboardInput.text;

        theKeyboard.SetActive(false);

        string symbol = input;

        if (symbol.Equals(""))
        {
            symbol = "braf";                        //Default. 
            input = symbol;                         //Used for creating the 'gene' object global variable.  
        }

        string species = "human";

        fetchGene = true;

        getGeneID(species, symbol);
    }

    private void transcriptDictationActivated()
    {
        transcriptID_keyboardInput.IsActive();
        transcriptDictation = true;
    }

    public void numberVoiceInputRecognition(int numberSpoken)
    {
        transcriptID_keyboardInput.text += numberSpoken.ToString();
    }

    public void deleteLastNumber()
    {
        string currInput = transcriptID_keyboardInput.text;
        int lengthCurrInput = currInput.Length;
        transcriptID_keyboardInput.text = currInput.TrimEnd(currInput[lengthCurrInput - 1]);
    }

    public void addDecimalPoint()
    {
        transcriptID_keyboardInput.text += ".";
    }

    public void addZeroToTranscriptInput()
    {
        transcriptID_keyboardInput.text += "0";
    }

    public void readyToFetchTranscriptData()
    {
        input = transcriptID_keyboardInput.text;

        theKeyboard.SetActive(false);

        string[] inputString = input.Split(new[] { "." }, StringSplitOptions.None);
        string leadingDigitsPart = inputString[0];
        int lengthOfInput = leadingDigitsPart.Length;
        int numberOfLeadingZeros = 11 - lengthOfInput;
        string leadingZeros = "";

        for(int i=0; i<numberOfLeadingZeros; i++)
        {
            leadingZeros += "0"; 
        }

        string symbol = "ENST" + leadingZeros + input.ToString();

        if (symbol.Equals("ENST") || symbol.Equals("ENST00000000000"))
        {
            symbol = "ENST00000288602.10";          //Default. 
            input = symbol;                         //Used for creating the 'gene' object global variable.  
        } else
        {
            transcriptID_keyboardInput.text = symbol;
        }

        fetchTranscript = true;

        StartCoroutine(getTranscriptData(symbol));
    }

    void handleDefaultGeneVoiceCommand(string symbol)
    {
        geneName_keyboardInput.text = symbol;
        fetchGene = true;
        getGeneID("human", symbol);
    }

    /* ______________
    |               |
    |  Method #1 :  |
    |_______________| */

    //Calling the 'Cross References' endpoint. http://rest.ensembl.org/documentation/info/xref_external 

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
                string geneName = input;

                if(geneName == "")
                {
                    geneName = geneName_keyboardInput.text;
                }

                string desc = object_sequenceResult["desc"].ToString();
                string id = object_sequenceResult["id"].ToString();
                string seq = object_sequenceResult["seq"].ToString();
                string molecule = object_sequenceResult["molecule"].ToString();

                Debug.Log("Id: " + id + " Description: " + desc + " Sequence: " + seq);

                gene = new GeneSequenceAndChrosomome(geneName, desc, id, seq, molecule);

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

        SaveGeneToGlobalObject(arrayOfTranscripts, allTranscripts, lengthsOfTranscripts, numberOfTranscripts, gene);
    }

    /* ______________
    |               |
    |  Method #6:   |
    |_______________| */

    public IEnumerator getTranscriptData(string id)
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
        Debug.Log(theTranscriptAsObj + "__________________");
        readyToBuild = true;
    }

    /* ______________
      |               |
      |  Method #7:   |
      |_______________| */

    //Saving the gene data that we want to be able to access in other scenes.

    public void SaveGeneToGlobalObject(JToken arrayOfTranscripts, List<JToken> allTranscripts, List<int> lengthsOfTranscripts, int numberOfTranscript, GeneSequenceAndChrosomome gene)
    {
        GlobalControl.Instance.arrayOfTranscripts = arrayOfTranscripts;
        GlobalControl.Instance.allTranscripts = allTranscripts;
        GlobalControl.Instance.lengthsOfTranscripts = lengthsOfTranscripts;
        GlobalControl.Instance.numberOfTranscripts = numberOfTranscripts;
        GlobalControl.Instance.gene = gene;
        GlobalControl.Instance.indexOfTranscript = 0;
        GlobalControl.Instance.numberOfExonsInTranscript = 0;
        GlobalControl.Instance.indexOfExon = 0;
        GlobalControl.Instance.chromosomeStart = chromosomeStart;
        GlobalControl.Instance.chromosomeEnd = chromosomeEnd;
        GlobalControl.Instance.parentGeneName = gene.GeneName;
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

        if (fetchGene)
        {
            SceneManager.LoadScene("2_GeneBuilder");
        }
        else if (fetchTranscript)
        {
            SceneManager.LoadScene("3_TranscriptBuilder");
        }
    }
}