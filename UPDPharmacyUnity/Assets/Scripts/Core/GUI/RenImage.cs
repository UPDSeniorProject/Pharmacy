using UnityEngine;
using System.Collections;

[System.Serializable]
public class Size
{
    public int Width;
    public int Height;

    public Size(int width, int height)
    {
        this.Width = width;
        this.Height = height;
    }
}

[System.Serializable]
public class RenImage : RenGUIElement {
    /// <summary>
    /// Texture to display. If <c>null</c> RenImage will not render.
    /// </summary>
    public Texture2D Image = null;

    /// <summary>
    /// Scale mode for the image
    /// </summary>
    public ScaleMode ScaleMode = ScaleMode.StretchToFill;

    /// <summary>
    /// If set to true, the image is drawn inside a box with border (the border is set by the
    /// <c>style</c>), while if false, the image will have no border.
    /// </summary>
    public bool DrawBox = false;

    /// <summary>
    /// Color to tint the image.
    /// </summary>
    public Color tint = Color.white;

	/// <summary>
    /// 
    /// </summary>
    override protected bool Show()
    {
        if (Image == null)
        {
            OnErrorOcurred("No Image set for RenImage");
            return true;
        }
        Color backup = GUI.color;

        GUI.color = tint;

        if (DrawBox)
            GUI.Box(_ActualPosition, Image);
        else
            GUI.DrawTexture(_ActualPosition, Image, ScaleMode, true, 0);

        GUI.color = backup;

        return false;
    }

    public Size GetSize()
    {
        return new Size(Image.width, Image.height);
    }

    public void ScaleToSize(Size s)
    {
        Position.height = s.Height;
        Position.width = s.Width;

    }

    public override string ToString()
    {
        return "RenImage";
    }
}
