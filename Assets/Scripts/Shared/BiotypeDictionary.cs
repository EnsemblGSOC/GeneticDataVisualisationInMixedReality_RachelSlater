using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiotypeDictionary : MonoBehaviour {

    public Dictionary<string, string> transcriptBioTypeDict = new Dictionary<string, string>();
    public Dictionary<string, Color> transcriptColorDict = new Dictionary<string, Color>();

    // Use this for initialization
    void Start () {
        transcriptBioTypeDict.Add("protein_coding", "Protein Coding");
        transcriptBioTypeDict.Add("known_protein_coding", "Protein Coding");
        transcriptBioTypeDict.Add("novel_protein_coding", "Protein Coding");
        transcriptBioTypeDict.Add("putative_protein_coding", "Protein Coding");
        transcriptBioTypeDict.Add("nonsense_mediated_decay", "Protein Coding");
        transcriptBioTypeDict.Add("nonstop_decay", "Protein Coding");

        transcriptBioTypeDict.Add("non coding", "Processed Transcript");
        transcriptBioTypeDict.Add("3prime_overlapping_ncRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("antisense", "Processed Transcript");
        transcriptBioTypeDict.Add("lincRNA", "Processed Transcript)");
        transcriptBioTypeDict.Add("retained_intron", "Processed Transcript");
        transcriptBioTypeDict.Add("sense_intronic", "Processed Transcript");
        transcriptBioTypeDict.Add("sense_overlapping", "Processed Transcript");
        transcriptBioTypeDict.Add("macro_lncRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("miRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("piRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("rRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("siRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("snRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("snoRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("tRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("vaultRNA", "Processed Transcript");
        transcriptBioTypeDict.Add("unclassified_processed_transcript", "Processed Transcript");

        transcriptBioTypeDict.Add("processed_pseudogene", "Pseudogene");
        transcriptBioTypeDict.Add("unprocessed_pseudogene", "Pseudogene");
        transcriptBioTypeDict.Add("transcribed_pseudogene", "Pseudogene");
        transcriptBioTypeDict.Add("translated_pseudogene", "Pseudogene");
        transcriptBioTypeDict.Add("polymorphic_pseudogene", "Pseudogene)");
        transcriptBioTypeDict.Add("unitary_pseudogene", "Pseudogene");
        transcriptBioTypeDict.Add("ig_pseudogene", "Pseudogene");

        transcriptBioTypeDict.Add("tec", "To be experimentally confirmed");

        transcriptBioTypeDict.Add("artifact", "Artifact");

        transcriptBioTypeDict.Add("IG_gene", "Immunoglobin Gene");

        transcriptBioTypeDict.Add("TR_gene", "T Cell Receptor Gene");

        GlobalControl.Instance.transcriptBioTypeDict = transcriptBioTypeDict;

        createTranscriptColorDictionary();
    }

    private void createTranscriptColorDictionary()
    {
        transcriptColorDict.Add("Protein Coding", Color.cyan);
        transcriptColorDict.Add("Processed Transcript", Color.magenta);
        transcriptColorDict.Add("Pseudogene", Color.yellow);
        transcriptColorDict.Add("To be experimentally confirmed", Color.grey);
        transcriptColorDict.Add("Artifact", Color.green);
        transcriptColorDict.Add("Immunoglobin Gene", Color.blue);
        transcriptColorDict.Add("T Cell Receptor Gene", Color.red);

        GlobalControl.Instance.transcriptColorDict = transcriptColorDict;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
