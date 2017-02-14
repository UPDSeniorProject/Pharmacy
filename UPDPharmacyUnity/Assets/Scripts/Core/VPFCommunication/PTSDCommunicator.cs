using UnityEngine;
using System.Collections;
using VirtualHumanFramework.Core.Messages;
using System;
using VirtualHumanFramework.Core.Messages.Events.Virtual;
using VirtualHumanFramework.Core.Events.Virtual;
using VirtualHumanFramework.Core.Messages.Signals;
using System.IO;

public class PTSDCommunicator : AbstractVPFCommunicator
{
	public string Audio_Clips_Url;
	public string PTSD_Woz_Server_Address;

	protected PTSDVHSoundManager PTSDSoundManager;

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
		//return "file:///" + ServerAddress + "/../Audio/" + CharacterID + "/";
		//return "file:///" + "C:/vpf_databases" + "/Audio/" + CharacterID + "/";
		if(findClosest.getCheckoutCounterNumber() == 1){
			return "file:///" + Audio_Clips_Url + "/Audio/" + "37453" + "/";
		}else if(findClosest.getCheckoutCounterNumber() == 2){
			return "file:///" + Audio_Clips_Url + "/Audio/" + "37448" + "/";
		}else if(findClosest.getCheckoutCounterNumber() == 3){
			return "file:///" + Audio_Clips_Url + "/Audio/" + "37449" + "/";
		}else if(findClosest.getCheckoutCounterNumber() == 4){
			return "file:///" + Audio_Clips_Url + "/Audio/" + "37448" + "/";
		}else{
			return "file:///" + Audio_Clips_Url + "/Audio/" + "37453" + "/";
		}
    }
	
	public string GetAudioDirectory()
	{		
		//return "file:///" + ServerAddress + "/../Audio/" + CharacterID + "/";
		//return "file:///" + "C:/vpf_databases" + "/Audio/" + CharacterID + "/";
		if(findClosest.getCheckoutCounterNumber() == 1){
			return Audio_Clips_Url + "/Audio/" + "37453" + "/";
		}else if(findClosest.getCheckoutCounterNumber() == 2){
			return Audio_Clips_Url + "/Audio/" + "37448" + "/";
		}else if(findClosest.getCheckoutCounterNumber() == 3){
			return Audio_Clips_Url + "/Audio/" + "37449" + "/";
		}else if(findClosest.getCheckoutCounterNumber() == 4){
			return Audio_Clips_Url + "/Audio/" + "37448" + "/";
		}else{
			return Audio_Clips_Url + "/Audio/" + "37453" + "/";
		}
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

		PTSDSoundManager = gameObject.GetComponent<PTSDVHSoundManager>();
		
		PTSDSoundManager.StoppedEvent += HandleSoundManagerStoppedEvent;;
    }

	void OnEnable()
	{
		Messenger<int>.AddListener("CurrentActiveNPCs", sendCurrentActiveNPCsMessage);
	}
	
	void OnDisable()
	{
		Messenger<int>.RemoveListener("CurrentActiveNPCs", sendCurrentActiveNPCsMessage);
	}

    void HandleSoundManagerStoppedEvent (LipSyncInfo info, LipSyncAudioEventArgs args)
    {
    	io.SendMessage(new VHFCharacterFinishedSpeaking(CharacterID));
    }

	void sendCurrentActiveNPCsMessage(int shoppersCount)
	{
		Debug.Log("broadcast message received for shoppers count : "+shoppersCount);
		if(io != null){
			Debug.Log("broadcast message : "+shoppersCount);
			io.SendMessage(new VHFPTSDCommand(-1, "currentShopperCount*"+shoppersCount));
		}
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
    
	PTSDSimulatorIO io;
	ThreadSafeList<VHFMessage> messageQueue;

	public override void Initialize() {
		Initialize(false); // VPF2 handles logging
		
		messageQueue = new ThreadSafeList<VHFMessage>();
		
        RenParameterParser parameterParser = IPS.GetComponent<RenParameterParser>();
        
		//ServerAddress = Application.dataPath;
		
		// Getting the parameter values from command line
		//string simulatorAddress = parameterParser.GetParameter("simulatorAddress", "10.136.2.198");
		//CharacterID = parameterParser.GetParameterAsInt("characterID", 7233);

		Debug.Log ("Server Address : " + ServerAddress);
		Debug.Log ("Character ID : " + CharacterID);
		string simulatorAddress = ServerAddress;

		if (CharacterID != 0)
		{
	        io = PTSDSimulatorIO.CreateNewCommunicator(this, CharacterID, simulatorAddress);
	        io.Start();
		}
		else
		{
			Debug.Log("Invalid character id!");
			throw new Exception("Invalid character ID!");
		}

		PTSDSoundManager = gameObject.GetComponent<PTSDVHSoundManager>();
		PTSDSoundManager.StoppedEvent += new LipSyncAudioEvent(CharacterFinishedSpeaking);
		
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
		if (message is VHFVirtualActionOccurred)
        {
            TriggerEvent(message as VHFVirtualActionOccurred);
        }	
		else if (message is VHFNewLookTarget)
		{
			//ChangeLookTarget((message as VHFNewLookTarget).Target);	
		}	
		else if (message is VHFInterruptCurrentAction)
		{
			InterruptCurrentAction();
		}
		else if (message is VHFGrayOut)
		{
			GrayOut(message as VHFGrayOut);
		}
		else if (message is VHFPTSDCommand)
		{
			PTSDCommand(message as VHFPTSDCommand);
		}
	}
	
	// TODO: Figure out how to notify the simulator of a complete action 
	//       if there is no audio file
	public void TriggerEvent(VHFVirtualActionOccurred VirtualAction) {
        
		//Add text to transcript
		string speech = VirtualAction.SpeechText;
		int index = speech.IndexOf(")");
		string cashier_speech = VirtualAction.SpeechText; 
		Debug.Log(index);
		if(index > 0){
			cashier_speech = speech.Substring(index+1);
			Debug.Log(speech.Substring(index+1));
		}
		Transcript.AddEntry(VHTranscriptName, cashier_speech);
		
		// Play any associated audio file
        if (!string.IsNullOrEmpty(VirtualAction.AudioFileName))
        {
			// Super duper hack for playing audio files for PTSD cashiers. Need to figure out a way to do this better
			string tempAudioFileName = VirtualAction.AudioFileName.Substring(6);
			string[] audioFileNames = GetFileNames(GetAudioDirectory());
			string audioFileName = VirtualAction.AudioFileName;

			foreach(string tAudioFileName in audioFileNames)
			{
				Debug.Log (tAudioFileName);
				string tempDirAudioFileName = tAudioFileName.Substring(6);
				if(tempDirAudioFileName.Equals(tempAudioFileName))
				{
					Debug.Log ("Matched audio file : "+ tempAudioFileName + " with dir file : " + tempDirAudioFileName);
					audioFileName = tAudioFileName;
					break;
				}
			}

			Debug.Log("Playing audio file: " + GetAudioURL() + audioFileName);
			PlayAudio(audioFileName, VirtualAction.AudioFileName); //remove first slash
			//Debug.Log("Playing audio file: " + GetAudioURL() + VirtualAction.AudioFileName);
			//PlayAudio(VirtualAction.AudioFileName); //remove first slash
        }
		
		if (!string.IsNullOrEmpty(VirtualAction.Animation))
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

	public void PTSDCommand(VHFPTSDCommand ptsdCommand)
	{
		string message = ptsdCommand.PTSDMessage;
		Debug.Log (message.Substring(0,12));
		if(message == "startCheckout")
		{
			Debug.Log("Starting check out...");
			GUIcontrol.isCheckoutStarted = true;
		}
		else if(message.Substring(0,13) == "cashierSpeech")
		{
			//Add text to transcript
			string cashier_speech = message.Substring(13);
			Transcript.AddEntry(VHTranscriptName, cashier_speech);
		}
		else if(message == "returnCorrectChange")
		{
			Debug.Log ("Will be returning the right change");
			Messenger.Broadcast("return right change");
		}
		else if(message == "returnWrongChange")
		{
			Debug.Log ("Will be returning the WRONG change");
			Messenger.Broadcast("return wrong change");
		}
		else if(message.Substring(0,12) == "changeVolume")
		{
			string direction = (message.Substring(13));
			GameObject audioObject = GameObject.Find ("Audio");
			AudioSource ads = audioObject.GetComponent<AudioSource>();
			if(direction == "Up"){
				ads.volume = ads.volume + 0.02f;
			}else{
				ads.volume = ads.volume - 0.02f;
			}

			AudioSource[] audiosources = audioObject.GetComponentsInChildren<AudioSource>();
			foreach(AudioSource audiosource in audiosources){
				Debug.Log ("Editing audio of something");
				if(direction == "Up"){
					audiosource.volume = audiosource.volume + 0.02f;
				}else{
					audiosource.volume = audiosource.volume - 0.02f;
				}
			}
		}
        else if(message.Substring(0,10) == "changeNPCs")
        {
            string direction = message.Substring(11);
            if(direction == "Up")
                Messenger<int>.Broadcast("NPCChangeCount", 1);
            else
                Messenger<int>.Broadcast("NPCChangeCount", -1);
        }
        else if (message.Substring(0, 16) == "EndBumpEncounter")
        {
            Messenger.Broadcast("EndBumpEncounter");
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
		PTSDSoundManager = gameObject.GetComponent<PTSDVHSoundManager>();
		PTSDSoundManager.InterruptLipSync();
		
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
	
	public void PlayAudio(string fileName, string originalLipSyncFileName,System.Object param = null)
    {
        StartCoroutine(PlayLocalAudio(GetAudioURL(), fileName, originalLipSyncFileName));
    }

	public override void PlayAudio(string fileName, System.Object param = null)
	{
		Debug.Log ("Trying to call the inherited PlayAudio method. Something is wrong");
	}

    protected IEnumerator PlayLocalAudio(string URL, string fileName, string lipsyncFileName) 
    {
		PTSDSoundManager = gameObject.GetComponent<PTSDVHSoundManager>();

	    //	AddDebugLine("Going to download audio from: " + URL + " with fileName: " + fileName);
        WWW www = new WWW(URL + fileName);
		WWW wwwLipSync = new WWW(URL + lipsyncFileName);

        // Wait here until the file has been loaded
        yield return www;
		yield return wwwLipSync;
		
		if(www.error != null && www.error != "")
			Debug.Log("Audio error: " + www.error);
		else
			Debug.Log("Got audio file successfully!");

		if(wwwLipSync.error != null && wwwLipSync.error != "")
			Debug.Log("Lip sync audio error: " + wwwLipSync.error);
		else
			Debug.Log("Got lip sync audio file successfully!");
		
        //AddDebugLine("Audio connection waited: " + sc.GetElapsedTime() + " before returning");

        AudioClip clip = www.GetAudioClip(false, false);
		if(!clip.isReadyToPlay)
			AddDebugLine("Clip not ready to play");

		AudioClip lipsyncClip = wwwLipSync.GetAudioClip(false, false);
		if(!lipsyncClip.isReadyToPlay)
			AddDebugLine("Lip sync clip not ready to play");
		
		//The filename comes with a path, so we remove the path to just keep the name of the wav file.
        clip.name = fileName.Substring(fileName.LastIndexOf('/') + 1);
		lipsyncClip.name = lipsyncFileName.Substring(lipsyncFileName.LastIndexOf('/') + 1);
		
		Debug.Log("ClipName: " + clip.name);
		Debug.Log("Lipsync ClipName: " + lipsyncClip.name);
		
      //  AddDebugLine("Clip Length: " + clip.length + " name: " + clip.name + " and has: " + clip.samples);

        LipSyncInfo lipSync = new LipSyncInfo(lipsyncClip);
		LipSyncInfo audioFileLipSync = new LipSyncInfo(clip);
		
		//Call the VirtualHumanWillTalk event.
		OnVirtualHumanWillTalk();
		
	    //Add to the sound manager for play.
		PTSDSoundManager.EnqueueLipSync(lipSync, audioFileLipSync);
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

	private static string[] GetFileNames(string path)
	{
		string[] files = Directory.GetFiles(path);
		for(int i = 0; i < files.Length; i++)
			files[i] = Path.GetFileName(files[i]);
		return files;
	}
	
}