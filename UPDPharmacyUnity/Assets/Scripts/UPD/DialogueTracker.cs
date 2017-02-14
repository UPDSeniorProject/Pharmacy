using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTracker : MonoBehaviour {
    // stores user input
    FeedbackStats feedbackStats;

	
	void Start () {
        // stores feedback input
        feedbackStats = new FeedbackStats();
	}
	
	void Update () {}
    
    public FeedbackStats GetFeedbackStats()
    {
        return feedbackStats;
    }
    
}
