using UnityEngine;
using System.Collections.Generic;

public class RenManager : RenBehaviour
{
    #region Book-keeping fields
    /// <summary>
    /// Contains a list of all registered RenBehaviours.
    /// This is used for Pause and Resume Events. 
    /// 
    /// RenBehaviours register on Start. This should not be
    /// evaded unless you are completely sure of what you are
    /// doing.
    /// </summary>
    protected List<RenBehaviour> ActiveRenBehaviours;

    /// <summary>
    /// Reference to the Behaviour that paused the interaction
    /// </summary>
    protected RenBehaviour BehaviourPausingInteraction;
    #endregion

    #region GUIElements
    /// <summary>
    /// Generic output window to display debug messages.
    /// </summary>
    public RenDebugBox DebugBox = new RenDebugBox();

    /// <summary>
    /// This button is used to terminate the simulation.
    /// The button will only be shown if 
    /// </summary>
    public RenButton EndSimulation = new RenButton("Finish Interaction");
	
	public bool HideMouse = false;
	
    #endregion

    #region Debug fields
    /// <summary>
    /// Defines if Debug calls get added to the debugBox.
    /// </summary>
    public bool verbose = true;

#if UNITY_EDITOR
    public bool SetRedirectTest = true;
#endif

    #endregion

    #region MonoBehaviour functions
    new protected void Awake()
    {
        base.Awake();
		DisplayGUIBeforeLoadEnd = true; // allow error messages...
        ActiveRenBehaviours = new List<RenBehaviour>();
        BehaviourPausingInteraction = null;

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
        UnityEngine.Debug.Log("Not capturing all key input!! WEB GL!");
#endif


    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        ForceGUIUpdate();
       
        
        this.AddGUIElement(DebugBox, false);  
#if UNITY_EDITOR
        if (SetRedirectTest)
        {
            SetRedirect("");
        }
#endif
		
		if (HideMouse)
		{
			Cursor.visible = false;	
		}
	}


	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        //Updating the viewport
        if (CurrentViewportSize.Width != Screen.width ||
            CurrentViewportSize.Height != Screen.height)
        {
            CurrentViewportSize.Width = Screen.width;
            CurrentViewportSize.Height = Screen.height;
            ScaleFactor.x = (float)CurrentViewportSize.Width / (float)TargetViewportSize.Width;
            ScaleFactor.y = (float)CurrentViewportSize.Height / (float)TargetViewportSize.Height;
            _ViewportSizeUpdated = true;

           // Debug("Current Viewport Size Changed to: (W:" + CurrentViewportSize.Width + ", H: " + CurrentViewportSize.Height + ")");


        }
        else
        {
            _ViewportSizeUpdated = false;
        }

        if (_ForceGUIUpdate)
        {
            if (_ForcedGUIFrames > FORCE_GUI_FRAMES_MAX)
            {
                _ForceGUIUpdate = false;
            }
            else
                _ForcedGUIFrames++;
        }
	}
#endregion

#region Pause/Resume Simulation


#region Register/Unregiste Behaviour
    /// <summary>
    /// Adds a behaviour to the list of active behaviours.
    /// </summary>
    /// <param name="b">Behaviour to be added.</param>
    public void RegisterRenBehaviour(RenBehaviour b)
    {
        ActiveRenBehaviours.Add(b);
    }

    /// <summary>
    /// Removes a behaviour to the list of active behaviours.
    /// </summary>
    /// <param name="b">Behaviour to be Removes.</param>
    public void UnregisterRenBehaviour(RenBehaviour b)
    {
        ActiveRenBehaviours.Remove(b);
    }
#endregion

    public void PauseSimulation(RenBehaviour requester)
    {
        if (BehaviourPausingInteraction == null)
        {
            BehaviourPausingInteraction = requester;
            foreach (RenBehaviour b in ActiveRenBehaviours)
            {
                if (b != requester)
                    b.OnRenPaused();
            }

            
        }
    }

    public void ResumeSimulation(RenBehaviour requester)
    {
        if (requester == BehaviourPausingInteraction)
        {
            foreach (RenBehaviour b in ActiveRenBehaviours)
            {
                b.OnRenResumed();
            }
        }
    }

#endregion

#region Debug

    public void Debug(string s)
    {
        if (verbose)
        {
            DebugBox.AddLine(s);
        }
    }

#endregion

