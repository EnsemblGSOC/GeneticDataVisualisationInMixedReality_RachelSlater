using System;
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

//This script builds a single transcript.

public class SingleTranscriptGenerator : MonoBehaviour
{
    //Global variable:
    public JObject singleTranscript;

    //Local variables:
    public Transform transcript;
    public Transform exon;
    public Transform transcriptTitleLabel;
    public TextMesh transcriptText;
    public Boolean readyToBuildExonsAndIntrons = false;
    public int transcriptStart;
    public int transcriptEnd;
    public string transcriptId;
    public double scalingFactorOfTranscript;
    public float leftSide;
    public float rightSide;
    public Vector3 transcriptPosition;
    public GameObject theParent;
    public JArray exons;
    public bool readyToFetchRESTData = false;
    public JArray sortedArrayOfExons;

    void Start()
    {
        singleTranscript = GlobalControl.Instance.singleTranscript;
        buildTranscript();
        StartCoroutine("BuildExonsAndIntrons");
    }

    private void buildTranscript()
    {
        transcriptId = singleTranscript["id"].Value<String>();
        //Getting the length of the actual transcript. 
        transcriptStart = singleTranscript["start"].Value<int>();
        transcriptEnd = singleTranscript["end"].Value<int>();
        int lengthOfTranscript = (transcriptEnd > transcriptStart) ? transcriptEnd - transcriptStart + 1 : transcriptStart - transcriptEnd + 1;

        var theTranscript = Instantiate(transcript, new Vector3(0.2f, 0.1f, 2), Quaternion.identity);
        theTranscript.name = "Transcript";
        theTranscript.localScale = new Vector3(0.8f, 0.04f, 0.04f);

        theParent = new GameObject();
        theTranscript.SetParent(theParent.transform);
        theParent.name = "TheSelectedTranscript";

        Color transcriptColor = Color.grey;

        var render = theTranscript.GetComponent<Renderer>();
        render.material.color = transcriptColor;
        var transcriptBounds = render.bounds;
        var totalLengthOfRenderedTranscript = transcriptBounds.size.x;
        transcriptPosition = theTranscript.transform.position;
        scalingFactorOfTranscript = totalLengthOfRenderedTranscript / lengthOfTranscript;

        leftSide = 0.2f - (totalLengthOfRenderedTranscript / 2);
        rightSide = 0.2f + (totalLengthOfRenderedTranscript / 2);

        /* _______________
        |                |
        |  Gene Title :  |
        |________________| */

        //Updating the Gene Title. 
        var transcriptTitle = Instantiate(transcriptTitleLabel, new Vector3(-0.18f,0.33f, 2), Quaternion.identity);
        transcriptTitle.name = "TranscriptTitle";
        var geneText = transcriptTitle.GetComponent<TextMesh>();

        geneText.text = "Transcript Name:  " + singleTranscript["display_name"].Value<string>() + "  |    Id: " + singleTranscript["id"].Value<string>();
        geneText.color = Color.white;
        geneText.fontSize = 10;

        transcriptTitle.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        GameObject theText = new GameObject();
        theText.name = "AllSceneText";
        transcriptTitle.transform.SetParent(theText.transform);

        StartCoroutine("attachSequenceRetrievalScript");

        readyToBuildExonsAndIntrons = true;
    }

    void Update()
    {

    }

    IEnumerator BuildExonsAndIntrons()
    {
        while (!readyToBuildExonsAndIntrons)
        {
            yield return new WaitForSeconds(10);
        }
        buildExonsAndIntrons();
    }

