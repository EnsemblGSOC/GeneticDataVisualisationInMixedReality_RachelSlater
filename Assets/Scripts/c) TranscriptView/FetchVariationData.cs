using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class FetchVariationData : MonoBehaviour, IInputClickHandler
{
    /* _________________
    |                    |
    |  Internal Class:   |
    |____________________| */

    public class ExonVariationDataModel
    {
        public ExonVariationDataModel(string id, string source, string alleles, string clinicalSignificance, string consequenceType, string start, string end, string phenotypes)
        {
            Id = id;
            Source = source;
            Alleles = alleles;
            ClinicalSignificance = clinicalSignificance;
            ConsequenceType = consequenceType;
            Start = start;
            End = end;
            Phenotypes = phenotypes;
        }

        public string Id { get; set; }
        public string Source { get; set; }
        public string Alleles { get; set; }
        public string ClinicalSignificance { get; set; }
        public string ConsequenceType { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Phenotypes { get; set; }
    }

    //Global variables:
    public RetrieveRESTData.GeneSequenceAndChrosomome gene;
    public JObject singleTranscript;

    //Local variables:
    public string exonId;
    public string geneId;
    public string transcriptId;
    public string translationId;
    public string focussedExonStart;
    public string focussedExonEnd;
    public string region;
    public string exonSequence;
    public List<JObject> variationsInExon = new List<JObject>();
    public JArray arrayOfTranscriptVariations;
    Dictionary<int, JObject> transcriptVariations = new Dictionary<int, JObject>();
    Dictionary<int, string> mutatedAlleleDictionary = new Dictionary<int, string>();
    public Boolean readyToAccessDataModel = false;
    public string exonMegabaseCoordsSearch;
    public bool triggerWaitPeriod;

    public string variation_Id;
    public string variation_Source;
    public string variation_Alleles;
    public string variation_ClinicalSignificance;
    public string variation_ConsequenceType;
    public string variation_Start;
    public string variation_End;
    public string variation_Phenotypes;

    public ExonVariationDataModel eachVariation;
    public List<ExonVariationDataModel> listOfExonVariations = new List<ExonVariationDataModel>();

    void Start()
    {
        gene = GlobalControl.Instance.gene;
        geneId = gene.Id;

        singleTranscript = GlobalControl.Instance.singleTranscript;
        transcriptId = singleTranscript["id"].ToString();

        JObject transcriptTranslationObj = (JObject)singleTranscript["Translation"];
        translationId = transcriptTranslationObj.GetValue("id").ToString();

        exonMegabaseCoordsSearch = region + ":" + focussedExonStart + "-" + focussedExonEnd;

        StartCoroutine("Wait");        //Necessary as otherwise request limit is exceeded from previous script. 
        triggerWaitPeriod = true;
        triggerWaitPeriod = false;
    }

    IEnumerator Wait()
    {
        while (triggerWaitPeriod)
        {
            yield return new WaitForSeconds(1);
        }
        StartCoroutine("fetchVariationData");
    }

    public IEnumerator fetchVariationData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://rest.ensembl.org" + "/overlap/region/homo_sapiens/" + exonMegabaseCoordsSearch + "?feature=variation;content-type=application/json"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }

            else // Success
            {
                string transcriptVariationData = request.downloadHandler.text;
                arrayOfTranscriptVariations = JArray.Parse(transcriptVariationData.ToString());
                StartCoroutine(extractMatchingVariationCoords(arrayOfTranscriptVariations));
            }
        }
    }

    public IEnumerator extractMatchingVariationCoords(JArray arrayOfTranscriptVariations)
    {
        for (int i = 0; i < arrayOfTranscriptVariations.Count; i++)
        {
            JObject eachVariation = arrayOfTranscriptVariations[i].ToObject<JObject>();

            //Setting all values for data model. 
            variation_Id = eachVariation.GetValue("id").ToString();
            variation_Source = eachVariation.GetValue("source").ToString();
            variation_Alleles = eachVariation.GetValue("alleles").ToString();
            variation_ClinicalSignificance = eachVariation.GetValue("clinical_significance").ToString();
            variation_ConsequenceType = eachVariation.GetValue("consequence_type").ToString();
            variation_Start = eachVariation.GetValue("start").ToString();
            variation_End = eachVariation.GetValue("end").ToString();

            //Setting up the dictionary for mutation alleles.
            int lengthOfAllelesString = variation_Alleles.Length;
            var allelesValue = variation_Alleles.Substring(1, lengthOfAllelesString - 1);
            var alleles = allelesValue.Split(',');

            string originalSeq = "";
            string mutatedSeq = "";

            if (alleles.Length == 2)
            {
                originalSeq = alleles[0];
                mutatedSeq = alleles[1];

                int startPositionOfVariation = Convert.ToInt32(variation_Start) - Convert.ToInt32(focussedExonStart);
                mutatedAlleleDictionary.Add(startPositionOfVariation, mutatedSeq);
            }

            StartCoroutine(getDataByVariationId(variation_Id, i, arrayOfTranscriptVariations.Count));

            if (i == 14 || i == 28 || i == 42 || i == 56 || i == 70 || i == 84 || i == 98 || i == 112 || i == 126)       //As API request limit is 15 per second. Need to wait.
            {
                yield return new WaitForSecondsRealtime(2);
            }
        }
    }

    public IEnumerator getDataByVariationId(string rsId, int iterator, int totalCount)
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://rest.ensembl.org" + "/variation/human/" + rsId + "?phenotypes=1;content-type=application/json"))

        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isError) // Error
            {
                Debug.Log(request.error);
            }

            else // Success
            {
                string phenotypeVariation = request.downloadHandler.text;
                string[] firstSplit = phenotypeVariation.Split(new[] { "\"phenotypes\":", ",\"MAF\"" }, StringSplitOptions.None);

                variation_Phenotypes = firstSplit[1];

                eachVariation = new ExonVariationDataModel(variation_Id, variation_Source, variation_Alleles, variation_ClinicalSignificance, variation_ConsequenceType, variation_Start, variation_End, variation_Phenotypes);
                listOfExonVariations.Add(eachVariation);

                //DO BE DELETED FOR EMULATOR TESTING:
                if (iterator == totalCount - 1)
                {
                    new WaitForSecondsRealTime(1);
                    buildMutatedSequence();
                }
            }
        }
    }

    //DO BE DELETED FOR EMULATOR TESTING:

    private void buildMutatedSequence()
    {
        int sequenceLength = Convert.ToInt32(focussedExonEnd) - Convert.ToInt32(focussedExonStart);

        var exonSeqAsArray = exonSequence.ToCharArray();
        string sequenceShowingAllMutations = "";

        for (int k = 0; k < sequenceLength; k++)
        {
            if (mutatedAlleleDictionary.ContainsKey(k))
            {
                string mutSeq = "";
                mutatedAlleleDictionary.TryGetValue(k, out mutSeq);
                Regex.Replace(mutSeq, @"\s+", "");
                string[] seq = mutSeq.Split(new[] { "\"", "\"]" }, StringSplitOptions.None);
                string SNP = seq[0];
                sequenceShowingAllMutations += SNP;

                int lengthOfMutation = SNP.Length;
                if(lengthOfMutation > 1)
                {
                    Debug.Log(" a long one " + lengthOfMutation);
                    k += lengthOfMutation;
                }
            }
            else
            {
                sequenceShowingAllMutations += exonSeqAsArray[k].ToString();
            }
        }
        Regex.Replace(sequenceShowingAllMutations, @"\s+", "");
        sequenceShowingAllMutations = sequenceShowingAllMutations.Replace(System.Environment.NewLine, "");
        sequenceShowingAllMutations = sequenceShowingAllMutations.Replace(" ", String.Empty);

        Debug.Log(exonSequence.Length + " >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> " + exonSequence);
        Debug.Log(sequenceShowingAllMutations.Length + " _______________________________________ " + sequenceShowingAllMutations);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        for (int j = 0; j < listOfExonVariations.Count; j++)
        {
            Debug.Log(listOfExonVariations[j].Alleles + "______" + listOfExonVariations[j].Start);
        }
    }

    void Update()
    {
    }
}