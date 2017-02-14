//#undef UNITY_EDITOR //Used to check the online code... :S annoying but VS is really annoying on that.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VHLoader : AbstractRenLoader
{
    #region VH Descriptors
    /// <summary>
    /// Mesh to be loaded for this character.
    /// </summary>
    public RenAssetLocationDescriptor MeshDescriptor;
    /// <summary>
    /// Array with the descriptors of the animations for this character.
    /// </summary>
    public RenAssetLocationDescriptor[] AnimationDescriptors;

    /// <summary>
    /// Material to be loaded for this character
    /// </summary>
    public RenAssetLocationDescriptor MaterialDescriptor;

    #endregion

    #region Protected Fields and References
    /// <summary>
    /// This is used to store all the renderers by name.
    /// </summary>
    protected Dictionary<string, SkinnedMeshRenderer> renderers;

    /// <summary>
    /// Instance of the AssetLoader
    /// </summary>
    protected RenAssetLoader Loader;
    /// <summary>
    /// Instance of the ParameterParser.
    /// </summary>
    protected RenParameterParser Parser;

    /// <summary>
    /// Instance to this VirtualHuman Abstract Communicator
    /// </summary>
    protected AbstractVPFCommunicator Comm;

    #endregion

    #region Default Parameter Values
    /// <summary>
    /// 
    /// </summary>
    public int DefaultScriptId = 76;

	/// <summary>
	/// The name of the batch argument containing the character ID. This needs to be set when 
	/// multiple characters are loaded into the same scene.
	/// </summary>
	public string BatchIdArgName = "characterID";

    /// <summary>
    /// Default VPF communicator to use.
    /// </summary>
    public string DefaultVPFCommunicator = AbstractVPFCommunicator.VPF_1_LOCAL;

    /// <summary>
    /// Default UserName for the transcript.
    /// </summary>
    public string DefaultUserName = AbstractVPFCommunicator.DEFAULT_USERNAME;

    /// <summary>
    /// Default in case the use_gui parameter is not present.
    /// </summary>
    public bool DefaultUseUnityGUI = true;

    /// <summary>
    /// Default in case use_rl parameter is not present.
    /// </summary>
    public bool DefaultUseReflectiveLearning = true;

    /// <summary>
    /// Default in case use_timetriggers parameter is not present
    /// </summary>
    public bool DefaultUseTimeTriggers = true;

    /// <summary>
    /// Default S3Bucket, "" and null disable use of S3Bucket and leave VPF2 as source for audio.
    /// </summary>
    public string DefaultS3Bucket = "";

	public GUISkin DefaultUserGUISkin = null;
	public GUISkin DefaultVHGUISkin = null;


    #endregion

    #region MonoBehaviour functions
    protected override void Awake()
    {
        base.Awake();
        //Hide while we load!
        //renderer.enabled = false;
    }
    
    // Use this for initialization
    protected override void Start () {
        base.Start(); //loads IPS
        Loader = IPS.GetComponent<RenAssetLoader>(); //get an instance of the Loader
        Comm = gameObject.GetComponent<AbstractVPFCommunicator>();

        //Computes the dictionary of renderers.
        GetRenderers();

        //Add listeners
        Loader.AssetLoaded += new AssetLoadedEventHandler(AssetLoadedEvent);
        Loader.AssetLoadingError += new AssetLoadingErrorEventHandler(AssetLoaderErrorEvent);
        
#if UNITY_EDITOR
        //Load the Virtual Human only on the UnityEditor.
        //On Web, we want the parameters to fill the descriptors first.
        LoadVPFCommunicator();
        LoadVH();
#else
        //Subscribe to the loadingEvent.
        Parser = IPS.GetComponent<RenParameterParser>();
        if (Parser.HasParsedParameters())
        {
            ApplyParameters(Parser, new RenParametersParsedArgs());
        }
        else
        {
            Parser.ParametersParsed += new RenParametersParsedEventHandler(ApplyParameters);
        }
#endif
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Constructs a dictionary with all the renderers for this virtual
    /// human.
    /// </summary>
    protected void GetRenderers()
    {
        renderers = new Dictionary<string, SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] smrs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in smrs)
        {
            renderers[smr.name] = smr;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parser"></param>
    /// <param name="args"></param>
    protected void ApplyParameters(RenParameterParser parser, RenParametersParsedArgs args)
    {
        LoadVPFCommunicator(parser);
		LoadAppActions(parser);

        MeshDescriptor.URL = GetAssetBundleURL(RenParameterParser.MESH_BUNDLE, parser);
        MeshDescriptor.Version = 1;

        RenAssetLocationDescriptor desc = new RenAssetLocationDescriptor(
            GetAssetBundleURL(RenParameterParser.ANIMATION_BUNDLE, parser), 1);
		if(desc.URL != "") {
			AnimationDescriptors = new RenAssetLocationDescriptor[1];
        	AnimationDescriptors[0] = desc;
		}


        LoadVH();
    }

    #endregion

    #region VPF Communicator
    protected void LoadVPFCommunicator()
    {
        if (Comm == null)
        {
            Comm = AbstractVPFCommunicator.AddCommunicator(DefaultVPFCommunicator, gameObject,DefaultS3Bucket);
            Comm.ScriptId = DefaultScriptId;
            Comm.UserName = DefaultUserName;
            Comm.UseUnityGUI = DefaultUseUnityGUI;
            Comm.UseUnityRelfectiveLearning = DefaultUseReflectiveLearning;
            Comm.UseUnityTimeTriggers = DefaultUseTimeTriggers;
			
			Comm.UserSkin = DefaultUserGUISkin;
			Comm.VHSkin = DefaultVHGUISkin;
			

            //This get called in time for Start to hit the init.
            //Comm.Initialize();
        }
    }


    protected void LoadVPFCommunicator(RenParameterParser parser)
    {
        if (Comm == null)
        {
            Comm = AbstractVPFCommunicator.AddCommunicator(
                parser.GetParameter(RenParameterParser.COMMUNICATOR, DefaultVPFCommunicator), gameObject,
                parser.GetParameter(RenParameterParser.S3_BUCKET, DefaultS3Bucket));

            Comm.ScriptId = parser.GetParameterAsInt(RenParameterParser.SCRIPT_ID, DefaultScriptId);
            Comm.UserName = parser.GetParameter(RenParameterParser.USER_NAME, DefaultUserName);
            Comm.UseUnityGUI = parser.GetParameterAsBool(RenParameterParser.USE_GUI, DefaultUseUnityGUI);
            Comm.UseUnityRelfectiveLearning = parser.GetParameterAsBool(RenParameterParser.USE_REFLECTIVE_LEARNING, DefaultUseReflectiveLearning);
            Comm.UseUnityTimeTriggers = parser.GetParameterAsBool(RenParameterParser.USE_TIME_TRIGGERS, DefaultUseTimeTriggers);
			
			Comm.UserSkin = DefaultUserGUISkin;
			Comm.VHSkin = DefaultVHGUISkin;

			if (Comm is OfflineSimulatorCommunicator)
			{
				(Comm as OfflineSimulatorCommunicator).BatchIdArgName = BatchIdArgName;
			}

            
            //This line sucks. I'm happy to comment it out. bye bye            
            //Comm.Initialize();
        }
    }

    #endregion
	
	#region AppActionHandlers
	protected void LoadAppActions(RenParameterParser parser) 
	{
		
		
	}
	#endregion
	
    #region Convinience Functions

    /// <summary>
    /// Checks if a descriptor is in the animationDescriptors list.
    /// </summary>
    /// <param name="desc">Descriptor to look for</param>
    /// <returns><c>true</c> if the descriptor is in the list, <c>false</c> if not.</returns>
    protected bool IsInAnimations(RenAssetLocationDescriptor desc)
    {
        foreach (RenAssetLocationDescriptor anim in AnimationDescriptors)
        {
            if (anim == desc)
                return true;
        }
        
        return false;
    }

    protected string GetAssetBundleURL(string bundle, RenParameterParser parser)
    {
		string bundleName = parser.GetParameter(bundle, "");
		if(bundleName != "") {
			return Comm.GetAssetBundleURL() + bundleName + ".unity3d";
		}else{
			return "";
		}
    }

    #endregion

    #region AbstractRenLoader function implementations
    #region Loading Book-keeping
    protected bool MeshLoaded = false;
    protected int AnimationsLoaded = 0;
    protected bool MaterialLoaded = false;
    #endregion

    public override float GetLoadingProgress()
    {
		int ExpectedLoad = AnimationDescriptors.Length;
		if(MeshDescriptor.URL != "") ExpectedLoad++;
		if(MaterialDescriptor.URL != "") ExpectedLoad++;
		
		if(ExpectedLoad != 0) {		
	        float loaded = (float)AnimationsLoaded;
	        if (MeshLoaded) loaded++;
	        if (MaterialLoaded) loaded++;
			
	        return loaded / (float)(ExpectedLoad);
		}else {
			return 1.0f;	
		}
    }

    #endregion

    #region AssetLoaded Callbacks



    /// <summary>
    /// Listener for the AssetLoaded Event.
    /// </summary>
    /// <param name="args">Arguments of the event.</param>
    protected void AssetLoadedEvent(RenAssetLoaderEventArgs args)
    {
        //If an AssetLoadedEvent happens, we need to make sure it's for this VH
        if (MeshDescriptor == args.Descriptor)
        {
            ApplyMesh(args);
        }
        else if(IsInAnimations(args.Descriptor))
        {
            ApplyAnimations(args);
        }
        else if (MaterialDescriptor == args.Descriptor)
        {
            ApplyMaterial(args);
        }
    }

    /// <summary>
    /// This is an ugly way of applying the material. However it does work.
    /// In general what this function does, is look for all the meshes in the 
    /// virtual human and checks which ones have the material that is not
    /// related to the eyes. The loaded Material gets applied to all those meshes.
    /// </summary>
    /// <param name="args"></param>
    private void ApplyMaterial(RenAssetLoaderEventArgs args)
    {
        Object[] array;
        if (args.MaterialsArray != null)
        {
            array = args.MaterialsArray;
        }
        else
        {
            array = args.Array;
        }


        if (array != null && array.Length > 0)
        {
            Material mat = null;
            foreach(Material m in array) {
                if (!m.name.ToLower().Contains("eye"))
                {
                    mat = m;
                    break;
                }
            }
            
            foreach (SkinnedMeshRenderer smr in renderers.Values)
            {
                if (!smr.sharedMaterial.name.ToLower().Contains("eye"))
                {
                    smr.sharedMaterial = mat;
                }
            }
        }
        else
        {
           AddDebugLine("No Array or array is empty for loaded Material with descriptor: " + args.Descriptor);
        }

        MaterialLoaded = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    private void ApplyAnimations(RenAssetLoaderEventArgs args)
    {
        if (args.Array != null && args.Array.Length > 0)
        {
            //TODO: Make sure if there is multiple VH with same
            //Animatios this doesn't trigger twice....
            AnimationsLoaded++;
            VHAnimationManager aniManager = gameObject.GetComponent<VHAnimationManager>();
            if (aniManager != null)
            {

                foreach (Animation anim in args.Array)
                {
                    foreach (AnimationState state in anim)
                    {
                        aniManager.AddClip(anim.GetClip(state.name), state.name);
                    }
                    if (anim.clip != null)
                    {
                        aniManager.SetBaseClip(anim.clip, true);
                    }
                }
            }
            else
            {
                AddDebugLine("No animation manager on VH this is highly not recommended.");
                foreach (Animation anim in args.Array)
                {
                    foreach (AnimationState state in anim)
                    {
                        gameObject.GetComponent<Animation>().AddClip(anim.GetClip(state.name), state.name);
                    }
                    if (anim.clip != null)
                    {
                        gameObject.GetComponent<Animation>().clip = anim.clip;
                    }

                }
            }
        }
        else
        {
            AddDebugLine("No array or array empty for loaded animations with descriptor: " + args.Descriptor);
        }

    }

    private void ApplyMesh(RenAssetLoaderEventArgs args)
    {
        if (args.Array != null && args.Array.Length > 0)
        {
            //AddDebugLine("Array of meshes has: " + args.Array.Length);

            SkinnedMeshRenderer smr;
            
            foreach (Mesh m in args.Array)
            {
				try{
	                smr = renderers[m.name];
    	            smr.sharedMesh = m;
				}catch(KeyNotFoundException)
				{
					//Do nothing... 
				}
            }

#if !UNITY_EDITOR
            ApplyMaterial(args);
#endif

        }
        else
        {
            AddDebugLine("No Array or array empty for loaded meshes with descriptor: " + args.Descriptor);
        }
        MeshLoaded = true;
    }

    protected void AssetLoaderErrorEvent(RenAssetLoaderEventArgs args)
    {
        //If an AssetLoaderErrorEvent happens, we need to make sure it's for this VH
        if (MeshDescriptor == args.Descriptor)
        {
            AddDebugLine("Unable to load Mesh for VH: " + args.Descriptor + " returned with error: " + args.Error);
        }
        else if(IsInAnimations(args.Descriptor))
        {
            AddDebugLine("Unable to load Animations for VH: " + args.Descriptor + " returned with error: " + args.Error);
        }
        else if (MaterialDescriptor == args.Descriptor)
        {
            AddDebugLine("Unable to load Materials for VH: " + args.Descriptor + " returned with error: " + args.Error); 
        }
    }

    #endregion

    #region Loading Calls
    /// <summary>
    /// Loads the virtual human.
    /// </summary>
    public void LoadVH()
    {
        LoadMesh(); 
        LoadAnimations();

#if UNITY_EDITOR
        //Material is loaded with the mesh in WebPlayer.
        LoadMaterial();
#endif
    }
	
	public override string ToString() 
	{
		string anims = "LoadedAnims: " + AnimationsLoaded; 
		
		foreach(RenAssetLocationDescriptor desc in AnimationDescriptors) 
		{
			anims += "\n[" + desc.URL + "]";
		}
		
		return "VHLoader [\n Mesh = \"" + MeshDescriptor.URL + "\" and has loaded: " + MeshLoaded + "\n Animations= [" + anims + "]\nMaterial = \"" + MaterialDescriptor.URL + "\" and has loaded: " + MaterialLoaded + " ]";
		
		
	}

    /// <summary>
    /// 
    /// </summary>
    protected void LoadMesh()
    {
		if(MeshDescriptor.URL != "")
        	Loader.LoadAssets<UnityEngine.Mesh>(MeshDescriptor);
		else
			MeshLoaded = true;
    }

    /// <summary>
    /// 
    /// </summary>
    protected void LoadAnimations()
    {
        foreach (RenAssetLocationDescriptor desc in AnimationDescriptors)
        {
			if(desc.URL != "")
            	Loader.LoadAssets<UnityEngine.Animation>(desc);
			else
				AnimationsLoaded++;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected void LoadMaterial()
    {
		if(MaterialDescriptor.URL !=  "" && MaterialDescriptor.URL != null)
        	Loader.LoadAssets<UnityEngine.Material>(MaterialDescriptor);
		else
			MaterialLoaded = true;
    }

    #endregion
}
