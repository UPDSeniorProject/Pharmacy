using UnityEngine;
using System.Collections;

public class RenWindow : RenGUIElement{

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
    public string Title = "";

    /// <summary>
    /// Text to display
    /// </summary>
    public string Text = "";

    /// <summary>
    /// ID for display of the window.
    /// </summary>
    public int Id = 1;


    override protected bool Show()
    {
        Color backupColor = Color.white;
        if (UseTint)
        {
            backupColor = GUI.color;
            GUI.color = TintColor;
        }

        _ActualPosition = GUI.Window(Id, _ActualPosition, RenderWindow, Title);
        Position = _Manager.InvertPosition(_ActualPosition);

		if(UseTint)
		{
			GUI.color = backupColor;
		}

        return false;
    }

    protected void RenderWindow(int Id)
    {
        GUI.backgroundColor = Color.clear;
        GUILayout.TextArea(Text);
    }


}
