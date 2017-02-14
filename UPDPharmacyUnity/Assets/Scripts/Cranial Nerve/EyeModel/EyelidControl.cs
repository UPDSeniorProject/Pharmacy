using UnityEngine;
using System.Collections;

public class EyelidControl : MonoBehaviour {
	public Transform eyelidValue;
	public bool useRotation;
	public bool negate; // don't ask me man, ask the idiots who create these models.
	
	public float value;
	public float upperRange;
	public float lowerRange;
    public float initialPos;

    public void Start()
    {
        initialPos = upperRange;

    }


	public void LateUpdate () {
		value = eyelidValue.localPosition.x * (upperRange - lowerRange) + lowerRange;
		value = Mathf.Max(lowerRange, value);
		value = Mathf.Min(upperRange, value);
	
		if (negate)
			value = - value;
	
		if (useRotation)
			transform.localEulerAngles = new Vector3(value ,transform.localEulerAngles.y, transform.localEulerAngles.z);	
		else 
			transform.localPosition = new Vector3(value, transform.localPosition.y,transform.localPosition.z); 
	}
}
