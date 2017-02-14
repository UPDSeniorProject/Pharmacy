using UnityEngine;
using System.Collections;

public class CranialNerveAppActionHandler : RenAppActionHandler
{
    #region Initialization

    public Vector3 Forward = Vector3.forward;
    public GameObject LookTarget = null;
    public GameObject Head = null;


    protected VHAnimationManager AnimationManager;
    protected RenToolManager ToolManager;
    protected HandTool Hand;
    protected EyeChartTool EyeChart;
    protected OphthalmoscopeTool Ophthalmoscope;

    protected MouseEyeControl LeftEyeControl = null;
    protected MouseEyeControl RightEyeControl = null;
    protected EyeMovement LeftEye = null;
    protected EyeMovement RightEye = null;

    protected HeadLookBehaviour HeadLook = null;

    public bool SeeingDouble;

    protected bool LookingForFinger = false;
    protected float LookingForFingerTimeout = 0.0f;

    protected BlinkBehaviour Blink = null;

    protected AbstractVPFCommunicator Comm = null;
    protected VPF2Communicator VPF2Comm = null;

    #region UNITY DEBUGGING


#if UNITY_EDITOR
    /*************************************************************************
     * This section includes code for easy debugging inside unity. 
     * This includes some minor GUI elements that allow to access functionality
     * that should be triggered by speeches.
     *************************************************************************/
    /// <summary>
    /// Boolean specifying if the debugging features for the editor will be added.
    /// </summary>
    public bool EnableDebug = false;
    /// <summary>
    /// Tests if given the current direction the VH is seeing double.
    /// </summary>
    public RenButton TestDoubleVisionButton;
    /// <summary>
    /// Button that allows the user to fake a "Follow my finger" instruction.
    /// </summary>
    public RenButton FollowFinger;

    /// <summary>
    /// 
    /// </summary>
    public RenButton Left, Right, Up, Down, Straight, Swap;


    void TestDoubleVisionButton_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (seeingDouble())
            AddDebugLine("Sees double");
        
    }


    void FollowFinger_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (LeftEyeControl.IsFollowingObject() || RightEyeControl.IsFollowingObject())
        {
            btn.Label = "Follow Finger";
            stopTrackingFinger();
        }
        else
        {
            btn.Label = "Stop Following Finger";
            startTrackingFinger();
        }
    }

    bool MoveHead = false;

    void Straight_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        ResumeHeadBehaviour();
    }

    void Right_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (MoveHead)
            turnHeadRight();
        else
            lookRight();
    }

    void Left_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (MoveHead)
            turnHeadLeft();
        else
            lookLeft();
    }

    void Down_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (MoveHead)
            turnHeadDown();
        else
            lookDown();
    }

    void Up_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (MoveHead)
            turnHeadUp();
        else
            lookUp();
    }

    void Swap_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        checkFingerShake();

        MoveHead = !MoveHead;
        if (MoveHead)
            Swap.Label = "Swap to Eyes";
        else
            Swap.Label = "Swap to Head";
    }

