using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmpathyCoach : MonoBehaviour {

    // public
    public GameObject[] Messages;
    public GameObject MessageBubble;
    public GameObject NextButton;
    public GameObject CloseButton;
    public GameObject BlackBrain;
    public GameObject BlueBrain;

    // private
    int currentMessage = 0;

    int INTRODUCE_BUBBLE = 0,
        DEEPBREATH_BUBBLE = 2,
        PARAPHRASE_BUBBLE = 4,
        CLEARLIMIT_BUBBLE = 6;




	void Start () {
	}
	
	void Update () {
    }

    public void ShowEmpathyTask(int taskNumber)
    {
        if (taskNumber == 0)
        {
            IntroduceYourselfBubble();
        }
        else if (taskNumber == 1)
        {
            DeepBreathBubble();
        }
        else if (taskNumber == 2)
        {
            ParaphraseBubble();
        }
        else if (taskNumber == 3)
        {
            ClearLimitBubble();
        }
    }

    void ShowMessage()
    {
        if (currentMessage < 0 || currentMessage > Messages.Length)
            return;

        // make sure only message at a time
        for (int i = 0; i < Messages.Length; i++)
        {
            Messages[i].SetActive(false);
        }

        Messages[currentMessage].SetActive(true);
    }

    public void IntroduceYourselfBubble()
    {
        ShowMessage();
    }

    
    public void DeepBreathBubble()
    {
        ShowMessage();
    }

    public void ClearLimitBubble()
    {
        ShowMessage();
    }

    public void ParaphraseBubble()
    {
        ShowMessage();
    }

    public void ToggleMessageBox()
    {
        MessageBubble.SetActive(!MessageBubble.activeInHierarchy);
    }

    public void OpenMessageBubble()
    {
        MessageBubble.SetActive(true);

        // make sure only one is showing at a time
        ShowMessage();
    }

    public void CloseMessageBubble()
    {
        MessageBubble.SetActive(false);
        NextMessage();  // prepare next message
        ShowBlackBrain();
    }

    public void hide()
    {
        BlackBrain.SetActive(false);
        BlueBrain.SetActive(false);
    }

    public void NextMessage()
    {

        Messages[currentMessage].SetActive(false);
        currentMessage++;

        // account for overflow
        currentMessage %= Messages.Length;
        Messages[currentMessage].SetActive(true);

        if (endOfMessage())
            displayCloseButton();
        else
            displayNextButton();
    }

    public void ShowBlueBrain(int currentMessage)
    {
        BlueBrain.SetActive(true);
        BlackBrain.SetActive(false);

        // note, this assumes each empathy message is only two screens long! WATCH OUT
        this.currentMessage = currentMessage * 2;
    }

    public void ShowBlackBrain()
    {
        BlueBrain.SetActive(false);
        BlackBrain.SetActive(true);
    }

    void displayCloseButton()
    {
        NextButton.SetActive(false);
        CloseButton.SetActive(true);
    }


    void displayNextButton()
    {
        CloseButton.SetActive(false);
        NextButton.SetActive(true);
    }

    public bool endOfMessage()
    {
        return 
            (currentMessage == PARAPHRASE_BUBBLE + 1 ||
            currentMessage == CLEARLIMIT_BUBBLE + 1 ||
            currentMessage == INTRODUCE_BUBBLE + 1 ||
            currentMessage == DEEPBREATH_BUBBLE + 1);
    }
}
