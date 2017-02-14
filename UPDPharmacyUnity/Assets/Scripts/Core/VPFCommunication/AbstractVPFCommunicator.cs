using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// General abstract class that describes the general behaviour expected to have
/// full functionality from a VPF Server.
/// </summary>
public abstract class AbstractVPFCommunicator : AbstractRenLoader
{

    #region Constants
    public const string VPF_1_WEB = "vpf1";
    public const string VPF_2_WEB = "vpf2";
    public const string VERG_DEV = "vergdev";
    public const string VPF_1_LOCAL = "local1";
	public const string VPF_2_LOCAL = "local2";
	public const string OFFLINE = "offline";
	public const string VPF_1_PTSD = "ptsdvpf1";
	public const string VPF_2_SMART = "smart2";
    public const string VPF_2_UPD = "upd2";

    public const string DEFAULT_USERNAME = "UnityWeb";
    #endregion

    #region Factory Methods
    /// <summary>
    /// Adds a VPFCommunicator according to a string.
    /// </summary>
    /// <param name="s">String to be parsed.</param>
    /// <param name="obj">The game object to add the communicator to</param>
    /// <param name="S3Bucket">The S3Bucket </param>
    /// <returns>The closest matching VPFCommunicatorType. Defaults to Smart VPF2. </returns>
    public static AbstractVPFCommunicator AddCommunicator(string s, GameObject obj, string S3Bucket = null)
    {
        s = s.ToLower();

        if (s.Contains(VPF_1_WEB))
           return obj.AddComponent<VPF1Communicator>();
        else if (s.Contains(VPF_2_WEB))
            return obj.AddComponent<VPF2Communicator>();
        else if (s.Contains(VPF_1_LOCAL))
            return obj.AddComponent<VPF1LocalCommunicator>();
        else if (s.Contains(VPF_2_LOCAL)) {
			return obj.AddComponent<VPF2LocalCommunicator>();
        }else if (s.Contains(OFFLINE)) {
            Debug.Log("Offline? " + s);
			return obj.AddComponent<OfflineSimulatorCommunicator>();
		}else if (s.Contains(VPF_1_PTSD)) {
			return obj.AddComponent<PTSDVPF1Communicator>();
		}
        else if (s.Contains(VERG_DEV))
        {
            VPF2Communicator c = obj.AddComponent<VPF2Communicator>();
            c.ServerAddress = "http://vergdev.cise.ufl.edu/";
            return c;

        }else if (s.Contains (VPF_2_SMART))
        {
            VPF2Communicator c = obj.AddComponent<VPF2Communicator>();
            SmartFindURL(c, S3Bucket);

            return c;
        }
        else if (s.Contains(VPF_2_UPD))
        {
            UPDVPF2Communicator c = obj.AddComponent<UPDVPF2Communicator>();
            SmartFindURL(c, S3Bucket);
            return c;
        }
        else
        {
            RenManager man = GameObject.Find("IPSRen").GetComponent<RenManager>();
            man.Debug("VPFType set: " + s + " not found!");
            VPF2Communicator c = obj.AddComponent<VPF2Communicator>();
            SmartFindURL(c, S3Bucket);
            return c;
        }
    }

    private static void SmartFindURL(VPF2Communicator c, string S3Bucket = null)
    {
        if (S3Bucket != null && S3Bucket != "")
        {
            c.ServerAddress = string.Format("http://{0}.s3-website-us-east-1.amazonaws.com/", S3Bucket);
        }
        else {

            string url = "";
            if (Application.isWebPlayer)
            {
                url = Application.absoluteURL;
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                url = Application.dataPath;
            }

            //If we have a url, we extract the server address from it.
            if (url != "")
            {
                //take the URL up to the first / after http://
                int i = 7;
                if (url.StartsWith("https"))
                {
                    i = 8;
                }

                url = url.Substring(0, url.IndexOf('/', i) + 1);
                c.ServerAddress = url;
            }
        }
    }
    #endregion

    #region ServerLocation
    /// <summary>
    /// Address to the actual server.
    /// </summary>
    public string ServerAddress = "";
    /// <summary>
    /// Path to the AssetBundles.
    /// </summary>
    public string AssetBundlePath = "";

    /// <summary>
    /// Method that gets the URL to the AssetBundle.
    /// </summary>
    /// <returns>URL to the AssetBundles.</returns>
    public virtual string GetAssetBundleURL()
    {
        return ServerAddress + AssetBundlePath;
    }
    #endregion

