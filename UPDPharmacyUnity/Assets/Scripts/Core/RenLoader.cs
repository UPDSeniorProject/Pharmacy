using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class AbstractRenLoader : RenBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    protected bool HasFinishedLoading;

    protected override void Awake()
    {
        base.Awake();
        HasFinishedLoading = false;
    }

    public abstract float GetLoadingProgress();

    /// <summary>
    /// Generic way of returning progress.
    /// Not the most efficient way. GetLoadingProgress should
    /// set HasFinishedLoading!
    /// </summary>
    /// <returns></returns>
    public virtual bool HasFinished()
    {
        if (!HasFinishedLoading)
        {
            HasFinishedLoading = GetLoadingProgress() >= 1.0f;
        }
        return HasFinishedLoading;
    }
}

public class RenLoader : AbstractRenLoader
{
    /// <summary>
    /// 
    /// </summary>
    public RenImage LoadingScreen;

    /// <summary>
    /// 
    /// </summary>
    public RenImage ProgressBar;

    /// <summary>
    /// 
    /// </summary>
    public RenLabel ProgressLabel;

    /// <summary>
    /// 
    /// </summary>
    protected Size ProgressBarSize;

    protected Size CurrentProgressBarSize;

    protected List<AbstractRenLoader> Loaders;

    public event RenLoadingEvent LoadingFinished;

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        //This should be the only behaviour that does this.
        //Allows display of loading screen!
        DisplayGUIBeforeLoadEnd = false;

        //Ideally we wouldn't need a reference to the Manager yet
        //However the manager has the Target Viewport Size and 
        //scaling type which we need to compute the positions
        //and sizes of the loading screen elements.
        RenManager _Manager = GameObject.Find("IPSRen").GetComponent<RenManager>();

        SetupLoadingBackground(_Manager);

        //Compute scaling factors
        float scaleX = 0.0f; 
        float scaleY = 0.0f; 

        switch(_Manager.ResizeGUI) {
            case ResizeType.None:
                scaleX = Screen.width / (float)960;
                scaleY = Screen.height / (float)600;
                break;
            case ResizeType.ScaleBasedOnTargetViewportSize:
                scaleX = (float)_Manager.TargetViewportSize.Width / (float)960;
                scaleY = (float)_Manager.TargetViewportSize.Height / (float)600;
                break;
            case ResizeType.UsePositionAsRelative:
                scaleX = 1.0f / 960f;
                scaleY = 1.0f / 600f;
                break;
        }

        ProgressBar.DrawBox = false;
        ProgressBarSize = ProgressBar.GetSize();
        ProgressBarSize.Height = (int)(ProgressBarSize.Height * scaleY);
        ProgressBarSize.Width = (int)(ProgressBarSize.Width * scaleX);

        CurrentProgressBarSize = new Size(0, ProgressBarSize.Height);
        ProgressBar.Position = new Rect(318 * scaleX, 527 * scaleY, 0, ProgressBarSize.Height);

        //Debug.Log("Progress Bar Position: " + ProgressBar.Position);

        AddGUIElement(ProgressBar, false);

        ProgressLabel.LabelText = "0.0%";
        ProgressLabel.Position = new Rect(820 * scaleX, 527 * scaleY, 100 * scaleX, ProgressBarSize.Height);
        AddGUIElement(ProgressLabel, false);
		
		/*
		RenButton button = new RenButton();
		button.Position = new Rect(0,0,300,100);
		button.Label = "Print Loaders";
		button.ButtonPressed += new ButtonPressedEventHandler(print_loaders);
		AddGUIElement(button, true);
*/
        //Find all loaders.
        FindAllLoaders();
    }

    private void SetupLoadingBackground(RenManager _Manager)
    {
        //The order in which they are added is important.
        //The Loading screen goes first so it is rendered first.
        LoadingScreen.DrawBox = false;
        //This positions the loading screen in the whole camera view.
        switch (_Manager.ResizeGUI)
        {
            case ResizeType.None:
                LoadingScreen.Position = new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight);
                break;
            case ResizeType.ScaleBasedOnTargetViewportSize:
                LoadingScreen.Position = new Rect(0, 0, _Manager.TargetViewportSize.Width, _Manager.TargetViewportSize.Height);
                break;
            case ResizeType.UsePositionAsRelative:
                LoadingScreen.Position = new Rect(0,0, 1.0f, 1.0f);
                break;
        }
            


        AddGUIElement(LoadingScreen, false);
    }
	
	protected void print_loaders(RenButton btn, ButtonPressedEventArgs args) 
	{
		string loaders = "";
		foreach (AbstractRenLoader l in Loaders)
        {
            float p = l.GetLoadingProgress();
			loaders += "\n" + l + " => " + p;
        }			
		
		Application.ExternalCall("alert", loaders);
		
	}

    protected override void Start()
    {
        StartCoroutine(UpdateOwnProgress());
    }

    /// <summary>
    /// 
    /// </summary>
    protected void FindAllLoaders()
    {
        Loaders = new List<AbstractRenLoader>();
        GameObject[] objects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject o in objects)
        {
            AbstractRenLoader[] loaders = o.GetComponentsInChildren<AbstractRenLoader>();
            foreach (AbstractRenLoader loader in loaders)
            {
                if (loader != this)
                {
                    Loaders.Add(loader);
                }
            }
        }
		
		AddDebugLine("Ren Loader has: " +  Loaders.Count + " active loaders");
    }

    public void RegisterAsLoader(AbstractRenLoader loader)
    {
        if (!Loaders.Contains(loader))
        {
            Loaders.Add(loader);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnGUI()
    {
        if (!HasFinishedLoading)
        {
            UpdateProgressBarSize(); //this changes HasFinishedLoading
            base.OnGUI();
            if (HasFinishedLoading)
            {
                //Doing this here makes sure 100% is reached
                //and the next frame won't display.
                SetDisplayAllGUI(false);
                OnLoadingFinished();
            }
        }
        
    }

    private void OnLoadingFinished()
    {
        if (LoadingFinished != null)
        {
            LoadingFinished(this, new RenLoadingEventArgs());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateProgressBarSize()
    {
        float p = GetLoadingProgress();
        CurrentProgressBarSize.Width = (int)(ProgressBarSize.Width * p);

        ProgressBar.ScaleToSize(CurrentProgressBarSize);
        ProgressLabel.LabelText = p.ToString("P1");

    }


    /// <summary>
    /// Averages the progress of all AbstractRenLoaders.
    /// </summary>
    /// <returns></returns>
    public override float GetLoadingProgress()
    {
        float progress = OwnProgress;
        foreach (AbstractRenLoader l in Loaders)
        {
            float p = l.GetLoadingProgress();
            progress += p;
        }
		

        //compute actual progress
        progress = progress / (Loaders.Count + 1);

        if (progress >= 1.0f) HasFinishedLoading = true;

        return progress;
    }

    private float OwnProgress = 0.0f;
    private IEnumerator UpdateOwnProgress()
    {
        while (OwnProgress < 1.0f)
        {
            OwnProgress += 0.1f;
            yield return new WaitForSeconds(0.2f);
        }
    }

}

public class RenLoadingEventArgs : System.EventArgs { }

public delegate void RenLoadingEvent(RenLoader loader, RenLoadingEventArgs args);
