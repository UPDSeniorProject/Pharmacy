using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VPF1Communicator : AbstractVPFCommunicator
{
    #region Extra Script/Transcript book-keeping
    /// <summary>
    /// Topic of the last triggered speech. Not sure why this is useful, but the FindResponse webservices 
    /// uses it, so we keep a copy to send it to it.
    /// </summary>
    protected int PreviousTopicId = -1;
    #endregion

    #region MonoBehaviour functions
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
    }

	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Initialization

    /// <summary>
    /// Sets up the base parameters for accessing the web service.
    /// </summary>
    /// <param name="log"></param>
    public override void Initialize(bool log)
    {
        BaseParameters = new Dictionary<string, string>();
        BaseParameters["username"] = "vpfunity";
        BaseParameters["password_md5"] = "63c170b9749d076d3d41a70125d96c01";                                    
        BaseParameters["encoding"] = "xml";

       

        base.Initialize(log);
    }

    #endregion

    #region Abstract Method Implementations and related

    #region ServerLocation and Identification
    protected override string DefaultServerAddress
    {
        get
        {
            return "http://vpf.cise.ufl.edu/VirtualPeopleFactory/";
        }
    }

    protected override string DefaultAssetBundlePath
    {
        get
        {
            return "UnityAssetBundles/";
        }
    }

    public override string ToString()
    {
        return "VPF 1 Web";
    }
    #endregion

    #region FindResponse

    public override void FindResponse(string userInput)
    {
        AddTranscriptEntry(UserTranscriptName, LastUserInput);
        Dictionary<string, string> param = GetParameters("FindResponseScript", "FindMostRelevantResponsesUnity", "" + ScriptId);
        
        param["userInput"] = userInput;
        param["transcript_id"] = "" + TranscriptId;
        
        if (ActId != -1)
        {
            param["actId"] = "" + ActId;
        }


        if (PreviousTopicId != -1)
        {
            param["previousTopic_id"] = "" + PreviousTopicId;
        }

        StartCoroutine(AccessWebService(param, new WebCallback(FindResponseCallback)));
    }
	
	public void TriggerResponse(string userInput)
    {
        Dictionary<string, string> param = GetParameters("FindResponseScript", "FindMostRelevantResponsesUnity", "" + ScriptId);
        
        param["userInput"] = userInput;
        param["transcript_id"] = "" + TranscriptId;
        
        if (ActId != -1)
        {
            param["actId"] = "" + ActId;
        }


        if (PreviousTopicId != -1)
        {
            param["previousTopic_id"] = "" + PreviousTopicId;
        }

        StartCoroutine(AccessWebService(param, new WebCallback(FindResponseCallback)));
    }

    protected void FindResponseCallback(WWW www)
    {
        Debug.Log("Got response: " + www.text);

        XMLNode xml = XMLParser.Parse(www.text);
        XMLNode array = xml.GetNode("array>0");

        XMLNodeList responses = array.GetNodeList("array_item");
        if (responses == null)
        {
            //Display the no answer speechoptions.
            NoAnswer();
        }else if (responses.Count > 1)
        {
            //ShowSpeechOptions!
            Debug.Log("SpeechOptions! will have: " + responses.Count);
            List<SpeechOption> options = new List<SpeechOption>();
            string text;
            //TODO: Construct Speech Options.
            //
            foreach (XMLNode n in responses)
            {
                text = n.GetHTMLDecodedTextNode("trigger_text");
                options.Add(new SpeechOption(text, n));
            }

            SpeechOptions.DisplaySpeechOptions(options);

        }
        else if (responses.Count == 1)
        {
            TriggerAnswer(responses[0] as XMLNode);
        }
    }

    protected void TriggerAnswer(XMLNode answer)
    {
        //Update Previous topic!
        PreviousTopicId = int.Parse(answer.GetTextNode("topic_id"));
        
        //Include in the transcript
        AddTranscriptEntry(VHTranscriptName, answer.GetHTMLDecodedTextNode("speech_text"));

        //TODO: FIX AWFUL WAY OF DOING APP ACTIONS!/*
		if(answer.GetNodeList("app_actions>0>array_item")!= null)
		{ 
	        foreach(XMLNode appaction in answer.GetNodeList("app_actions>0>array_item"))
	        {
	            gameObject.SendMessage((string)appaction["_text"], null, SendMessageOptions.RequireReceiver);
	        }
		}
        PlayAudio(answer.GetHTMLDecodedTextNode("audio_file_name").Replace(".mp3", ".ogg"), 
            answer.GetTextNode("audio_file_list_id"));

    }

    #endregion

    #region SpeechOptions
    protected override void HandleAnswerSelected(SpeechOption o, SpeechOptionSelectedArgs args)
    {
        XMLNode node = o.Obj as XMLNode;
        Dictionary<string, string> param = GetParameters("UnityTranscript", "insertUtterance", "" + TranscriptId);
        param["user_input"] = LastUserInput;
        param["speech_text"] = node.GetTextNode("speech_text");
        param["topic_id"] = node.GetTextNode("topic_id");
        param["trigger_id"] = node.GetTextNode("trigger_id");
        param["trigger_map_id"] = node.GetTextNode("trigger_map_id");
        param["speech_option_selected"] = "" + o.index;

        //Populate speech options for web service call
        for (int i = 0; i < 3 && i < args.options.Count; i++)
        {
            param["speech_option" + (i + 1)] = args.options[i].Text;
        }

        //trigger the answer
        TriggerAnswer(node);
        //save the utterance in the transcript!
        StartCoroutine(AccessWebService(param, new WebCallback(print)));  
    }

    protected override void HandleStatement(SpeechOptionSelectedArgs args)
    {
        Dictionary<string, string> param = GetParameters("UnityTranscript", "insertStatement", "" + TranscriptId);
        param["user_input"] = LastUserInput;

        StartCoroutine(AccessWebService(param, new WebCallback(print)));
    }

    protected override void HandleSubmitSuggestion(SpeechOptionSelectedArgs args)
    {
        Dictionary<string, string> param = GetParameters("UnityTranscript", "insertNewSuggestedResponse", "" + TranscriptId);
        param["user_input"] = LastUserInput;


        StartCoroutine(AccessWebService(param, new WebCallback(print)));
    }

    #endregion

    #region GetTranscriptId
    public override void GetTranscriptId()
    {
        Dictionary<string, string> param = GetParameters("UnityTranscript", "insertUnityTranscript", "");
        param["user"] = UserName;
        param["script_id"] = "" + ScriptId;
        param["encoding"] = "json"; //json is easier to parse when result is integer!
        
        StartCoroutine(AccessWebService(param, new WebCallback(SetTranscriptId_Callback)));
    }

    protected void SetTranscriptId_Callback(WWW www)
    {
        if (!System.Int32.TryParse(www.text, out TranscriptId))
        {
            AddDebugLine("Couldn't set Transcript ID. Result is: " + www.text);
        }
        LoadingEventsReceived++;
    }
    #endregion
    
    #region Audio

    public override void PlayAudio(string fileName, System.Object param = null)
    {
        int id = -1;
        if (param != null)
        {
            id = int.Parse(param as string);
        }

        StartCoroutine(DownloadAndPlay(GetAudioURL(), fileName, id));
    }

    protected IEnumerator DownloadAndPlay(string URL, string fileName, int id = -1) 
    {
        WWW www = new WWW(URL + fileName);
        ServerConnection sc = new ServerConnection(www);

        ActiveConnections.Add(sc);
        yield return www;
        ActiveConnections.Remove(sc);
        Debug.Log("Audio connection waited: " + sc.GetElapsedTime() + " before returning");


        AudioClip clip = www.GetAudioClip(false, false);
        clip.name = fileName;

        Debug.Log("Clip Length: " + clip.length + " name: " + clip.name);

        LipSyncInfo lipSync = new LipSyncInfo(clip);

        if (!AnimationManager.ContainsClipWithName(lipSync.AnimationName))
        {
           /** TODO: Get Phonemes from server **/
            AddDebugLine("No lipsync animation for lipSync with name: " + lipSync.AnimationName);
            AddDebugLine("Should get phonemes for list with id: " + id);
        }

        //Add to the sound manager for play.
        SoundManager.EnqueueLipSync(lipSync);
    }

    protected override string GetAudioURL()
    {
        return ServerAddress + "script_audio_files/script_" + ScriptId + "/";
    }


    #endregion

    #region TimeTriggers

    protected override void TriggerTimeTrigger(TimeTrigger tt)
    {
        //update previous topic 
        PreviousTopicId = tt.TopicId;
        AddTranscriptEntry(VHTranscriptName, tt.SpeechText);

        PlayAudio(tt.AudioFile.Replace(".mp3", ".ogg"), tt.AudioFileListId);

        //TODO:Tigger AppActions!
        //TODO: FIX AWFUL WAY OF DOING APP ACTIONS!/*
        foreach (string app in tt.AppActions)
        {
            gameObject.SendMessage(app, null, SendMessageOptions.RequireReceiver);
        }


        //Insert the utterance
        Dictionary<string, string> param = GetParameters("UnityTranscript", "insertUtterance", "" + TranscriptId);
        param["user_input"] = "";
        param["speech_text"] = tt.SpeechText;
        param["topic_id"] = "" +  tt.TopicId;
        param["trigger_id"] = "" + tt.TriggerID;
        param["trigger_map_id"] = "" + tt.TriggerMapID;
        param["speech_option_selected"] = "";

        StartCoroutine(AccessWebService(param, new WebCallback(print)));
    }

    #endregion

    #region ReflectiveLearning

    /// <summary>
    /// Saves the response through the WebService.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="args"></param>
    protected override void ReflectiveLearning_AnswerSubmittedEvent(ReflectiveLearningPrompt prompt, ReflectiveLearningEventArgs args)
    {
        Dictionary<string, string> param = GetParameters("UnityTranscript", "insertReflectiveAnswer", "" + TranscriptId);

        param["answer"] = args.Answer;
        param["reflective_prompt_id"] = "" + prompt.Id;
        param["seconds_since_start"] = "" + (int)(TotalInteractionTime);

        StartCoroutine(AccessWebService(param, new WebCallback(print)));
    }

    #endregion

    #region ScriptEvents

    protected override void GetScriptEvents(bool useReflectiveLearning, bool useTimeTriggers)
    {
        if (useTimeTriggers || useReflectiveLearning)
        {//Only Make the call if we need this.
            //Interaction shouldn't start until we have this data.
            LoadingEventsExpected++; 

            Dictionary<string, string> param = GetParameters("UnityScript", "GetScriptEventData", ScriptId);

            param["get_time_triggers"] = "" + useTimeTriggers;
            param["get_reflective_learning"] = "" + useReflectiveLearning;

            StartCoroutine(AccessWebService(param, new WebCallback(ParseScriptEvents)));
        }
    }

    protected void ParseScriptEvents(WWW www)
    {
        Debug.Log(www.text);

        XMLNode xml = XMLParser.Parse(www.text);
        xml = xml.GetNode("array>0");

        if (UseUnityTimeTriggers)
        {
            XMLNodeList timetriggers = xml.GetNodeList("time_triggers>0>array_item");
            if (timetriggers != null)
            {
                foreach (XMLNode tt in timetriggers)
                {
                    TimeTriggers.Add(new TimeTrigger(tt));                    
                }
            }
        }

        if (UseUnityRelfectiveLearning)
        {
            XMLNodeList RLMomentsList = xml.GetNodeList("reflective_moments>0>array_item");
            if (RLMomentsList != null)
            {
                foreach (XMLNode rlm in RLMomentsList)
                {
                    RLPanel.AddMoment(new ReflectiveLearningMoment(rlm));
                }
            }
        }
        //Make sure we increase this for loading progress purposes.
        LoadingEventsReceived++;
    }

    #endregion

    public override void PlayTaggedAction(string tag)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region WebService Access

    protected string GetWebServiceURL()
    {
        return ServerAddress + "virtual_patient_mvc/View/web_service.php";
    }

    Dictionary<string, string> BaseParameters;

    /// <summary>
    /// Returns a copy of the base parameters. (Allows for multiple web service calls without changing them).
    /// </summary>
    /// <returns></returns>
    protected Dictionary<string, string> GetBaseWebServiceParameters()
    {
        //creates copy!!
        return new Dictionary<string, string>(BaseParameters); 
    }

    /// <summary>
    /// Gets a copy of the base parameters and set the model, method and primary_key_value 
    /// (Basic stuff you need while accesing a VPF1 WebService)
    /// </summary>
    /// <param name="model">Model you want to access</param>
    /// <param name="method">Method in that model you want to access.</param>
    /// <param name="primary_key">Primary key (usually id in the model) that you want to send.</param>
    /// <returns>Dictionary with all the parameters set. Just set any additional params.</returns>
    protected Dictionary<string, string> GetParameters(string model, string method, string primary_key)
    {
        Dictionary<string, string> param = GetBaseWebServiceParameters();
        param["model"] = model;
        param["method"] = method;
        param["primary_key_value"] = primary_key;
        
        return param;
    }

    protected Dictionary<string, string> GetParameters(string model, string method, int primary_key)
    {
        return GetParameters(model, method, "" + primary_key);
    }

    /// <summary>
    /// Access the VPF1 WebService. Shortcut that includes the base webservice url.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    protected IEnumerator AccessWebService(Dictionary<string, string> parameters, WebCallback callback)
    {
        return AccessWebService(GetWebServiceURL(), parameters, callback);
    }

    #endregion

    #region TEMPORARY FUNCTIONS
    private void print(WWW www)
    {
        Debug.Log(www.text);
    }
    #endregion
}
