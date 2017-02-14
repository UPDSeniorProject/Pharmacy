using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeChartTool : CranialNerveTool {

    public int SelectedLine = 1;
    public GameObject Pointer;

    public float[] LineYPositions = {
        0.0f, //not used
        44.3f, //line 1
        37.8f, //line 2
        33.4f, //line 3
        29.5f, //line 4
        26.8f, //line 5
        24.2f, //line 6
        22.2f, //line 7
        20.3f //line 8
                                   };
    public bool UseLineGUI = false;
    public Rect LineGUIElementsInitialPosition;
    public GUISkin LineGUISkin = null;
    protected Dictionary<RenGUIElement, int> lineGUIElements = new Dictionary<RenGUIElement, int>();

    

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        this.Visible = false;

        RenLabel LineLabel = new RenLabel();
        LineLabel.LabelText = "Select the number of line you want to point to";
        LineLabel.ShouldRender = false;
        LineLabel.Position = new Rect(LineGUIElementsInitialPosition.xMin, LineGUIElementsInitialPosition.yMin - 30,
            (LineGUIElementsInitialPosition.width + 5) * 8 - 5, LineGUIElementsInitialPosition.height);

        LineLabel.tint = Color.yellow;
        AddGUIElement(LineLabel);
        lineGUIElements[LineLabel] = 0;


        for (int i = 1; i < 9; i++)
        {
            RenButton btn = new RenButton();
            btn.Label = "" + i;

            btn.Position = new Rect(LineGUIElementsInitialPosition.xMin + (LineGUIElementsInitialPosition.width + 5) * (i-1),
                LineGUIElementsInitialPosition.yMin,
                LineGUIElementsInitialPosition.width,
                LineGUIElementsInitialPosition.height);



            btn.ButtonPressed += LineGUIButtonPressed;
            AddGUIElement(btn);
            lineGUIElements[btn] = i;
            btn.ShouldRender = false;
        }

        if (LineGUISkin != null)
        {
            foreach (RenGUIElement e in lineGUIElements.Keys)
            {
                e.Skin = LineGUISkin;
            }
        }

        HelpBox.text = "- You can ask the virtual patient to read the selected line.\n" +
                       "- You can also ask the virtual patient which is the lowest line he or she can read.\n" +
                       "- Remember you can always ask the patient to cover one eye to test visual acuity bilaterally.";
	}

    void LineGUIButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        if(args.button == MouseButton.MOUSE_LEFT)
            SetLine(lineGUIElements[btn]);
    }
	
	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
	}

    public override void ActivateTool()
    {
        base.ActivateTool();
        this.Visible = true;
        foreach (RenGUIElement e in lineGUIElements.Keys)
        {
            e.ShouldRender = true;
        }

    }

    public override void DeactivateTool()
    {
        this.Visible = false;

        foreach (RenGUIElement e in lineGUIElements.Keys)
        {
            e.ShouldRender = false;
        }
    }

    public override void CheckInput()
    {
        KeyCode key = KeyCode.Alpha1;
        KeyCode numpad = KeyCode.Keypad1;

        for (; key < KeyCode.Alpha9; key++, numpad++)
        {
            if (Input.GetKeyDown(key) || Input.GetKeyDown(numpad))
            {
                SetLine(key - KeyCode.Alpha0);
            }
        }
    }

    public void SetLine(int line)
    {
        if (SelectedLine != line)
        {
            Vector3 newPos = Pointer.transform.localPosition;
            newPos.y = LineYPositions[line];
            Debug.Log("New Pos: " + newPos);

            Pointer.transform.localPosition = newPos;

            SelectedLine = line;
        }
    }

    public override string Name
    {
        get { return "EyeChartTool"; }
    }
}
