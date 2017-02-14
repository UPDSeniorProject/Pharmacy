using UnityEngine;
using System.Collections;

public class CranialNerveToolManager : RenToolManager {

    public Camera EyeCamera = null;
    #region HelpButton

    public RenButton HelpButton = new RenButton();
    protected string HelpText = "You can ask the patient to:\n" +
                                "- Cover / uncover left, right eye\n" +
                                "- Tilt head: left, right, up, down\n" +
                                "- Look: left, right, up, down\n" +
                                "- Look straight ahead\n" +
                                "- Follow finger\n" +
                                "- Report number of fingers held up\n" +
                                "- Report sensation in their face\n" +
                                "- Report when finger is seen\n" +
                                "- Read eye chart line\n" +
                                "- Wink left, right, or both eyes\n" +
                                "- Smile, puff checks or stick out tongue\n" +
                                "- Raise eyebrows or frown";
    public RenTextBox HelpTextBox = new RenTextBox();


    #endregion


    protected override void Start()
    {
        base.Start();

        if (EyeCamera == null)
        {
            EyeCamera = GameObject.Find("Eye Camera").GetComponent<Camera>();
            if (EyeCamera == null)
            {
                throw new System.Exception("Couldn't find Eye Camera for Virtual Human");
            }
            else
                EyeCamera.enabled = false;
        }

        #region GUI Elements
        HelpButton.ButtonPressed +=
            HelpButton_ButtonPressed;
        AddGUIElement(HelpButton);


        HelpTextBox.Skin = Resources.Load("VPF2/VPF2Skin", typeof(GUISkin)) as GUISkin;
        HelpTextBox.ShouldRender = false;
        HelpTextBox.useScroll = true;
        HelpTextBox.scrollRect = new Rect(5, 5, 260, 135);
        HelpTextBox.Position = new Rect(0, 0, 240, 270);
        HelpTextBox.text = HelpText;
        AddGUIElement(HelpTextBox);
        #endregion
    }

    void HelpButton_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if (HelpTextBox.ShouldRender)
        {           
            HelpTextBox.ShouldRender = false;
        }
        else
        {
            HelpTextBox.ShouldRender = true;
            StartCoroutine(HideHelpAfterSeconds(10));
        }

    }

    private IEnumerator HideHelpAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HelpTextBox.ShouldRender = false;
    }

    protected override void ToolActivated(PhysicalExaminationTool tool, ToolEventArgs args)
    {
        base.ToolActivated(tool, args);
        CranialNerveTool cnTool = (CranialNerveTool)tool;
        EyeCamera.enabled = cnTool.RequiresEyeCamera();
    }

    protected override void ToolDeactivated(PhysicalExaminationTool tool, ToolEventArgs args)
    {
        base.ToolDeactivated(tool, args);
        EyeCamera.enabled = false;
    }

}
