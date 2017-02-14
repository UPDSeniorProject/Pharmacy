using UnityEngine;
using System.Collections;

public class PupilControl : RenBehaviour {
	
	public Transform pupilValue;

	private float value;
	
	public float upperRange;
	public float lowerRange;
	public float dilateSpeed;
	public float contractSpeed;
	public float movementExponent;
	public float target;
	public float inTarget;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		if(isPaused) return;
		
		target = pupilValue.localPosition.x;
		inTarget = target;
		target = (target + 0.5f);
		target = (1.0f - target) * (upperRange - lowerRange) + lowerRange;
	
		var diff = value - target;

		if (diff < 0)
    		value = Mathf.Min(value + (dilateSpeed * Time.deltaTime * Mathf.Pow(Mathf.Abs(diff)/(upperRange - lowerRange),movementExponent)) , target);
    	else
    		value = Mathf.Max(value - (contractSpeed * Time.deltaTime * Mathf.Pow((diff/(upperRange - lowerRange)),movementExponent)), target);

	    transform.localScale = new Vector3(value, value, 1);		
	}
}
