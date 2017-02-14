using UnityEngine;
using System.Collections;

public class Stethoscope : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;
	public bool showBox = false;
	public int count = 0;
	private bool dragging = false;
	public GameObject steth;
	public GameObject steth2;
	public Camera mainCam;
	public Transform steth1Move;
	public Transform steth2Move;
	public GameObject VirtualHuman;

	
	void OnGUI() {
	
		if(showBox) {
			//print ("runing");
			if(GUI.Button (new Rect(400,200,200,80), "You used the stethoscope")) {
				showBox = false;
				steth2.SetActive(false);
				steth.SetActive(true);
				Cursor.visible = true;
				dragging = false;
				print (showBox);
				//return object to original position 
				startingPosition();
			}
		}
	}
	
	void defaultSteth() {
		steth2.SetActive(false);
		steth.SetActive(true);
	}
	
	void OnMouseEnter() {
		if(!showBox) {
			steth2.SetActive(true);
			steth.SetActive(false);
		}
	}  
	
	void OnMouseExit() {
		if(!showBox) {
			if(!dragging) {
				defaultSteth ();
			}
		}
	}
	
	void OnMouseDown() {
		dragging = true;
		//steth2.SetActiveRecursively(true);
		//steth.SetActiveRecursively(false);
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		Cursor.visible = false;
	}
	
	void OnMouseDrag() {
		//steth.SetActiveRecursively(false);
		//steth2.SetActiveRecursively(true);
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
	}
	
	void OnMouseUp() {
		Cursor.visible = true;
		dragging = false;
		//lying down chest recognition:
		if (VirtualHuman.activeSelf) {
			print ("human Lying Down.");
			if(steth2.transform.position.x <= 32 && steth2.transform.position.x >= 28) {
				if(steth2.transform.position.y <= 159 && steth2.transform.position.y >= 157) {
					if(steth2.transform.position.z <= 538 && steth2.transform.position.z >= 534) {
						showBox = true;
						
					}
				}
			}
		}
		//sitting up
		if(!VirtualHuman.activeSelf) {
			print ("human Sitting up.");
			if(steth2.transform.position.x >= 30 && steth2.transform.position.x <= 35) {
				if(steth2.transform.position.y >= 156 && steth2.transform.position.y <= 161) {
					if(steth2.transform.position.z >= 532 && steth2.transform.position.z <= 535) {
						showBox = true;
						print (showBox);
					}
				}
			}
		}
		//not placed on chest
		if(!showBox) {
			steth2.SetActiveRecursively(false);
			steth.SetActiveRecursively(true);
			print (showBox);
			//return object to original position 
			startingPosition();
		}
	}
	
	
	void startingPosition() {
		steth1Move.transform.position = new Vector3(19.7815F, 151.561F, 541.967F);
		steth2Move.transform.position = new Vector3(20.28314F, 152.4271F, 547.3314F);
	}
	

}