    private void buildExonsAndIntrons()
    {
        JArray exons = singleTranscript["Exon"].Value<JArray>();
        sortedArrayOfExons = new JArray(exons.OrderBy(JToken => JToken["start"]));

        int exonNum = sortedArrayOfExons.Count();

        float counterTranscriptLength = 0;  //Will keep track of how far along the transcript we are (so the correct 'x' coordinates can be calculated for introns/exons). 
        float renderSizeAsFloatExon = 0;

        //Creating a new child to hold the exons. 
        GameObject subParent = new GameObject();
        subParent.transform.name = "Exons";
        subParent.transform.SetParent(theParent.transform);

        Color exonColor = Color.cyan;
        Color intronColor = Color.white;

        for (int j = 0; j < exonNum; j++)           //Instantiating the number of exons associated with each transcript. 
        {
            int eachStart = sortedArrayOfExons[j]["start"].ToObject<int>();
            int eachEnd = sortedArrayOfExons[j]["end"].ToObject<int>();
            int lengthOfEachExon = (eachEnd > eachStart) ? eachEnd - eachStart + 1 : eachStart - eachEnd + 1;
            string eachExonRegion = sortedArrayOfExons[j]["seq_region_name"].ToString();

            float x = 0;

            // ----> For first exon/intron:
            if (j == 0)
            {
                if (eachStart == transcriptStart)       //Render exon straight away if there is no intron sequence in between. 
                {
                    double exonLength = Math.Abs(lengthOfEachExon);
                    double scaledExonForRender = exonLength * scalingFactorOfTranscript;
                    renderSizeAsFloatExon = (float)scaledExonForRender;

                    x = leftSide + (renderSizeAsFloatExon / 2);   //The 'x' coordinate for this instantiated exon. 

                    counterTranscriptLength += (leftSide + renderSizeAsFloatExon);
                }
                else
                {
                    double intronLength = Math.Abs(transcriptStart - eachStart);    //Otherwise there must be an intron in between. 
                    double renderSize = intronLength * scalingFactorOfTranscript;
                    float renderSizeAsFloat = (float)renderSize;

                    var anIntron = Instantiate(exon, new Vector3((leftSide + (renderSizeAsFloat / 2)), transcriptPosition.y, 2), Quaternion.identity);

                    anIntron.name = "Intron";
                    anIntron.SetParent(subParent.transform);
                    anIntron.localScale = new Vector3(renderSizeAsFloat, 0.015f, 0.07f);
                    var intronRender = anIntron.GetComponent<Renderer>();
                    intronRender.material.color = intronColor;

                    counterTranscriptLength += (leftSide + renderSizeAsFloat);

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

                x = leftSide + (renderSizeAsFloatExon / 2);
                counterTranscriptLength += renderSizeAsFloatExon;
            }

            else if (j > 0)
            {
                int prevExonEnd = sortedArrayOfExons[j - 1]["end"].ToObject<int>();
                double intronLength = Math.Abs(eachStart - prevExonEnd);
                double renderSize = intronLength * scalingFactorOfTranscript;
                float renderSizeAsFloat = (float)renderSize;

                var anIntron = Instantiate(exon, new Vector3((counterTranscriptLength + (renderSizeAsFloat / 2)), transcriptPosition.y, 2), Quaternion.identity);
                anIntron.name = "Intron";
                anIntron.SetParent(subParent.transform);
                anIntron.localScale = new Vector3(renderSizeAsFloat, 0.015f, 0.07f);
                var intronRender = anIntron.GetComponent<Renderer>();
                intronRender.material.color = intronColor;

                counterTranscriptLength += renderSizeAsFloat;

                double exonLength = Math.Abs(lengthOfEachExon);
                double scaledExonForRender = exonLength * scalingFactorOfTranscript;
                renderSizeAsFloatExon = (float)scaledExonForRender;

                x = counterTranscriptLength + (renderSizeAsFloatExon / 2);
                counterTranscriptLength += renderSizeAsFloatExon;
            }

            var eachExonInTranscript = Instantiate(exon, new Vector3(x, transcriptPosition.y, 2), Quaternion.identity);
            eachExonInTranscript.localScale = new Vector3(renderSizeAsFloatExon, 0.04f, 0.08f);     //Resizing according to relative sizes of sequences. 

            var scriptAttached = eachExonInTranscript.GetComponent<ExonFunctionality>();
            scriptAttached.indexOfExon = j;
            eachExonInTranscript.name = "Exon" + j.ToString();  //So the exons can be identified.
            scriptAttached.exonStart = eachStart.ToString();
            scriptAttached.exonEnd = eachEnd.ToString();
            scriptAttached.exonRegion = eachExonRegion; 

            eachExonInTranscript.SetParent(subParent.transform); //Saves exons inside the parent object of corresponding transcript. 

            //Apply colours to exons. 
            var exonRender = eachExonInTranscript.GetComponent<Renderer>();
            exonRender.material.color = exonColor;

            readyToFetchRESTData = true;
        }
    }

    IEnumerator attachSequenceRetrievalScript()
    {
        while (!readyToFetchRESTData)
        {
            yield return new WaitForSeconds(2);
        }
        GameObject attachScriptToGetSequenceData = GameObject.Find("Transcript");
        var scriptAdded = attachScriptToGetSequenceData.AddComponent<SequenceRetrieval>();
        scriptAdded.singleTranscriptId = transcriptId;
        scriptAdded.exons = sortedArrayOfExons;
    }
}