    #region Script/Transcript Book-keeping
        /// <summary>
    /// Id of the character.
    /// </summary>
    public int CharacterID = -1;
	
	/// <summary>
    /// Id of the script.
    /// </summary>
    public int ScriptId = -1;

    /// <summary>
    /// 
    /// </summary>
    public int TranscriptId = -1;
    /// <summary>
    /// 
    /// </summary>
    public int ActId = -1;

    /// <summary>
    /// 
    /// </summary>
    public string UserName = DEFAULT_USERNAME;

    /// <summary>
    /// Defines if the interaction should be logged.
    /// </summary>
    public bool LogInteraction = true;

    /// <summary>
    /// Stores the last input by the user.
    /// </summary>
    protected string LastUserInput;

    /// <summary>
    /// List of TimeTriggers
    /// </summary>
    protected List<TimeTrigger> TimeTriggers;

    /// <summary>
    /// This is the time the interaction has been running. Pauses such as RL are not counted.
    /// </summary>
    protected float ActiveInteractionTime;

    /// <summary>
    /// This is the total time including pauses.
    /// </summary>
    protected float TotalInteractionTime;
    
    #endregion

    #region Loading Book-keeping and Functions

    public int LoadingEventsExpected;
    public int LoadingEventsReceived;

    public override float GetLoadingProgress()
    {
        if (LoadingEventsExpected > 0)
        { //prevent division by zero
            float progress =  (float)LoadingEventsReceived / (float)LoadingEventsExpected;
            HasFinishedLoading = (progress >= 1.0f);
            return progress;
        }
        else
        {
            HasFinishedLoading = true;
            return 1.0f;
        }
    }

    #endregion

    #region Virtual Human protected references
    /// <summary>
    /// Reference to the VHSoundManager. 
    /// </summary>
    protected VHSoundManager SoundManager;

    /// <summary>
    /// Reference to the AnimationManager
    /// </summary>
    protected VHAnimationManager AnimationManager;
	
    /// <summary>
    /// Reference to the HeadLookBehavior
    /// </summary>
	protected HeadLookBehaviour HeadLook;
	
    #endregion

    #region GUIElements
    /// <summary>
    /// Determines if Unity will control input and transcript display.
    /// In some cases you might not want that and have external (HTML/javascript for example)
    /// control of that.
    /// </summary>
    public bool UseUnityGUI = true;

    /// <summary>
    /// Input Box.
    /// </summary>
    public RenInputBox InputBox;


    /// <summary>
    /// Error Box
    /// </summary>
    public RenWindow ErrorBox;

    #region Transcript
    /// <summary>
    /// Transcript view.
    /// </summary>
    public RenTranscriptBox Transcript;

    public string UserTranscriptName = "You";
    public string VHTranscriptName = "Patient";
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public RenSpeechOptions SpeechOptions;

    /// <summary>
    /// 
    /// </summary>
    public GUISkin UserSkin = null;
    /// <summary>
    /// 
    /// </summary>
    public GUISkin VHSkin = null;
	 /// <summary>

    /// <summary>
    /// 
    /// </summary>
    public GUISkin ErrorSkin = null;
    #endregion

    #region Scrip Event Related (RL & Time Triggers)
    /// <summary>
    /// This defines if unity will be the one handling the reflective learning display.
    /// </summary>
    public bool UseUnityRelfectiveLearning;

    /// <summary>
    /// This defines if unity wil be the ne handling time triggers.
    /// </summary>
    public bool UseUnityTimeTriggers;


    /// <summary>
    /// Reference to the Reflective Learning panel if any.
    /// </summary>
    public ReflectiveLearningPanel RLPanel;

    #endregion

    #region MonoBehaviour Methods

    protected override void Update()
    {
        base.Update();
        TotalInteractionTime += Time.deltaTime;
        if (!isPaused)
        {
            ActiveInteractionTime += Time.deltaTime;

            foreach (TimeTrigger tt in TimeTriggers)
            {
                if(tt.WillTrigger(ActiveInteractionTime) )
                {
					tt.Triggered();
                    TriggerTimeTrigger(tt);
                }
            }

            bool HasConnectionProblem = false;
            foreach (ServerConnection w in ActiveConnections)
            {//NO REMOVE OR ADD IN THIS FOR LOOP 
                //ActiveConnections is threadsafelist.
                if (w.HasWaitedMoreThan(MaxTimeAllowedWithNoError))
                {
                    
                    ErrorBox.Text = LateServerResponseErrorMessage + "\n" + w.GetElapsedTime().ToString("n2") + " seconds with no response.";
                    HasConnectionProblem = true;
                }
            }
            ErrorBox.ShouldRender = HasConnectionProblem;
        }
    }

