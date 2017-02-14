using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReflectiveLearningPanel : RenBehaviour
{

    #region Constants
    public const string START_LABEL = "Start";
    public const string END_LABEL = "End";
    #endregion

    #region BookKeeping Fields

    /// <summary>
    /// This contains all moments in the present interaction.
    /// They are indexed in the dictionary by label.
    /// </summary>
    protected Dictionary<string, ReflectiveLearningMoment> Moments;

    /// <summary>
    /// Defines if Moments have been added or deleted since last Update.
    /// This is used to see if it has 
    /// </summary>
    public bool MomentsUpdated;

    /// <summary>
    /// 
    /// </summary>
    public bool HasEndMoment;

    /// <summary>
    /// 
    /// </summary>
    public bool ShowingEndMoment;

    /// <summary>
    /// 
    /// </summary>
    public ReflectiveLearningMoment CurrentMoment;
    /// <summary>
    /// 
    /// </summary>
    private int CurrentPromptIndex;
    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    public event ReflectiveLearningEvent AnswerSubmittedEvent;

    #endregion

    #region GUI Elements

    #region Elements
    public RenTextBox PromptDisplay = new RenTextBox();
    public RenInputBox AnswerBox = new RenInputBox(true, "ReflectiveLearningInputBox");
    public RenImage PictureFrame = new RenImage();
    public RenButton SubmitAnswer = new RenButton("Submit Answer");
    #endregion

    #region Positions
    public Rect PromptPosition = new Rect(165, 5, 740, 100);
    public Rect PictureFramePosition = new Rect(60, 5, 100, 100);

    public Rect AnswerBoxPosition = new Rect(60, 110, 840, 430);   
    public Rect SubmitAnswerPosition = new Rect(60, 545, 840, 50);
    #endregion

    public bool Displaying;
    
    #endregion

    #region MonoBehaviour Functions

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        DisplayGUIOnPaused = true; //Only one that should be able to (or a pause menu perhaps)
        Moments = new Dictionary<string, ReflectiveLearningMoment>();

        PromptDisplay.Position = PromptPosition;
        AddGUIElement(PromptDisplay);

        AnswerBox.Position = AnswerBoxPosition;

        AddGUIElement(AnswerBox);

        PictureFrame.Position = PictureFramePosition;
        PictureFrame.DrawBox = true;
        AddGUIElement(PictureFrame);

        SubmitAnswer.Position = SubmitAnswerPosition;
        SubmitAnswer.ButtonPressed += new ButtonPressedEventHandler(SubmitAnswer_ButtonPressed);
        AddGUIElement(SubmitAnswer);

        MomentsUpdated = false;
        HasEndMoment = false;
        ShowingEndMoment = false;

        //Set the current method to null
        CurrentMoment = null;

        //hide all of them!
        SetDisplayAllGUI(false);

    }
	
	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (MomentsUpdated)
        {
            //Check if we have special moments.
            if (TryTriggerMoment(START_LABEL))
            {
                Debug.Log("Triggered!");
            }
 
            //this is a different try because not having a Start moment
            //doesn't mean we can't have an end one.
            try
            {
                if (!HasEndMoment && Moments[END_LABEL] != null)
                {
                    //This makes sure we don't get added to the event twice.
                    HasEndMoment = true;
                    //this means we will stall at the end.
                    Manager.SimulationWillEnd += new SimulationEventHandler(Manager_SimulationWillEnd);
                }
            }
            catch (KeyNotFoundException)
            {
            }
            MomentsUpdated = false;
        }
    }



    protected override void OnGUI()
    {
        FixGUIPositions();
        base.OnGUI();
    }

    #endregion

    #region GUI specific function
    /// <summary>
    /// This methods makes a lot of assumptions about the setup 
    /// of the ReflectiveLearning panel, assuming that if you don't 
    /// have an image of a mentor or character making the question, 
    /// you want a different layout.
    /// </summary>
    protected virtual void FixGUIPositions()
    {
        if (Displaying)
        {
            //Check if we have a picture frame!!!
            PromptDisplay.Position = PromptPosition;
            if (PictureFrame.Image != null)
            {
                PictureFrame.Position = PictureFramePosition;

            }
            else
            {
                PromptDisplay.Position.x = PictureFramePosition.x;
                PromptDisplay.Position.width = AnswerBoxPosition.width;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="show"></param>
    protected override void SetDisplayAllGUI(bool show = true)
    {
        Displaying = show;
        base.SetDisplayAllGUI(show);
    }
    #endregion



    public void AddMoment(ReflectiveLearningMoment moment)
    {
        Moments[moment.Label] = moment;
        MomentsUpdated = true;
    }

    public void RemoveMoment(ReflectiveLearningMoment moment)
    {
        Moments.Remove(moment.Label);
        MomentsUpdated = true;
    }


    public bool TryTriggerMoment(string label)
    {
        if (CurrentMoment != null)
            return false; //Soomething else

        try 
        {
            if (!Moments[label].HasTriggered)
            {
                Manager.PauseSimulation(this);

                
                CurrentMoment = Moments[label];
                CurrentPromptIndex = -1;//

                SetDisplayAllGUI(true);

                DisplayNextPrompt();

                return true;
            }
        }
        catch (KeyNotFoundException){ }

        return false;
    }

    void SubmitAnswer_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (AnswerBox.text != null && AnswerBox.text != "")
        {
            OnAnswerSubmittedEvent(CurrentMoment.Prompts[CurrentPromptIndex], new ReflectiveLearningEventArgs(AnswerBox.text));

            DisplayNextPrompt();
        }
        else
        {
            Debug.Log("Show some error message!");
        }
    }

    protected void DisplayNextPrompt()
    {
        CurrentPromptIndex++;

        if (CurrentPromptIndex < CurrentMoment.Prompts.Count)
        {
            //Update the Prompt Text.
            PromptDisplay.text = CurrentMoment.Prompts[CurrentPromptIndex].Text;
            //Clear the Answer Box
            AnswerBox.text = "";
        }
        else
        {
            //hide GUI
            SetDisplayAllGUI(false); // hide the panel.
            //clear values
            CurrentPromptIndex = -1;
            CurrentMoment = null;
            //Allow simulation to continue.
            Manager.ResumeSimulation(this);
        }
    }


    protected void Manager_SimulationWillEnd(SimulationEventArgs args)
    {
        if (TryTriggerMoment(END_LABEL))
        {
            Manager.AddToWaitToEndList(this);
            ShowingEndMoment = true;
        }
    }

    protected void OnAnswerSubmittedEvent(ReflectiveLearningPrompt prompt, ReflectiveLearningEventArgs args)
    {
        if (AnswerSubmittedEvent != null)
        {
            AnswerSubmittedEvent(prompt, args);
        }
        else
        {
            Debug.Log("RL Answer not getting saved :(");
        }
    }

}

public class ReflectiveLearningEventArgs : System.EventArgs
{
    public string Answer;

    public ReflectiveLearningEventArgs(string answer)
    {
        this.Answer = answer;
    }
}

public delegate void ReflectiveLearningEvent(ReflectiveLearningPrompt prompt, ReflectiveLearningEventArgs args);
