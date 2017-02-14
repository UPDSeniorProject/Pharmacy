using UnityEngine;
using System.Collections;

public class MainPaymentController : MonoBehaviour {

	public enum ButtonType {
		button1,							
		button1cent,	
		button1dime,
		button1nickel,
		button1quarter,
		button5,
		button10,
		button20
	}
	public static ButtonType currentlyActiveButton;

	// Use this for initialization
	void Start () {
		currentlyActiveButton = ButtonType.button20;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Vertical") > 0.7){
			//MainPaymentController.currentlyActiveButton = ButtonType.button5;
			Debug.Log ("Moving on.....");
		}
	}
}
