using UnityEngine;
using System.Collections.Generic;

public class LipSyncInfo
{
    /// <summary>
    /// Name of the LipSync Animation
    /// </summary>
    public string AnimationName;
    /// <summary>
    /// AudioClip to play
    /// </summary>
    public AudioClip Audio;
    /// <summary>
    /// List of phonemes to create a new Animation if it doesn't exist.
    /// </summary>
    public List<Phoneme> Phonemes;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="audio"></param>
    /// <param name="phonemes"></param>
    public LipSyncInfo(AudioClip audio, string animationName = null, XMLNodeList phonemes = null)
    {

        this.AnimationName = animationName;
        this.Audio = audio;
        if (this.AnimationName == null && this.Audio != null)
        {
            this.AnimationName = GetLipSyncAnimationNameForAudioClip(this.Audio);
        }
        
        if (phonemes != null)
        {
            this.Phonemes = new List<Phoneme>();
            foreach (XMLNode n in phonemes)
            {
                //TODO: Create List depends on data format I haven't finalized yet.
                Debug.Log("Do something with this phoneme" + n.ToString());
            }
        }
        else
        {
            Phonemes = null;
        }
    }

    public static string GetLipSyncAnimationNameForAudioClip(AudioClip clip) 
    {
        //Removes extension
        //Sends to lower case
        //Adds "Default_"
        Debug.Log(clip.name);
        int index = clip.name.LastIndexOf('.');
        string name;
        if (index != -1)
        {
            name = clip.name.Remove(index);
        }else{
            name = clip.name;
        }

        return "Default_" + name.ToLower();
    }

}
