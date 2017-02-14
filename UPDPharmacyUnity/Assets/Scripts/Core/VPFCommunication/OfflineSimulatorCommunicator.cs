using UnityEngine;
using System.Collections;
using VirtualHumanFramework.Core.Messages;
using System;
using VirtualHumanFramework.Core.Messages.Events.Virtual;
using VirtualHumanFramework.Core.Events.Virtual;
using VirtualHumanFramework.Core.Messages.Signals;
using VirtualHumanFramework.Core.Messages.Messages;

public class OfflineSimulatorCommunicator : AbstractVPFCommunicator
{
	public string BatchIdArgName = "characterID";
	private VHFVirtualActionOccurred currentVHFVirtualActionOccurred;

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
	
    // TODO-ACR: Change this to return the filepath to the audio files
    protected override string GetAudioURL()
    {		
		return "file:///" + ServerAddress + "/../Audio/" + CharacterID + "/";
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
		UseUnityGUI = true;
		
		SoundManager.StoppedEvent += HandleSoundManagerStoppedEvent;;
    }

    void HandleSoundManagerStoppedEvent (LipSyncInfo info, LipSyncAudioEventArgs args)
    {
    	io.SendMessage(new VHFCharacterFinishedSpeaking(CharacterID));
    }
	
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
		
		
		VHFMessage message = messageQueue.Dequeue();
		if (message != null)
		{
			ProcessMessage(message);
		}
    }
    #endregion
	
	#region Initialization

	//DRG: Why were you creating a new field for this when it already exists in AbstractVPFComm?
	//int CharacterID;
    OfflineSimulatorIO io;
	ThreadSafeList<VHFMessage> messageQueue;
	
	public RenTextBox testBox;

	public override void Initialize() {
		Initialize(false); // VPF2 handles logging
		
		messageQueue = new ThreadSafeList<VHFMessage>();
		
        RenParameterParser parameterParser = IPS.GetComponent<RenParameterParser>();
        
		ServerAddress = Application.dataPath;
		
		// Getting the parameter values from command line
        string simulatorAddress = parameterParser.GetParameter("simulatorAddress", "localhost");
		CharacterID = parameterParser.GetParameterAsInt(BatchIdArgName, 133);
		if (CharacterID != 0)
		{
	        io = OfflineSimulatorIO.CreateNewCommunicator(this, CharacterID, simulatorAddress);
	        io.Start();
		}
		else
		{
			Debug.Log("Invalid character id!");
			throw new Exception("Invalid character ID!");
		}
		
		SoundManager.StoppedEvent += new LipSyncAudioEvent(CharacterFinishedSpeaking);
	
		
		testBox = Transcript.GetTranscriptTextBox(UserTranscriptName);
		
	}
	
	#endregion
	
	#region Messages from Offline Simulator
	
	public void QueueMessage(VHFMessage message)
	{
		messageQueue.Enqueue(message);
	}
	
	public void ProcessMessage(VHFMessage message)
	{
		Debug.Log("Processing message: " + message.ToString());
		if (message is VHFProgramStopped)
		{
			Application.Quit();
		}
		else if (message is VHFVirtualActionOccurred)
        {
            TriggerEvent(message as VHFVirtualActionOccurred);
        }	
		else if (message is VHFNewLookTarget)
		{
			ChangeLookTarget((message as VHFNewLookTarget).Target);	
		}	
		else if (message is VHFInterruptCurrentAction)
		{
			InterruptCurrentAction();
		}
		else if (message is VHFGrayOut)
		{
			GrayOut(message as VHFGrayOut);
		}
	}
	
	// TODO: Figure out how to notify the simulator of a complete action 
	//       if there is no audio file
	public void TriggerEvent(VHFVirtualActionOccurred VirtualAction) {	
		//currentVHFVirtualActionOccurred = VirtualAction;
		// Play any associated audio file
        if (!string.IsNullOrEmpty(VirtualAction.AudioFileName))
        {
			Debug.Log("Playing audio file: " + GetAudioURL() + VirtualAction.AudioFileName);
            PlayAudio(VirtualAction.AudioFileName); //remove first slash
        }

		//If we find a looping count animation then loop the animation *repeat* times
		if(VirtualAction.Animation != null && VirtualAction.Animation.Equals("scrub_loop_count"))
		{
			int repeat = int.Parse (VirtualAction.SpeechText.Substring (0,3));
			AnimationManager.RepeatAnimationAndReturnToIdle(VirtualAction.Animation, 0.5f,WrapMode.Loop,1, repeat);
	    	//AnimationManager.CrossFadeAnimation (VirtualAction.Animation,0.5f,WrapMode.Loop,1);
		} else if (!string.IsNullOrEmpty(VirtualAction.Animation))
		{
			AnimationManager.CrossFadeAnimation(VirtualAction.Animation);
		}
		
		if (!string.IsNullOrEmpty(VirtualAction.LookTarget))
		{
			ChangeLookTarget(VirtualAction.LookTarget);
		}
	}
	
	public void GrayOut(VHFGrayOut grayOut)
	{
		Debug.Log("Starting to gray out...");
		
		Camera activeCamera = Camera.current;
		
		foreach (Camera c in Camera.allCameras)
		{
			var gc = activeCamera.GetComponent("GrayoutController");
			if (gc != null)
			{
				GrayoutController grayoutController = gc as GrayoutController;
				if (grayOut.IsGrayed) 
				{
					Debug.Log("!!!!! Activating grayout !!!!!");
					grayoutController.ActivateGrayout();
				}
				else
				{
					Debug.Log("!!!!! Deactivating grayout !!!!!");
					grayoutController.DeactivateGrayout();			
				}
			}
		}
	}
	
	public void ChangeLookTarget(string lookTarget)
	{
		if (string.IsNullOrEmpty(lookTarget))
		{
 			if (HeadLook.DefaultLookTarget != null)
			{
	            HeadLook.SetLookTarget(HeadLook.DefaultLookTarget);
				Debug.Log("No look target provided... looking at default look target");
			}
			else
			{
	            HeadLook.SetLookTarget(Camera.current.gameObject);
				Debug.Log("No look target provided... looking at current camera");
			}
		}
		else
		{
			GameObject target = GameObject.Find(lookTarget);
			if (target != null) 
			{
				HeadLook.SetLookTarget(target);	
				Debug.Log("Looking at " + lookTarget);
			}
			else
			{
				if (HeadLook.DefaultLookTarget != null)
				{
		            HeadLook.SetLookTarget(HeadLook.DefaultLookTarget);
					Debug.Log("No look target provided... looking at default look target");
				}
				else
				{
		            HeadLook.SetLookTarget(Camera.current.gameObject);
					Debug.Log("No look target provided... looking at current camera");
				}
			}
		}
	}
	
	public void InterruptCurrentAction()
	{
		SoundManager.InterruptLipSync();
		
		AnimationManager.InterruptAnimation();
		
		// Reset head gaze
		if (HeadLook.DefaultLookTarget != null)
		{
			HeadLook.SetLookTarget(HeadLook.DefaultLookTarget);
		}
		else
		{
			HeadLook.SetLookTarget(Camera.main.gameObject);
		}
	}
	
	public void CharacterFinishedSpeaking(LipSyncInfo info, LipSyncAudioEventArgs args)
	{
		io.SendMessage(new VHFCharacterFinishedSpeaking(CharacterID));	
	}
		
	#endregion
	
	#region Audio
	
	public override void PlayAudio(string fileName, System.Object param = null)
    {
        StartCoroutine(PlayLocalAudio(GetAudioURL(), fileName));
    }

    protected IEnumerator PlayLocalAudio(string URL, string fileName) 
    {
	    //	AddDebugLine("Going to download audio from: " + URL + " with fileName: " + fileName);
        WWW www = new WWW(URL + fileName);

        // Wait here until the file has been loaded
        yield return www;
		
		if(www.error != null && www.error != "")
			AddDebugLine("Audio error: " + www.error);
		else
			Debug.Log("Got audio file successfully!");
		
        //AddDebugLine("Audio connection waited: " + sc.GetElapsedTime() + " before returning");

        AudioClip clip = www.GetAudioClip(false, false);
		if(!clip.isReadyToPlay)
			AddDebugLine("Clip not ready to play");
		
		//The filename comes with a path, so we remove the path to just keep the name of the wav file.
        clip.name = fileName.Substring(fileName.LastIndexOf('/') + 1);
		
		Debug.Log("ClipName: " + clip.name);
		
      //  AddDebugLine("Clip Length: " + clip.length + " name: " + clip.name + " and has: " + clip.samples);

        LipSyncInfo lipSync = new LipSyncInfo(clip);
		
		//Call the VirtualHumanWillTalk event.
		OnVirtualHumanWillTalk();
		
	    //Add to the sound manager for play.
    	SoundManager.EnqueueLipSync(lipSync);
    }
	
	#endregion
	
	
    #region Not-yet implemented abstract methods
    public override void FindResponse(string userInput)
    {
		Transcript.AddEntry(UserTranscriptName, userInput);
        //throw new System.NotImplementedException();
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
	
	public override void PlayTaggedAction (string tag)
	{
		throw new NotImplementedException ();
	}

    #endregion

	protected void OnApplicationQuit() {
		if (io != null)
		{
			io.Stop();
		}
	}
	
}