using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Generic class for implementing tools used for a physical examination.
/// This generic class includes a button that toggles the tool's activation.
/// </summary>
public abstract class PhysicalExaminationTool : RenBehaviour {
    
    /// <summary>
    /// This button will be used to activate the tool.
    /// </summary>
    public RenButton button = new RenButton();

    /// <summary>
    /// 
    /// </summary>
    public RenTextBox HelpBox = new RenTextBox();

    /// <summary>
    /// Event triggered when this tool is activated
    /// </summary>
    public event ToolActivatedEventHandler ToolActivated;

    /// <summary>
    /// Event triggered when this tool is deactivated
    /// </summary>
    public event ToolDeactivatedEventHandler ToolDeactivated;

    /// <summary>
    /// Protected boolean for the current activation state of this tool.
    /// </summary>
    protected bool _isActive = false;

    /// <summary>
    /// Public accessor to _isActive. Should not be able to set!
    /// </summary>
    public bool IsActive { get { return _isActive; } }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        AddGUIElements(button, HelpBox);
        HelpBox.ShouldRender = false;

        button.ButtonPressed += new ButtonPressedEventHandler(this.OnButtonPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Update()
    {
        base.Update();
		
		if(_isActive) 
		{	//Call convinience method for checking keyboard input.
			CheckInput();	
		}
    }

    /// <summary>
    /// Function called when each tool is Activated. Each tool should override this function and handle it's activation.
    /// </summary>
    public abstract void ActivateTool();
    /// <summary>
    /// Function called when each tool is Deactivated. Each tool should override this function and handle it's deactivation.
    /// </summary>
    public abstract void DeactivateTool();
    /// <summary>
    /// This function is called during the Update of the Tool, the function should be implemented by each tool. The purpose of this
    /// function is to check the Input status and update the status of the tool as needed.
    /// </summary>
	public abstract void CheckInput();

    /// <summary>
    /// Name of the Physical ExaminationTool
    /// </summary>
    public abstract string Name { get; }

    protected void UpdateHelpBox()
    {
        if (_isActive && HelpBox.text != "")
        {
            HelpBox.ShouldRender = true;
        }
        else
        {
            HelpBox.ShouldRender = false;
        }
    }

    /// <summary>
    /// Toggles the activation of the tool. This function is called when the button is pressed.
    /// </summary>
    public void ToggleActivation()
    {
        if (_isActive)
        {
            DeactivateTool();
            OnToolDeactivated(new ToolEventArgs());            
        }
        else
        {
            ActivateTool();
            OnToolActivated(new ToolEventArgs());
        }

        _isActive = !_isActive;
        UpdateHelpBox();
    }

    public void ManagerToolDeactivation(RenToolManager manager)
    {
        if (manager != null)
        {
            DeactivateTool();
            _isActive = false;
        }
        else
        {
            AddDebugLine("Method ManagerToolDeactivation should only be called from manager");
        }
        UpdateHelpBox();
    }

    public void ManagerToolActivation(RenToolManager manager)
    {
        if (manager != null)
        {
            ActivateTool();
            _isActive = true;
        }
        else
        {
            AddDebugLine("Method ManagerToolDeactivation should only be called from manager");
        }
        UpdateHelpBox();
    }


    void OnButtonPressed(RenButton sender, ButtonPressedEventArgs args)
    {

        if (args.button == MouseButton.MOUSE_LEFT)
        {
            ToggleActivation();
        }
    }

    protected virtual void OnToolActivated(ToolEventArgs args)
    {
        if (ToolActivated != null)
        {
            ToolActivated(this, args);
        }
        else
        {
            //Debug.Log("No handler for the activation of " + this.ToString());
        }
    }

    protected virtual void OnToolDeactivated(ToolEventArgs args)
    {
        if (ToolDeactivated != null)
        {
            ToolDeactivated(this, args);
        }
        else
        {
            //Debug.Log("No handler for the deactivation of " + this.ToString());
        }
    }

    /// <summary>
    /// Override the ToString method for debugging purposes.
    /// </summary>
    /// <returns> A string representation of this tool </returns>
    public override String ToString()
    {
        return "PhysicalExaminationTool";
    }

}

public class ToolEventArgs : EventArgs
{
}

public delegate void ToolActivatedEventHandler(PhysicalExaminationTool btn, ToolEventArgs args);
public delegate void ToolDeactivatedEventHandler(PhysicalExaminationTool btn, ToolEventArgs args);