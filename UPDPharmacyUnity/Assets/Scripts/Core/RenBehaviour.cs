using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 
 */
public class RenBehaviour : MonoBehaviour {

    /// <summary>
    /// Determines if this behaviour is paused.
    /// </summary>
   	public bool isPaused = false;

    /// <summary>
    /// 
    /// </summary>
	protected bool HasLoadingFinished = false;

    /// <summary>
    /// 
    /// </summary>
    protected bool DisplayGUIBeforeLoadEnd = false;

    /// <summary>
    /// 
    /// </summary>
    protected bool DisplayGUIOnPaused = false;

    /// <summary>
    /// Every script has a reference to the IPSRen GameObject. 
    /// </summary>
    protected GameObject IPS = null;
    /// <summary>
    /// Every script has a reference to the RenManager script of the IPSRen GameObject.
    /// </summary>
    protected RenManager Manager;

    /// <summary>
    /// Determines if this script has registered itself with the RenManager.
    /// </summary>
	protected bool Registered = false;

    /// <summary>
    /// List containing all the GUI elements this script must render.
    /// </summary>
    protected List<RenGUIElement> GUIElements = new List<RenGUIElement>();


    protected virtual void InitProtocol()
    {
        if (IPS == null)
        {
            IPS = GameObject.Find("IPSRen");
            Manager = IPS.GetComponent<RenManager>();

            RenLoader loader = IPS.GetComponent<RenLoader>() as RenLoader;
            loader.LoadingFinished += new RenLoadingEvent(LoadingFinishedEvent);
        }
    }
	
	protected virtual void RegisterBehaviour()
	{
		if(Manager != null){
			if(!Registered) {
				Manager.RegisterRenBehaviour(this);
				Registered = true;
			}
		}else
			Debug.LogError("RenManager not found!");
	}

    protected void LoadingFinishedEvent(RenLoader loader, RenLoadingEventArgs args)
    {
        HasLoadingFinished = true;
    }

    protected virtual void Awake()
    {
       
    }

	/// <summary>
	/// This is used for initialization.
	/// </summary>
    protected virtual void Start()
    {
        InitProtocol();
		RegisterBehaviour();
	}

    	
	/// <summary>
    /// Update is called once per frame
	/// </summary>
    protected virtual void Update ()
    {

	}

    /// <summary>
    /// Update that happens after all other updates have taken place.
    /// </summary>
    protected virtual void LateUpdate()
    {
    }

    protected virtual void OnDisbale()
    {
        Manager.UnregisterRenBehaviour(this);
    }

    protected virtual void OnDestroy()
    {

        //Manager.UnregisterRenBehaviour(this);
    }

    /// <summary>
    /// Displays all GUI Elements in the behavior!
    /// </summary>
    protected virtual void OnGUI()
    {
        if (CanDisplayGUI())
        {
            foreach (RenGUIElement e in GUIElements)
            {
                e.Display();
            }
        }
    }
	
	/// <summary>
	/// Decides whether or not this RenBehaviour will render it's GUI elements
	/// </summary>
	public bool RenderGUI = true;
	
    private bool CanDisplayGUI()
    {
        return (DisplayGUIBeforeLoadEnd || HasLoadingFinished) //Loading conditions
                     && (!isPaused || DisplayGUIOnPaused) && RenderGUI; //Pause conditions.
    }
    
    /// <summary>
    /// Generic handler for pausing the simulation.
    /// </summary>
    public virtual void OnRenPaused() 
    {
        isPaused = true;
    }

    /// <summary>
    ///  Generic handler for resuming the simulation.    
    /// </summary>
    public virtual void OnRenResumed() 
    {
        isPaused = false;
    }

    /// <summary>
    /// Adds a new <c>RenGUIElement</c> to this behaviour. No error handler is set for the element.
    /// </summary>
    /// <param name="element">GUIElement to be added.</param>
    public void AddGUIElement(RenGUIElement element)
    {
        AddGUIElement(element, false);
    }

    /// <summary>
    /// Adds a new <c>RenGUIElement</c> to this behaviour.
    /// </summary>
    /// <param name="element">GUIElement to be added.</param>
    /// <param name="verbose">Defines if the GenericGUIErrorHandler will be added for the element.</param>
    public virtual void AddGUIElement(RenGUIElement element, bool verbose)
    {
        GUIElements.Add(element);
        if (verbose)
        {
            element.ErrorOcurred += new GUIErrorEventHandler(GenericGUIErrorHandler);
        }
    }

    /// <summary>
    /// Convenience function to add multiple GUI elements in a single call.
    /// </summary>
    /// <param name="elements"></param>
    public virtual void AddGUIElements(params RenGUIElement[] elements) {
        foreach (RenGUIElement e in elements)
        {
            AddGUIElement(e, false);
        }
    }

    /// <summary>
    /// Generic GUIErrorHandler. Added to the Elements if verbose is set to true when adding.
    /// </summary>
    /// <param name="element">Element that triggered the event.</param>
    /// <param name="args">Arguments of the error.</param>
    protected void GenericGUIErrorHandler(RenGUIElement element, RenGUIErrorEventArgs args)
    {
        AddDebugLine("ERROR on " + element.ToString() + ": " + args.errorMsg);
    }

    /// <summary>
    /// Adds a Debug Line if there is a manager.
    /// </summary>
    /// <param name="line">Line to be added to debug.</param>
    protected void AddDebugLine(string line)
    {
        if (Manager != null)
        {
            Manager.Debug(line);
        }
        else
        {

            Debug.LogWarning("Trying to write Debug line: \n\"" + line + "\"\n and manager not set :(");
        }
    }

    /// <summary>
    /// Changes the position of the GameObject. Convinience method since changing position is not
    /// straight forward.
    /// </summary>
    /// <param name="x">New x position</param>
    /// <param name="y">New y position</param>
    /// <param name="z">New z position</param>
    protected void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x,y,z));
    }

    /// <summary>
    /// Changes the position of the GameObject. Convinience method since changing position is not
    /// straight forward.
    /// </summary>
    /// <param name="pos">Vector with the new position.</param>
    protected void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    /// <summary>
    /// Straight forward way of either hiding or displaying all the GUIElements
    /// Associated with this behavior.
    /// </summary>
    /// <param name="show"></param>
    protected virtual void SetDisplayAllGUI(bool show = true)
    {
        foreach (RenGUIElement e in GUIElements)
        {
            e.ShouldRender = show;
        }
    }

	///<summary>
	///This function should only be called for scripts created during runtime
	///NOTICE: using this function incorrectly could lead to unexpected behaviours
	///</summary>
	public void ForceLoadingFinished(bool value) {
		HasLoadingFinished = value;
	}



}
