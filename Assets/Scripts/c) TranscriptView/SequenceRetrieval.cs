using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SequenceRetrieval : MonoBehaviour
{
    //Local variables:
    public string singleTranscriptId;
    public JArray exons;

    void Start()
    {
        StartCoroutine("getEachExonSequence");
    }

    IEnumerator getEachExonSequence()
    {
        int numberOfExons = exons.Count;

        for (int j = 0; j < numberOfExons; j++)
        {
            string exonId = exons[j]["id"].ToString();
            StartCoroutine(getTranscriptSequence(exonId, j));

            if (j == 14 || j == 28 || j == 42 || j == 56 || j == 70 || j == 84 || j == 98)       //As API request limit is 15 per second. Need to wait.
            {
                yield return new WaitForSecondsRealtime(1);
            }
        }
    }


    public IEnumerator getTranscriptSequence(string exonId, int j)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://rest.ensembl.org" + "/sequence/id/" + exonId + "?content-type=text/plain"))
        {   //'mask_feature=1' --> Mask features on the sequence. If sequence is genomic, mask introns. If sequence is cDNA, mask UTRs. Incompatible with the 'mask' option.
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }

            else // Success
            {
                string exonSequenceData = request.downloadHandler.text;
                string id = "";
                string chromosomeLocation = "";
                string sequence = "";

                try
                {
                    JObject exonSequenceJson = JObject.Parse(exonSequenceData.ToString());
                    id = exonSequenceJson["id"].ToString();
                    chromosomeLocation = exonSequenceJson["desc"].ToString();
                    sequence = exonSequenceJson["seq"].ToString();
                }
                catch (JsonReaderException e)
                {
                    Debug.Log("JSON not recognised --> " + exonSequenceData);
                }

                GameObject theMatchingExon = GameObject.Find("Exon" + j.ToString());
                var exonFuncScript = theMatchingExon.GetComponent<ExonFunctionality>();
                exonFuncScript.exonId = id;
                exonFuncScript.exonNucleotideSequence = sequence;
                exonFuncScript.exonChromosomeLocation = chromosomeLocation;
                exonFuncScript.exonNucleotideSeqLength = sequence.Length;
            }
        }
    }
}

