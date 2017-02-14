using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandTool : CranialNerveTool {

    public int FingersHeldUp;


    public bool UseFingerGUI = false;
    public Rect FingerGUIInitialPosition;
    public GUISkin FingerGUISkin = null;

    protected Dictionary<RenGUIElement, int> FingerGUI = new Dictionary<RenGUIElement,int>();


    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        FingersHeldUp = 0;

        RenLabel FingerLabel = new RenLabel();
        FingerLabel.LabelText = "Select number of fingers to hold up";
        FingerLabel.ShouldRender = false;
        FingerLabel.Position = new Rect(FingerGUIInitialPosition.xMin, FingerGUIInitialPosition.yMin - 30,
            (FingerGUIInitialPosition.width + 5) * 6 - 5, FingerGUIInitialPosition.height);

        FingerLabel.tint = Color.yellow;
        AddGUIElement(FingerLabel);
        FingerGUI[FingerLabel] = 0;


        for (int i = 0; i < 6; i++)
        {
            RenButton btn = new RenButton();
            btn.Label = "" + i;

            btn.Position = new Rect(FingerGUIInitialPosition.xMin + (FingerGUIInitialPosition.width + 5) * i,
                FingerGUIInitialPosition.yMin,
                FingerGUIInitialPosition.width,
                FingerGUIInitialPosition.height);

            

            btn.ButtonPressed += FingerCountButtonPressed;
            AddGUIElement(btn);
            FingerGUI[btn] = i;
            btn.ShouldRender = false;
        }

        if (FingerGUISkin != null)
        {
            foreach (RenGUIElement e in FingerGUI.Keys)
            {
                e.Skin = FingerGUISkin;
            }
        }

        HelpBox.text = "- Use your mouse to drag the hand tool across the screen.\n" +
                       "- You can ask the virtual patient to follow your finger.\n" +
                       "- You can also ask the virtual human to count the fingers you are holding up.";

        Visible = false;
    }

    void FingerCountButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (args.button == MouseButton.MOUSE_LEFT)
        {
            SetFingers(FingerGUI[btn]);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    public override void CheckInput()
    {
        KeyCode key = KeyCode.Alpha0;
        KeyCode numpad = KeyCode.Keypad0;

        for (; key < KeyCode.Alpha6; key++, numpad++)
        {
            if (Input.GetKeyDown(key) || Input.GetKeyDown(numpad))
            {
                SetFingers(key - KeyCode.Alpha0);
            }
        }
    }

    public override void ActivateTool()
    {
        base.ActivateTool();
        Visible = true;
        GetComponent<Animation>().Play("default");
        if (UseFingerGUI)
        {
            foreach (RenGUIElement e in FingerGUI.Keys)
                e.ShouldRender = true;
        }
    }

    public override void DeactivateTool()
    {
        Visible = false;
        if (UseFingerGUI)
        {
            foreach (RenGUIElement e in FingerGUI.Keys)
                e.ShouldRender = false;
        }

        StopFollow();
    }

    protected MouseEyeControl RightEyeControl = null, LeftEyeControl = null;
    protected void FindEyeControls()
    {
        if (RightEyeControl == null || LeftEyeControl == null)
        {
            GameObject Model = GameObject.Find("CranialNerveModel");
            foreach (MouseEyeControl ctrl in Model.GetComponents<MouseEyeControl>())
            {
                if (ctrl.eyeSide == EyeSide.Right)
                    RightEyeControl = ctrl;
                else if (ctrl.eyeSide == EyeSide.Left)
                    LeftEyeControl = ctrl;
                else
                    AddDebugLine("Eye Control with unexpected side: " + ctrl);
            }
        }
    }

    protected void StopFollow()
    {
        FindEyeControls();

        RightEyeControl.StopFollowingObject(this.gameObject);
        LeftEyeControl.StopFollowingObject(this.gameObject);
    }

    public void SetFingers(int fingers)
    {
        if (FingersHeldUp != fingers)
        {
            FingersHeldUp = fingers;
            string animationName = "" + FingersHeldUp;

            if (FingersHeldUp > 0)
            {
                animationName = (FingersHeldUp - 1) + animationName;
            }
            else
                animationName = "10";

            GetComponent<Animation>().Play(animationName);
        }
    }

    public override bool RequiresEyeCamera()
    {
        return true;
    }


    public override string Name
    {
        get
        {
            return "HandTool";
        }
    }

    public override string ToString()
    {
        return "HandTool";
    }
}


