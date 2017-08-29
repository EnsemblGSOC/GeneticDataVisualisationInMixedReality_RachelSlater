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
using UnityEngine.UI;
using System;

public class ExonGenerator : MonoBehaviour
{
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
    public Transform exon;
    public int[] exonNumberInEachTranscript;
    Dictionary<string, int> exonDictionary = new Dictionary<string, int>();
    Dictionary<int, float> positionDictionary = new Dictionary<int, float>();

    void Start()
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
    }

    public void readyToBuildExon()
    {
        for (int i = 0; i < numberOfTranscripts; i++)   //Need to go through entire array of transcripts.
        {
            //Locating the correct parent object & accessing it's transcript.
            var theParent = GameObject.Find("ParentObject" + i.ToString());
            var eachTranscript = theParent.GetComponentInChildren<TranscriptFunctionality>();
            int exonNum = eachTranscript.numberOfExonsInTranscript;
            Boolean strandIsPositive = eachTranscript.strandNumIsPositive;

            //Creating a new child to hold the exons. 
            GameObject subParent = new GameObject();
            subParent.transform.name = "Exons";
            subParent.transform.SetParent(theParent.transform);

            //Getting the 'start' and 'end' value for each transcript.
            JObject aTranscript = arrayOfTranscripts[i].Value<JObject>();
            JToken tStart = aTranscript.GetValue("start");
            int transcriptStart = tStart.ToObject<int>();
            JToken tEnd = aTranscript.GetValue("end");
            int transcriptEnd = tEnd.ToObject<int>();

            //Retrieving the 'exon' property. 
            JObject theTranscript = allTranscripts[i].ToObject<JObject>();
            JArray exons = eachTranscript.sortedArrayOfExons;

            //Generating a random color for every exon in each transcript. 
            Color exonColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
            Color intronColor = new Color(0f, 0f, 0f, 1.0f);

            //Total length of the rendered transcript in view.
            var transcriptRenderer = eachTranscript.GetComponent<Renderer>();
            var transcriptsBounds = transcriptRenderer.bounds;
            var totalLengthOfRenderedTranscript = transcriptsBounds.size.x;

            //The critical variables for rendering exons/introns within the already rendered transcript structure. 
            int lengthOfTranscript = (transcriptEnd > transcriptStart) ? transcriptEnd - transcriptStart + 1 : transcriptStart - transcriptEnd + 1;
            var currPositionOfTranscript = eachTranscript.transform.position;
            float currTranscriptXCoord = currPositionOfTranscript.x;
            float leftSideTranscript = currTranscriptXCoord - (totalLengthOfRenderedTranscript / 2);
            float rightSideTranscript = 0 + (totalLengthOfRenderedTranscript / 2);
            double scalingFactorOfTranscript = totalLengthOfRenderedTranscript / lengthOfTranscript;

            float counterTranscriptLength = 0;  //Will keep track of how far along the transcript we are (so the correct 'x' coordinates can be calculated for introns/exons). 
            float renderSizeAsFloatExon = 0;
            Boolean alignmentIsSet = false;

            for (int j = 0; j < exonNum; j++)           //Instantiating the number of exons associated with each transcript. 
            {
                int eachStart = exons[j]["start"].ToObject<int>();
                int eachEnd = exons[j]["end"].ToObject<int>();

                ////Checking for exon sequence alignment. (See code just below which adds all exons to a dictionary. This is what is searched). 
                //if (exonDictionary.ContainsValue(eachStart) && alignmentIsSet == false)
                //{
                //    var keyForExistingExon = exonDictionary.FirstOrDefault(g => g.Value == eachStart).Key;
                //    string[] splitKey = keyForExistingExon.Split('_');
                //    int transcriptIndex = int.Parse(splitKey[0]);
                //    int exonIndex = int.Parse(splitKey[1]);

                //    var findParent = GameObject.Find("ParentObject" + transcriptIndex.ToString());
                //    var findTranscript = findParent.GetComponentInChildren<TranscriptFunctionality>();
                //    GameObject theExon = GameObject.Find("ParentObject" + transcriptIndex.ToString() + "Exons/Exon" + exonIndex.ToString());

                //    float existingExonPosition = theExon.transform.position.x;

                //    positionDictionary.Add(i, existingExonPosition);

                //    alignmentIsSet = true;
                //}

                ////Adding each exon to a Dictionary so they are easily searchable later when we're searching through exons to check for alignment. 
                ////https://www.dotnetperls.com/dictionary
                ////The format of the key is "index of transcript_exonindex".
                //string concatenatedKeyForDictionary = i.ToString() + "_" + j.ToString();
                //exonDictionary.Add(concatenatedKeyForDictionary, eachStart);

                float yCoordOfTranscript = eachTranscript.transform.position.y; //The same 'y' coordinate for every exon in the transcript. 
                float x = 0;

                int lengthOfEachExon = (eachEnd > eachStart) ? eachEnd - eachStart + 1 : eachStart - eachEnd + 1;

                // ----> For first exon/intron:
                if (j == 0)
                {
                    if (eachStart == transcriptStart)       //Render exon straight away if there is no intron sequence in between. 
                    {
                        double exonLength = Math.Abs(lengthOfEachExon);
                        double scaledExonForRender = exonLength * scalingFactorOfTranscript;
                        renderSizeAsFloatExon = (float)scaledExonForRender;

                        x = leftSideTranscript + (renderSizeAsFloatExon / 2);   //The 'x' coordinate for this instantiated exon. 

                        counterTranscriptLength += (leftSideTranscript + renderSizeAsFloatExon);
                    }
                    else
                    {
                        double intronLength = Math.Abs(transcriptStart - eachStart);    //Otherwise there must be an intron in between. 
                        double renderSize = intronLength * scalingFactorOfTranscript;
                        float renderSizeAsFloat = (float)renderSize;

                        var anIntron = Instantiate(exon, new Vector3((leftSideTranscript + (renderSizeAsFloat / 2)), yCoordOfTranscript, 2), Quaternion.identity);
                        anIntron.name = "Intron";
                        anIntron.SetParent(subParent.transform);
                        anIntron.localScale = new Vector3(renderSizeAsFloat, 0.01f, 0.05f);
                        var intronRender = anIntron.GetComponent<Renderer>();
                        intronRender.material.color = intronColor;

                        var scriptAttachedIntron = anIntron.GetComponent<ExonFunctionality>();  //Making sure all instantiated introns/exons have the parent transripts index, as an identifier.
                        scriptAttachedIntron.sameIndexAsParentTranscript = i;

                        counterTranscriptLength += (leftSideTranscript + renderSizeAsFloat);

                        double exonLength = Math.Abs(lengthOfEachExon);
                        double scaledExonForRender = exonLength * scalingFactorOfTranscript;
                        renderSizeAsFloatExon = (float)scaledExonForRender;

                        x = counterTranscriptLength + (renderSizeAsFloatExon / 2);
                        counterTranscriptLength += renderSizeAsFloatExon;
                    }
                }

                // ----> For the other introns/exons (following the first one):
                if (j > 0 && eachStart == exons[j - 1]["end"].ToObject<int>())      //If the start of this exon is the same as the end of the previous one. 
                {
                    double exonLength = Math.Abs(lengthOfEachExon);
                    double scaledExonForRender = exonLength * scalingFactorOfTranscript;
                    renderSizeAsFloatExon = (float)scaledExonForRender;

                    x = leftSideTranscript + (renderSizeAsFloatExon / 2);
                    counterTranscriptLength += renderSizeAsFloatExon;
                }

                else if (j > 0)
                {
                    int prevExonEnd = exons[j - 1]["end"].ToObject<int>();
                    double intronLength = Math.Abs(eachStart - prevExonEnd);
                    double renderSize = intronLength * scalingFactorOfTranscript;
                    float renderSizeAsFloat = (float)renderSize;

                    var anIntron = Instantiate(exon, new Vector3((counterTranscriptLength + (renderSizeAsFloat / 2)), yCoordOfTranscript, 2), Quaternion.identity);
                    anIntron.name = "Intron";
                    anIntron.SetParent(subParent.transform);
                    anIntron.localScale = new Vector3(renderSizeAsFloat, 0.01f, 0.05f);
                    var intronRender = anIntron.GetComponent<Renderer>();
                    intronRender.material.color = intronColor;

                    var scriptAttachedIntron = anIntron.GetComponent<ExonFunctionality>();  
                    scriptAttachedIntron.sameIndexAsParentTranscript = i;

                    counterTranscriptLength += renderSizeAsFloat;

                    double exonLength = Math.Abs(lengthOfEachExon);
                    double scaledExonForRender = exonLength * scalingFactorOfTranscript;
                    renderSizeAsFloatExon = (float)scaledExonForRender;

                    x = counterTranscriptLength + (renderSizeAsFloatExon / 2);
                    counterTranscriptLength += renderSizeAsFloatExon;
                }

                var eachExonInTranscript = Instantiate(exon, new Vector3(x, yCoordOfTranscript, 2), Quaternion.identity);
                eachExonInTranscript.localScale = new Vector3(renderSizeAsFloatExon, 0.03f, 0.06f);     //Resizing according to relative sizes of sequences. 

                var scriptAttached = eachExonInTranscript.GetComponent<ExonFunctionality>();
                scriptAttached.indexOfExon = j;
                scriptAttached.sameIndexAsParentTranscript = i;
                eachExonInTranscript.name = "Exon" + j.ToString();  //So the exons can be identified. 

                eachExonInTranscript.SetParent(subParent.transform); //Saves exons inside the parent object of corresponding transcript. 

                //Apply colours to exons. 
                var exonRender = eachExonInTranscript.GetComponent<Renderer>();
                exonRender.material.color = exonColor;
            }
        }

        foreach (KeyValuePair<int, float> pair in positionDictionary)
        {
            var findParent = GameObject.Find("ParentObject" + pair.Key.ToString());
            var currPosition = findParent.transform.position;
            currPosition = new Vector3(pair.Value, currPosition.y, currPosition.z);
        }

        setParentOfGene();
    }

    private void setParentOfGene()
    {
        GameObject theGene = new GameObject();
        theGene.name = "Gene";

        for(int j=0; j<numberOfTranscripts; j++)
        {
            GameObject eachParent = GameObject.Find("ParentObject" + j.ToString());
            eachParent.transform.SetParent(theGene.transform);
        }

        GameObject addAdditionalsToParent = GameObject.Find("AdditionalElements");
        addAdditionalsToParent.transform.SetParent(theGene.transform);
    }

    void Update()
    {

    }
}

