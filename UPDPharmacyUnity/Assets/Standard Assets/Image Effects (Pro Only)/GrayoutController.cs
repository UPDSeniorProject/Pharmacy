using UnityEngine;
using System.Collections;

// Apply this component to the Main Camera
// smoothly triggers and removes the grayout state
// tune the rate of the fade-in and fade-out in the property inspector
// call ActivateGrayout () and DeactivateGrayout () methods from other scripts
// Expects the Main Camera to also have a GrayoutEffect script component

public class GrayoutController : MonoBehaviour
{
	
	// SET IN PROPERTY INSPECTOR //----------------------------------------------------------
	public float RampUpSeconds;		// suggest value of 2.50 seconds
	public float RampDownSeconds;	// suggest value of 1.25 seconds
	public bool ShowGUI;
	// SET IN PROPERTY INSPECTOR //----------------------------------------------------------
	
	private bool Grayout = false;
	private GrayoutEffect MyGrayoutEffect;
	private float GrayoutTime;
	

	void Start ()
	{
		MyGrayoutEffect = gameObject.GetComponent<GrayoutEffect> ();
		Grayout = false;
		GrayoutTime = 0;
	}
	
	
	// PUBLIC METHODS //---------------------------------------------------------------------
	public void ActivateGrayout ()
	{
		Grayout = true;	
		GrayoutTime = MyGrayoutEffect.effectAmount * RampUpSeconds;
	}
	
	public void DeactivateGrayout ()
	{
		Grayout = false;
		GrayoutTime = RampDownSeconds - (MyGrayoutEffect.effectAmount * RampDownSeconds);
	}
	// PUBLIC METHODS //---------------------------------------------------------------------
	
	

	void Update ()
	{
		
		if (Grayout) {
			if (MyGrayoutEffect.effectAmount < 1) {
				GrayoutTime = GrayoutTime + Time.deltaTime;
				if (GrayoutTime > RampUpSeconds)
					GrayoutTime = RampUpSeconds;
				MyGrayoutEffect.effectAmount = Mathf.Lerp (0, 1, (GrayoutTime / RampUpSeconds));
			}
		} else {	
			if (MyGrayoutEffect.effectAmount > 0) {
				GrayoutTime = GrayoutTime + Time.deltaTime;
				if (GrayoutTime > RampDownSeconds)
					GrayoutTime = RampDownSeconds;
				MyGrayoutEffect.effectAmount = Mathf.Lerp (1, 0, (GrayoutTime / RampDownSeconds));
			}
		}

	}
	
	
	
	// For Demo Purposes
	void OnGUI ()
	{
		if(ShowGUI) {
			if (Grayout) {
				if (GUI.Button (new Rect (10, 10, 120, 20), "Grayout Off"))
					DeactivateGrayout ();	
			} else {
				if (GUI.Button (new Rect (10, 10, 120, 20), "Grayout On"))
					ActivateGrayout ();
			}
		}
	}
	
}
