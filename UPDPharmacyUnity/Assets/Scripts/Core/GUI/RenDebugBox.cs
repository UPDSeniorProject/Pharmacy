using UnityEngine;
using System.Collections;

[System.Serializable]
public class RenDebugBox : RenTextBox {

    private int lines = 0;

    public int LineLength = 60;
    

	/// <summary>
	/// This is a very specific control.
    /// The constructor set the default values
    /// for it to work correctly.
	/// </summary>
	public RenDebugBox () : base() {
        isMultiLine = true;
        canEdit = false;
        Position = new Rect(0, 0, 690, 190);
        ShouldRender = false;

        useScroll = true;
        scrollRect = new Rect(10, 20, 700, 200);

	}

    public void AddLine(string line)
    {
        text += line + "\n";
        int linesUsed = (line.Length / LineLength) + 1;
        lines += linesUsed;

        Position.height = Skin.box.CalcSize(new GUIContent(text)).y;
        scrollPosition.y = Position.height - scrollRect.height;

        ShouldRender = true;
    }

    public void Clear()
    {
        lines = 0;
        ShouldRender = false;
        text = "";
    }

    public override string ToString()
    {
        return "RenDebugBox";
    }
}
