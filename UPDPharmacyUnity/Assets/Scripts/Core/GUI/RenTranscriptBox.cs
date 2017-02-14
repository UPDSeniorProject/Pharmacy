using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class RenTranscriptBox : RenGUIElement {

    public Dictionary<string, RenTextBox> transcripts;

    Vector2 ScrollPosition = Vector2.zero;

    public Rect ScrollRect = new Rect(0, 0, 300, 300);
    public Rect BoxRect = new Rect(0, 0, 300, 300);

    int Lines = 0;

    public int LineLength = 32;

    public float LineHeight = 23.4f;


    public RenTranscriptBox()
        : base()
    {
        Name = "TranscriptBox";
        transcripts = new Dictionary<string, RenTextBox>();
    }

    public bool HasTranscript(string characterName)
    {
        try
        {
            if (transcripts[characterName] != null)
            {
                return true;
            }
            else
                return false;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public bool AddNewTranscript(string characterName, GUISkin skin)
    {
        if(!HasTranscript(characterName) ) {

            RenTextBox box = new RenTextBox();
            box.Position = this.Position;
            box.Position.width = box.Position.width - 20;
            box.Skin = skin;
			
            box.Name = "";
            box.canEdit = false;
            box.isMultiLine = true;
            box.useScroll = false;
            for (int i = 0; i < Lines; i++)
            {
                //add new lines.
                box.text += "\n";
            }

            transcripts[characterName] = box;

            return true; 
        }else
            return false;
    }
	
	public RenTextBox GetTranscriptTextBox(string characterName) {
		if(HasTranscript(characterName)){
			return transcripts[characterName];
		}else 
			return null;
	}
	
    public bool AddEntry(string characterName, string text)
    {
        if (HasTranscript(characterName))
        {
			text = DecodeHtmlChars(text);
            string entry = characterName + ": " + text;
            int newLines = ComputeLines(entry);
            Lines += newLines;
            float boxHeight = Lines * LineHeight;
            if (boxHeight < ScrollRect.height)
            {
                boxHeight = ScrollRect.height;
            }
            else
            {
                Position.height = boxHeight;
                ScrollPosition.y = Position.height - ScrollRect.height;
            }

            string otherEntries = EmptyLines(newLines);

            foreach (KeyValuePair<string, RenTextBox> pair in transcripts)
            {
                pair.Value.Position.height = boxHeight;
                if (pair.Key != characterName)
                {
                    pair.Value.text += otherEntries;
                }
                else
                    pair.Value.text += entry + "\n";
            }

            return true;
        }
        else
        {
            Debug.Log("No transcript for: " + characterName);
            return false;
        }

    }

    protected string EmptyLines(int count)
    {
        string lines = "";
        for (int i = 0; i < count; i++) lines += "\n";
        return lines;
    }

    protected int ComputeLines(string entry)
    {
        string[] lines = entry.Split('\n');
        int count = lines.Length;
		//Debug.Log("Counted: " + count + " lines on entry at the beginning: " + entry);

        foreach (string line in lines)
        {
            int currentLineLength = 0;
            string[] words = line.Split(' ');
            foreach (string word in words)
            {
                if (word.Length < LineLength)
                {
					//Debug.Log("Word: " + word + " is of length: " + word.Length + " making the line length to: " + currentLineLength + " and LineLength is : " + LineLength + " and count is : " + count);
                    currentLineLength += word.Length;
                    if (currentLineLength > LineLength)
                    {
                        currentLineLength = word.Length;
                        count++;
                    }
                }
                else
                {
                    int wordLength = word.Length;

                    count += (wordLength / LineLength) + 1;
                    currentLineLength = wordLength % LineLength;
                }
            }
        }

		if(count > 2){
			//count = count -1;
		}

        //Debug.Log("Counted: " + count + " lines on entry: " + entry);
        return count;
    }

    protected override bool Show()
	{
	
		GUI.Box(BoxRect, "");

        //force no horizontal scroll
        GUI.skin.horizontalScrollbar = GUIStyle.none;
	

        ScrollPosition = GUI.BeginScrollView(BoxRect, ScrollPosition, Position);
		
        foreach (KeyValuePair<string, RenTextBox> pair in transcripts)
        {
            pair.Value.Display();
        }


        GUI.EndScrollView();

        return false;
    }

    public override string ToString()
    {
        return "RenTranscriptBox";
    }
	
	public string DecodeHtmlChars(string aText)
	{
		aText = aText.Replace("&amp;#039;","'");
	    return aText;
	}


}
