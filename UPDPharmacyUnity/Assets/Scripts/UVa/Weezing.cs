using UnityEngine;
using System.Collections;

public class Weezing : RenBehaviour {

	public AudioClip PantingLoop;
	public AudioClip WeezingClip;
    public RenButton WeezingButton= new RenButton ();
	
	public GameObject VirtualHuman;
	public AbstractVPFCommunicator Communicator = null;
	
	public bool RampDownVolumeFlag = false;
	
	// Use this for initialization
	protected override void Start () 
	{
	    base.Start ();

		WeezingButton.Label="Listen to Lungs";
		WeezingButton.ButtonPressed += HandleWeezingButtonButtonPressed; 
		AddGUIElement(WeezingButton);
	}

	
	void HandleWeezingButtonButtonPressed (RenButton btn, ButtonPressedEventArgs args)
	{
		GetComponent<AudioSource>().clip = WeezingClip;
        GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
		
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		
		GetVPFCommunicator ();
		RampDownVolume ();
		
		
		if(!VirtualHuman.GetComponent<AudioSource>().isPlaying) 
		{
			GetComponent<AudioSource>().volume = 1;
		}


		
		
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().clip = PantingLoop;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().Play ();
		}
	}

	void HandleCommunicatorVirtualHumanWillTalk (GameObject VirtualHHuman)
	{
		RampDownVolumeFlag = true;
	}

	/// <summary>
	/// Gets the VPF communicator.
	/// </summary>
	void GetVPFCommunicator ()
	{
		if(Communicator == null) 
		{
			Communicator = VirtualHuman.GetComponent<AbstractVPFCommunicator>();
			if(Communicator != null) 
			{
				Communicator.VirtualHumanWillTalk += HandleCommunicatorVirtualHumanWillTalk;
			}
		}
	}

	void RampDownVolume ()
	{
		if(RampDownVolumeFlag && GetComponent<AudioSource>().volume > 0) 
		{
			GetComponent<AudioSource>().volume = GetComponent<AudioSource>().volume - 0.05f;
		}else if(GetComponent<AudioSource>().volume == 0) {
			RampDownVolumeFlag = false;	
		}
	}
}
