using UnityEngine;
using System.Collections;

[System.Serializable]
public class RenLabel : RenGUIElement {

    public string LabelText = "";

    public bool DrawBox = false;

    public Color tint = Color.white;

    protected override bool Show()
    {
        Color backup = GUI.color;
        GUI.color = tint;

        if (DrawBox)
        {
            GUI.Box(_ActualPosition, LabelText);
        }
        else
        {
            GUI.Label(_ActualPosition, LabelText);
        }


        GUI.color = backup;

        return false;
    }

    public override string ToString()
    {
        return "RenLabel";
    }
}
