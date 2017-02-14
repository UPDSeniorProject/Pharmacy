using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class TutorialAudioManager : MonoBehaviour {
	public AudioClip OpeningClip;
	public AudioClip IntroClip;
	public AudioClip Confirmation;
	public List<AudioClip> TutorialClips = new List<AudioClip>();

	private AudioSource mSource;
	private int currentClip = 0;
	// Use this for initialization
	void Start () {
		mSource = GetComponent<AudioSource>();
	}

	public void PlayOpeningClip()
	{
		mSource.clip = OpeningClip;
		mSource.Play();
		//StartCoroutine(PlaySound());
    }

	public void PlayIntroClip()
	{
		/*if(mSource.isPlaying)
			mSource.Stop();

        mSource.clip = IntroClip;
		mSource.Play();*/
		StartCoroutine(PlaySound(IntroClip));
		//StartCoroutine(PlaySound());
	}

	public void PlayConfirmation()
	{
		//Debug.Log(mSource.isPlaying);

		if(mSource.isPlaying)
			mSource.Stop();
			//mSource.Stop();

		mSource.clip = Confirmation;
		mSource.Play();
		//StartCoroutine(PlaySound(Confirmation));
	}

	public void PlayAudioClip()
	{
		if(currentClip < TutorialClips.Count)
		{
		StartCoroutine(PlaySound(TutorialClips[currentClip]));
		currentClip++;
		}
		//mSource.Play();
	}

	IEnumerator PlaySound(AudioClip clip){
		//while(mSource.isPlaying)
		//{
			yield return new WaitForSeconds(2f);
		//}
		mSource.clip = clip;//TutorialClips[currentClip];
		mSource.Play();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
