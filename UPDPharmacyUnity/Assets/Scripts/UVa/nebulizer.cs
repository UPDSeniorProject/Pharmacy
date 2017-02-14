using UnityEngine;
using System.Collections;

public class nebulizer : MonoBehaviour {
	private Vector3 screenPoint;
	private Vector3 offset;
	private bool showBox = false;
	private bool dragging = false;
	private Camera mainCam;
	public GameObject neb1;
	public GameObject neb2;
	public GameObject VirtualHuman;
	public Transform neb1Move;
	public Transform neb2Move;
	
	void defaultNeb() {
		neb1.SetActive(true);
		neb2.SetActive (false);
	}
	
	void OnMouseEnter() {
		if(!showBox) {
			neb1.SetActive (false);
			neb2.SetActive (true);
		}
	}
	
	void OnMouseExit() {
		if(!showBox) {
			if(!dragging) {
				defaultNeb();
			}
		}
	}
	
	void OnMouseDown() {
		dragging = true;
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		Cursor.visible = false;
	}
	
	void OnMouseDrag() {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
	}
	
	void OnMouseUp() {
		dragging = false;
		Cursor.visible = true;
		
		//patient lying down
		if(VirtualHuman.activeSelf) {
			print ("Human Lying Down");
			if(neb2.transform.position.x >= 20 && neb2.transform.position.x <= 28) {
				if(neb2.transform.position.y >=155 && neb2.transform.position.y <=163) {
					if(neb2.transform.position.z >=520 && neb2.transform.position.z <= 525) {
						showBox = true;
					}
				}
			}
		}
		//patient sitting up
		if(!VirtualHuman.activeSelf) {
			if(neb2.transform.position.x >= 30 && neb2.transform.position.x <= 35) {
				if(neb2.transform.position.y >=161 && neb2.transform.position.y <=166) {
					if(neb2.transform.position.z >=513 && neb2.transform.position.z <= 518) {
						showBox = true;
					}
				}
			}
		}
		
		//not placed on head
		if(!showBox) {
			neb2.SetActive (false);
			neb1.SetActive (true);
			startingPos();
		}
	}
	
	void startingPos() {
		neb1.transform.position = new Vector3(16.90788F, 151.9422F, 533.9341F);
		neb2.transform.position = new Vector3(17.87342F, 153.2551F, 534.2739F);
	}
	
	void OnGUI() {
		if(showBox) {
			if(GUI.Button(new Rect(400, 200, 200, 80), "You used the Nebulizer.")) {
				showBox = false;
				neb1.SetActive (true);
				neb2.SetActive (false);
				Cursor.visible = true;
				dragging = false;
				print (showBox);
				startingPos();
			}						
	}
}
}
