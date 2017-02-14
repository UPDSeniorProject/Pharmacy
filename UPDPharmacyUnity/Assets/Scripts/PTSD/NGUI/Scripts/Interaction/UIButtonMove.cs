//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using AnimationOrTween;
using System.Collections.Generic;

/// <summary>
/// Attaching this to an object lets you activate tweener components on other objects.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Tween")]
public class UIButtonMove : MonoBehaviour
{
	public Trigger trigger = Trigger.OnHover;
	
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

	private List<Vector3> route = new List<Vector3>();
	private bool aButtonDelay = false;
	private WalletWindowManager Wallet;

	void Start () { mStarted = true;  
	
		myTransform = transform;
		//startPosition = transform.position;
		startRotation = transform.rotation;
		UI = GetComponentInChildren<UISprite>();
		startDepth = UI.depth;  
		startYRotation = transform.eulerAngles.y;
		Wallet = GameObject.Find("Wallet Window").GetComponent<WalletWindowManager>();
	}

	void  Awake () {
		UI = GetComponentInChildren<UISprite>();
		startDepth = UI.depth;  
		Debug.Log(startDepth);
	}

	void OnEnable () {
		//if (mStarted && mHighlighted) OnHover(UICamera.IsHighlighted(gameObject)); 
	
		//Messenger.AddListener("setCurrentlySelected", setCurrentlySelected);
		//Messenger.AddListener("setCurrentlyUnselected", setCurrentlySelected);
	}

	void OnDisable()
	{
		//Messenger.RemoveListener("setCurrentlySelected", setCurrentlySelected);
	//	Messenger.RemoveListener("setCurrentlyUnselected", setCurrentlySelected);
	}

	/*void setCurrentlySelected()
	{
		if (enabled )
		{
				UI.depth = 10;
				if(!isPayingOrNot)
					Messenger<string>.Broadcast("FirstButtonHoverOver",buttontype.ToString());
			mHighlighted = true;
		}
	}

	void setCurrentlyUnselected()
	{
		if (enabled )
		{
				if(!isPayingOrNot)
					Messenger<string>.Broadcast("FirstButtonHoverOverEnd",buttontype.ToString());
				UI.depth = startDepth;
			mHighlighted = false;
		}
	}*/

	public void SetSelected()
	{
		UI.depth = 30;
		mHighlighted = true;
		//if(!isPayingOrNot)
			//Messenger<string>.Broadcast("FirstButtonHoverOver",buttontype.ToString());
//		Wallet.FirstButtonHoverOver(buttontype.ToString());
		}

	public void SetUnSelected()
	{
		//Debug.Log("ISPAYING:"+isPayingOrNot);
		//Debug.Log(startDepth);
		UI.depth = startDepth;
		mHighlighted = false;
		//if(!isPayingOrNot)
		//	Messenger<string>.Broadcast("FirstButtonHoverOverEnd",buttontype.ToString());
//		Wallet.FirstButtonHoverOverEnd(buttontype.ToString());
	}

	/*void OnHover (bool isOver)
	{
		if (enabled )
		{
			if(isOver ){
				UI.depth = 10;
				if(!isPayingOrNot){
					Messenger<string>.Broadcast("FirstButtonHoverOver",buttontype.ToString());
				}
			}
			else if (!isOver){
				if(!isPayingOrNot){
					Messenger<string>.Broadcast("FirstButtonHoverOverEnd",buttontype.ToString());
				}
				//myTransform.position = startPosition;
				//myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y-0.1f, myTransform.position.z);
				
				//myTransform.eulerAngles = new Vector3(0f,80f,0f);
				//isRotatedOrNot = false;
				//Debug.Log ("back");
				UI.depth = startDepth;
			}
			mHighlighted = isOver;
		}
	}

	
	void setPaymentState(){
		
		route.Clear();
	}
	
	void setPaymentState2(){
		isPayingOrNot = false;
		UI.depth = startDepth;route.Clear();
		
	}

	void OnPress (bool isPressed)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnPress ||
				(trigger == Trigger.OnPressTrue && isPressed) ||
				(trigger == Trigger.OnPressFalse && !isPressed))
			{
				//Play(isPressed);
			}
		}
	}

	void OnClick ()
	{
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
		else if (enabled && isPayingOrNot){
			
			Messenger<Transform, string>.Broadcast("moveButtonBackToWallet", myTransform, buttontype.ToString());
			isPayingOrNot = false;
			myTransform.eulerAngles = new Vector3(0f,startYRotation,0f);
			UI.depth = startDepth;
			//if(WalletWindowManager.Button1_list.Count>0)
			//	WalletWindowManager.Button1_list.Remove(myTransform);
			//Debug.Log(WalletWindowManager.Button1_list.Count);
		
		}
	}

	void OnDoubleClick ()
	{
		if (enabled && trigger == Trigger.OnDoubleClick)
		{
			//Play(true);
			
		}
	}

	void OnSelect (bool isSelected)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnSelect ||
				(trigger == Trigger.OnSelectTrue && isSelected) ||
				(trigger == Trigger.OnSelectFalse && !isSelected))
			{
				//Play(true);
			}
		}
	}

	void OnActivate (bool isActive)
	{
		if (enabled)
		{
			if (trigger == Trigger.OnActivate ||
				(trigger == Trigger.OnActivateTrue && isActive) ||
				(trigger == Trigger.OnActivateFalse && !isActive))
			{
				//Play(isActive);
			}
		}
	}*/

	void Update ()
	{
		if(mHighlighted)
			Debug.Log("ID:" + this.GetInstanceID() + " " + this.name);
		if(aButtonDelay && !Input.GetButtonDown("A Button"))
			aButtonDelay = !aButtonDelay;

		if(Input.GetButtonDown("A Button") && !aButtonDelay)
		{
		if (enabled && !isPayingOrNot && mHighlighted)
		{
				aButtonDelay = true;
			UI.depth = 20;
			myTransform.eulerAngles = new Vector3(0f,90f,0f);
			//WalletWindowManager.Button1_list.Add(myTransform);
			//string t = buttontype.ToString;
				//mHighlighted = false;
				SetUnSelected();
				isPayingOrNot = true;
			//Messenger<Transform, string>.Broadcast("moveButtonToPay", myTransform, buttontype.ToString());
		//		Wallet.moveButtonToPay(myTransform, buttontype.ToString());
				Wallet.ResetHighlight();
			//Messenger<string>.Broadcast("FirstButtonHoverOverEnd", buttontype.ToString());
				//Wallet.GetComponent<WalletWindowManager>().moveButtonToPay();
			
			

			//Debug.Log(WalletWindowManager.Button1_list.Count);
			
		}
		else if (enabled && isPayingOrNot && mHighlighted){
				//mHighlighted = false;
				aButtonDelay = true;
				SetUnSelected();
			Messenger<Transform, string>.Broadcast("moveButtonBackToWallet", myTransform, buttontype.ToString());
			isPayingOrNot = false;
			myTransform.eulerAngles = new Vector3(0f,startYRotation,0f);
			UI.depth = startDepth;

			//if(WalletWindowManager.Button1_list.Count>0)
			//	WalletWindowManager.Button1_list.Remove(myTransform);
			//Debug.Log(WalletWindowManager.Button1_list.Count);
			
		}
		}
	}
}