#endif


    #endregion

    #region MonoBehaviour Methods

    
    protected override void Start()
    {
        base.Start();
        AnimationManager = gameObject.GetComponent<VHAnimationManager>();
        ToolManager = GameObject.Find("ToolManager").GetComponent<RenToolManager>();

        foreach (MouseEyeControl c in this.GetComponentsInChildren<MouseEyeControl>())
        {
            if (c.eyeSide == EyeSide.Left)
                LeftEyeControl = c;
            else if (c.eyeSide == EyeSide.Right)
                RightEyeControl = c;
            else
                AddDebugLine("Unexpected MouseEyeControl with Side: " + c.eyeSide);
        }

        LeftEyeControl.ExtraoccularEvent += ExtraocularEventHandler;

        foreach(EyeMovement m in this.GetComponentsInChildren<EyeMovement>())
        {
            if(m.eyeSide == EyeSide.Left) 
                LeftEye = m;
            else if(m.eyeSide == EyeSide.Right) 
                RightEye = m;
            else
                AddDebugLine("Unexpected EyeMovement with Side: " + m.eyeSide);
        }

        

        if (LeftEyeControl == null)
        {
            AddDebugLine("Couldn't find Controller for Left Eye. Are you sure you included the CranialNerveModel?");
        }
        if (RightEyeControl == null)
        {
            AddDebugLine("Couldn't find Controller for Right Eye. Are you sure you included the CranialNerveModel?");
        }

        Blink = this.GetComponentInChildren<BlinkBehaviour>();
        if (Blink == null)
        {
            AddDebugLine("Couldn't find Blink Behaviour. Are you sure you included the CranialNerveModel?");
        }

        //Set's a reference to the hand
        try
        {
            Hand = (HandTool)ToolManager.GetTool("HandTool");
        }
        catch (System.Exception e)
        {
            Debug.Log("Couldn't find the HandTool\n" +  e.Message);
        }
        if (Hand == null)
        {
            Debug.Log("Why is this NULL??");
        }


        //Set's a reference to the EyeChart
        try
        {
            EyeChart = (EyeChartTool)ToolManager.GetTool("EyeChartTool");
        }
        catch (System.Exception)
        {
            AddDebugLine("Couldn't find the EyeChartTool");
        }

        //Set's a reference to the Ophthalmoscope
        try
        {
            Ophthalmoscope = (OphthalmoscopeTool)ToolManager.GetTool("OphthalmoscopeTool");
            Ophthalmoscope.CranialNerveHandler = this;

            Ophthalmoscope.FundusEvent += Ophthalmoscope_FundusEvent;
        }
        catch (System.Exception)
        {
            AddDebugLine("Couldn't find the Ophthalmoscope");
        }


        try
        {
            IsInSpotlight[] IsInSpotlights = Ophthalmoscope.gameObject.GetComponentsInChildren<IsInSpotlight>();
            foreach (IsInSpotlight i in IsInSpotlights)
            {
                i.SpotlightEvent += i_SpotlightEvent;
            }
        }
        catch (System.Exception)
        {
        }

        


        if (LookTarget == null)
        {
            LookTarget = GameObject.Find("Look Target");
            if (LookTarget == null)
            {
                AddDebugLine("No Look Target found. This should usually be a child of the IPSRen GameObject");
            }
        }

        if (Head == null)
        {
            Head = GameObject.Find("Head");
            if (Head == null)
            {
                AddDebugLine("Couldn't find the head of the Virtual Human, please set manually");
            }
        }


        if (HeadLook == null)
        {
            HeadLook = gameObject.GetComponent<HeadLookBehaviour>();
            if (HeadLook == null)
            {
                AddDebugLine("Couldn't find the Head Behaviour");
            }
        }

        GetCommunicator();


#if UNITY_EDITOR
        if (EnableDebug)
        {
            FollowFinger.ButtonPressed += FollowFinger_ButtonPressed;
            FollowFinger.Label = "Follow Finger";
            AddGUIElement(FollowFinger);

            TestDoubleVisionButton.ButtonPressed += TestDoubleVisionButton_ButtonPressed;
            AddGUIElement(TestDoubleVisionButton);

            Up.ButtonPressed += Up_ButtonPressed;
            Down.ButtonPressed += Down_ButtonPressed;
            Left.ButtonPressed += Left_ButtonPressed;
            Right.ButtonPressed += Right_ButtonPressed;
            Straight.ButtonPressed += Straight_ButtonPressed;

            Swap.ButtonPressed += Swap_ButtonPressed;
            if (MoveHead)
                Swap.Label = "Swap to Eyes";
            else
                Swap.Label = "Swap to Head";

            AddGUIElements(Up, Down, Left, Right, Straight, Swap);
        }
#endif
    }


    protected override void Update()
    {
        base.Update();

        UpdateLookForFinger();
        UpdateWaitForStage();
    }

    
    private void UpdateLookForFinger()
    {
        if (LookingForFinger)
        {
            if (canSeeHand(50))
            {
                LookingForFinger = false;
                LookingForFingerTimeout = 0.0f;
                PlayTaggedAction(CAN_SEE_FINGER_NOW);
            }
            else
            {
                LookingForFingerTimeout += Time.deltaTime;
                if (LookingForFingerTimeout > 100.0f)
                {
                    LookingForFinger = false;
                    LookingForFingerTimeout = 0.0f;
                    PlayTaggedAction(COULDNT_SEE_FINGER);
                }
            }
        }
    }


    #endregion

    protected AbstractVPFCommunicator GetCommunicator()
    {
        if (Comm == null)
        {
            Comm = GetComponent<AbstractVPFCommunicator>();
        }



        return Comm;
    }

    protected VPF2Communicator GetVPF2Communicator()
    {
        if (VPF2Comm == null)
        {
            VPF2Comm = GetComponent<VPF2Communicator>();
        }
        return VPF2Comm;
    }

    #endregion

    #region Not-implemented assume correct:

    protected const string NORMAL_RESULT = "For the purpose of this case assume the result of this test was normal";
    protected void DisplayMessage(string message)
    {
        GetVPF2Communicator();
        VPF2Comm.InstructionsTextBox.text = message;
        VPF2Comm.InstructionsTextBox.ShouldRender = true;
    }

    #endregion


    #region Vision Tests

    protected bool canReadLine()
    {
        return false;

    }

    protected bool seeingDouble()
    {
        if (SeeingDouble && Covered == EyeSide.None)
        {
            Vector3 LeftEyeDirection = LeftEye.gameObject.transform.rotation * Forward;
            Vector3 RightEyeDirection = RightEye.gameObject.transform.rotation * Forward;

            float angle = Vector3.Angle(LeftEyeDirection, RightEyeDirection);

            //could definitely get a better approximation
            return angle > 3;

        }

        return false;
    }

    protected bool canSeeHand(float angle)
    {
        if (Hand != null && Hand.IsActive)
        {
            if (!FollowingFinger)
            {
                Vector3 FromLeftEye = Hand.transform.position - LeftEye.transform.position;
                Vector3 FromRightEye = Hand.transform.position - RightEye.transform.position;

                float AngleLeft = Vector3.Angle(Vector3.forward, FromLeftEye);
                float AngleRight = Vector3.Angle(Vector3.forward, FromRightEye);

                return (AngleLeft < angle && Covered != EyeSide.Left) || (AngleRight < angle && Covered != EyeSide.Right);
            }
            else
                return true;
        }else
            return false;
    }

    #endregion

    #region Cover Eyes

    public EyeSide Covered = EyeSide.None;

    public void coverLeftEye()
    {
        StopHeadBehaviour();

        switch (Covered)
        {
            case EyeSide.None:
                AnimationManager.PlayAnimation("cover_left_eye");
                break;
            case EyeSide.Right:
                AnimationManager.PlayAnimation("swap_cover_right_left");
                break;
            case EyeSide.Left:
                //TODO: figure out if this is the right behavior.
                //Do nothing
                break;
        }

        Covered = EyeSide.Left;
        Blink.CloseEye(Covered);
    }

    public void swapCoveredEye()
    {
        switch (Covered)
        {
            case EyeSide.None:
                //TODO: figure out if this is the right behavior.
                //Do nothing?
                break;
            case EyeSide.Right:
                AnimationManager.PlayAnimation("swap_cover_right_left");
                Covered = EyeSide.Left;
                break;
            case EyeSide.Left:
                AnimationManager.PlayAnimation("swap_cover_left_right");
                Covered = EyeSide.Right;
                break;
        }
        Blink.CloseEye(Covered);
    }

    public void coverRightEye()
    {
        StopHeadBehaviour();

        switch (Covered)
        {
            case EyeSide.None:
                AnimationManager.PlayAnimation("cover_right_eye");
                break;
            case EyeSide.Left:
                AnimationManager.PlayAnimation("swap_cover_left_right");
                break;
            case EyeSide.Right:
                //TODO: figure out if this is the right behavior.
                //Do nothing
                break;
        }

        Covered = EyeSide.Right;
        Blink.CloseEye(Covered);
    }

    public void uncoverEye()
    {
        switch (Covered)
        {
            case EyeSide.Left:
                AnimationManager.PlayAnimation("uncover_left_eye");
                break;
            case EyeSide.Right:
                AnimationManager.PlayAnimation("uncover_right_eye");
                break;
            case EyeSide.None:
                //Do nothing?
                break;
        }
        Covered = EyeSide.None;
        Blink.Resume();
        ResumeHeadBehaviour();
    }

    public EyeSide CoveredEye()
    {
        return Covered;
    }

    #endregion

    #region Eye Chart

    public void readChartLine()
    {
        if (EyeChart != null && EyeChart.IsActive)
        {
            //Shouldn't be able to read line 8
            if (!seeingDouble() && EyeChart.SelectedLine < 8)
            {
                if (EyeChart.SelectedLine != 1)
                    PlayedLineGreaterThan1 = true;

                PlayEyeChartLine(EyeChart.SelectedLine);
            }
            else if (EyeChart.SelectedLine == 1)
            {
                PlayEyeChartLine(1);
            }
            else
            {
                PlayedCantRead = true;
                PlayTaggedAction(CANT_READ_LINE);
            }
        }
        else
        {
            PlayedCantRead = true;
            PlayTaggedAction(CANT_READ_LINE);
        }


        CheckAccuityDiscovery();
    }

    public void readChartLowestLineAble()
    {
        if (seeingDouble())
        {
            PlayEyeChartLine(1);
        }
        else
        {
            PlayedLineGreaterThan1 = true;
            PlayEyeChartLine(7); //7 corresponds to 20/20 vision
        }
        CheckAccuityDiscovery();
    }



    #endregion


    #region Finger Examination

    public void checkFingerCount()
    {
        //ToolManager.HandleMessage("HandTool.FingerCount");
        if (canSeeHand(100))
        {
            if (seeingDouble())
            {
                PlayNumeral(Hand.FingersHeldUp*2);
            }
            else
            {
                PlayNumeral(Hand.FingersHeldUp);
            }
        }
        else
            PlayTaggedAction(CANT_SEE_HAND);



    }

    public void checkFingerShake()
    {
        DisplayMessage(NORMAL_RESULT);

        //TODO: implement
       // throw new System.NotImplementedException();
    }

    public void startLookingForFinger()
    {
        LookingForFinger = true;
        LookingForFingerTimeout = 0.0f;
    }

    bool FollowingFinger = false;

    public void startTrackingFinger()
    {
        if (!Hand.IsActive)
        {
            //This is probably not the best way of forcing activation,
            //but it is a simple way to make sure the Tool Manager deactivates all 
            //other tools and stuff correctly.
            Hand.ToggleActivation();
        }

        if (Hand != null)
        {
            Hand.SetFingers(1); //Put one finger up

            LeftEyeControl.FollowObject(Hand.gameObject);
            RightEyeControl.FollowObject(Hand.gameObject);
            FollowingFinger = true;
        }
        else
        {
            if (Hand == null)
                AddDebugLine("HAND IS NULL!!!");
            else
                AddDebugLine("Hand is not active or is: " + Hand);
        }
    }

    public void stopTrackingFinger()
    {
        LeftEyeControl.StopFollowingObject();
        RightEyeControl.StopFollowingObject();
        FollowingFinger = false;
    }

    #endregion

    #region Control Head Behaviour

    public void StopHeadBehaviour()
    {
        // TODO: implement
        //throw new System.NotImplementedException();
       // AddDebugLine("Covered: " + Covered);
    }

    public void ResumeHeadBehaviour()
    {
        // TODO: implement
        //throw new System.NotImplementedException();
      //  AddDebugLine("Covered: " + Covered);
        HeadLook.SetLookTarget(Camera.main.gameObject);
    }

    #region Turn Head

    public void turnHeadUp()
    {
        HeadLook.SetLookTarget(LookTarget);
        LookTarget.transform.position = Head.transform.position + new Vector3(0, 50, 100);
    }

    public void turnHeadDown()
    {
        HeadLook.SetLookTarget(LookTarget);
        LookTarget.transform.position = Head.transform.position + new Vector3(0, -50, 100);
    }

    public void turnHeadLeft()
    {
        HeadLook.SetLookTarget(LookTarget);
        LookTarget.transform.position = Head.transform.position + new Vector3(-20, 0, 10);
    }

    public void turnHeadRight()
    {
        HeadLook.SetLookTarget(LookTarget);
        LookTarget.transform.position = Head.transform.position + new Vector3(20, 0, 10);
    }

    #endregion

    #region Look position

    public void look_user()
    {
        LeftEyeControl.StopFollowingObject();
        RightEyeControl.StopFollowingObject();

    }

    public void lookStraight()
    {
        LookTarget.transform.position = Head.transform.position + new Vector3(0, 0, 40);
        LeftEyeControl.FollowObject(LookTarget);
        RightEyeControl.FollowObject(LookTarget);
        // TODO: implement
        //throw new System.NotImplementedException();
    }

    public void lookUp()
    {
//        LookTarget.transform.position = Head.transform.position + new Vector3(0, 50, 40);
        LeftEyeControl.SetFixedScreenPosition(0, 20);
//        LeftEyeControl.FollowObject(LookTarget);
        RightEyeControl.SetFixedScreenPosition(0, 20);
//        RightEyeControl.FollowObject(LookTarget);

    }

    public void lookDown()
    {
        //LookTarget.transform.position = Head.transform.position + new Vector3(0, -50, 40);
        //LeftEyeControl.FollowObject(LookTarget);
        //RightEyeControl.FollowObject(LookTarget);
        LeftEyeControl.SetFixedScreenPosition(0, -30);
        RightEyeControl.SetFixedScreenPosition(0, -30);
    }

    public void lookRight()
    {
        //LookTarget.transform.position = Head.transform.position + new Vector3(50, 0, 40);
        //LeftEyeControl.FollowObject(LookTarget);
        //RightEyeControl.FollowObject(LookTarget);
        LeftEyeControl.SetFixedScreenPosition(60, 0);
        RightEyeControl.SetFixedScreenPosition(60, 0);
    }

    public void lookLeft()
    {
        //LookTarget.transform.position = Head.transform.position + new Vector3(-50, 0, 40);
        //LeftEyeControl.FollowObject(LookTarget);
        //RightEyeControl.FollowObject(LookTarget);
        LeftEyeControl.SetFixedScreenPosition(-60, 0);
        RightEyeControl.SetFixedScreenPosition(-60, 0);
    }
    #endregion

    #endregion

    #region Eye Lids

    public void lowerLeftEyelid()
    {
        //TODO: implement
        throw new System.NotImplementedException();
    }

    public void lowerRightEyelid()
    {
        //TODO: implement
        throw new System.NotImplementedException();
    }

    public void raiseLeftEyelid()
    {
        //TODO: implement
        throw new System.NotImplementedException();
    }

    public void raiseRightEyelid()
    {
        //TODO: implement
        throw new System.NotImplementedException();
    }

    #endregion

    #region Touch

    bool TriggeredTouch = false;
    public void startCheckTouchingHead()
    {
        DisplayMessage("For purpose of this case assume the virtual patient has normal sensation on all six regions");
        if (!TriggeredTouch)
        {
            StartCoroutine(TriggerTouch());
        }
    }

    public IEnumerator TriggerTouch()
    {
        yield return new WaitForSeconds(5);
        PlayTaggedAction(DISCOVERY_FACIAL_SENSATION);
    }

    #endregion

    #region Facial Expressions 

     #region Winking

    public void winkLeft()
    {
        Blink.Wink(EyeSide.Left);
    }

    public void winkRight()
    {
        Blink.Wink(EyeSide.Right);
    }

    public void closeEyes()
    {
        Blink.CloseBothEyes();
    }

    #endregion

    public void wrinkleForhead()
    {
        DisplayMessage(NORMAL_RESULT);
    }


    public void frown()
    {
        DisplayMessage(NORMAL_RESULT);
    }

    public void puffCheeks()
    {
        DisplayMessage(NORMAL_RESULT);
    }

    public void raiseEyebrows()
    {
        DisplayMessage(NORMAL_RESULT);
    }

    public void smile()
    {
        DisplayMessage(NORMAL_RESULT);
    }
    public void stickOutTongue()
    {
        DisplayMessage(NORMAL_RESULT);

    }
    #endregion


    #region Tutorial
    private bool WaitingForFundus = false;
    public void waitForFundus()
    {
        //AddDebugLine("Waiting for fundus");
        OphthalmoscopeTool ophthalmoscope = (OphthalmoscopeTool)ToolManager.GetTool("OphthalmoscopeTool");
        WaitingForFundus = true;
        ophthalmoscope.FundusEvent += FundusEvent;
        GetCommunicator();
    }

    private void FundusEvent(OphthalmoscopeTool tool, FundusEventArgs args)
    {
        //AddDebugLine("CNAppAction: FundusEvent " + args);
        if (args.Showing && WaitingForFundus)
        {
            GetCommunicator();
            if (Comm is VPF2Communicator)
            {
                VPF2Communicator v2 = (VPF2Communicator)Comm;
                v2.CanMoveOnWithTaggedActions();
                WaitingForFundus = false;
            }
            //else
            //{
            //    AddDebugLine("Comm wasn't VPF2 :  " + Comm);
          //  }
        }
        //else
           // AddDebugLine("Wasn't waiting for fundus or event wasn't showing...");
    }


    private bool WaitingForStage = false;
    public void waitForStage()
    {
        WaitingForStage = true;
    }

    public void UpdateWaitForStage()
    {
        if (WaitingForStage)
        {
            
            GetCommunicator();
            if (Comm is VPF2Communicator)
            {
                VPF2Communicator v2 = (VPF2Communicator)Comm;
                if (v2.StagesConcluded() > 0)
                {
                    v2.CanMoveOnWithTaggedActions();
                    WaitingForStage = false;
                }
            }
        }
    }
    

    #endregion

    #region Discoveries 

    bool CheckedLeftPupil = false, CheckedRightPupil = false, TriggeredPupillary = false;
    protected void i_SpotlightEvent(IsInSpotlight spotlight, IsInSpotlightArgs args)
    {
        if (args.In)
        {
            switch (spotlight.Side)
            {
                case EyeSide.Left:
                    CheckedLeftPupil = true;
                    break;
                case EyeSide.Right:
                    CheckedRightPupil = true;
                    break;
                default:
                    break;
            }

            if (CheckedLeftPupil && CheckedRightPupil && !TriggeredPupillary)
            {
                TriggeredPupillary = true;
                PlayTaggedAction(DISCOVERY_PUPILLARY_RESPONSE);
            }

        }
    }

    bool CheckedLeftFundus = false, CheckedRightFundus = false, TriggeredFundus = false;
    void Ophthalmoscope_FundusEvent(OphthalmoscopeTool tool, FundusEventArgs args)
    {
        if (args.Showing)
        {
            switch (args.Side)
            {
                case EyeSide.Right:
                    CheckedRightFundus = true;
                    break;
                case EyeSide.Left:
                    CheckedLeftFundus = true;
                    break;

                default:
                    break;
            }

            if (CheckedLeftFundus && CheckedRightFundus && !TriggeredFundus)
            {
                TriggeredFundus = true;
                Debug.Log("Fundus");
                PlayTaggedAction(DISCOVERY_FUNDOSCOPIC_EXAM);
            }

        }
    }

    bool PlayedCantRead = false;
    bool PlayedLineGreaterThan1 = false;
    bool TriggeredVisualAccuity = false;
    void CheckAccuityDiscovery()
    {
        if (PlayedCantRead && PlayedLineGreaterThan1 && !TriggeredVisualAccuity)
        {
            TriggeredVisualAccuity = true;
            PlayTaggedAction(DISCOVERY_VISUAL_ACUITY);
        }
    }

    bool TriggeredExtraocular = false;
    void ExtraocularEventHandler(MouseEyeControl ctrl, ExtraocularEventArgs args)
    {
        if (!TriggeredExtraocular)
        {
            TriggeredExtraocular = true;
            PlayTaggedAction(DISCOVERY_EXTRAOCULAR_MOVEMENTS);
        }
    }

    

    #endregion


    #region Tags

    protected void PlayTaggedAction(string s)
    {
        AbstractVPFCommunicator c = GetCommunicator();
        if (c != null)
        {
            c.PlayTaggedAction(s);
        }
        else
        {
            AddDebugLine("Trying to play Tagged Action with name: \"" + s + "\" but couldn't find VPF Communicator");
        }
    }

    protected void PlayNumeral(int i)
    {
        PlayTaggedAction(NUMERALS[i]);
    }

    protected void PlayEyeChartLine(int i)
    {
        PlayTaggedAction(LINES[i]);
    }


    #region Constants

    protected string[] NUMERALS = 
    { "Numeral 0", //0
        "Numeral 1", //1
        "Numeral 2", //2
        "Numeral 3", //3
        "Numeral 4", //4
        "Numeral 5", //5
        "Numeral 6", //6
        "Numeral 7", //7 (this shouldn't happen, have it for completeness)
        "Numeral 8", //8
        "Numeral 9", //9 (this shouldn't happen, have it for completeness)
        "Numeral 10" //10
    };
    protected const string CANT_SEE_HAND = "Cannot see hand";

    protected const string CAN_SEE_FINGER_NOW = "Can see finger now";
    protected const string COULDNT_SEE_FINGER = "Could not see finger";

    protected string[] LINES = 
    {
        "Eyechart line 0", //0 doesn't exist, but for convenience we add it anyway
        "Eyechart line 1", // 1
        "Eyechart line 2", // 2
        "Eyechart line 3", // 3
        "Eyechart line 4", // 4
        "Eyechart line 5", // 5
        "Eyechart line 6", // 6
        "Eyechart line 7", // 7
        "Eyechart line 8", // 8
    };
    protected const string CANT_READ_LINE = "Eyechart cannot read";
    protected const string CANT_READ_ANY_LINE = "Eyechart cannot read any";

    protected const string DISCOVERY_PUPILLARY_RESPONSE = "Discovery: Pupillary response exam";
    protected const string DISCOVERY_FUNDOSCOPIC_EXAM = "Discovery: Fundoscopic exam";
    protected const string DISCOVERY_VISUAL_ACUITY = "Discovery: Visual Acuity test";
    protected const string DISCOVERY_FACIAL_SENSATION = "Discovery: Facial sensation";
    protected const string DISCOVERY_EXTRAOCULAR_MOVEMENTS = "Discovery: Extraocular movements";
    protected const string DISCOVERY_VISUAL_FIELDS = "Discovery: Visual fields";

    #endregion

    #endregion
}
