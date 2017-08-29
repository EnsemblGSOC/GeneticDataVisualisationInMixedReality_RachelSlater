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
using UnityEngine.UI;

//This class implements IInputClickHandler to handle the tap gesture.

public class CollapseTranscript : MonoBehaviour, IInputClickHandler
{
    //Local variables:
    public float nextXPosition;
    public float previousRenderSize;
    public float firstExonIntronxCoord;
    public float firstExonIntronLength;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        GameObject collapseTranscript = GameObject.Find("CollapseTranscript");
        //collapseTranscript.GetComponent<Button>().interactable = false;
        //collapseTranscript.GetComponent<Button>().enabled = false;
        Destroy(collapseTranscript.GetComponent<Button>());

        float totalCollapsedExonIntronLength = 0;

        GameObject theTranscript = GameObject.Find("Transcript");
        Vector3 transcriptSize = theTranscript.transform.localScale;

        GameObject components = GameObject.Find("Exons");
        var exonsAndIntrons = components.GetComponentsInChildren<ExonFunctionality>();
        int numberOfExonsAndIntrons = exonsAndIntrons.Count();

        for (int i = 0; i < numberOfExonsAndIntrons; i++)
        {
            if (exonsAndIntrons[i].name.Contains("Exon"))
            {
                var eachExon = exonsAndIntrons[i];
                Vector3 exonSize = eachExon.transform.localScale;

                eachExon.transform.localScale = new Vector3(0.03f, 0.12f, exonSize.z);
                totalCollapsedExonIntronLength += 0.03f;

                var render = eachExon.GetComponent<Renderer>();
                var exonBounds = render.bounds;
                var renderedExonLength = exonBounds.size.x;

                Vector3 exonPosition = eachExon.transform.position;

                if (i == 0)
                {
                    nextXPosition = exonPosition.x;
                    previousRenderSize = renderedExonLength;
                    firstExonIntronxCoord = exonPosition.x;
                    firstExonIntronLength = renderedExonLength;
                }
                else
                {
                    nextXPosition = (nextXPosition + previousRenderSize / 2 + renderedExonLength / 2);
                    previousRenderSize = renderedExonLength;
                    eachExon.transform.position = new Vector3(nextXPosition, exonPosition.y, 2);
                }
            }

            else if (exonsAndIntrons[i].name.Equals("Intron"))
            {
                var eachIntron = exonsAndIntrons[i];
                Vector3 intronSize = eachIntron.transform.localScale;
                eachIntron.transform.localScale = new Vector3(0.015f, 0.02f, intronSize.z);
                totalCollapsedExonIntronLength += 0.015f;

                var render = eachIntron.GetComponent<Renderer>();
                var intronBounds = render.bounds;
                var renderedIntronLength = intronBounds.size.x;

                Vector3 intronPosition = eachIntron.transform.position;

                if (i == 0)
                {
                    nextXPosition = intronPosition.x;
                    previousRenderSize = renderedIntronLength;
                    firstExonIntronxCoord = intronPosition.x;
                    firstExonIntronLength = renderedIntronLength;
                }
                else
                {
                    nextXPosition = (nextXPosition + previousRenderSize / 2 + renderedIntronLength / 2);
                    previousRenderSize = renderedIntronLength;
                    eachIntron.transform.position = new Vector3(nextXPosition, intronPosition.y, 2);
                }
            }
            theTranscript.transform.localScale = new Vector3(0, 0, 0);
        }

        GameObject parentObj = GameObject.Find("TheSelectedTranscript");
        var currPosition = parentObj.transform.position;
        parentObj.transform.position = new Vector3(0.04f, 0.04f, currPosition.z);
    }








    //DELETE - just for Unity testing. 
    void OnMouseDown()
    {
        GameObject collapseTranscript = GameObject.Find("TEST (TO DELETE) (1)");
        Destroy(collapseTranscript.GetComponent<Button>());

        float totalCollapsedExonIntronLength = 0;

        GameObject theTranscript = GameObject.Find("Transcript");
        Vector3 transcriptSize = theTranscript.transform.localScale;

        GameObject components = GameObject.Find("Exons");
        var exonsAndIntrons = components.GetComponentsInChildren<ExonFunctionality>();
        int numberOfExonsAndIntrons = exonsAndIntrons.Count();

        for (int i = 0; i < numberOfExonsAndIntrons; i++)
        {
            if (exonsAndIntrons[i].name.Contains("Exon"))
            {
                var eachExon = exonsAndIntrons[i];
                Vector3 exonSize = eachExon.transform.localScale;

                eachExon.transform.localScale = new Vector3(0.03f, 0.12f, exonSize.z);
                totalCollapsedExonIntronLength += 0.03f;

                var render = eachExon.GetComponent<Renderer>();
                var exonBounds = render.bounds;
                var renderedExonLength = exonBounds.size.x;

                Vector3 exonPosition = eachExon.transform.position;

                if (i == 0)
                {
                    nextXPosition = exonPosition.x;
                    previousRenderSize = renderedExonLength;
                    firstExonIntronxCoord = exonPosition.x;
                    firstExonIntronLength = renderedExonLength;
                }
                else
                {
                    nextXPosition = (nextXPosition + previousRenderSize / 2 + renderedExonLength / 2);
                    previousRenderSize = renderedExonLength;
                    eachExon.transform.position = new Vector3(nextXPosition, exonPosition.y, 2);
                }
            }

            else if (exonsAndIntrons[i].name.Equals("Intron"))
            {
                var eachIntron = exonsAndIntrons[i];
                Vector3 intronSize = eachIntron.transform.localScale;
                eachIntron.transform.localScale = new Vector3(0.02f, 0.02f, intronSize.z);
                totalCollapsedExonIntronLength += 0.02f;

                var render = eachIntron.GetComponent<Renderer>();
                var intronBounds = render.bounds;
                var renderedIntronLength = intronBounds.size.x;

                Vector3 intronPosition = eachIntron.transform.position;

                if (i == 0)
                {
                    nextXPosition = intronPosition.x;
                    previousRenderSize = renderedIntronLength;
                    firstExonIntronxCoord = intronPosition.x;
                    firstExonIntronLength = renderedIntronLength;
                }
                else
                {
                    nextXPosition = (nextXPosition + previousRenderSize / 2 + renderedIntronLength / 2);
                    previousRenderSize = renderedIntronLength;
                    eachIntron.transform.position = new Vector3(nextXPosition, intronPosition.y, 2);
                }
            }
            theTranscript.transform.localScale = new Vector3(0, 0, 0);
        }

        GameObject parentObj = GameObject.Find("TheSelectedTranscript");
        var currPosition = parentObj.transform.position;
        parentObj.transform.position = new Vector3(0.04f, -0.04f, currPosition.z);
    }
}