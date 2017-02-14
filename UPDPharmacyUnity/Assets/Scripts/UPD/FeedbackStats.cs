using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FeedbackStats
{
#if UNITY_EDITOR
    List<string> EmpatheticSayings = new List<string>
    {
        "I understand what you're going through",
        "How can we fix this?",
        "I understand how important the medicine is for you",
        "What is your side of the story?",
        "So you're scared because you might lose your job"
    };

    List<string> ReactiveSayings = new List<string>
    {
        "You're harassing the other customers",
        "You're bothering everybody else",
        "Are you on something?",
        "Why are you acting so weird?",
        "I'm going to need you to stop acting crazy"
    };
#endif


    // storing what the user states in here
    int Empathetic = 0;
    int Reactive = 0;

    int totalWordsSaid = 0;

    public FeedbackStats()
    {
        // initialize dictionaries
        // Empathetic = new List<string>();
        // Reactive = new List<string>();
    }

    public int GetEmpatheticCount()
    {
        return Empathetic;
    }

    public int GetReactiveCount()
    {
        return Reactive;
    }

    public void OnDialogue()
     {
         totalWordsSaid++;
    }

    public void StoreReactive(string message)
    {
        OnDialogue();
        Reactive++;
    }

    public void StoreEmpathetic(string message)
    {
        OnDialogue();
        Empathetic++;
    }

    public string GetRandomReactive()
    {
#if UNITY_EDITOR
        int upperBound = ReactiveSayings.Count;
        System.Random r = new System.Random();
        int Location = (int)Math.Floor(r.NextDouble() * upperBound);
        return ReactiveSayings.ElementAt(Location);
#else
        return "";
#endif
    }

    public string GetRandomEmpathetic()
    {
#if UNITY_EDITOR
        int upperBound = EmpatheticSayings.Count;
        System.Random r = new System.Random();
        int Location = (int)Math.Floor(r.NextDouble() * upperBound);
        return EmpatheticSayings.ElementAt(Location);
#else
        return "";
#endif
    }

    public bool NeedsHelp()
    {
        return (totalWordsSaid % 5 == 4);
    }

    public override string ToString()
    {

        StringBuilder s = new StringBuilder();
        s.Append("Total: " + totalWordsSaid +
                        ", Empathetic: " + Empathetic +
                        ", Reactive: " + Reactive +
                        ", Needs Help: " + (totalWordsSaid % 5));
        return s.ToString();
    }
}
