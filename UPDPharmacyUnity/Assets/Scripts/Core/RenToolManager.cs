using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenToolManager : RenBehaviour {

	/// <summary>
	/// A list with all available tools.
	/// </summary>
    public Dictionary<string, PhysicalExaminationTool> Tools = new Dictionary<string, PhysicalExaminationTool>();
	
	/// <summary>
	/// Tool currently in use. Null when no tool has been selected.
	/// </summary>
    private PhysicalExaminationTool ActiveTool;

    protected override void Awake()
    {
        base.Awake();
        //Add PhysicalExaminationTools in children to the list of tools.
        foreach (PhysicalExaminationTool t in GetComponentsInChildren<PhysicalExaminationTool>())
        {
            if (GetTool(t.Name) == null)
                Tools[t.Name] = t;
        }


        foreach (PhysicalExaminationTool t in Tools.Values)
        {
            AddToolEvents(t);
        }
    }

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        ActiveTool = null;
	}


    public void AddTool(PhysicalExaminationTool t)
    {
        Tools[t.Name] = t;
        AddToolEvents(t);
    }

    protected void AddToolEvents(PhysicalExaminationTool t)
    {
        t.ToolActivated += new ToolActivatedEventHandler(ToolActivated);
        t.ToolDeactivated += new ToolDeactivatedEventHandler(ToolDeactivated);
    }

    protected virtual void ToolActivated(PhysicalExaminationTool tool, ToolEventArgs args)
    {
        Debug.Log("Tool with name: " + tool.Name + " activated");
        ActiveTool = tool;
        foreach (PhysicalExaminationTool t in Tools.Values)
        {
            if (t != ActiveTool)
                t.ManagerToolDeactivation(this);
        }
    }

    protected virtual void ToolDeactivated(PhysicalExaminationTool tool, ToolEventArgs args)
    {   
        ActiveTool = null;
        Debug.Log("Tool with name: " + tool.Name + " deactivated");
    }
	
	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
	}

    /// <summary>
    /// Get a tool by name
    /// </summary>
    /// <param name="Name">Name of the tool</param>
    /// <returns>The tool or null if not found</returns>
    public PhysicalExaminationTool GetTool(string Name)
    {
        try
        {
            return Tools[Name];
        }
        catch (KeyNotFoundException)
        {
            return null;
        }


    }


}
