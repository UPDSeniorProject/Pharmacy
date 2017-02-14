//#undef UNITY_EDITOR //Used to check the online code... annoying but VS is really annoying on that.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class RenAssetLocationDescriptor : IEquatable<RenAssetLocationDescriptor>
{
    #region Public fields
    /// <summary>
    /// 
    /// </summary>
    public string URL = null;
    /// <summary>
    /// 
    /// </summary>
    public int Version = -1;
    #endregion

    #region Constructor
    public RenAssetLocationDescriptor(string url, int version)
    {
        this.URL = url;
        this.Version = version;
    }
    #endregion

    #region Convinience functions
    public override string ToString()
    {
        return "Asset(URL: " + URL + ", Version: " + Version + ")";
    }
    #endregion

    #region IEquatable<RenAssetLocationDescriptor>

    public bool Equals(RenAssetLocationDescriptor other)
    {
        return this.URL == other.URL && this.Version == other.Version;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(RenAssetLocationDescriptor desc1, RenAssetLocationDescriptor desc2)
    {
        if ((object)desc1 == null || ((object)desc2) == null)
            return System.Object.Equals(desc1, desc2);

        return desc1.Equals(desc2);
    }

    public static bool operator !=(RenAssetLocationDescriptor desc1, RenAssetLocationDescriptor desc2)
    {
        if (desc1 == null || desc2 == null)
            return !System.Object.Equals(desc1, desc2);

        return !(desc1.Equals(desc2));
    }
    #endregion
}

public class RenAssetLoader : AbstractRenLoader
{

    #region Book-keeping fields
    /// <summary>
    /// Saved Loaded Arrays.
    /// </summary>
    protected Dictionary<string, UnityEngine.Object[]> loadedArrays;

    /// <summary>
    /// List of active WWW objects. This is used to compute loading percentage.
    /// </summary>
    protected List<WWW> activeWebAccesses;
    #endregion

    #region Events
    /// <summary>
    /// Event triggered when an AssetBundle is loaded.
    /// </summary>
    public event AssetLoadedEventHandler AssetLoaded;

    /// <summary>
    /// Event triggered when there is an Error loading an AssetBundle
    /// </summary>
    public event AssetLoadingErrorEventHandler AssetLoadingError;
    #endregion

    #region MonoBehaviour Functions
    /// <summary>
    /// Executred on first frame (We need to this on Awake, before a lot
    /// of Objects load assets on Start)
    /// </summary>
    /// 
    protected override void Awake()
    {
        loadedArrays = new Dictionary<string, UnityEngine.Object[]>();
        activeWebAccesses = new List<WWW>();
    }

    protected override void Update()
    {
        base.Update();

    }
    #endregion

    #region Loading Function
    /// <summary>
    /// Loads an Assets.
    /// </summary>
    /// <typeparam name="T">Type of the asset to be loaded. Examples: <c>UnityEngine.Mesh</c>, <c>UnityEngine.Animation</c></typeparam>
    /// <param name="desc"></param>
    public void LoadAssets<T>(RenAssetLocationDescriptor desc) where T : UnityEngine.Object
    {
        StartCoroutine(DownloadAndCache<T>(desc));
    }

    public void LoadAssets<T>(string url, int version) where T : UnityEngine.Object
    {
        LoadAssets<T>(new RenAssetLocationDescriptor(url, version));
    }


    private IEnumerator DownloadAndCache<T>(RenAssetLocationDescriptor desc) where T: UnityEngine.Object
    {

#if UNITY_EDITOR
        string path = desc.URL;
        if (path.EndsWith("/")) path = path.Substring(0, path.Length-1);  
		
		if(path.Trim() != ""){
		
        	loadedArrays[desc.URL] = Resources.LoadAll(path, typeof(T));
        	if (loadedArrays[desc.URL] == null)
        	{
            	AddDebugLine("Something went wrong.. object set to null for: " + desc);
        	}
        	else
        	{
            	OnAssetLoaded(new RenAssetLoaderEventArgs(desc, loadedArrays[desc.URL]));
        	}
		}
        
        yield return null;
#else
        //Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        using (WWW www = WWW.LoadFromCacheOrDownload(desc.URL, desc.Version))
        {
            activeWebAccesses.Add(www);
            yield return www; //wait for download.
            if (www.error != null)
            {
                OnAssetLoadingError(new RenAssetLoaderEventArgs(desc, www.error));
                yield return null;
            }
            else
            {
                AssetBundle assetBundle = www.assetBundle;

                loadedArrays[desc.URL] = assetBundle.LoadAllAssets();
                

                OnAssetLoaded(new RenAssetLoaderEventArgs(desc, loadedArrays[desc.URL]));
                
                assetBundle.Unload(false);
            }

            activeWebAccesses.Remove(www);
        }

#endif
    }

    /// <summary>
    /// Returns progress!
    /// </summary>
    /// <returns></returns>
    public override float GetLoadingProgress()
    {
        if (activeWebAccesses.Count == 0)
            return 1.0f;
        else
        {
            float progress = 0.0f;
            foreach (WWW w in activeWebAccesses)
            {
                progress += w.progress;
            }

            return progress / (float)(activeWebAccesses.Count);
        }
    }
    #endregion

    #region Event Functions
    public virtual void OnAssetLoadingError(RenAssetLoaderEventArgs args)
    {
        if (AssetLoadingError != null)
        {
            AssetLoadingError(args);
        }
        else
        {
            AddDebugLine("WWW download: " + args.Error + " in asset " + args.Descriptor);
        }
    }


    public virtual void OnAssetLoaded(RenAssetLoaderEventArgs args)
    {
        if (AssetLoaded != null)
        {
            AssetLoaded(args);
        }
        else
        {
            AddDebugLine("No handler in loader!!! Remember to add you handler");
        }
    }
    #endregion
}

public class RenAssetLoaderEventArgs : EventArgs
{
    #region Public Fields
    public RenAssetLocationDescriptor Descriptor;

    public UnityEngine.Object[] Array = null;

    public UnityEngine.Object[] MaterialsArray = null;

    public string Error = "";
    #endregion

    #region Constructors
    public RenAssetLoaderEventArgs(RenAssetLocationDescriptor desc, UnityEngine.Object[] arr)
    {
        this.Descriptor = desc;
        this.Array = arr;
    }

    public RenAssetLoaderEventArgs(RenAssetLocationDescriptor desc, UnityEngine.Object[] arr, UnityEngine.Object[] materials)
    {
        this.Descriptor = desc;
        this.Array = arr;
        this.MaterialsArray = materials;
    }

    public RenAssetLoaderEventArgs(RenAssetLocationDescriptor desc, string error)
    {
        this.Descriptor = desc;
        this.Error = error;
    }
    #endregion
}

#region Delegates
public delegate void AssetLoadedEventHandler(RenAssetLoaderEventArgs args);

public delegate void AssetLoadingErrorEventHandler(RenAssetLoaderEventArgs args);

#endregion