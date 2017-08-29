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
using UnityEngine.EventSystems;

//This script builds the stack of transcripts.
//It renders them according to their relative size. 
//The stack has the longest transcript at the top.

public class GeneGenerator : MonoBehaviour
{
    //Global variables:
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
    public string parentGeneName;

    //Local variable:
    public Transform transcript;
    public Boolean scalerPanelAndTitleAdded = false;
    public Transform transcriptNumLabel;
    public Transform chromStartLabel;
    public Transform chromEndLabel;
    public Transform geneTitleLabel;

    void Start()
    {
        arrayOfTranscripts = GlobalControl.Instance.arrayOfTranscripts;
        allTranscripts = GlobalControl.Instance.allTranscripts;
        lengthsOfTranscripts = GlobalControl.Instance.lengthsOfTranscripts;
        numberOfTranscripts = GlobalControl.Instance.numberOfTranscripts;
        gene = GlobalControl.Instance.gene;
        chromosomeStart = GlobalControl.Instance.chromosomeStart;
        chromosomeEnd = GlobalControl.Instance.chromosomeEnd;
        parentGeneName = GlobalControl.Instance.parentGeneName;

        int shortestT = lengthsOfTranscripts.Min();  

        int lengthOfChromosome = (chromosomeEnd > chromosomeStart) ? chromosomeEnd - chromosomeStart + 1 : chromosomeStart - chromosomeEnd + 1;
        double chromsomeLengthScaler = (lengthOfChromosome / 1.5);

        //Rendering the total chromosome as an object so it can act as a scaler for length. 
        var chromosomeScalerObj = Instantiate(transcript, new Vector3(0, 0.2f, 2), Quaternion.identity);
        chromosomeScalerObj.localScale = new Vector3((float)((chromsomeLengthScaler / chromsomeLengthScaler) / 1.5), 0.01f, 0.01f); //Resizing based on total length. 
        //Polishing - name in Unity hierarchy & color. 
        chromosomeScalerObj.name = "Chromosome Scaler";
        var render = chromosomeScalerObj.GetComponent<Renderer>();
        render.material.color = Color.white;

        //Total length of the newly rendered chromosome scaler. 
        var chromosomeScalerBounds = render.bounds;
        var totalLengthOfRenderedScaler = chromosomeScalerBounds.size.x;
        //Creating a reasonable scaling factor for rendering the transcripts (relative to the length of the chromosome coords). 
        double scalingFactorOfChromosome = totalLengthOfRenderedScaler / lengthOfChromosome;
        //The game object is rendered at '0' x, so it's actual starting point in the scene is half the total length. 
        float leftSideScaler = 0 - (totalLengthOfRenderedScaler / 2);

        float y = (float) 0.14;

        for (int i = 0; i < numberOfTranscripts; i++)
        {
            //The transcripts are rendered such that the longest is on top of the stack, and those below it are shorter. 
            int longestTranscript = lengthsOfTranscripts.Max();
            indexOfTranscript = lengthsOfTranscripts.ToList().IndexOf(longestTranscript);

            //Getting the length of the actual transcript. 
            JObject eachTranscript = allTranscripts[indexOfTranscript].ToObject<JObject>();
            int transcriptStart = (int)eachTranscript.GetValue("start");
            int transcriptEnd = (int)eachTranscript.GetValue("end");
            int lengthOfTranscript = (transcriptEnd > transcriptStart) ? transcriptEnd - transcriptStart + 1 : transcriptStart - transcriptEnd + 1;
            double lengthTranscript = Math.Abs(lengthOfTranscript);
            //Calculating the relative render length of this transcript based on the scaling factor. 
            double scaledTranscriptLengthForRender = lengthOfTranscript * scalingFactorOfChromosome;
            float scaledSizeAsFloatForTranscript = (float)scaledTranscriptLengthForRender;

            //Getting the length difference between the chromosome starting point and the transcript starting point. And scaling it as above. 
            int lengthTranscriptStartToChromosomeStart = (transcriptStart > chromosomeStart) ? transcriptStart - chromosomeStart + 1 : chromosomeStart - transcriptStart + 1;
            double transcriptStartToChromosomeStart = Math.Abs(lengthTranscriptStartToChromosomeStart);
            double scaledTranscriptForRender = transcriptStartToChromosomeStart * scalingFactorOfChromosome;
            float scaledSizeAsFloat = (float)scaledTranscriptForRender;

            //The 'x' coordinate for each rendered transcript:
            float x = leftSideScaler + scaledSizeAsFloat + (scaledSizeAsFloatForTranscript / 2);
            //Debug.Log(x + " $$$$$$$$$$");
            var eachTranscriptInGene = Instantiate(transcript, new Vector3(x, y, 2), Quaternion.identity);
            y -= 0.04f;       //The 'space' between the rendered transcripts in the stack. 'y' coordinate. 

            //Re-sizing the transcript relative to the total length of the chromosome.
            double transcriptLengthScaler = ((lengthsOfTranscripts[indexOfTranscript]) / 1.5);
            double scalingFactor = (transcriptLengthScaler / chromsomeLengthScaler) / 1.5;
            eachTranscriptInGene.localScale = new Vector3((float)scalingFactor, 0.03f, 0.03f);

            var scriptAttached = eachTranscriptInGene.GetComponent<TranscriptFunctionality>();

            scriptAttached.indexOfTranscript = indexOfTranscript;   //Storing these values in the the variables of this script (attached to 'Transcript' prefab). 
            scriptAttached.lastTranscriptIndex = lengthsOfTranscripts.ToList().IndexOf(shortestT);
            eachTranscriptInGene.name = "Transcript";

            var theParent = new GameObject();
            eachTranscriptInGene.SetParent(theParent.transform);
            theParent.name = "ParentObject" + indexOfTranscript.ToString();
            theParent.AddComponent<BoxCollider>();
            theParent.AddComponent<TranscriptTapResponder>();

            lengthsOfTranscripts[indexOfTranscript] = 0;    //Setting the current biggest to zero so we will find the next biggest in the List.

            /* __________________________
            |                            |
            |  Labels for Transcripts :  |
            |____________________________| */

            //Getting the correct parent object. 
            var parent = GameObject.Find("ParentObject" + indexOfTranscript.ToString());
            var transcriptInParent = theParent.GetComponentInChildren<TranscriptFunctionality>();

            //Creating the child object to hold all the text. 
            GameObject childObjectForText = new GameObject();
            childObjectForText.transform.name = "Text";
            childObjectForText.transform.SetParent(parent.transform);

            //Getting total render length of transcript in order to calculate correct label psoition. 
            var transcriptRenderer = transcriptInParent.GetComponent<Renderer>();
            var transcriptsBounds = transcriptRenderer.bounds;
            var xCoordForTranscript = transcriptsBounds.size.x;
            var positionOfTranscript = transcriptInParent.transform.position;

            //Creating a label for each transcript. 
            var transcriptLabel = Instantiate(transcriptNumLabel, new Vector3(leftSideScaler - 0.11f, positionOfTranscript.y + 0.01f, positionOfTranscript.z), Quaternion.identity);
            transcriptLabel.name = "Label-TranscriptNum";
            transcriptLabel.transform.SetParent(childObjectForText.transform);
            var labelForTranscript = transcriptLabel.GetComponent<TextMesh>();
            labelForTranscript.text = "Transcript " + (i + 1).ToString();
            transcriptLabel.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        }

        /* ____________________________
        |                              |
        |  Label for Megabase Scale :  |
        |______________________________| */

        if (!scalerPanelAndTitleAdded)
        {
            var chromosomeScalerObjPosition = chromosomeScalerObj.transform.position;
            float rightSideScaler = 0 + (totalLengthOfRenderedScaler / 2);

            var seqStartLabel = Instantiate(chromStartLabel, new Vector3(leftSideScaler, 0.23f, chromosomeScalerObjPosition.z), Quaternion.identity);
            seqStartLabel.name = "Label_SequenceStart";
            var startText = seqStartLabel.GetComponent<TextMesh>();

            var seqEndLabel = Instantiate(chromEndLabel, new Vector3(rightSideScaler, 0.23f, chromosomeScalerObjPosition.z), Quaternion.identity);
            seqEndLabel.name = "Label_SequenceEnd";
            var endText = seqEndLabel.GetComponent<TextMesh>();

            startText.color = Color.blue;
            endText.color = Color.blue;
            startText.text = "Chromosome Megabase Scale:   " + chromosomeStart.ToString();
            endText.text = chromosomeEnd.ToString();

            startText.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
            endText.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);

            //Repositioning right-side text so it fits nicely:
            var renderEndLabel = seqEndLabel.GetComponent<Renderer>();
            var endLabelBounds = renderEndLabel.bounds;
            var totalLengthEndLabel = endLabelBounds.size.x;
            var currPositionMegabase = seqEndLabel.transform.position;
            seqEndLabel.transform.position = new Vector3(currPositionMegabase.x - (totalLengthEndLabel), currPositionMegabase.y, currPositionMegabase.z);

            /* _______________
            |                |
            |  Gene Title :  |
            |________________| */

            //Updating the Gene Title. 
            var geneTitle = Instantiate(geneTitleLabel, new Vector3(0.2f, 0.2f, 0.2f), Quaternion.identity);
            geneTitle.name = "GeneTitle";
            var geneText = geneTitle.GetComponent<TextMesh>();
            geneText.text = "Gene Name:  " + gene.GeneName + "    |    Id:  " + gene.Id + "    |    Details:  " + gene.Desc;
            geneText.color = Color.white;
            geneText.fontSize = 8;

            geneTitle.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            var renderTitle = geneTitle.GetComponent<Renderer>();
            var renderTitleBounds = renderTitle.bounds;
            var totalLengthRenderedTitle = renderTitleBounds.size.x;
            float titleStartingPoint = 0 - totalLengthRenderedTitle / 2;

            //Setting the gene title's position. 
            geneTitle.transform.position = new Vector3(titleStartingPoint, currPositionMegabase.y + 0.03f, currPositionMegabase.z);

            /* __________________
            |                   |
            |  Setting Parent:  |
            |___________________| */

            GameObject parentComponent = new GameObject();
            parentComponent.name = "AdditionalElements";
            chromosomeScalerObj.transform.SetParent(parentComponent.transform);
            geneTitle.transform.SetParent(parentComponent.transform);
            seqStartLabel.transform.SetParent(parentComponent.transform);
            seqEndLabel.transform.SetParent(parentComponent.transform);

            scalerPanelAndTitleAdded = true;
        }
    }

    void Update()
    {

    }

    public interface IFocusable : IEventSystemHandler
    {
        void OnFocusEnter();
        void OnFocusExit();
    }
}