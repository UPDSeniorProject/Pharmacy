using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class FeedbackMenu : MonoBehaviour {

    // public interface
    public Camera feedbackCam;
    public FeedbackStats feedbackStats;

    public Text empatheticText;
    public Text reactiveText;

	// Use this for initialization
	void Start ()
    {
        // GET the feedback
        feedbackStats = getFeedbackFromScene();
    }

	// Update is called once per frame
	void Update () {
	}

    string printField(string fieldName, Dictionary<TimeSpan, string> field)
    {
        // to build
        string text = "";

        if (field.Count == 0) // nothing in category
        {
            return text;
        }

        if (fieldName.Equals("Empathetic"))
        {
            text += "Congrats on including the following EMPATHETIC statements!\n\n";
        }

        else if (fieldName.Equals("Reactive"))
        {
            text += "Try to improve your responses on the following REACTIVE statements...\n\n";
        }

        else // not a valid field
        {
            return text;
        }

        foreach (KeyValuePair<TimeSpan, string> kvp in field)
        {
            text += "At time " + kvp.Key + ": " + kvp.Value;
        }

        return text;
    }

    FeedbackStats getFeedbackFromScene()
    {
        // GET camera
        GameObject c = GameObject.Find("CVS Pharmacy Camera");

        // GET dialogue tracker
        DialogueTracker dt = c.GetComponent("DialogueTracker") as DialogueTracker;

        // GET stats
        return dt.GetFeedbackStats();
    }
}
