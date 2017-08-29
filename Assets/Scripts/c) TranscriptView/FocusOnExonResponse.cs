using HoloToolkit.Unity.InputModule;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

// This class implements IFocusable to respond to gaze changes.
// It highlights the object being gazed at.

public class FocusOnExonResponse : MonoBehaviour, IFocusable
{
    //Local variables:
    private int focussedTranscriptIndex;
    public Transform seqTextPanel;
    public Vector3 originalExonSize;
    public Vector3 originalExonPosition;
    public Vector3 originalTranscriptPosition;
    public GameObject theTranscript;
    public GameObject exonDataDisplay;

    public void OnFocusEnter()
    {
        theTranscript = GameObject.Find("TheSelectedTranscript");
        originalTranscriptPosition = theTranscript.transform.position;

        if (gameObject.name.Contains("Exon"))
        {
            originalExonSize = gameObject.transform.localScale;
            originalExonPosition = gameObject.transform.position;

            GameObject transcript = GameObject.Find("Transcript");

            if (transcript.transform.localScale == new Vector3(0, 0, 0))
            {
                exonDataDisplay = GameObject.Find("EmptySeqExonPanel");
                exonDataDisplay.transform.localScale = new Vector3(1, 1, 1);

                GameObject exonDataPanel = GameObject.Find("DataValuesExonPanel");
                var allExonDataValues = exonDataPanel.GetComponentsInChildren<Text>();

                var theFocussedExonsData = gameObject.GetComponent<ExonFunctionality>();
                string id = theFocussedExonsData.exonId;
                string sequence = theFocussedExonsData.exonNucleotideSequence;
                string chromosomeLocation = theFocussedExonsData.exonChromosomeLocation;

                allExonDataValues[0].text = id;
                allExonDataValues[1].text = chromosomeLocation;
                allExonDataValues[2].text = sequence;

                var dataPanelPosition = exonDataDisplay.transform.position;
                exonDataDisplay.transform.position = new Vector3(originalExonPosition.x + 0.1f, dataPanelPosition.y, dataPanelPosition.z);

                //Display effects: (Focussed exon gets large, and whole transcript goes backwards.
                gameObject.transform.localScale = new Vector3(originalExonSize.x * 4, originalExonSize.y * 4, originalExonSize.z * 4);
                theTranscript.transform.position = new Vector3(originalTranscriptPosition.x, originalTranscriptPosition.y, originalTranscriptPosition.z + 2);

                //Getting variation data:
                GameObject fetchVariationDataButton = GameObject.Find("FetchVariationData");
                var attachingRequestScript = fetchVariationDataButton.AddComponent<FetchVariationData>();

                //Need to be available to make requests for variation data:
                attachingRequestScript.exonId = id;
                attachingRequestScript.focussedExonStart = theFocussedExonsData.exonStart;
                attachingRequestScript.focussedExonEnd = theFocussedExonsData.exonEnd;
                attachingRequestScript.region = theFocussedExonsData.exonRegion;
                attachingRequestScript.exonSequence = sequence;
            }
        }

    }

    public void OnFocusExit()
    {
        exonDataDisplay = GameObject.Find("EmptySeqExonPanel");
        exonDataDisplay.transform.localScale = new Vector3(0, 0, 0);

        gameObject.transform.localScale = new Vector3(originalExonSize.x, originalExonSize.y, originalExonSize.z);
        theTranscript.transform.position = new Vector3(originalTranscriptPosition.x, originalTranscriptPosition.y, originalTranscriptPosition.z);
    }
}
