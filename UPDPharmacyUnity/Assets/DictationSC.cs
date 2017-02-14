using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class DictationSC : MonoBehaviour {
	DictationRecognizer Dictation = new DictationRecognizer();

	// Use this for initialization
	void Start () {
		Dictation.Start ();
		Dictation.DictationResult += DictationRecognizer_DictationResult;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
	{
		print (text);
	}
}
