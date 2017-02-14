using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class RenSpeechOptions : RenGUIElement {

    /// <summary>
    /// 
    /// </summary>
    public event SpeechOptionSelected OptionSelectedEvent;

    /// <summary>
    /// Defines if the option to specify the statement was meant as a statement
    /// will be displayed. Sometimes you might not want to give that option. Just
    /// set this bool to false.
    /// </summary>
    public bool UseStatementSpeechOption = true;
    /// <summary>
    /// This will determine if the user's choice is not among the ones in the speech options.
    /// If selected the option should submit the user's input as a suggestion.
    /// </summary>
    public bool UseNotAboveSpeechOption = true;

    /// <summary>
    /// In general all <c>RenGUIElement</c> objects specify position as a rectangle with a top
    /// left corner. However, RenSpeechOptions is a list populated with a variable number of options
    /// Most of the time you'll get 5, 3 options plus statement and submit suggestion. However, you
    /// might get less options. Use this set to true to specify the position as lower left corner, so that the
    /// list is anchored at the bottom and populated upward.
    /// <seealso cref="RenSpeechOptions.ComputedPosition"/>
    /// </summary>
    public bool PositionSpecifiedAsLowerLeftCorner = true;

    /// <summary>
    /// The width and height of this Rect are used as the width and height of each one of the specific options.
    /// <seealso cref="RenSpeechOptions.ComputedPosition"/>
    /// </summary>
    public Rect OptionSize = new Rect(0, 0, 600, 50);

    /// <summary>
    /// This Rect has the actual position at which the options themselves will be displayed. 
    /// This is computed as follows:
    ///   * X,Y: Position(x,y) if PositionSpecifiedAsLowerLeftCorner is set to false
    ///       or Position(x,y-ComputedPosition.Height) if PositionSpecifiedAsLowerLeftCorner is set to true.
    ///   * Width: OptionSize.width.
    ///   * Height: OptionSize * Number of options 
    /// </summary>
    protected Rect ComputedPosition = new Rect(0, 0, 0, 0);

    /// <summary>
    /// This Rect has the Position of the label that prompts the user to select from the choices.
    /// This is computed as follows:
    ///     * X,Y: ComputedPosition(x, y-OptionSize.height)
    ///     * Width, Height: OptionSize(Width,Height)
    /// </summary>
    protected Rect LabelPosition = new Rect(0, 0, 0, 0);

    public string LabelTextOnNoSpeechOptions = "Sorry we did not find an answer";
    public string LabelTextOnMultipleOptions = "Did you mean one of the following?";
    
    public string StatementTextOnNoSpeechOptions = "Yes, I was making a statement and do not expect a response.";
    public string StatementTextOnMultipleOptions = "I was making a statement and do not expect a response.";
    
    public string NotAboveTextOnNoSpeechOptions = "No, I was asking a question and expected a response.";
    public string NotAboveTextOnMultipleOptions = "I was asking a different question and expected a response.";


    /// <summary>
    /// 
    /// </summary>
    public SpeechOption Statement = new SpeechOption("", SpeechOptionType.SO_STATEMENT);
    /// <summary>
    /// 
    /// </summary>
    public SpeechOption NotAbove = new SpeechOption("", SpeechOptionType.SO_NOT_ABOVE);

    List<SpeechOption> Options;
    GUIContent[] items;

    string LabelText = "";
    protected int SelectedIndex;

    public void DisplayNoAnswerChoices()
    {
        Options = new List<SpeechOption>();

        LabelText = LabelTextOnNoSpeechOptions;

        Statement.Text = StatementTextOnNoSpeechOptions;
        Options.Add(Statement);

        NotAbove.Text = NotAboveTextOnNoSpeechOptions;
        Options.Add(NotAbove);



        SetItems();
        ShouldRender = true;
    }

    

    public void DisplaySpeechOptions(List<SpeechOption> options)
    {

        LabelText = LabelTextOnMultipleOptions;

        if (UseStatementSpeechOption)
        {
            Statement.Text = StatementTextOnMultipleOptions;
            options.Add(Statement);
        }

        if (UseNotAboveSpeechOption)
        {
            NotAbove.Text = NotAboveTextOnMultipleOptions;
            options.Add(NotAbove);
        }

        this.Options = options;

        SetItems();
        ShouldRender = true;
    }

    protected void SetItems()
    {
        items = new GUIContent[Options.Count];
        int i = 0;
        foreach (SpeechOption o in Options)
        {
            items[i] = o.GetGUIContent(i + 1);

            i++;
        }
        SelectedIndex = -1;
    }

    
    protected override bool Show()
    {
        //We compute position everytime to allow for easy editing in the UnityEditor.
        //This could be moved somewhere else for efficiency.
        {//Compute position.
            ComputedPosition.height = OptionSize.height * items.Length;
            LabelPosition.height = OptionSize.height;

            //Notice both are changing here!
            ComputedPosition.width = LabelPosition.width = OptionSize.width;
            ComputedPosition.x = LabelPosition.x = _ActualPosition.x;

            if (PositionSpecifiedAsLowerLeftCorner)
            {
                ComputedPosition.y = _ActualPosition.y - ComputedPosition.height;
            }
            else
            {
                ComputedPosition.y = _ActualPosition.y;
            }

            LabelPosition.y = ComputedPosition.y - OptionSize.height;
        }

        GUI.Label(LabelPosition, LabelText);

        int newSelectedItemIndex = GUI.SelectionGrid(ComputedPosition, SelectedIndex, items, 1);
        if (newSelectedItemIndex != SelectedIndex)
        {
            SelectedIndex = newSelectedItemIndex;
            OnOptionSelected(Options[SelectedIndex], new SpeechOptionSelectedArgs(Options));
        }

        


        return false;
    }

    public override string ToString()
    {
        return "RenSpeechOptions";
    }

    

    protected void OnOptionSelected(SpeechOption o, SpeechOptionSelectedArgs args)
    {
        if (OptionSelectedEvent != null)
        {
            OptionSelectedEvent(o, args);
        }
        else
        {
            //Debug.Log("No handler");
        }
    }

}


public enum SpeechOptionType
{
    SO_ANSWER = 0,
    SO_STATEMENT = 1,
    SO_NOT_ABOVE = 3
}

public class SpeechOption
{
    public System.Object Obj;
    public string Text;
    public SpeechOptionType Type;
    public int index;

    public SpeechOption(string text, System.Object obj)
    {
        this.Obj = obj;
        this.Text = text;
        this.Type = SpeechOptionType.SO_ANSWER;
    }

    public SpeechOption(string text, SpeechOptionType type)
    {
        this.Obj = null;
        this.Text = text;
        this.Type = type;
    }

    public GUIContent GetGUIContent(int number)
    {
        index = number;
        return new GUIContent("" + number + ". " + Text);
    }
}

public class SpeechOptionSelectedArgs : System.EventArgs 
{
    public List<SpeechOption> options;

    public SpeechOptionSelectedArgs(List<SpeechOption> o)
    {
        options = o;
    }
}

public delegate void SpeechOptionSelected(SpeechOption o, SpeechOptionSelectedArgs args);