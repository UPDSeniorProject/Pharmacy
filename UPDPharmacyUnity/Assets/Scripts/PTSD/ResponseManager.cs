using UnityEngine;

public class ResponseManager : MonoBehaviour
{
	public UILabel ResponseText;
    public UILabel ResponseText2;
	//public bool fillWithDummyData = false;

	UIInput mInput;
	bool mIgnoreNextEnter = false;

	void Awake ()
	{
		mInput = GameObject.Find("ResponseInput").GetComponent<UIInput>();
		ResponseText = GameObject.Find("ResponseText").GetComponent<UILabel>();
        ResponseText2 = GameObject.Find("ResponseText2").GetComponent<UILabel>();
	}
    private string currentTaskName;
	private string currentComparisonTaskName = "";
    void OnEnable()
    {
        Messenger<string>.AddListener("response manager task", setTaskName);
		Messenger<string>.AddListener("response manager unitprice task", setTaskNameUnitPrice);
		Messenger<string, string>.AddListener("response manager price comparison task", setTaskNamePC);
		Messenger<string, string>.AddListener("response manager unit price comparison task", setTaskNameUnitPricePC);
    }
    void OnDisable()
    {
        Messenger<string>.RemoveListener("response manager task", setTaskName);
		Messenger<string>.RemoveListener("response manager unitprice task", setTaskNameUnitPrice);
		Messenger<string, string>.RemoveListener("response manager price comparison task", setTaskNamePC);
		Messenger<string, string>.RemoveListener("response manager unit price comparison task", setTaskNameUnitPricePC);
    }


    void setTaskName(string name)
    {
        currentTaskName = name;
        string[] sub = currentTaskName.Split('_');
        ResponseText.text = "Enter the price for:"+"\n" + sub[2] + "\n"+"Press \"A\" to submit"+"\n";
    }
	
	// function for the UNIT price task
	void setTaskNameUnitPrice(string name)
    {
        currentTaskName = name;
        string[] sub = currentTaskName.Split('_');
        ResponseText.text = "Enter the UNIT PRICE for:"+"\n" + sub[2] + "\n"+"Press \"A\" to submit"+"\n";
    }
	
	// function for the price comparison task
	void setTaskNamePC(string name, string comparisonName)
    {
        currentTaskName = name;
		currentComparisonTaskName = comparisonName;
        string[] sub = currentTaskName.Split('_');
		string[] sub2 = currentComparisonTaskName.Split('_');
        ResponseText.text = "Enter 'Item1' if " + sub[2] + "is cheaper " + "\n" + " else enter 'Item2' if " + sub2[2] + "is cheaper " + "\n" + "Press \"A\" to submit" + "\n"; ;
    }
	
	// function for the UNIT price comparison task
	void setTaskNameUnitPricePC(string name, string comparisonName)
    {
        currentTaskName = name;
		currentComparisonTaskName = comparisonName;
        string[] sub = currentTaskName.Split('_');
		string[] sub2 = currentComparisonTaskName.Split('_');
        ResponseText.text = "Enter 'Item1' if " + sub[2] + "is cheaper by UNIT price" + "\n" + " else enter 'Item2' if " + sub2[2] + "is cheaper by UNIT price" + "\n" + "Press \"A\" to submit" + "\n"; ;
    }

	/// <summary>
	/// Pressing 'enter' should immediately give focus to the input field.
	/// </summary>

	void Update ()
	{
		if (!mIgnoreNextEnter)
			{
				mInput.selected = true;
			}
        
		/*if (Input.GetKeyUp(KeyCode.Return))
		{
			if (!mIgnoreNextEnter && !mInput.selected)
			{
				mInput.selected = true;
			}
			mIgnoreNextEnter = false;
		}*/

        //if (mInput.selected == true && Input.GetKeyUp(KeyCode.Return))
        if (mInput.selected == true && Input.GetButtonDown("A Button"))
        {
            Submit();
            //mIgnoreNextEnter = false;
        }
	}

	/// <summary>
	/// Submit notification is sent by UIInput when 'enter' is pressed or iOS/Android keyboard finalizes input.
	/// </summary>

	void Submit ()
	{
        //Debug.Log("submit1");
		if (ResponseText != null)
		{
			// It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
			string text = NGUITools.StripSymbols(mInput.text);
            //Debug.Log("submit2");
			if (!string.IsNullOrEmpty(text))
			{
                //Debug.Log("submit3");ResponseText.text+
                ResponseText2.color = Color.yellow;
				ResponseText2.text = "You entered: "+text;
				mInput.text = "";
				mInput.selected = false;
                Messenger<string, string, string>.Broadcast("finish response", currentTaskName, currentComparisonTaskName, text);
			}
		}
		//mIgnoreNextEnter = true;
	}
}