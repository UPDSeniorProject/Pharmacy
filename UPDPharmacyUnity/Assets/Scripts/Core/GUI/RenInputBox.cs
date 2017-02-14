using UnityEngine;
using System.Collections;

[System.Serializable]
public class RenInputBox : RenTextBox {

    public RenInputBox(bool multiline = false, string name = "RenInputBox")
        : base()
    {
        canEdit = true;
        isMultiLine = multiline;
        Name = name;

    }

    public event RenInputBoxEnterEvent EnterPressed;

    protected override bool Show()
    {
        if (!base.Show())
        {
            //Check for focus.
            if (GUI.GetNameOfFocusedControl() == Name)
            {
                //Check last event.
                if (Event.current.type == EventType.KeyUp)
                {
                    //Check if it's an enter!
                    if (Event.current.keyCode == KeyCode.Return)
                    {
                        OnEnterPressed(new RenInputBoxEventArgs());
                    }
                }
            }
            return false;
        }
        else
            return true;
    }

    private void OnEnterPressed(RenInputBoxEventArgs args)
    {
        if (EnterPressed != null)
        {
            EnterPressed(this, args);
        }
        else
        {
            //not really an error. You could uncomment this line if you needed
            //to debug more
            //Debug.Log("No handler for EnterPressed!!"); 
        }

    }
	


}

public class RenInputBoxEventArgs : System.EventArgs
{

}

public delegate void RenInputBoxEnterEvent(RenInputBox box, RenInputBoxEventArgs args);