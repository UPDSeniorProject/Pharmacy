using UnityEngine;
using System.Collections;

public class OxTank : MonoBehaviour {
	public Camera mainCam;
	public Camera oxCam;
	public Transform myTransform;
	public bool decrease = false;
	public bool increase = false;
	public float speed = 1;
	public float t = 0;
	public float time = 4;
	
	//the ball moves up and down infinitely, need to find a way to limit it!!!!!
	
	void Update() {
		if(mainCam.GetComponent<Camera>().enabled) {
			oxCam.GetComponent<Camera>().enabled = false;
		}
		if(decrease) {
			while (t<1) {
				t += Time.deltaTime / time;
				myTransform.Translate (0, -speed * Time.deltaTime, 0);
			}
		}
		
		if(increase) {
			while (t<1) {
				t += Time.deltaTime / time;
				myTransform.Translate (0, speed * Time.deltaTime, 0);
			}
			
		}	

	}
	
	private void OnMouseDown()
	{
		mainCam.GetComponent<Camera>().enabled = false;
		oxCam.GetComponent<Camera>().enabled = true;
	}
	
	
//	void OnGUI() {
//		//decrease button
//		if(oxCam.camera.enabled) {
//			if(GUI.Button(new Rect(800, 300, 300, 50), "Decrease Oxygen Flow")) {
//				MoveObjDown();
//			}
//		}
//		//increase button
//		if(oxCam.camera.enabled) {
//			if(GUI.Button(new Rect(800, 200, 300, 50), "Increase Oxygen Flow")) {
//				MoveObjUp();
//			}
//		}
//		//done button
//		if(oxCam.camera.enabled) {
//			if(GUI.Button(new Rect(1200, 0, 200, 100), "Done")) {
//				oxCam.camera.enabled = false;
//				mainCam.camera.enabled = true;
//			}
//		}
//	}
	
	
	
	//move pressure ball DOWN
	void MoveObjDown() {
		decrease = true;
	}
	
	//move pressure ball UP
	void MoveObjUp() {
		increase = true;	
	}
	

}

