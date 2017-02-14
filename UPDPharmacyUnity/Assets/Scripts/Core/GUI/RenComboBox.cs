using UnityEngine;
using System.Collections;

/// <summary>
/// Taken and adapted from: http://wiki.unity3d.com/index.php?title=PopupList
/// </summary>

[System.Serializable]
public class RenComboBox : RenGUIElement
{
    public event ComboBoxOpenedEventHandler ComboBoxOpened;
    public event ComboBoxSelectedIndexChangedEventHandler IndexChanged;

    private bool forceToUnShow = false;
    private int useControlID = -1;
    private bool isClickedComboButton = false;
    private int selectedItemIndex = 0;

    public GUIContent buttonContent = new GUIContent("No-content");
    public GUIContent[] listContent;

    public bool dropDown = true;

    override protected bool Show()
    {
        if (forceToUnShow)
        {
            forceToUnShow = false;
            isClickedComboButton = false;
        }

        bool done = false;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.mouseUp:
                {
                    if (isClickedComboButton)
                    {
                        done = true;
                    }
                }
                break;
        }

        if (GUI.Button(_ActualPosition, buttonContent))
        {
            if (useControlID == -1)
            {
                useControlID = controlID;
                isClickedComboButton = false;
            }

            if (useControlID != controlID)
            {
                forceToUnShow = true;
                useControlID = controlID;
            }
            isClickedComboButton = true;

            //Trigger the event of the box openning.
            OnComboBoxOpened();
        }

        if (isClickedComboButton)
        {
            float itemHeight = GUI.skin.button.CalcHeight(listContent[0], 1.0f);
            float listHeight = itemHeight * listContent.Length;

            Rect listRect;

            if (dropDown)
            {
                listRect = new Rect(_ActualPosition.x, _ActualPosition.y + itemHeight,
                      _ActualPosition.width, listHeight);
            }
            else
            { //dropUp
                listRect = new Rect(_ActualPosition.x, _ActualPosition.y - listHeight,
                      _ActualPosition.width, listHeight);
            }

            //draw empty box?
            GUI.Box(listRect, "");

            int newSelectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex, listContent, 1);

            if (newSelectedItemIndex != selectedItemIndex)
            {
                SelectedItemIndex = newSelectedItemIndex; //property!
                OnIndexChanged();
            }
        }

        if (done)
            isClickedComboButton = false;

        return false;
    }

    public void AddItem(string s)
    {
        //new array with increased size!
        GUIContent[] newArray = new GUIContent[listContent.Length + 1];

        for (int i = 0; i < listContent.Length; i++)
        {
            newArray[i] = listContent[i];
        }
        newArray[listContent.Length] = new GUIContent(s);

        listContent = newArray;
    }

    public void AddItems(string[] items)
    {
        //new array with increased size!
        GUIContent[] newArray = new GUIContent[listContent.Length + items.Length];
        for (int i = 0; i < listContent.Length; i++)
        {
            newArray[i] = listContent[i];
        }
        for (int i = 0; i < items.Length; i++)
        {
            newArray[listContent.Length + i] = new GUIContent(items[i]);
        }

        listContent = newArray;
    }

    public void AddItems(System.Collections.Generic.List<string> items)
    {
        AddItems(items.ToArray());
    }

    public void SetItems(string[] items)
    {
        listContent = new GUIContent[items.Length];
        for (int i = 0; i < listContent.Length; i++)
        {
            listContent[i] = new GUIContent(items[i]);
        }
    }

    public void SetItems(System.Collections.Generic.List<string> items)
    {
        SetItems(items.ToArray());
    }

    protected void OnIndexChanged()
    {
        if (IndexChanged != null)
        {
            IndexChanged(this, new System.EventArgs());
        }
        else
        {
            //not really an error. You could uncomment this line if you needed
            //to debug more
            //Debug.Log("No handler for IndexChanged!"); 
        }
    }

    protected void OnComboBoxOpened()
    {
        if (ComboBoxOpened != null)
        {
            ComboBoxOpened(this, new System.EventArgs());
        }
        else
        {
            //not really an error. You could uncomment this line if you needed
            //to debug more
            //Debug.Log("No handler for ComboBoxOpened!"); 
        }
    }

    public int SelectedItemIndex
    {
        get
        {
            return selectedItemIndex;
        }
        set
        {
            selectedItemIndex = value;
            buttonContent = listContent[selectedItemIndex];
        }
    }

    public GUIContent SelectedItem
    {
        get
        {
            return listContent[selectedItemIndex];
        }
    }

    public override string ToString()
    {
        return "RenComboBox";
    }

}


public delegate void ComboBoxSelectedIndexChangedEventHandler(RenComboBox box, System.EventArgs args);
public delegate void ComboBoxOpenedEventHandler(RenComboBox box, System.EventArgs args);