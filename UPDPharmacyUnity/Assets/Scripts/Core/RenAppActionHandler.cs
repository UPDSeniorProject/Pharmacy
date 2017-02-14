using UnityEngine;
using System.Collections;

public class RenAppActionHandler : RenBehaviour {

    System.Type Type;
    protected AbstractVPFCommunicator _Comm;


    protected override void Start()
    {
        base.Start();
        Type = this.GetType();
 

    }

    protected AbstractVPFCommunicator GetCommunicator()
    {
        if(_Comm == null)
        {
            _Comm = gameObject.GetComponent<AbstractVPFCommunicator>();
        }
        return _Comm;
    }
    

    public void TriggerAppAction(string action)
    {
        try
        {
            System.Reflection.MethodInfo method = Type.GetMethod(action);
            if (method != null)
            {
                method.Invoke(this, null);
            }
            else
            {
                AddDebugLine("Could not find method with name: " + action + " in current AppActionHandler with type: " + Type.Name);
            }
        }
        catch (System.Exception e)
        {
            AddDebugLine("Exception trigger AppAction: " + action + " with message: " + e.Message);
        }
        
    }
	
	
    

}

/// <summary>
/// Basic delegate of the base AppAction.
/// </summary>
public delegate void AppAction();
