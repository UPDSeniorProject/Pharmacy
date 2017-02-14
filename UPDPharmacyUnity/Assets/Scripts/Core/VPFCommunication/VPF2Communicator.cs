using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class VPF2Communicator : AbstractVPFCommunicator
{

    public RenTextBox InstructionsTextBox = new RenTextBox();
    public RenTextBox TutorialTextBox = new RenTextBox();

    #region Server Location and Identification

    protected override string DefaultServerAddress
    {
        get
        {
            return "http://vpf2.cise.ufl.edu/";
        }
    }

    protected override string DefaultAssetBundlePath
    {
        get
        {
            return "Content/unity/assetBundles/";
        }
    }
	
    protected override string GetAudioURL()
    {		
        return ServerAddress + "Uploads/Audio/Speeches/";
    }

    public override string ToString()
    {
        return "VPF 2 Web";
    }
	
    #endregion

    #region MonoBehaviour functions
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
		UseUnityGUI = false;

        InstructionsTextBox.Skin = Resources.Load("VPF2/VPF2Skin", typeof(GUISkin)) as GUISkin;
        InstructionsTextBox.ShouldRender = false;
        InstructionsTextBox.canEdit = false;
        InstructionsTextBox.Position = new Rect(45,145,250,100);
        AddGUIElement(InstructionsTextBox);

        TutorialTextBox.Skin = Resources.Load("VPF2/VPF2Skin", typeof(GUISkin)) as GUISkin;
        TutorialTextBox.ShouldRender = false;
        TutorialTextBox.useScroll = true;
        TutorialTextBox.scrollRect = new Rect(5, 5, 260, 135);
        TutorialTextBox.Position = new Rect(0, 0, 240, 270);
    

        TutorialTextBox.canEdit = false;
        AddGUIElement(TutorialTextBox);
		
        SoundManager.StoppedEvent += SoundManager_SpeechEnded;

		//StartCoroutine (TestFunc ());
	}
	
	public IEnumerator TestFunc()
	{
		yield return new WaitForSeconds(3);
		//	TriggerAnimationAndNode("{\"AudioFileName\":\"81880_Yes_I_am_afraid_that_my_swallow.wav\",\"ScenarioID\":10111,\"CharacterID\":10242,\"Animation\":[{\"Name\":\"hd_Nod Yes Once\",\"Delay\":0.36445417477328934}]}");
		//TriggerEvent("{\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ScenarioID\":10111,\"CharacterID\":10242,\"SpeechID\":34854,\"SpeechText\":\"No, I'm not having any headaches. \",\"AudioFileName\":\"81862_No_Im_not_having_any_headaches.wav\",\"AnimationID\":null,\"Animation\":[{\"Name\":\"hd_Nod Yes Once\",\"Delay\":1},{\"Name\":\"bdy_Hand Gesture\",\"Delay\":0.5}],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[],\"Topics\":[],\"Discoveries\":[],\"MiscXML\":\"\",\"ReflectiveMoment\":null,\"extraElement\":null}");
		//TriggerEvent("{\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ScenarioID\":10111,\"CharacterID\":10242,\"SpeechID\":34854,\"SpeechText\":\"No, I'm not having any headaches. \",\"AudioFileName\":\"81862_No_Im_not_having_any_headaches.wav\",\"AnimationID\":null,\"Animation\":[{\"Name\":\"hd_Nod Yes Once\",\"Delay\":1},{\"Name\":\"bdy_Hand Gesture\",\"Delay\":0.5}],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[],\"Topics\":[],\"Discoveries\":[],\"MiscXML\":\"\",\"ReflectiveMoment\":null,\"extraElement\":null}");
	    //TriggerEvent("{\"AudioFileName\":\"81880_Yes_I_am_afraid_that_my_swallow.wav\",\"ScenarioID\":10111,\"CharacterID\":10242,\"Animation\":[{\"Name\":\"hd_Nod Yes Once\",\"Delay\":0.4237839241549876},{\"Name\":\"bdy_Hands In Lap\",\"Delay\":0.4661623165704864}]}");
		//TriggerEvent("{\"ScenarioID\":10111,\"CharacterID\":10242,\"SpeechID\":34890,\"SpeechText\":\"Yes, I am afraid that my swallowing problem is really going to alter that.\",\"AudioFileName\":\"81880_Yes_I_am_afraid_that_my_swallow.wav\",\"AnimationID\":null,\"Animation\":null,\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"MiscXML\":\"\",\"Tags\":[],\"Topics\":[],\"Discoveries\":[],\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ReflectiveMoment\":null,\"extraElement\"null}");
		//TriggerEvent("{\"ScenarioID\":9107,\"CharacterID\":9234,\"SpeechID\":34890,\"SpeechText\":\"Yes, I am afraid that my swallowing problem is really going to alter that.\",\"AudioFileName\":\"74919_yes_i_had_a_really_bad_headache.wav\",\"AnimationID\":null,\"Animation\":null,\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"MiscXML\":\"\",\"Tags\":[],\"Topics\":[],\"Discoveries\":[],\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ReflectiveMoment\":null,\"extraElement\"null}");
	    //TriggerEvent("{\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ScenarioID\":10111,\"CharacterID\":10242,\"SpeechID\":40440,\"SpeechText\":\"I don't really eat. I drink a lot.\",\"AudioFileName\":\"91224_I_dont_really_eat_I_drink_a_lo.wav\",\"AnimationID\":null,\"Animation\":[{\"Name\":\"hd_Nod Yes Once\",\"Delay\":0.3},{\"Name\":\"bdy_Hand Gesture\",\"Delay\":0.5}],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[],\"Topics\":[],\"Discoveries\":[],\"MiscXML\":\"\",\"ReflectiveMoment\":null,\"extraElement\":null}");
		//TriggerEvent("{\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ScenarioID\":14152,\"CharacterID\":18315,\"SpeechID\":61595,\"SpeechText\":\"Doctor, I don’t know how I’ll be able to pay for this treatment, are there other options?\",\"AudioFileName\":\"149949_Doctor_I_dont_know_how_Ill_be.wav\",\"AnimationID\":null,\"Animation\":[],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[\"Empathic Opportunity\"],\"Topics\":[],\"Discoveries\":[],\"MiscXML\":\"\",\"ReflectiveMoment\":null,\"extraElement\":null}");
		//TriggerEvent("{"\IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ScenarioID\":14152,\"CharacterID\":18315,\"SpeechID\":61025,\"SpeechText":"\nHello Doctor. \n","AudioFileName":"148275__Hello_Doctor_.wav","AnimationID":null,"AnimationList":[{"Name":"hd_Nod Yes Once","Delay":0}],"AppActions":[],"VideoURL":null,"ImageTransition":null,"Tags":[],"Topics":["\nGreeting\n"],"Discoveries":["Patient rapport"],"MiscXML":"","ReflectiveMoment":null,"extraElement":null}");
	    //{"IsCritical":false,"IsOptional":false,"IsActionOnly":false,"ScenarioID":14152,"CharacterID":18315,"SpeechID":61025,"SpeechText":"\nHello Doctor. \n","AudioFileName":"148275__Hello_Doctor_.wav","AnimationID":null,"Animation":"","AnimationList":[{"Name":"hd_Nod Yes Once","Delay":0}],"AppActions":[],"VideoURL":null,"ImageTransition":null,"Tags":[],"Topics":["\nGreeting\n"],"Discoveries":["Patient rapport"],"MiscXML":"","ReflectiveMoment":null}
		TriggerEvent ("{\"IsCritical\":false,\"IsOptional\":false,\"IsActionOnly\":false,\"ScenarioID\":14152,\"CharacterID\":18315,\"SpeechID\":61016,\"SpeechText\":\"Yo estuve despierta toda la noche por el tooth pain\",\"AudioFileName\":\"148266__I_have_been_up_with_tooth_pain_.wav\",\"AnimationID\":null,\"Animation\":\"\",\"AnimationList\":[{\"Name\":\"bdy_RubJaw\",\"Delay\":0.72},{\"Name\":\"hd_Tilt Head Right\",\"Delay\":1.3}],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[],\"Topics\":[\"\nChief Complaint\n\"],\"Discoveries\":[\"Painful at night\"],\"MiscXML\":\"\",\"ReflectiveMoment\":null}");
	}
    

        // Update is called once per frame
    protected override void Update()
    {
        base.Update();
       
    }
    #endregion
	
	#region Initialization
	
	public override void Initialize() {
		Initialize(false); // VPF2 handles logging
		
	}
	
	#endregion

    #region Web Messages

    #region Messages from Web

	public virtual JSONObject TriggerEvent(string s) {
		JSONObject obj = new JSONObject(s);
		
		string xmlString = obj["MiscXML"].str;
		HandleMiscXML(xmlString);
		
		foreach (JSONObject o in obj["AppActions"].list)
		{
			
			//Debug.Log(o.str);
			if (o.str != null)
				gameObject.SendMessage(o.str, null, SendMessageOptions.RequireReceiver);
		}
		
		/*string Animation = obj["Animation"].str;
        if (Animation != "" && AnimationManager.ContainsClipWithName(Animation))
        {
            AnimationManager.CrossFadeOverAndReturnToIdle(Animation);
        }*/
		List<VPFAnimationInfo> AnimationInfo = new List<VPFAnimationInfo>();
		foreach (JSONObject o in obj["AnimationList"].list)
		{
			string name = "";
			float start = 0f;
			foreach(JSONObject o2 in o.list)
			{
				if(o2.type == JSONObject.Type.STRING)
					name = o2.str;
				if(o2.type == JSONObject.Type.NUMBER)
					start = (float)o2.n;
			}
			AnimationInfo.Add(new VPFAnimationInfo(name, start ));
		}
		if(AnimationInfo.Count > 0)
			AnimationManager.CrossFadeOverAndReturnToIdle(AnimationInfo);

		bool isActionOnly = obj["IsActionOnly"].b;
		if (!isActionOnly)
		{
			string audio = obj["AudioFileName"].str;
			
			if (audio != "" && audio != null)
			{
				int ScenarioID = (int)obj["ScenarioID"].n;
				int CharacterID = (int)obj["CharacterID"].n;
				
				audio = ScenarioID + "/" + CharacterID + "/" + audio;
				PlayAudio(audio);
			}
		}

        return obj;
	}
	
	public void TriggerAnimation(string animationName)
	{
		if (!string.IsNullOrEmpty(animationName))
		{
			AnimationManager.PlayAnimation(animationName);
		}
	}

	public void TriggerHeadAnimation(string animationName) 
	{
		if(!string.IsNullOrEmpty(animationName))
		{
			AnimationManager.PlayHeadAnimation(animationName);
		}
	}

	public void TriggerAnimationAndNode(string s)
	{
		JSONObject obj = new JSONObject(s);
		
		List<VPFAnimationInfo> AnimationInfo = new List<VPFAnimationInfo>();
		foreach (JSONObject o in obj["Animation"].list)
		{
			string name = "";
			float start = 0f;
			foreach(JSONObject o2 in o.list)
			{
				if(o2.type == JSONObject.Type.STRING)
					name = o2.str;
				if(o2.type == JSONObject.Type.NUMBER)
					start = (float)o2.n;
			}
			AnimationInfo.Add(new VPFAnimationInfo(name, start ));
		}
        if (AnimationInfo.Count > 0)
            AnimationManager.CrossFadeOverAndReturnToIdle(AnimationInfo);
        else
            Debug.Log("Called Trigger AnimationAndNode with no animation info");
		
		string audio = obj["AudioFileName"].str;
		
		if (audio != "" && audio != null)
		{
			int ScenarioID = (int)obj["ScenarioID"].n;
			int CharacterID = (int)obj["CharacterID"].n;
			
			audio = ScenarioID + "/" + CharacterID + "/" + audio;
			PlayAudio(audio);
		}
	}

    private void HandleMiscXML(string xmlString)
    {
        if (xmlString != "" && xmlString != null)
        {
            XMLNode xml = XMLParser.Parse(xmlString);

            string instructions = xml.GetTextNode("xml>0>instructions");

            if (instructions != null)
            {
                InstructionsTextBox.text = instructions;
                InstructionsTextBox.ShouldRender = true;
            }
            else
            {
                InstructionsTextBox.text = "";
                InstructionsTextBox.ShouldRender = false;
            }

            string tutorial = xml.GetTextNode("xml>0>tutorial");
            if (tutorial != null)
            {
                TutorialTextBox.text = tutorial.Trim();
                TutorialTextBox.ShouldRender = true;
            }


            XMLNode next = xml.GetNode("xml>0>next>0");
            if (next != null)
            {
                NextActionNode nextActionNode = new NextActionNode(next, this);
                StopTaggedActionsToPlayQueue = nextActionNode.Stop;
                TaggedActionsToPlay.Enqueue(nextActionNode);
            }
        }
        else
        {
            InstructionsTextBox.ShouldRender = false;
            InstructionsTextBox.text = "";
        }
    }
	
	#endregion

    #region Messages To Web

    public override void PlayTaggedAction(string tag)
    {
        if (Application.isWebPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalCall("PlayTaggedAction", tag);
        }
        else
        {
            Debug.Log("PlayTaggedAction not implemented for Application: " + Application.platform);
        }
    }

    

    
    #endregion

    #endregion

    #region Audio

    public override void PlayAudio(string fileName, System.Object param = null)
    {
        StartCoroutine(DownloadAndPlay(GetAudioURL(), fileName));
    }

    protected IEnumerator DownloadAndPlay(string URL, string fileName) 
    {
        //	AddDebugLine("Going to download audio from: " + URL + " with fileName: " + fileName);


        WWW www;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("WebGL: " + URL + fileName);
            www = new WWW(URL + fileName);
        }
        else
        {
            www = new WWW(URL + fileName);
        }
        ServerConnection sc = new ServerConnection(www);

        ActiveConnections.Add(sc);
        yield return www;
		
		if(www.error != null && www.error != "")
			AddDebugLine("Audio error: " + www.error);
		
        ActiveConnections.Remove(sc);
        //AddDebugLine("Audio connection waited: " + sc.GetElapsedTime() + " before returning");

        AudioClip clip = www.GetAudioClip(false, false);
		if(clip.loadState != AudioDataLoadState.Loaded)
			AddDebugLine("Clip not ready to play: " + clip.loadState);
		
		//The filename comes with a path, so we remove the path to just keep the name of the wav file.
        clip.name = fileName.Substring(fileName.LastIndexOf('/') + 1);

        //Debug.Log("Clip Length: " + clip.length + " name: " + clip.name + " and has: " + clip.samples);

        LipSyncInfo lipSync = new LipSyncInfo(clip);
		
        if (!AnimationManager.ContainsClipWithName(lipSync.AnimationName))
        {
            /** TODO: Get Phonemes from server **/
            //  AddDebugLine("No lipsync animation for lipSync with name: " + lipSync.AnimationName);
            //  AddDebugLine("Should get phonemes for list with id: " + id);
            Debug.Log("No animation clip with name: " + lipSync.AnimationName);
        }else
        {
            //Debug.Log("Should talk now");
        }

		//Call the VirtualHumanWillTalk event.
		OnVirtualHumanWillTalk();
			
   		//Add to the sound manager for play.
        SoundManager.EnqueueLipSync(lipSync);
    }

    Queue<NextActionNode> TaggedActionsToPlay = new Queue<NextActionNode>();
    bool StopTaggedActionsToPlayQueue = false;

    public void CanMoveOnWithTaggedActions()
    {
        //AddDebugLine("CanMoveOn" + SoundManager.IsPlaying() + " -- and has: " + TaggedActionsToPlay.Count);
        StopTaggedActionsToPlayQueue = false;
        if (!SoundManager.IsPlaying() && TaggedActionsToPlay.Count > 0)
        {
            StartCoroutine(TaggedActionsToPlay.Dequeue().Play());
        }
    }

    void SoundManager_SpeechEnded(LipSyncInfo info, LipSyncAudioEventArgs args)
    {
        if (TaggedActionsToPlay.Count > 0 && !StopTaggedActionsToPlayQueue)
        {
             StartCoroutine(TaggedActionsToPlay.Dequeue().Play()) ;
        }
    }
	
	#endregion

    #region Stages

    protected List<string> ConcludedStages = new List<string>();

    public void StageConcluded(string id)
    {
        ConcludedStages.Add(id);
    }

    public int StagesConcluded()
    {
        return ConcludedStages.Count;
    }

    #endregion


    #region Not-yet implemented abstract methods
    public override void FindResponse(string userInput)
    {
		
        throw new System.NotImplementedException();
    }

    public override void GetTranscriptId()
    {
		//Web VPF2 handles this we don't need an event for it.
		LoadingEventsReceived++;	
    }

    protected override void HandleAnswerSelected(SpeechOption o, SpeechOptionSelectedArgs args)
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleSubmitSuggestion(SpeechOptionSelectedArgs args)
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleStatement(SpeechOptionSelectedArgs args)
    {
        throw new System.NotImplementedException();
    }

    protected override void GetScriptEvents(bool UseRL, bool UseTimeTriggers)
    {
		//We don't need this one!
        //throw new System.NotImplementedException();
    }

    protected override void TriggerTimeTrigger(TimeTrigger tt)
    {
        throw new System.NotImplementedException();
    }

    protected override void ReflectiveLearning_AnswerSubmittedEvent(ReflectiveLearningPrompt prompt, ReflectiveLearningEventArgs args)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}

public class NextActionNode
{
    public string Tag;
    public bool Stop;
    public float Delay;

    protected AbstractVPFCommunicator VPFCommunicator;

    public NextActionNode(XMLNode next, AbstractVPFCommunicator VPFCommunicator)
    {
        this.VPFCommunicator = VPFCommunicator;
        XMLNode tag = next.GetNode("tag>0");
        if (tag.GetValue("_text") != null)
        {
            Tag = tag.GetValue("_text");
        }

        if (next.GetValue("@stop") != null)
        {
            Stop = true;
        }
        else
        {
            Stop = false;
        }

        if (next.GetValue("@delay") != null)
        {
            Delay = float.Parse(next.GetValue("@delay"));
        }
        else
            Delay = -1;
    }

    public bool HasDelay()
    {
        return Delay > 0.0f;
    }

    public IEnumerator Play()
    {
        if (HasDelay())
        {
            yield return new WaitForSeconds(Delay);
        }

        VPFCommunicator.PlayTaggedAction(Tag);
    }

}