using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTTODELETEEXONDATA : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void OnMouseDown()
    {
        GameObject testo = GameObject.Find("EmptySeqExonPanel");
        testo.transform.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
