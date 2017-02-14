using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
	public float smooth = 3f;		// a public variable to adjust smoothing of camera motion
	Transform standardPos;			// the usual position for the camera, specified by a transform in the game
	Transform lookAtPos;			// the position to move the camera to when using head look
	bool press=true;
	void Start()
	{
		// initialising referencesLookAtPosCamPos
		standardPos = GameObject.Find ("CamPos").transform;
		
		if(GameObject.Find ("LookAtPos")){
			lookAtPos = GameObject.Find ("LookAtPos").transform;
			transform.forward = lookAtPos.forward;
			transform.position = lookAtPos.position;
			transform.parent = lookAtPos;
		}
	}
	
	void FixedUpdate ()
	{
		// if we hold Alt
		//if(Input.GetButtonUp("Fire2") && lookAtPos)
		//{
		/*if(Input.GetButtonUp("Fire2") && lookAtPos)
			press=!press;
		if(press==true){
			// lerp the camera position to the look at position, and lerp its forward direction to match 
			transform.position = lookAtPos.position;
			//transform.forward = lookAtPos.forward;
		}
		else
		{	
			// return the camera to standard position and direction
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;
		}*/
		
	}
}
