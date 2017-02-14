using UnityEngine;
using System.Collections.Generic;

public class TimeTrigger  {
    public int TriggerMapID;
    public int TriggerID;
    public int Time;
    public string SpeechText;
    public int TopicId;
    public string AudioFile;
    public string AudioFileListId;
    public List<string> AppActions;

    protected bool HasTriggered;

    public TimeTrigger(XMLNode node)
    {
        TriggerMapID = int.Parse(node.GetTextNode("trigger_map_id"));
        TriggerID = int.Parse(node.GetTextNode("trigger_id"));
        Time = int.Parse(node.GetTextNode("time"));
        
        SpeechText = node.GetHTMLDecodedTextNode("speech_text");
        string topicIdText = node.GetTextNode("topic_id");
        if (topicIdText != "" && topicIdText != null)
            TopicId = int.Parse(topicIdText);
        else
            TopicId = -1;

        AudioFile = node.GetHTMLDecodedTextNode("audio_file");
        AudioFileListId = node.GetTextNode("audio_file_list_id");

        AppActions = new List<string>();
        XMLNodeList actions = node.GetNodeList("app_actions>0>array_item");
        if (actions != null)
        {
            foreach (XMLNode n in actions)
            {
                AppActions.Add(n["_text"] as string);
            }
        }

        HasTriggered = false;
    }

    public bool WillTrigger(float currentTime)
    {
        //If it hasn't triggered and it's time passed.
        return !HasTriggered && (this.Time < currentTime);
    }

    public void Triggered()
    {
        HasTriggered = true;
    }
}
