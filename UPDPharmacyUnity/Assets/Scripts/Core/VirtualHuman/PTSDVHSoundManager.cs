using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PTSDVHSoundManager : RenBehaviour {
	protected Queue<LipSyncInfo> AudioQueue;
	protected Queue<LipSyncInfo> OriginalAudioQueue;
	protected LipSyncInfo CurrentLipSync;
	
	
	protected bool isPlaying = false;
	
	public event LipSyncAudioEvent PlayEvent;
	public event LipSyncAudioEvent StoppedEvent;
	
	
	protected PTSDVHAnimationManager AnimationManager;
	
	private bool createAnimationManagerFlag = false;
	private GameObject virtualCashier = null;
	
	protected override void Start()
	{
		base.Start();
		AudioQueue = new Queue<LipSyncInfo>();
		OriginalAudioQueue = new Queue<LipSyncInfo>();
		CurrentLipSync = null;
		
		if(Application.isWebPlayer)
			this.StoppedEvent += VHSoundManager_StoppedEvent;
	}
	
	void VHSoundManager_StoppedEvent(LipSyncInfo info, LipSyncAudioEventArgs args)
	{
		Application.ExternalCall("SubmitCharacterFinishedSpeakingEvent", info.AnimationName);
	}
	
	public void EnqueueLipSync(LipSyncInfo info, LipSyncInfo originalAudioFileInfo)
	{
		if (info.Audio.length > 0)
		{
			if (!isPlaying && AudioQueue.Count == 0) 
			{//if not playing anything and queue empty, just play it!
				OriginalAudioQueue.Clear();
				PlayLipSync(info, originalAudioFileInfo);
			}else
			{
				AudioQueue.Enqueue(info);
				OriginalAudioQueue.Enqueue(originalAudioFileInfo);
			}
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	protected override void Update()
	{
		base.Update();

		if (isPlaying && !GetComponent<AudioSource>().isPlaying)
		{
			//Trigger the stop event.
			OnStoppedEvent(CurrentLipSync, new LipSyncAudioEventArgs(LipSyncAudioEventType.LIP_SYNC_AUDIO_STOPPED));               
			//Check if we should play something else.
			if (AudioQueue.Count > 0)
			{
				PlayLipSync(AudioQueue.Dequeue(), OriginalAudioQueue.Dequeue());
			}
			else
			{
				CurrentLipSync = null;
				isPlaying = false;
			}
		}
	}
	
	protected void PlayLipSync(LipSyncInfo info, LipSyncInfo originalAudioFileInfo)
	{

		if(!createAnimationManagerFlag)
		{
			if(findClosest.getCheckoutCounterNumber() == 1){
				virtualCashier = GameObject.Find ("VirtualHuman1");
				AnimationManager = virtualCashier.GetComponent<PTSDVHAnimationManager>();
			}else if(findClosest.getCheckoutCounterNumber() == 2){
				virtualCashier = GameObject.Find ("VirtualHuman2");
				AnimationManager = virtualCashier.GetComponent<PTSDVHAnimationManager>();
			}else if(findClosest.getCheckoutCounterNumber() == 3){
				virtualCashier = GameObject.Find ("VirtualHuman3");
				AnimationManager = virtualCashier.GetComponent<PTSDVHAnimationManager>();
			}else if(findClosest.getCheckoutCounterNumber() == 4){
				virtualCashier = GameObject.Find ("VirtualHuman4");
				AnimationManager = virtualCashier.GetComponent<PTSDVHAnimationManager>();	
			}else{
				virtualCashier = GameObject.Find ("VirtualHuman1");
				AnimationManager = virtualCashier.GetComponent<PTSDVHAnimationManager>();		
			}
			createAnimationManagerFlag = true;
		}
		
		//Update state of the sound manager
		CurrentLipSync = info;
		isPlaying = true;
		
		//FaceFX doesn't seem to play the audio correctly.
		virtualCashier.GetComponent<AudioSource>().clip = originalAudioFileInfo.Audio;
		virtualCashier.GetComponent<AudioSource>().Play();
		
		Debug.Log("Lip sync file name : " + info.AnimationName);
		Debug.Log("Audio file name during lip sync: " + originalAudioFileInfo.AnimationName);
		
		if (AnimationManager.ContainsClipWithName(info.AnimationName))
		{
			//We play it through the animation manager to make sure the lipSync
			//works the best it can.
			Debug.Log("Playing lip sync: " + info.AnimationName);
			AnimationManager.PlayLipSync(info);
		}
		
		//Trigger event.
		OnPlayEvent(info, new LipSyncAudioEventArgs(LipSyncAudioEventType.LIP_SYNC_AUDIO_PLAY));
	}
	
	public void InterruptLipSync()
	{
		// Stop the current audio from playing and clear the audio queue, 
		// in case anything else is queued up
		GetComponent<AudioSource>().Stop();
		AudioQueue.Clear();
		OriginalAudioQueue.Clear();
		AnimationManager.InterruptLipSync();		
	}
	
	protected void OnPlayEvent(LipSyncInfo info, LipSyncAudioEventArgs args)
	{
		if (PlayEvent != null)
		{
			
			PlayEvent(info, args);
		}
	}
	
	protected void OnStoppedEvent(LipSyncInfo info, LipSyncAudioEventArgs args)
	{
		if (StoppedEvent != null)
		{
			
			StoppedEvent(info, args);
		}
	}
	
	public bool IsPlaying()
	{
		return isPlaying;
	}

}