    /// <summary>
    /// Function executed at Start. If the ServerAddress and AssetBundlePath
    /// have not being set, they are set to the default values.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        Initialize();
    }
    #endregion

    #region Initialization

    public void RegisterAsLoader()
    {
        InitProtocol(); //Make sure we have reference to IPS
        (IPS.GetComponent<RenLoader>() as RenLoader).RegisterAsLoader(this);

        LoadingEventsExpected = 0;
        LoadingEventsReceived = 0;
    }

    public virtual void Initialize()
    {
        Initialize(true);
    }

    public virtual void Initialize(bool log)
    {
        RegisterAsLoader();

        ActiveConnections = new ThreadSafeList<ServerConnection>();

        if (AssetBundlePath == "")
        {
            AssetBundlePath = DefaultAssetBundlePath;
        }

        if (ServerAddress == "")
        {
            ServerAddress = DefaultServerAddress;
        }

        LogInteraction = log;


        if (LogInteraction)
        {
            LoadingEventsExpected++;

            GetTranscriptId();
        }

        if (UseUnityGUI)
        {
			if(UserSkin == null) {
            	UserSkin = Resources.Load("Skins/UserSkin") as GUISkin;
			}
			if(VHSkin == null) {	
            	VHSkin = Resources.Load("Skins/VHSkin") as GUISkin;
			}
			
            CreateGUI();
        }

        CreateErrorBox();


        if (UseUnityRelfectiveLearning)
        {
            //The RLPanel should ALWAYS be attached to the camera!
            RLPanel = Camera.main.GetComponent<ReflectiveLearningPanel>();
            RLPanel.AnswerSubmittedEvent += new ReflectiveLearningEvent(ReflectiveLearning_AnswerSubmittedEvent);
        }

        TimeTriggers = new List<TimeTrigger>();
        GetScriptEvents(UseUnityRelfectiveLearning, UseUnityTimeTriggers);

        //get the references to the soundmanager and the animationmanagers.
        SoundManager = gameObject.GetComponent<VHSoundManager>();
        AnimationManager = gameObject.GetComponent<VHAnimationManager>();
		HeadLook = gameObject.GetComponent<HeadLookBehaviour>();

        //Reset time of the interaction
        TotalInteractionTime = ActiveInteractionTime = 0.0f;
    }    

    /// <summary>
    /// Method to create the basic GUI.
    /// </summary>
    protected virtual void CreateGUI()
    {
        //InputBox
        InputBox = new RenInputBox();
        InputBox.EnterPressed += new RenInputBoxEnterEvent(InputBox_EnterPressed);
        //InputBox.Position = new Rect(180, 555, 600, 40);
        InputBox.Position = new Rect(20, 420, 400, 40);


        AddGUIElement(InputBox, true);

        //Transcript
        Transcript = new RenTranscriptBox();
        //Transcript.ScrollRect = new Rect(180, 390, 580, 160);
        //Transcript.BoxRect = new Rect(180, 390, 600, 160);
        //Transcript.Position = new Rect(0, 0, 600, 160);
        Transcript.ScrollRect = new Rect(20, 250, 390, 160);
        Transcript.BoxRect = new Rect(20, 250, 400, 160);
        Transcript.Position = new Rect(20, 250, 400, 160);
        
        
		// Commenting out the following two lines because LineHeight and LineLength are public variables that should not be set in code
		// If you want to set the length and width of the line then please do so on the TranscriptBox component of your communicator. 
		//Transcript.LineHeight = 24;
        //Transcript.LineLength = 162;
        
		
		Transcript.AddNewTranscript(UserTranscriptName, UserSkin);
        Transcript.AddNewTranscript(VHTranscriptName, VHSkin);

        AddGUIElement(Transcript, true);

        //SpeechOptions
        SpeechOptions = new RenSpeechOptions();
        SpeechOptions.ShouldRender = false;
        SpeechOptions.Skin = UserSkin;
        SpeechOptions.Position = new Rect(440, 385, 0, 0); //width and heigh are ignored
        SpeechOptions.PositionSpecifiedAsLowerLeftCorner = true;
        SpeechOptions.OptionSize = new Rect(0, 0, 600, 25);

        SpeechOptions.OptionSelectedEvent += new SpeechOptionSelected(HandleSpeechOptionSelected);

        AddGUIElement(SpeechOptions, true);
    }

    /// <summary>
    /// Sets up the Error Box.
    /// </summary>
    protected virtual void CreateErrorBox()
    {
        //The Error box should always exist. Let's us know if there are issues connecting to 
        //the server.
        ErrorSkin = Resources.Load("Skins/ErrorSkin") as GUISkin;
        ErrorBox = new RenWindow();
        ErrorBox.Position = new Rect(695, 200, 200, 200);
        ErrorBox.ShouldRender = false;
        ErrorBox.TintColor = Color.red;
        ErrorBox.UseTint = true;
        ErrorBox.Skin = ErrorSkin;
        ErrorBox.Title = "Error";

        AddGUIElement(ErrorBox);
    }


    #endregion

    #region GUI related functions
    /// <summary>
    /// Callback on enter in the input box.
    /// </summary>
    /// <param name="box"></param>
    /// <param name="args"></param>
    protected void InputBox_EnterPressed(RenInputBox box, RenInputBoxEventArgs args)
    {
        LastUserInput = box.text;
        FindResponse(box.text);
        box.text = "";
    }

    protected void AddTranscriptEntry(string user, string text)
    {
        if (Transcript != null)
            Transcript.AddEntry(user, text);
    }

    #region SpeechOptions
    /// <summary>
    /// 
    /// </summary>
    public string VHApologizeOnNoResponse = "Sorry, I don't understand your question. Can you ask it another way?";

    protected virtual void DisplaySpeechOptions(List<SpeechOption> options)
    {
        SpeechOptions.DisplaySpeechOptions(options);
    }

    protected virtual void HandleSpeechOptionSelected(SpeechOption o, SpeechOptionSelectedArgs args)
    {
        {

            SpeechOptions.ShouldRender = false;
           // AddTranscriptEntry(UserTranscriptName, LastUserInput);

            switch (o.Type)
            {
                case SpeechOptionType.SO_ANSWER:
                    HandleAnswerSelected(o, args);
                    break;
                case SpeechOptionType.SO_NOT_ABOVE:
                    HandleSubmitSuggestion(args);
                    AddTranscriptEntry(VHTranscriptName, VHApologizeOnNoResponse);
                    break;
                case SpeechOptionType.SO_STATEMENT:
                    HandleStatement(args);
                    break;
            }

        }
    }

    protected virtual void NoAnswer()
    {
        SpeechOptions.DisplayNoAnswerChoices();
    }


    #endregion

    #endregion

    #region Abstract Methods
    /// <summary>
    /// Get the URL for the audio of this script.
    /// </summary>
    /// <returns>URL for the audio for this script</returns>
    protected abstract string GetAudioURL();
    
    /// <summary>
    /// Find Response method.
    /// </summary>
    /// <param name="userInput"></param>
    public abstract void FindResponse(string userInput);

    /// <summary>
    /// Play audio.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="param">Any object your communicator might need to figure out how to play
    /// the audio, in some cases it would just be the id need to get the phonemes back
    /// from the data base. Just leave it as null if you don't need it.</param>
    public abstract void PlayAudio(string fileName, System.Object param = null);

    /// <summary>
    /// Access the Server to get an id for the new transcript and sets it in the book-keeping.
    /// </summary>
    public abstract void GetTranscriptId();

    /// <summary>
    /// Gets the script specific events. This should include any time triggers, script level misc, and reflective learning prompts
    /// </summary>
    protected abstract void GetScriptEvents(bool UseRL, bool UseTimeTriggers);

    #region SpeechOptions Abstract Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="o"></param>
    /// <param name="args"></param>
    protected abstract void HandleAnswerSelected(SpeechOption o, SpeechOptionSelectedArgs args);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected abstract void HandleSubmitSuggestion(SpeechOptionSelectedArgs args);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected abstract void HandleStatement(SpeechOptionSelectedArgs args);
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tt"></param>
    protected abstract void TriggerTimeTrigger(TimeTrigger tt);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="args"></param>
    protected abstract void ReflectiveLearning_AnswerSubmittedEvent(ReflectiveLearningPrompt prompt, ReflectiveLearningEventArgs args);

    /// <summary>
    /// Play a speech or action by tag/event name
    /// </summary>
    /// <param name="tag"></param>
    public abstract void PlayTaggedAction(string tag);

    #region ServerLocation Defaults
    /// <summary>
    /// Default value for the address. This property should always return the
    /// URL of the VPF Server you are describing.
    /// </summary>
    protected abstract string DefaultServerAddress
    {
        get;
    }
    /// <summary>
    /// Default value for the address to the asset bundles.
    /// </summary>
    protected abstract string DefaultAssetBundlePath
    {
        get;
    }
    #endregion

    #endregion

    #region Web Service Access

    /// <summary>
    /// Stores the active connections.
    /// </summary>
    protected ThreadSafeList<ServerConnection> ActiveConnections;

    /// <summary>
    /// Error to be displayed when the response from the server is 
    /// taking too long.
    /// </summary>
    protected string LateServerResponseErrorMessage = "There seems to be an error connecting to the server. If you see this message please raise your hand";

    /// <summary>
    /// This is the Maximum time in seconds that we will wait
    /// before showing an error message...
    /// </summary>
    protected float MaxTimeAllowedWithNoError = 5.0f; // Maybe 10 is better?

    /// <summary>
    /// Encodes a string in a URL safe form. It's only a call to EscapeURL from Unity.
    /// However it was encapsuled in this function in case you want to do something fancier.
    /// </summary>
    /// <param name="s">String to be encoded</param>
    /// <returns>The encoded string</returns>
    protected string URLEncode(string s)
    {
        return WWW.EscapeURL(s);
    }

    /// <summary>
    /// Converts a <c>Dictionary<string,string></c> into a query string.
    /// Basic format is: <c>key=value&</c> for each key. The ? is not added by this method
    /// </summary>
    /// <param name="parameters">A dictionary with the parameters</param>
    /// <returns>The query string.</returns>
    protected string DictionaryToQueryString(Dictionary<string, string> parameters)
    {
        string result = "";

        foreach(KeyValuePair<string, string> entry in parameters) 
        {
            result += entry.Key + "=" + URLEncode(entry.Value) + "&";
        }

        return result;
    }

    /// <summary>
    /// Given a URL and a dictionary expressing the query parametes, this function returns the appended
    /// URL with the query string.
    /// </summary>
    /// <param name="URL"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    protected string GetURLForQuery(string URL, Dictionary<string, string> parameters)
    {
        return URL + "?" + DictionaryToQueryString(parameters);
    }


    /// <summary>
    /// Nice method to access generic web services. This is ideally called as part of a StartCoroutine to take advantage
    /// of multi-threading.
    /// </summary>
    /// <param name="url">Path to the web service page.s</param>
    /// <param name="parameters">Dictionary with the parameters.</param>
    /// <param name="callback">Function to be called when getting the result back. </param>
    /// <returns></returns>
    protected IEnumerator AccessWebService(string url, Dictionary<string, string> parameters, WebCallback callback)
    {
        
        string query = GetURLForQuery(url, parameters);
        Debug.Log(query);
        WWW www = new WWW(query);

        ServerConnection connection = new ServerConnection(www);
        ActiveConnections.Add(connection);

        yield return www;

        if(callback != null)
            callback(www);

        ActiveConnections.Remove(connection);
    }

    #endregion
	
	#region VirtualHuman Events
	public event VirtualHumanEvent VirtualHumanWillTalk;
	public event VirtualHumanEvent VirtualHumanEndedSpeech;
	
	protected void OnVirtualHumanWillTalk() 
	{
		if(VirtualHumanWillTalk != null) {
			VirtualHumanWillTalk(this.gameObject);			
		}
	}
	
	protected void OnVirtualHumanEdedSpeech()
	{
		if(VirtualHumanEndedSpeech != null) {
			VirtualHumanEndedSpeech(this.gameObject);	
		}
	}

	#endregion
	
}

/// <summary>
/// Basic function description of a callback for a webservice call.
/// </summary>
/// <param name="www"> WWW element containing the results.</param>
public delegate void WebCallback(WWW www);


///<summary>
///Delegate for all virutla human events.
///<param name="VirtualHuman"> Reference to the Virtual Human. </param>
///</summary>
public delegate void VirtualHumanEvent(GameObject VirtualHHuman);
