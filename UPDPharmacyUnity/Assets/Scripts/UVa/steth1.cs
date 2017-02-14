using UnityEngine;
using System.Collections;

public class steth1 : MonoBehaviour {
	public GameObject steth;
	public GameObject steth2;
	public Camera mainCam;
	
	
	void defaultSteth() {
		steth2.SetActiveRecursively(false);
		steth.SetActiveRecursively(true);
	}
	
	void OnMouseEnter() {
		steth2.SetActiveRecursively(true);
		steth.SetActiveRecursively(false);
	}
	
	void OnMouseExit() {
		defaultSteth();
	}
}
