using UnityEngine;
using System.Collections;

public class ControllerPayment : MonoBehaviour {


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
	public ButtonType buttontype;
	
	bool mStarted = false;
	bool mHighlighted = false;
	public bool isPayingOrNot = false;
	private Transform myTransform;
	Vector3 startPosition;
	Quaternion startRotation;
	int startDepth;
	public UISprite UI;
	

	private float startYRotation;

	// Use this for initialization
	void Start () {
		myTransform = transform;
		//startPosition = transform.position;
		startRotation = transform.rotation;
		UI = GetComponentInChildren<UISprite>();
		startDepth = UI.depth;  
		startYRotation = transform.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(MainPaymentController.currentlyActiveButton.ToString() == buttontype.ToString())
		{
			UI.depth = 10;
			if(!isPayingOrNot){
				Messenger<string>.Broadcast("FirstButtonHoverOver",buttontype.ToString());
			}
			if(Input.GetButtonDown("A Button")){
				if (enabled && !isPayingOrNot)
				{
					UI.depth = 20;
					myTransform.eulerAngles = new Vector3(0f,90f,0f);
					//WalletWindowManager.Button1_list.Add(myTransform);
					//string t = buttontype.ToString;
					Messenger<Transform, string>.Broadcast("moveButtonToPay", myTransform, buttontype.ToString());
					Messenger<string>.Broadcast("FirstButtonHoverOverEnd", buttontype.ToString());
					
					isPayingOrNot = true;
					//Debug.Log(WalletWindowManager.Button1_list.Count);
				}
			}
			if(Input.GetButtonDown("B Button")){
				if (enabled && isPayingOrNot){
					
					Messenger<Transform, string>.Broadcast("moveButtonBackToWallet", myTransform, buttontype.ToString());
					isPayingOrNot = false;
					myTransform.eulerAngles = new Vector3(0f,startYRotation,0f);
					UI.depth = startDepth;
					//if(WalletWindowManager.Button1_list.Count>0)
					//	WalletWindowManager.Button1_list.Remove(myTransform);
					//Debug.Log(WalletWindowManager.Button1_list.Count);	
				}
			}
		}else{
			if(!isPayingOrNot){
				Messenger<string>.Broadcast("FirstButtonHoverOverEnd",buttontype.ToString());
			}
			UI.depth = startDepth;
		}
	}
}
