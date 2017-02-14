using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VHSoundManager : RenBehaviour {
    protected Queue<LipSyncInfo> AudioQueue;
    protected LipSyncInfo CurrentLipSync;

    protected AudioSource audioSource;
    

    protected bool isPlaying = false;

    public event LipSyncAudioEvent PlayEvent;
    public event LipSyncAudioEvent StoppedEvent;


    protected VHAnimationManager AnimationManager;


    protected override void Start()
    {
        base.Start();
        AudioQueue = new Queue<LipSyncInfo>();
        CurrentLipSync = null;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            AddDebugLine("SoundManager: Couldn't locate Audio Source. Audio won't play.");

        AnimationManager = gameObject.GetComponent<VHAnimationManager>();

		audioSource.volume = 6.0f;
        if(Application.isWebPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
            this.StoppedEvent += VHSoundManager_StoppedEvent;
    }

    void VHSoundManager_StoppedEvent(LipSyncInfo info, LipSyncAudioEventArgs args)
    {
        Application.ExternalCall("SubmitCharacterFinishedSpeakingEvent", info.AnimationName);
    }

    public void EnqueueLipSync(LipSyncInfo info)
    {
        if (info.Audio.length >= 0)
        {
            if (!isPlaying && AudioQueue.Count == 0) 
            {//if not playing anything and queue empty, just play it!
                PlayLipSync(info);
                
            }else
                AudioQueue.Enqueue(info);
        }else
        {
          //  Debug.Log("no audio length");
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
                PlayLipSync(AudioQueue.Dequeue());
            }
            else
            {
                CurrentLipSync = null;
                isPlaying = false;
            }
        }
    }

    protected void PlayLipSync(LipSyncInfo info)
    {
        //Update state of the sound manager
        CurrentLipSync = info;
        isPlaying = true;

        //FaceFX doesn't seem to play the audio correctly.
        if (audioSource != null)
        {
            audioSource.clip = info.Audio;
            audioSource.Play();
        }else
        {
            Debug.Log("No audio source.  Clip won't play.");
        }

        if (AnimationManager.ContainsClipWithName(info.AnimationName))
        {
            //We play it through the animation manager to make sure the lipSync
            //works the best it can.
            //Debug.Log("Playing lip sync: " + info.AnimationName);
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

public enum LipSyncAudioEventType
{
    LIP_SYNC_AUDIO_PLAY = 0,
    LIP_SYNC_AUDIO_PAUSED = 1,
    LIP_SYNC_AUDIO_STOPPED = 2
}

public class LipSyncAudioEventArgs : System.EventArgs
{
    public LipSyncAudioEventType Type;
    public LipSyncAudioEventArgs(LipSyncAudioEventType t)
    {
        Type = t;
    }
}

public delegate void LipSyncAudioEvent(LipSyncInfo info, LipSyncAudioEventArgs args);
