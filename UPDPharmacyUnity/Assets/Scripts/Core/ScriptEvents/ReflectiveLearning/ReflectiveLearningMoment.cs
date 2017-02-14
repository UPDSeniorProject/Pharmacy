using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReflectiveLearningPrompt
{
    public int Id;
    public string Text;
    public float DisplayOrder;

    public ReflectiveLearningPrompt(int Id, string Text = "")
    {
        this.Id = Id;
        this.Text = Text;
    }

    public ReflectiveLearningPrompt(XMLNode xml)
    {
        Id = int.Parse(xml.GetTextNode("prompt_id"));
        Text = xml.GetHTMLDecodedTextNode("prompt");
        DisplayOrder = float.Parse(xml.GetTextNode("display_order"));
    }
}

public class ReflectiveLearningMoment  {

    /// <summary>
    /// Ordered list of prompts in the moment.
    /// </summary>
    public List<ReflectiveLearningPrompt> Prompts;

    /// <summary>
    /// Unique Label that identifies this moment. (Makes it easy to know when to display it).
    /// </summary>
    public string Label;

    public int MomentId;

    public bool HasTriggered;

    public ReflectiveLearningMoment()
    {
        Prompts = new List<ReflectiveLearningPrompt>();
    }

    public ReflectiveLearningMoment(XMLNode xml)
    {
        Prompts = new List<ReflectiveLearningPrompt>();
        Label = xml.GetTextNode("description");
        MomentId = int.Parse(xml.GetTextNode("moment_id"));

        foreach (XMLNode n in xml.GetNodeList("prompts>0>array_item"))
        {
            Prompts.Add(new ReflectiveLearningPrompt(n));
        }

        HasTriggered = false;
    }


    
}
