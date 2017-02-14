using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToggleCameras : MonoBehaviour {
	public Camera carCamera;
	public Camera pharmacyCamera;
	public Camera privateRoomCamera;

	public GameObject PoliceCarMenu;
	public GameObject CVSSidebar;
	public GameObject CVSTutorial;
    public GameObject PrivateRoom;

    public Text totalTime;
    public Text pharmacyTime;
    public Text privateRoomTime;

	// fading logic is done here
	public Texture2D fadeOutTexture; 	// will overlay screen
	public float fadeSpeed = 0.02f;

	private int drawDepth = -1000;   	// texture's order in draw hierarchy. a low number means it renders on top
	private float alpha = 0.01f; 		// the texture's aplpha between 0 and 1
	private int fadeDir = 1;			// the direction to fade: in = -1 or out = 1
	private bool fading = false;

    private CVSTutorial cvsTutorialScript;
    public AudioSource[] audioSources;

    float TIME_CVSPharmacy;
    float TIME_PrivateRoom;
    float TIME_InteractionEnded;

	// used for fading logic
	private const int pauseTime = 30;	// pause will be reset to this
	private int pausing = pauseTime;	// iterates

	// Use this for initialization
	void Start () {

		// initialize in the car
		carCamera.enabled = true;
		pharmacyCamera.enabled = false;
		privateRoomCamera.enabled = false;

		UpdateMenus ();

		// center police car menu
		float 	w = Screen.width / 2,
				h = Screen.height / 2,
				z = Camera.main.nearClipPlane;

		Vector3 centerVector = new Vector3 (w, h, z);
		PoliceCarMenu.transform.position = carCamera.ScreenToWorldPoint( centerVector );

        audioSources = GetComponents<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	}

    void OnGUI()
    {

        // this function runs all time. The boolean "fading" keeps it from doing its
        // logic. fading is changed in Update function, after cop car arrives at CVS.

        if (fading)
        {

            // want it to do nothing at beginning
            if (pausing-- > 0)
            {

                // to do: for some reason pharamcy camera switches with no
                // black fader in front of it, so awkward transition. find out
                // why this is

                if (pharmacyCamera.enabled)
                    PaintBlack(pharmacyCamera);

                // do nothing
                return;
            }

            // fade out/in alpha using direction, speed, and Time.deltaTime

            // have to hardcode in the adjustment, since it's taking a long time on
            // VERG alienware computer. Macbook works fine.
            alpha += 0.03f * fadeDir /* + fadeDir * fadeSpeed * Time.deltaTime*/;

            // force (clamp) the number between 0 and 1 because GUI.color uses alpha between 0 and 1
            alpha = Mathf.Clamp01(alpha);

            // set color of our GUI (in this case our texture). All color values remain the same & the alpha is set to the alpha varaible
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha); // set alpha value
            GUI.depth = drawDepth;                                                // make sure black texture will render on top
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);

            // fading out
            // need to swap cameras after
            if (alpha > 0.95f && fadeDir == 1)
            {
                fading = false;
                fadeDir = 0;
                SwapCameras(true);
            }

            // fading in
            // no need to swap cameras
            if (alpha < 0.1f && fadeDir == -1)
            {
                // fires when fading is over
                fading = false;

                GameObject g = GameObject.Find("Pharmacy Tutorial");
                cvsTutorialScript = g.GetComponent("CVSTutorial") as CVSTutorial;
                cvsTutorialScript.onActiveCamera();
            }
        }
    }

    public void UpdateTime()
    {
        int TimeInteger = (int)Time.time;
        int Minutes = TimeInteger / 60;
        int Seconds = TimeInteger % 60;
    }

    public void RecordTimePharmacyStarts()
    {
        TIME_CVSPharmacy = Time.time;
    }

    public void PlayPoliceCallAudio()
    {
        // GetComponents<AudioSource>()[0].Play();
        audioSources[0].Play();
    }

    public void StopPoliceCallAudio()
    {
        if (audioSources[0].isPlaying)
            audioSources[0].Stop();
    }

    public void PlayPsychiatristAudio()
    {
        audioSources[1].volume = 1f;
        audioSources[1].Play();
    }

    public void StopPsychiatristAudio()
    {
        if (audioSources[1].isPlaying)
            audioSources[1].Stop();
    }

    public void RecordTimePrivateRoomStarts()
    {
        TIME_PrivateRoom = Time.time;
    }

    public void RecordTimeInteractionEnds()
    {
        TIME_InteractionEnded = Time.time;
    }

    public void FadeIn()
    {
        fadeDir = -1;           // texture goes from fully visible to invisible
        pausing = pauseTime;    // reset this
        fading = true;          // reset this as well
    }

    public void FadeOut()
    {       // similar logic to FadeIn()
        fadeDir = 1;            // texture goes from invisible to fully visible
        pausing = pauseTime;
        fading = true;
    }

    void PaintBlack(Camera c) {
		// bits of OnGUI logic meant to ensure that transition is clean

		// make sure alpha has value of 1
		alpha = 1.0f;

		// set color of our GUI (in this case our texture). All color values remain the same & the alpha is set to the alpha varaible
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha); // set alpha value
		GUI.depth = drawDepth; 

		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture);
	}


	public void SwapCameras(bool tutorial)
	{
		// swap the cameras
		if (carCamera.enabled) {
			
			// switch camera logic
			carCamera.enabled = false;
			pharmacyCamera.enabled = true;

            TIME_CVSPharmacy = Time.time;

			// fade in to the scene
			FadeIn ();
		} else if (pharmacyCamera.enabled) {
            // move private location
            MovePrivateLocation();

		} else { // private room camera
			privateRoomCamera.enabled = false;
			carCamera.enabled = true;
		}

		// update the sidebars, menus, et cetera
		UpdateMenus();
	}

    public void UpdateFeedbackTime()
    {
        // int TotalTime            = (int)(TIME_InteractionEnded - TIME_CVSPharmacy);
        int TotalPharmacyTime    = (int)(TIME_PrivateRoom - TIME_CVSPharmacy);
        int TotalPrivateRoomTime = (int)(TIME_InteractionEnded - TIME_PrivateRoom);


        // if (TotalTime < 0)            TotalTime *= -1;
        if (TotalPharmacyTime < 0)    TotalPharmacyTime *= -1;
        if (TotalPrivateRoomTime < 0) TotalPrivateRoomTime *= -1;

        int TotalTime = TotalPharmacyTime + TotalPrivateRoomTime;

        totalTime.text = "You completed the interaction in " + ConvertTimeToString(TotalTime);
        pharmacyTime.text = "Of that time, " + ConvertTimeToString(TotalPharmacyTime) + " was spent in a state of higher crisis,";
        privateRoomTime.text = ConvertTimeToString(TotalPrivateRoomTime) + " was spent in a state of lower crisis.";
        
    }

    public string ConvertTimeToString(int time)
    {
        int minutes = time / 60;

        string MinutesString = "";
        if (minutes == 0) { /* do nothing */ }
        else if (minutes == 1) { MinutesString += "1 minute and "; }
        else { MinutesString += minutes + " minutes and "; }

        int seconds = time % 60;
        string SecondsString = seconds + " second";
        if (seconds != 1) { SecondsString += "s"; }

        return MinutesString + SecondsString;
    }

    public void MovePrivateLocation()
    {
        // check CVS tutorial script to see if conditions are met
        if (!cvsTutorialScript.ReadyToGoToPrivateLocation())
        {
            return;
        }

        TIME_PrivateRoom = Time.time;

        cvsTutorialScript.MoveToPrivateLocation();
        pharmacyCamera.enabled = false;
        privateRoomCamera.enabled = true;

        // deactivate old sidebar
        // 
        // TODO: Move this to CVSTutorial Script, and have some
        // hide() function which handles this
        CVSSidebar.SetActive(false);
        CVSTutorial.SetActive(false);

        PrivateRoom pr = PrivateRoom.GetComponent("PrivateRoom") as PrivateRoom;

        // TODO: Implement this
        pr.Init();
    }

	void UpdateMenus() {
        PoliceCarMenu.SetActive (carCamera.enabled);
	}
}
