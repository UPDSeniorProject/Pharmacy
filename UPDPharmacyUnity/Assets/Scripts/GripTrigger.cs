using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class GripTrigger : MonoBehaviour {
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device dev;

    // Use this for initialization
    void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	void Update () {
       dev  = SteamVR_Controller.Input((int)trackedObj.index);
        if (dev.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Grip Registered");
        }
	}
     void OnTriggerStay(Collider col)
    {
        Debug.Log("You have collided with" + col.name);
        if (dev.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("You have gripped " + col.name);
            //call EventHandler.Arrest here.
        } 
    }
}
