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

public class ExonFunctionality : MonoBehaviour
{
    //Global variables:
    public JToken arrayOfTranscripts;
    public List<JToken> allTranscripts = new List<JToken>();
    public List<JToken> allExons = new List<JToken>();
    public List<int> lengthsOfTranscripts = new List<int>();
    public int numberOfTranscripts;
    public RetrieveRESTData.GeneSequenceAndChrosomome gene;
    public int indexOfTranscript;
    public int numberOfExonsInTranscript;
    public int indexOfExon;

    //Local variables:
    public JObject singleTranscript;
    public int sameIndexAsParentTranscript;
    public string exonId;
    public string exonNucleotideSequence;
    public string exonChromosomeLocation;
    public int exonNucleotideSeqLength;
    public string exonStart;
    public string exonEnd;
    public string exonRegion;

    void Start()
    {
        arrayOfTranscripts = GlobalControl.Instance.arrayOfTranscripts;
        allTranscripts = GlobalControl.Instance.allTranscripts;
        allExons = GlobalControl.Instance.allExons;
        lengthsOfTranscripts = GlobalControl.Instance.lengthsOfTranscripts;
        indexOfTranscript = GlobalControl.Instance.indexOfTranscript;
        numberOfTranscripts = GlobalControl.Instance.numberOfTranscripts;
        numberOfExonsInTranscript = GlobalControl.Instance.numberOfExonsInTranscript;
        gene = GlobalControl.Instance.gene;
        singleTranscript = GlobalControl.Instance.singleTranscript;
      
        attachDataToExon();
    }

    private void attachDataToExon()
    {
        //if (singleTranscript == null)
        //{
        //    JObject theExon = allExons[indexOfExon].ToObject<JObject>();
        //} else {
        //    JArray exons = singleTranscript["Exon"].Value<JArray>();
        //}
    }

    void Update()
    {
    }
}
