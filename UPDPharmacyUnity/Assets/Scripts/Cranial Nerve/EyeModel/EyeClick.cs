using UnityEngine;
using System.Collections;

public class EyeClick : RenBehaviour {
	
	
	public Transform eyelidOutNerve;
	
	public Transform eyelidNode;
	public Transform handRenderNode;
		
	public void OnMouseUp() {
		
		Debug.Log ("Guangyan's code sucks, figure out how to do correctly!!!");
		
		/**
		// Figure out what the active tool is
		// perform that action on the current eye
		
		DiagnosisTools otherScript = (DiagnosisTools)UnityEngine.GameObject.Find("Model").GetComponent("DiagnosisTools"); 
		
		switch(otherScript.selected) {
		case 0:
				// flashlight
			break;
		case 1:
				// pull up on eyelid
			
				handRenderNode.renderer.enabled = !handRenderNode.renderer.enabled;
				
				MonoBehaviour eyelidScript = (MonoBehaviour)eyelidNode.GetComponent("EyelidControl"); 
				eyelidScript.enabled = ! handRenderNode.renderer.enabled;
				eyelidOutNerve.localPosition = new Vector3(1, 0, 0);
				((EyelidControl)eyelidScript).Update();

			break;
			
		}
		
		**/

	}
	
}