#region Simulation End protocol
    /// <summary>
    /// Behaviours that care if the simulation will end should subscribe to this event.
    /// This will notify them and give them a chance to stall ending by adding themselves
    /// to the WaitToEnd List.
    /// </summary>
    public event SimulationEventHandler SimulationWillEnd;
    
    /// <summary>
    /// Private List of Behaviours that are preventing the simulation from finishing.
    /// </summary>
    private List<RenBehaviour> WaitToEnd;

    /// <summary>
    /// Method meant to be called from Javascript that would set the urlRedirect.
    /// TODO: Define if javascript decides redirection or unity stores it.
    /// Right now Javascript handles all the redirection code.
    /// </summary>
    /// <param name="urlRedirect"></param>
    public void SetRedirect(string urlRedirect)
    {
        this.AddGUIElement(EndSimulation, false);
        EndSimulation.ButtonPressed += new ButtonPressedEventHandler(EndSimulation_ButtonPressed);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="args"></param>
    private void EndSimulation_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        //make sure no one tries to click it twice.
        btn.ShouldRender = false;
        SimulationEndingEvent();
    }
    /// <summary>
    /// Function that triggers a simulation ending event giving behaviours a chance to
    /// stall ending.
    /// </summary>
    private void SimulationEndingEvent()
    {
        //Create new list of possible behaviours we need to wait on.
        WaitToEnd = new List<RenBehaviour>();

        //See if anyone cares the simulation is ending.
        //TimeTriggers & ReflectiveLearning might.
        if (SimulationWillEnd != null)
        {
            SimulationEventArgs end_args = new SimulationEventArgs();
            SimulationWillEnd(end_args);
        }

        //If by the end no behaviour stopeed the ending, just close Ren.
        if (WaitToEnd.Count == 0)
        {
            CloseRen();
        }

    }

    /// <summary>
    /// If a behaviour needs to stall ending, it should call this method.
    /// </summary>
    /// <param name="b"></param>
    public void AddToWaitToEndList(RenBehaviour b)
    {
        if (WaitToEnd == null)
        {
            Debug("Why is RenBehaviour " + b + " asking to get added to wait to end list when no end event has triggered?");
        }
        else
        {
            WaitToEnd.Add(b);
        }
    }

    /// <summary>
    /// Once a behavior that is stalling has ended. It can call this method to remove the lock.
    /// </summary>
    /// <param name="b"></param>
    public void ReadyToEnd(RenBehaviour b)
    {
        WaitToEnd.Remove(b);

        if (WaitToEnd.Count == 0)
        {
            CloseRen();
        }
    }

    /// <summary>
    /// Makes the external call to force the redirection.
    /// </summary>
    protected void CloseRen() 
    {
        if (Application.isWebPlayer)
        {
            Application.ExternalCall("IPSInteractionEnded", null);
        }
    }

#endregion

#region Resizable GUI fields

	public ResizeType ResizeGUI = ResizeType.None;
	public Size TargetViewportSize = new Size(720,480);
	public Size CurrentViewportSize = new Size(0,0);
    public Vector2 ScaleFactor = new Vector2(0, 0);

    protected bool _ViewportSizeUpdated = false;
    /// <summary>
    /// When set to true, the positions of GUI elements will be updated for the next FORCE_GUI_FRAMES_MAX (2) frames.
    /// Used on the first frame, but can be used to force updates later.
    /// </summary>
    protected bool _ForceGUIUpdate = false;
    protected int _ForcedGUIFrames = 0;

    private const int FORCE_GUI_FRAMES_MAX = 2;

    public bool ShouldForceGUIUpdate
    {
        get
        {
            return _ForceGUIUpdate;
        }
    }

    private void ForceGUIUpdate()
    {
        _ForceGUIUpdate = true;
        _ForcedGUIFrames = 0;
    }


    public bool ViewportSizeUpdated
    {
        get
        {
            return _ViewportSizeUpdated;
        }
    }

	public Rect GetPosition(Rect Position) {
		switch(ResizeGUI) {
		    case ResizeType.None:
			    return Position;
		    case ResizeType.ScaleBasedOnTargetViewportSize:
                return new Rect(Position.xMin * ScaleFactor.x, Position.yMin * ScaleFactor.y,
                    Position.width * ScaleFactor.x, Position.height * ScaleFactor.y);
		    case ResizeType.UsePositionAsRelative:
			    return new Rect(Position.xMin * CurrentViewportSize.Width, Position.yMin*CurrentViewportSize.Height,
			                Position.width * CurrentViewportSize.Width, Position.height * CurrentViewportSize.Height);
		}
		return Position;
	}

    public Vector2 GetVector2(Vector2 v2)
    {
        switch (ResizeGUI)
        {
            case ResizeType.None:
                return v2;
            case ResizeType.ScaleBasedOnTargetViewportSize:
                return new Vector2(v2.x * ScaleFactor.x, v2.y * ScaleFactor.y);
            case ResizeType.UsePositionAsRelative:
                return new Vector2(v2.x * CurrentViewportSize.Width, v2.y);
        }
        return v2;
    }

    public Rect InvertPosition(Rect Position)
    {
        switch (ResizeGUI)
        {
            case ResizeType.None:
			    return Position;
		    case ResizeType.ScaleBasedOnTargetViewportSize:
                return new Rect(Position.xMin / ScaleFactor.x, Position.yMin / ScaleFactor.y,
                    Position.width / ScaleFactor.x, Position.height / ScaleFactor.y);
		    case ResizeType.UsePositionAsRelative:
			    return new Rect(Position.xMin / CurrentViewportSize.Width, Position.yMin/CurrentViewportSize.Height,
			                Position.width / CurrentViewportSize.Width, Position.height / CurrentViewportSize.Width);
		}
        return Position;
    }

#endregion

}

[System.Serializable]
public enum ResizeType {
	None = 0,
	UsePositionAsRelative,
	ScaleBasedOnTargetViewportSize
};

public class SimulationEventArgs : System.EventArgs { }

public delegate void SimulationEventHandler(SimulationEventArgs args);
