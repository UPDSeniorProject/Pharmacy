using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public float speed;
	
	// Update is called once per frame
	void Update () {
		float rotateY = Input.GetAxis("Mouse Y") * speed * Time.deltaTime;
		float rotateX = Input.GetAxis("Mouse X") * speed * Time.deltaTime;

		if (transform.localEulerAngles.x > 310 && transform.localEulerAngles.x < 320)
		{
			if(rotateY > 0)
				transform.Rotate(rotateY, 0, 0, Space.Self);
		}
		else if(transform.localEulerAngles.x > 50 && transform.localEulerAngles.x < 60)
		{
			if(rotateY < 0)
				transform.Rotate(rotateY, 0, 0, Space.Self);
		}
		else
			transform.Rotate(rotateY, 0, 0, Space.Self);
		
		transform.Rotate (0, rotateX, 0, Space.World);
	}
}
