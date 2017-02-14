using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RenParameterParser : RenBehaviour {

    /// <summary>
    /// Dictionary used to store the parsed parameters.
    /// </summary>
    private Dictionary<string, string> parameters;

    /// <summary>
    /// Event triggered when the parameters are parsed.
    /// </summary>
    public event RenParametersParsedEventHandler ParametersParsed;

    /// <summary>
    /// Defines if the parser has parsed the parameters already.
    /// </summary>
    private bool hasParsed = false;

    /// <summary>
    /// Check if the Parser has parsed the parameters.
    /// </summary>
    /// <returns>Wether the parser already parsed the parameters.</returns>
    public bool HasParsedParameters()
    {
        return hasParsed;
    }

    /// <summary>
    /// Parses the parameters passed to the simulation. <c>Awake</c> happens before any Start.
    /// </summary>
    /// 
    protected override void Awake()
    {
		InitProtocol();
        parameters = new Dictionary<string, string>();
        //For now we only implement parameters on web
        if(Application.isWebPlayer) {
            //Call Javascript to let it know we are ready for the parameters.
            Application.ExternalCall("IPSRenUnityLoaded", "");
        
        }else if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("This is happening in a WEBGL Player!!");
            Application.ExternalCall("IPSRenUnityLoaded", "");
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			//Manager.Debug(" Windows player" );
            //TODO: add parsing of Windows stand-alone deploy.
            //In order to access the command line arguments you could use this line:
            string[] args = System.Environment.GetCommandLineArgs();
			string key;
			
            for (int i = 1; i < args.Length; i++)
            {
				if(args[i].StartsWith("--")) { //all parameter names should start with --
					key = args[i].Substring(2); //save the key
					if(i+1 < args.Length && !args[i+1].StartsWith("--")){ //if the next parameter is the value.
						parameters[key] = args[i+1];
						i++; //skip next as it was value
					}else {
						parameters[key] = "true";
					}
					Manager.Debug("Parameter[" + key+ " ] = "  + parameters[key] + "  was parsed");
				}else { 
					//Display error
					Manager.Debug("Unrecognized parameter: "  + args[i]);	
				}
            }
			//trigger event
			OnParametersParsed(new RenParametersParsedArgs());
        }
        else {
            //TODO: Do we ned something here?
			//Manager.Debug(" Something else" );
        }
	}

    public void ParametersFromWeb(string param) 
    {
        string[] getParameters = param.Split('&');

        string[] splitted;
        foreach (string s in getParameters)
        {
            if (s.Length <= 0) continue;

            splitted = s.Split('=');
            if (splitted.Length == 2)
            {
                parameters[splitted[0]] = splitted[1];
            }
            else
            {
                AddDebugLine("Invalid Formatting of: " + s);
            }
        }

        //Trigger the event of the parser
        OnParametersParsed(new RenParametersParsedArgs());
        
    }

    /// <summary> Get a parameter. </summary>
    /// <param name="key"> Key you want to look for. </param>
    /// <returns> Value as a string. <c>null</c> if not found. </returns>
    public string GetParameter(string key)
    {
        return GetParameter(key, null);
    }

    /// <summary>
    /// Get a parameter.
    /// </summary>
    /// <param name="key"> Key to look for. </param>
    /// <param name="default_value">Default value to return in case the key doesn't exist.</param>
    /// <returns>The value of the parameter, or <c>default_value</c> if not found.</returns>
    public string GetParameter(string key, string default_value)
    {
        try
        {
            return parameters[key];
        }
        catch (KeyNotFoundException)
        {
            return default_value;
        }
    }

    /// <summary> Funtion to get the parameter parsed as an int. </summary>
    /// <param name="key"> Key that you want to look for. </param>
    /// <returns> Value parsed. -1 if key is not found </returns>
    public int GetParameterAsInt(string key)
    {
        return GetParameterAsInt(key, -1);
    }

    /// <summary>
    /// Function to get the parameter parsed as an int.
    /// </summary>
    /// <param name="key">Key that you want to look for.</param>
    /// <param name="default_value">Default value to return in case of error.</param>
    /// <returns>Value parsed, or <c>default_value</c> if not possible to parse or key not present.</returns>
    public int GetParameterAsInt(string key, int default_value)
    {
        try
        {
            string p = parameters[key];
            int value;

            if (int.TryParse(p, out value))
            {
                return value;
            }
            else
            {
                AddDebugLine("Couldn't parse value: " + parameters[key] + " as integer");
                return default_value;
            }


        }
        catch (KeyNotFoundException)
        {
            return default_value;
        }
    }

    /// <summary>
    /// Method to get a key parsed as a boolean.
    /// </summary>
    /// <param name="key">Key to look for.</param>
    /// <param name="default_value">Default value in case the key is not found.</param>
    /// <returns>The value of the key parsed, or default_value on error/key not found.</returns>
    public bool GetParameterAsBool(string key, bool default_value)
    {
        try
        {
            string p = parameters[key];

            bool value;
            if (bool.TryParse(p, out value))
            {
                return value;
            }
            else
            {
                AddDebugLine("Couldn't parse value: " + parameters[key] + " as boolean");
                return default_value;
            }
        }
        catch (KeyNotFoundException)
        {
            return default_value;
        }
    }

    /// <summary>
    /// Checks if a key exists.
    /// </summary>
    /// <param name="key">Key to be checked.</param>
    /// <returns></returns>
    public bool HasParameter(string key)
    {
        try
        {
            return parameters[key] != null;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public void OnParametersParsed(RenParametersParsedArgs args)
    {
		hasParsed = true;
        if (ParametersParsed != null)
        {
            ParametersParsed(this, args);
        }
        else
        {
            //AddDebugLine("No handlers for the parser. Parameters have been parsed.");
        }
    }

    public const string SCRIPT_ID = "script_id";
    public const string MESH_BUNDLE = "mesh";
    public const string MATERIAL_BUNDLE = "material";
    public const string ANIMATION_BUNDLE = "animations";
    public const string COMMUNICATOR = "comm";
    public const string USER_NAME = "user";
    public const string USE_GUI = "use_gui";
    public const string USE_REFLECTIVE_LEARNING = "use_rl";
    public const string USE_TIME_TRIGGERS = "use_timetriggers";
    public const string APP_ACTIONS = "app_actions";
	public const string SHOW_ANIMATION_GUI = "anim_gui";
	public const string VH_APP_ACTIONS = "vh_app_actions";
    public const string S3_BUCKET = "s3_bucket";
}

/// <summary>
/// 
/// </summary>
public class RenParametersParsedArgs : EventArgs
{
}

/// <summary>
/// 
/// </summary>
/// <param name="parser"></param>
/// <param name="args"></param>
public delegate void RenParametersParsedEventHandler(RenParameterParser parser, RenParametersParsedArgs args);
