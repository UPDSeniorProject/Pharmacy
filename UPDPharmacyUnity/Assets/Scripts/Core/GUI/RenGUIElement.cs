using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class RenGUIElement {

    /// <summary>
    /// Saves the last element to trigger a OnFocusEvent.
    /// </summary>
    private static RenGUIElement LastFocused = null;

    /// <summary>
    /// Defines if this GUI Element should be rendered.
    /// </summary>
    public bool ShouldRender = true;
    /// <summary>
    /// Defines if this GUI Element is enabled.
    /// </summary>
    public bool IsEnabled = true;

    /// <summary>
    /// Position of the element. 
    /// </summary>
    public Rect Position = new Rect(0, 0, 50, 50);

    #region Resizable GUI 
    /// <summary>
    /// Stores the Last position we converted to an actual position. 
    /// </summary>
    public Rect _LastPositionConverted = new Rect(-1,-1,-1,-1);

    /// <summary>
    /// Position to be used for display. 
    /// </summary>
    public Rect _ActualPosition;

    #region RenManager Reference

    /// <summary>
    /// Reference to the RenManager, only used for converting positions.
    /// </summary>
    protected RenManager _Manager;

    #endregion

    #endregion
    
    /// <summary>
    /// 
    /// </summary>
    public GUISkin Skin = null;
    /// <summary>
    /// 
    /// </summary>
    protected GUISkin PreviousSkin = null;

    /// <summary>
    /// Saves 
    /// </summary>
    protected bool ErrorOnLastDisplay = false;

    /// <summary>
    /// Name to use for this GUIElement. This is REQUIRED for FocusEvent to work.
    /// </summary>
    public string Name = "";

    /// <summary>
    /// Event triggered when the Element cannot be displayed for some error.
    /// </summary>
    public event GUIErrorEventHandler ErrorOcurred;
    /// <summary>
    /// Event triggered when the Element gains focus. The element must have a <i>unique</i> Name for this to work.
    /// </summary>
    public event GUIFocusEventHandler FocusEvent;


    public RenGUIElement()
    {
    }

    public RenGUIElement(RenGUIElementDefaultConfig config)
    {
        Position = config.Position;
        if(config.Skin != null)
            Skin = config.Skin;

        if (config.Name != null && config.Name != "")
            Name = config.Name;
    }

    public void Display() 
    {
        if (_Manager == null)
        {
            GameObject IPS = GameObject.Find("IPSRen");
            _Manager = IPS.GetComponent<RenManager>();
        }

        if (ShouldRender)
        {
            GUI.enabled = IsEnabled;



            if (Skin != null)
            {
                PreviousSkin = GUI.skin;
                GUI.skin = Skin;
            }

            if (Name != "" && Name != null)
                GUI.SetNextControlName(Name);

            if (_LastPositionConverted != Position || _Manager.ViewportSizeUpdated || _Manager.ShouldForceGUIUpdate)
            {
                UpdatePosition();
            }

            ErrorOnLastDisplay = Show();

            //Check for focus!
            if (Name != "" && GUI.GetNameOfFocusedControl() == Name)
            {
                OnFocus(new RenGUIFocusEventArgs());
            }

            if (Skin != null)
            {
                GUI.skin = PreviousSkin;
            }

            GUI.enabled = true;
        }
        else if (_Manager.ShouldForceGUIUpdate)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        _LastPositionConverted = Position;

        _ActualPosition = _Manager.GetPosition(_LastPositionConverted);

    }

    protected void OnErrorOcurred(string error)
    {
        OnErrorOcurred(new RenGUIErrorEventArgs(error));
    }

    protected void OnErrorOcurred(RenGUIErrorEventArgs args)
    {
        if (!ErrorOnLastDisplay)
        {
            if (ErrorOcurred != null)
            {
                ErrorOcurred(this, args);
            }
            else
            {
                Debug.Log("Error occurred on GUI with message: " + args.errorMsg);
            }
        }
    }

    protected void OnFocus(RenGUIFocusEventArgs args)
    {
        //We only trigger if this is the first frame of focus.
        //LastFocus is static.
        if (LastFocused != this) 
        {
            if (FocusEvent != null)
            {
                FocusEvent(this, args);
            }
            else
            {
                //Debug.Log("No handler for Focus Event");
            }
            
            //No matter if it doesn't have a handler.
            LastFocused = this;
        }
    }

    public override string ToString()
    {
        return "RenGUIElement";
    }

    protected abstract bool Show();
}

[System.Serializable]
public class RenGUIElementDefaultConfig
{
    public Rect Position;
    public GUISkin Skin = null;
    public string Name = "";
}

public class RenGUIErrorEventArgs : System.EventArgs
{
    public string errorMsg;

    public RenGUIErrorEventArgs(string error)
    {
        errorMsg = error;
    }
}

public class RenGUIFocusEventArgs : System.EventArgs
{
}


public delegate void GUIErrorEventHandler(RenGUIElement element, RenGUIErrorEventArgs args);
public delegate void GUIFocusEventHandler(RenGUIElement element, RenGUIFocusEventArgs args);
