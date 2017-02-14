using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PoliceCarMenu : MonoBehaviour {
	// public
	public GameObject[] MenuTexts;
	public GameObject menu;
    public GameObject TutorialMenu;
	public Camera policeCarCamera;

    public GameObject PostcardMenu;
    ToggleCameras tc;

	// private
	int menuOption = 0;
	bool ReadyToSwapScreens = false;
    
	// Use this for initialization
	void Start () {
        tc = GameObject.Find("Cameras").GetComponent("ToggleCameras") as ToggleCameras;
    }

    public void NextMenu()
    {
        // on police car scene
        if (policeCarCamera.isActiveAndEnabled)
        {
            if (menuOption < MenuTexts.Length - 1)
            {
                // go to next screen
                NextMenuScreen();
            }

            else {

				// disable menu
				menu.SetActive(false);
				tc.FadeOut ();
            }
        }
    }

    public void SkipMenu()
    {
        ReadyToSwapScreens = true;
        SwapCameras(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Back()
    {
        if (menuOption > 0)
        {
            // make the current option invisible
            MenuTexts[menuOption].SetActive(false);

            // make the next one appear
            menuOption--;
            MenuTexts[menuOption].SetActive(true);

            CheckPostCard();
        }
    }

	public void SwapCameras()
	{
		SwapCameras (true);
	}

	public void ReadyToSwapCameras() 
	{
		ReadyToSwapScreens = true;
	}

	void NextMenuScreen()
	{
        MenuTexts[menuOption].SetActive(false);

        // make the next one appear
        menuOption++;
        MenuTexts[menuOption].SetActive(true);

        CheckPostCard();
	}

    void CheckPostCard()
    {
        // make the current option invisible

        // open postcard
        if (menuOption == 3)
        {
            tc.PlayPoliceCallAudio();

            PostcardMenu.SetActive(true);
            TutorialMenu.SetActive(false);
        }

        // close postcard
        else
        {
            // if playing audio
            if (menuOption == 4 || menuOption == 2)
                tc.StopPoliceCallAudio();
            
            PostcardMenu.SetActive(false);
            TutorialMenu.SetActive(true);
        }
    }

	public void SwapCameras(bool tutorial)
	{
		// first, check if the PoliceCarMenu camera is on
		if (policeCarCamera.enabled && ReadyToSwapScreens) 
		{
			// swap cameras
			tc.SwapCameras (tutorial);
			// Note: ^^ menu swapping also performed by this function ^^
			
			// so the function doesn't run twice
			ReadyToSwapScreens = false;
		}
	}
}
