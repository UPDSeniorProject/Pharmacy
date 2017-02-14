using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class RenButton : RenGUIElement {    
    /// <summary>
    /// Text Label for the button. Set to <c>null</c> or empty string if you want to use an image.
    /// </summary>
    public string Label = null;

    /// <summary>
    /// Image for the button. Set to <c>null</c> if you just want a label.
    /// </summary>
    public Texture Texture = null;

    /// <summary>
    /// Tool tip text to be displayed. Set to <c>null</c> or empty string to disable tool tip.
    /// </summary>
    public string ToolTip = null;

    /// <summary>
    /// Position of the tool tip.
    /// </summary>
    public Rect ToolTipPosition;


    public Color ToolTipColor;

    /// <summary>
    /// Event triggered when the button is pressed
    /// </summary>
    public event ButtonPressedEventHandler ButtonPressed;


    public void ClearButtonPressedEventHandlers()
    {
        foreach (Delegate d in ButtonPressed.GetInvocationList())
        {
            ButtonPressed -= (ButtonPressedEventHandler) d;
        }
    }

	public int CountButtonPressedEventHandlers() {

		return ButtonPressed.GetInvocationList().Length;

	}

    public RenButton() : base() 
    {
    }

    public RenButton(string label) : base () 
    {
        this.Label = label;
    }

    override protected bool Show()
    {
        bool error = false;
        //Stroes if button was pressed this frame
        bool pressed = false;

        if(Label != null && Label != "") {
            pressed = GUI.Button(_ActualPosition, Label);
        }else if(Texture != null) {
            pressed = GUI.Button(_ActualPosition, Texture);
        }else {
            OnErrorOcurred("RenButton must have label or texture to render");
            error = true;
        }

        

        if (pressed)
        {
            OnButtonPressed(new ButtonPressedEventArgs());
        }

        return error;
    }

    protected virtual void OnButtonPressed(ButtonPressedEventArgs args)
    {
        if (ButtonPressed != null)
        {
            ButtonPressed(this, args);
        }
        else
        {
            Debug.Log("No handler for this button");
        }
    }

    public override string ToString()
    {
        return "RenButton";
    }
}

public enum MouseButton
{
    MOUSE_LEFT = 0,
    MOUSE_RIGHT = 1,
    MOUSE_MIDDLE = 2,
}

/// <summary>
/// Arguments of a button pressed event
/// </summary>
public class ButtonPressedEventArgs : EventArgs
{
    public MouseButton button;

    public ButtonPressedEventArgs()
    {
        if (Input.GetMouseButton((int)MouseButton.MOUSE_LEFT))
        {
            button = MouseButton.MOUSE_LEFT;
        }
        else if (Input.GetMouseButton((int)MouseButton.MOUSE_RIGHT))
        {
            button = MouseButton.MOUSE_RIGHT;
        }
        else if (Input.GetMouseButton((int)MouseButton.MOUSE_MIDDLE))
        {
            button = MouseButton.MOUSE_MIDDLE;
        }
    }

    public override string ToString()
    {
        return "RenButton";
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="btn"></param>
/// <param name="args"></param>
public delegate void ButtonPressedEventHandler(RenButton btn, ButtonPressedEventArgs args);
