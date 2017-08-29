using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start()
    {
        keywords.Add("Find Gene", () =>
        {
            this.BroadcastMessage("readyToFetchGeneData");
        });

        keywords.Add("Find Transcript", () =>
        {
            this.BroadcastMessage("readyToFetchTranscriptData");
        });

        keywords.Add("Fetch braf gene", () =>
        {
            Debug.Log("voice command triggered___braf");
            this.BroadcastMessage("handleDefaultGeneVoiceCommand", "braf");
        });

        keywords.Add("Fetch TNF gene", () =>
        {
            this.BroadcastMessage("handleDefaultGeneVoiceCommand", "TNF");
        });

        keywords.Add("Fetch brca one gene", () =>
        {
            this.BroadcastMessage("handleDefaultGeneVoiceCommand", "brca1");
        });

        keywords.Add("Fetch RB one gene", () =>
        {
            this.BroadcastMessage("handleDefaultGeneVoiceCommand", "rb1");
        });

        keywords.Add("one", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 1);
        });

        keywords.Add("two", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 2);
        });

        keywords.Add("three", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 3);
        });

        keywords.Add("four", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 4);
        });

        keywords.Add("five", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 5);
        });

        keywords.Add("six", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 6);
        });

        keywords.Add("seven", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 7);
        });

        keywords.Add("eight", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 8);
        });

        keywords.Add("nine", () =>
        {
            this.BroadcastMessage("numberVoiceInputRecognition", 9);
        });

        keywords.Add("zero", () =>
        {
            this.BroadcastMessage("addZeroToTranscriptInput");
        });

        keywords.Add("delete number", () =>
        {
            this.BroadcastMessage("deleteLastNumber");
        });

        keywords.Add("add decimal", () =>
        {
            this.BroadcastMessage("addDecimalPoint");
        });


        //keywords.Add("Drop Sphere", () =>
        //{
        //    var focusObject = GazeGestureManager.Instance.FocusedObject;
        //    if (focusObject != null)
        //    {
        //        // Call the OnDrop method on just the focused object.
        //        focusObject.SendMessage("OnDrop", SendMessageOptions.DontRequireReceiver);
        //    }
        //});

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("____________Test - SpeechManager! ______________");
        }
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            Debug.Log("################ invoked #############");
            keywordAction.Invoke();
        }
    }
}
