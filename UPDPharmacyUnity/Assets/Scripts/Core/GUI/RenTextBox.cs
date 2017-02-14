using UnityEngine;
using System.Collections;

[System.Serializable]
public class RenTextBox : RenGUIElement {
    /// <summary>
    /// Text to display
    /// </summary>
    public string text = "";
    /// <summary>
    /// Determines if this is a TextBox that can be edited
    /// </summary>
    public bool canEdit = false;
    /// <summary>
    /// Determines if the TextBox can have multiple lines, this is only
    /// affected if the textbox can be editted.
    /// </summary>
    public bool isMultiLine = false;

    /// <summary>
    /// Determines if the box will have a scroll vertical.
    /// </summary>
    public bool useScroll = false;

    /// <summary>
    /// 
    /// </summary>
    public Vector2 scrollPosition = new Vector2(0,0);

    /// <summary>
    /// Use tint color.
    /// </summary>
    public Color TintColor = Color.white;

    /// <summary>
    /// 
    /// </summary>
    public bool UseTint = false;

    /// <summary>
    /// 
    /// </summary>
    public Rect scrollRect = new Rect(0, 0, 300, 300);
	


    override protected bool Show()
    {	
		Color backupColor = Color.white ;
        if (UseTint)
        {
            backupColor = GUI.color;
            GUI.color = TintColor;
        }

        if (useScroll)
        {
            Rect ConvertedScrollRect = _Manager.GetPosition(scrollRect);
            scrollPosition = GUI.BeginScrollView(ConvertedScrollRect, scrollPosition, _ActualPosition);
        }

        if (canEdit)
        {
            if (isMultiLine)
                text = GUI.TextArea(_ActualPosition, text);
            else
                text = GUI.TextField(_ActualPosition, text);
        }
        else
        {
            GUI.Box(_ActualPosition, text);
        }

        if (useScroll)
        {
            GUI.EndScrollView();
        }

        if (UseTint)
        {
            GUI.color = backupColor;
        }

        return false;
    }

    public override string ToString()
    {
        return "RenTextBox";
    }
}
