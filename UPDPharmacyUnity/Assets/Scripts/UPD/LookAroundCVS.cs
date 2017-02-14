using UnityEngine;
using System.Collections;

public class LookAroundCVS : MonoBehaviour {

    public float angle = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftArrow))  { 	
	    	transform.Rotate(Vector3.down, angle * Time.deltaTime);
	    	Debug.Log("Turning left");
	    }
	   
	    else if (Input.GetKey(KeyCode.RightArrow)) {
	    	transform.Rotate(Vector3.up, angle * Time.deltaTime);
	    	Debug.Log("Turning right");
	    }



	}
}
