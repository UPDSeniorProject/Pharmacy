using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class ItemsInCartGUI : MonoBehaviour {
	
	private static List<Transform> _itemsInCart = new List<Transform>();
	public static List<Transform> ItemsInCart {
		get{ return _itemsInCart; }
	}
	public float cartItemsWindowHeight = 80.0f;
	private bool _displayCartItemsWindow = false;
	private const int CARTITEMS_WINDOW_ID = 0;
	private float _offset = 10.0f;
	public float itemButtonWidth = 80.0f;
	public float itemButtonHeight = 80.0f;
	public float closeButtonWidth = 20.0f;
	public float closeButtonHeight = 20.0f;
	private Rect _cartItemsWindowRect = new Rect(0,0,0,0);
	private Vector2 _cartItemWindowSlider = Vector2.zero;
	
	private bool displayBillWindow = false;
	private const int BILL_WINDOW_ID = 1;
	private Rect billWindowRect = new Rect(0,0,0,0);
	private Vector2 billWindowSlider = Vector2.zero;
	public GUIStyle myStyle;
	
	private Transform pointOnline;
	
	private List<Vector3> path = new List<Vector3>(); 
	//private List<Vector3> route = new List<Vector3>();
	Transform screen;
	TextMesh textOnScreen;
	string textOnScreenString;
	public float totalPrice;
	
	public UILabel instructionLabel = null;
	public UISprite helpSprite = null;
	
	//PTSDVPF1Communicator communicator;
	PTSDCommunicator communicator;
	private Transform Hand1;
	private Transform grocery_bag;
	public GameObject Virtual_Cashier;
	// Use this for initialization
	
	private GameFlow gameFlow;
	
	public AudioClip scan_item_audio_clip;
	private AudioSource scanitem_audio_source;

	public static bool checkout_animation_playing = false;

	private bool scan_audio_played = false;

	private int fromMovetoPickup = 0;
	private bool transition = true;

	public bool abuttonPressed = false;
	public bool bbuttonPressed = false;
	public bool xbuttonPressed = false;
	public bool zoomPressed = false;
	
	private bool zoomIn = false;
	public bool hasPicked = false;
	public bool shoppingCartCollided = false;
	private bool hasPlayedClip = false;
	private TutorialAudioManager TutorialAudio;

	void Awake()
	{
		gameFlow = (GameFlow)GameObject.Find("GUI_prefab").GetComponent(typeof(GameFlow));

		if (instructionLabel == null)
			instructionLabel = GameObject.Find("InstructionsLabel").GetComponent<UILabel>();
		if (helpSprite == null)
			helpSprite = GameObject.Find("HelpSprite").GetComponent<UISprite>();
	}
	

	void OnEnable()
	{
		Messenger<Transform>.AddListener("move items on line", OnMovingItemsOnLine);
		Messenger.AddListener("get itmes quite", OnSettingItemsInCart);
		Messenger.AddListener("get itmes active", OnUnSettingItemsInCart);
		Messenger.AddListener("Display Bill on GUI", OnDiplayModeChanged);
		Messenger.AddListener("choose appropriate virtual human", setAppropriateCashierHand);

		Messenger<bool>.AddListener ("ZoomButton", ZoomPressed);
		Messenger<bool>.AddListener ("CartCollided", ShoppingCart);
	}

	void onDisable()
	{
		Messenger<Transform>.RemoveListener("move items on line", OnMovingItemsOnLine);
		Messenger.RemoveListener("get itmes quite", OnSettingItemsInCart);
		Messenger.RemoveListener("get itmes active", OnUnSettingItemsInCart);
    	Messenger.RemoveListener("Display Bill on GUI", OnDiplayModeChanged);
		Messenger.RemoveListener("choose appropriate virtual human", setAppropriateCashierHand);

		Messenger<bool>.RemoveListener ("ZoomButton", ZoomPressed);
		Messenger<bool>.RemoveListener ("CartCollided", ShoppingCart);
    }

	void ZoomPressed(bool zoom)
	{
		if (zoomIn && zoom)
			zoomPressed = true;
		if(!zoom)
			zoomIn = true;
	}

	void ShoppingCart(bool collide)
	{
		shoppingCartCollided = collide;
	}
	
	public void ResetTutorialGoals()
	{
		abuttonPressed = false;
		bbuttonPressed = false;
		xbuttonPressed = false;
		zoomPressed = false;
		shoppingCartCollided = false;

		zoomIn = false;
        hasPicked = false;
        //initialTransition = false;
    }
    void OnGUI(){
        /*if(PickUpItems.state == PickUpItems.State.idle){
			if(!Movement.directWalkOnly && !Movement.isCheckout)
				instructionLabel.text = "left/right/up/down for movement control";
			else if(!Movement.selectionModeLeft&&!Movement.selectionModeRight&&!Movement.isCheckout)
				instructionLabel.text = "Click left/right to enter selection mode";
			else if(Movement.selectionModeLeft||Movement.selectionModeRight)
				instructionLabel.text = "Click item to pick up. Click 'Zoom' to zoom in. Click 'Exit selection' to exit selection mode";
			else if(Movement.isCheckout&!Movement.isCheckoutStarted)
				instructionLabel.text = "Click 'start check out' button to check out";
			else if(Movement.isCheckoutStarted &!Movement.isWalletDisplayed)
				instructionLabel.text = "Click open wallet to pay, click on the monitor to view bill in detail";
			else if(Movement.isWalletDisplayed)
				instructionLabel.text = "Click money in wallet to pay";
		}
		else if(PickUpItems.state == PickUpItems.State.picked){
			instructionLabel.text = "Mouse left click to zoom in.";
			
		}*/
//        Debug.Log(GameFlow.state);
//        Debug.Log(PickUpItems.state);
//        Debug.Log(Movement1.directWalkOnly);
		if (GameFlow.state == GameFlow.State.Intro_page)
		{ 
			instructionLabel.text = "Press the \"A\" button on the controller to Continue";
		}
		if(GameFlow.state == GameFlow.State.Tasks_doing)
		{
			switch (PickUpItems.state)
			{ 
			case  PickUpItems.State.idle:
			case  PickUpItems.State.toCompare:
			case  PickUpItems.State.toCompareThree:
				if(gameFlow.showTutorial == false)
				{
					if (!(Movement1.directWalkOnly)&& !Movement1.isCheckout)
					{
						instructionLabel.text = "NAVIGATION :   Move around the store using the controller to find the items on your shopping list. Items that you can pick up will be highlighted as you hover over them with the hand icon.";
						//helpSprite.atlas.GetSprite("ui_navigation");
						helpSprite.spriteName = "ui_navigation";
					}
					else if (Movement1.directWalkOnly)
					{
						instructionLabel.text = "SELECTION :    Select the item you want to buy by pressing 'A' when the desired item is highlighted. To put back a selected item on the shelf, press 'B'.";
						//helpSprite.atlas.GetSprite("ui_navigation");
						helpSprite.spriteName = "ui_selection";
					}
				}
				else if (Movement1.directWalkOnly && gameFlow.showTutorial == true)
				{
                        Debug.Log("C:" + gameFlow.currentTutorialStep);
                        Debug.Log(InputManager.TutorialSteps.Count);
                            Debug.Log(InputManager.TutorialSteps[gameFlow.currentTutorialStep].TutorialGrouping);
					
					if (gameFlow.currentTutorialStep < InputManager.TutorialSteps.Count && InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task)
					{
						instructionLabel.text = InputManager.TutorialSteps[gameFlow.currentTutorialStep].Description + Environment.NewLine +  InputManager.TutorialSteps [gameFlow.currentTutorialStep].ProgressionText;//"(Press the [00ff00]A button[-] button to continue)";
						helpSprite.spriteName = "ui_navigation";
					}
					/*if(fromMovetoPickup == 1)
					{
						fromMovetoPickup++;
						gameFlow.currentTutorialStep++;
					}*/
				}
				break;
			case PickUpItems.State.picked:
			case PickUpItems.State.compared:
			case PickUpItems.State.comparedThree:
				if (Movement1.directWalkOnly && gameFlow.showTutorial == false)
				{
					instructionLabel.text = "VIEW ITEMS :   You can zoom in or rotate the item to read the labels of the selected item. To add the item to your cart press 'A'. To put back a selected item on the shelf, press 'B'. ";
					helpSprite.spriteName = "ui_view items";
				}
				else if (gameFlow.showTutorial == true)
				{
					
					/*if(fromMovetoPickup == 0)
					{
						fromMovetoPickup++;
						//gameFlow.currentTutorialStep++;
					}*/
					/*if(!initialTransition)
					{
						hasPicked = true;
						initialTransition = true;
					}*/
					if(InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task || InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task_Follow)
					{
						instructionLabel.text = InputManager.TutorialSteps[gameFlow.currentTutorialStep].Description + Environment.NewLine +  InputManager.TutorialSteps [gameFlow.currentTutorialStep].ProgressionText;//"(Press the [00ff00]A button[-] button to continue)";
						//instructionLabel.text = "Notice in the legend that the functionality of the right stick has changed.  Try rotating an item and zooming in on it before you pick it up with the [00ff00]A button[-].";
						helpSprite.spriteName = "ui_view items";
						for(int i = 0; i < gameFlow.Overlays.Count; i++)
						{
							if(gameFlow.Overlays[i].transform.name == InputManager.TutorialSteps[gameFlow.currentTutorialStep].SpriteName)
							{
								gameFlow.Overlays[i].GetComponent<UISprite>().enabled = true;
								gameFlow.Overlays[i].GetComponent<UISprite>().spriteName = InputManager.TutorialSteps[gameFlow.currentTutorialStep].OverlayName;
                                    gameFlow.Overlays[i].GetComponent<TweenAlpha>().enabled = true;
                                }
                                else
                                {
                                    gameFlow.Overlays[i].GetComponent<UISprite>().enabled = false;
                                    gameFlow.Overlays[i].GetComponent<TweenAlpha>().enabled = false;
                                }
                            }
                        }
                    }
                    break;
                case PickUpItems.State.checkout:
                    instructionLabel.text = "CHECKOUT :    During checkout you can interact with the cashier, have your items checked out and pay for your items with money from your wallet.";
                    helpSprite.spriteName = "ui_checkout";
                    
                    break;
            }
        }
        
        if(_displayCartItemsWindow){
            //_cartItemsWindowRect = GUI.Window(CARTITEMS_WINDOW_ID, new Rect(_offset, 10f , Screen.width - 2*_offset, cartItemsWindowHeight ), cartItemsWindow, "Items in cart");
            GUI.Box (new Rect (Screen.width - 30-Screen.width / 8f, Screen.height-30, 20, 20), _itemsInCart.Count.ToString());
            
        }
        if(displayBillWindow){
            billWindowRect = GUI.Window(BILL_WINDOW_ID, billWindowRect, billWindow, "Bill window");
		}
		
	}
	private void billWindow(int id){
		if(GUI.Button(new Rect(billWindowRect.width-20, 0, closeButtonWidth, closeButtonHeight), "X")){
			displayBillWindow = false;
		}
		
		//GUI.Label(new Rect(5,10,billWindowRect.width,70), "ateste");
		
		//string text = "This is the first line.\nThis is the second line.\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\ntest";
		float textHeight = myStyle.CalcHeight(new GUIContent(textOnScreenString), billWindowRect.width-25f);
		
		//billWindowSlider = GUI.BeginScrollView (new Rect(billWindowRect.width-25f, 20f ,20f, billWindowRect.height-20f), billWindowSlider, new Rect(20,20,billWindowRect.width-40f,40));  
		billWindowSlider = 	GUI.BeginScrollView(new Rect(0f, 20f, billWindowRect.width-5f, billWindowRect.height-25f), billWindowSlider, new Rect(0, 20f, billWindowRect.width-25f, textHeight+10f),false,false);
		//for( int count=0; count < _itemsInCart.Count; count++){
		//	GUI.Button (new Rect(_offset*0.5f + itemButtonWidth*count, _offset, itemButtonWidth, itemButtonHeight), count.ToString());
		//}
		GUI.Label(new Rect(10,20,billWindowRect.width-25f,textHeight),textOnScreenString,myStyle);
		//GUI.ScrollTo(new Rect (10,20,billWindowRect.width-25f,textHeight));
		GUI.EndScrollView();
		//GUI.Box (new Rect (Screen.width - 20, Screen.height-20, 20, 20), _itemsInCart.Count.ToString());
	}
	
	private void OnDiplayModeChanged(){
		displayBillWindow = true;
	}
	
	
	void Start () {

		scanitem_audio_source = (AudioSource) gameObject.AddComponent<AudioSource>();

		billWindowRect = new Rect(Screen.width/2,10f,Screen.width/2-10f,Screen.height/2-10f);

		instructionLabel = GameObject.Find("InstructionsLabel").GetComponent<UILabel>();
	}
	
	void Update () {
		if (gameFlow.showTutorial && gameFlow.currentTutorialStep >= 0 && gameFlow.currentTutorialStep < InputManager.TutorialSteps.Count) {
			if (InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task || InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task_Follow) {// && Input.GetButtonDown("A Button")) {
				FieldInfo myf = typeof(ItemsInCartGUI).GetField (InputManager.TutorialSteps [gameFlow.currentTutorialStep].StepGoal);
				if(!hasPlayedClip && InputManager.TutorialSteps [gameFlow.currentTutorialStep].PlaySound)
				{
					hasPlayedClip = true;
					gameFlow.TutorialAudio.GetComponent<TutorialAudioManager>().PlayAudioClip();
                }
                if (Convert.ToBoolean (myf.GetValue (this))) {
					//Debug.Log(gameFlow.currentTutorialStep);
					//Debug.Log(InputManager.TutorialSteps [gameFlow.currentTutorialStep].StepGoal);
					if(InputManager.TutorialSteps [gameFlow.currentTutorialStep].PlaySound)
					{
						hasPlayedClip = false;
						gameFlow.TutorialAudio.GetComponent<TutorialAudioManager>().PlayConfirmation();
					}
					gameFlow.currentTutorialStep++;
					ResetTutorialGoals ();

					for (int i = 0; i < gameFlow.Overlays.Count; i++) {
						gameFlow.Overlays [i].GetComponent<UISprite> ().enabled = false;
						gameFlow.Overlays [i].GetComponent<TweenAlpha> ().enabled = false;
					}
					if(InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Final)
					{
						GameFlow.state = GameFlow.State.Tutorial;
					}
				} else {
					//Debug.Log (PickUpItems.state);
					//Debug.Log (abuttonPressed + " " + hasPicked);
				}
				if (Input.GetButtonDown("A Button"))
					abuttonPressed = true;
				if (Input.GetButtonDown("B Button"))
					bbuttonPressed = true;
				if (Input.GetButtonDown("Start Button"))
					xbuttonPressed = true;
				if (PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared)
                    hasPicked = true;

            }
        }
    }
    
    public void setAppropriateCashierHand()
    {
        findClosest fc = new findClosest();

		Debug.Log ("Entering into function to activate the right communicator with closest point : " + findClosest.getCheckoutCounterNumber());

		if(findClosest.getCheckoutCounterNumber() == 1){
			communicator = GameObject.Find("VirtualHuman1").GetComponent<PTSDCommunicator>();
			Virtual_Cashier = GameObject.Find ("VirtualHuman1");
			Hand1=GameObject.Find("VirtualHuman1").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase/RightHandIndex0/RightHandIndex1/RightHandIndex2/RightHandIndex3/RightHandIndex4");  
			grocery_bag = GameObject.Find ("groceryBag1").transform;
			Debug.Log("Found Virtual Human 1 Hand:" + Hand1);
		}else if(findClosest.getCheckoutCounterNumber() == 2){
			communicator = GameObject.Find("VirtualHuman1").GetComponent<PTSDCommunicator>();
			Virtual_Cashier = GameObject.Find ("VirtualHuman2");
			Hand1=GameObject.Find("VirtualHuman2").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase/RightHandIndex0/RightHandIndex1/RightHandIndex2/RightHandIndex3/RightHandIndex4");  
			grocery_bag = GameObject.Find ("groceryBag2").transform;
			Debug.Log("Found Virtual Human 2 Hand:" + Hand1.name);
		}else if(findClosest.getCheckoutCounterNumber() == 3){
			communicator = GameObject.Find("VirtualHuman1").GetComponent<PTSDCommunicator>();
			Virtual_Cashier = GameObject.Find ("VirtualHuman3");
			Hand1=GameObject.Find("VirtualHuman3").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase/RightHandIndex0/RightHandIndex1/RightHandIndex2/RightHandIndex3/RightHandIndex4");  
			grocery_bag = GameObject.Find ("groceryBag3").transform;
			Debug.Log("Found Virtual Human 3 Hand:" + Hand1);
		}else if(findClosest.getCheckoutCounterNumber() == 4){
			communicator = GameObject.Find("VirtualHuman1").GetComponent<PTSDCommunicator>();
			Virtual_Cashier = GameObject.Find ("VirtualHuman4");
			Hand1=GameObject.Find("VirtualHuman4").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase/RightHandIndex0/RightHandIndex1/RightHandIndex2/RightHandIndex3/RightHandIndex4");  
			grocery_bag = GameObject.Find ("groceryBag4").transform;
			Debug.Log("Found Virtual Human 4 Hand:" + Hand1);		
		}else{
			communicator = GameObject.Find("VirtualHuman1").GetComponent<PTSDCommunicator>();
			Virtual_Cashier = GameObject.Find ("VirtualHuman1");
			Hand1=GameObject.Find("VirtualHuman1").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase/RightHandIndex0/RightHandIndex1/RightHandIndex2/RightHandIndex3/RightHandIndex4");  
			grocery_bag = GameObject.Find ("groceryBag1").transform;
			Debug.Log("Found Virtual Human 1 Hand:" + Hand1);		
		}
	}

	private void cartItemsWindow(int id){
		
		//if(GUI.Button(new Rect(_cartItemsWindowRect.width-20, 0, closeButtonWidth, closeButtonHeight), "X")){
		//	_displayCartItemsWindow = false;
		//}
		
		//GUI.Label(new Rect(5,10,_cartItemsWindowRect.width,70), "Total number of items: "+_itemsInCart.Count.ToString() + " Total amount: " + totalNeedToPay.ToString() + " dollars",myStyle);
		
		
		//_cartItemWindowSlider = GUI.BeginScrollView (new Rect(_offset*0.5f, 90,_cartItemsWindowRect.width-_offset, 90), _cartItemWindowSlider, new Rect(0,0,itemButtonWidth*_itemsInCart.Count+_offset,itemButtonHeight+_offset));  
		
		//for( int count=0; count < _itemsInCart.Count; count++){
		//	GUI.Button (new Rect(_offset*0.5f + itemButtonWidth*count, _offset, itemButtonWidth, itemButtonHeight), count.ToString());
		//}
		//GUI.EndScrollView();
		GUI.Box (new Rect (Screen.width - 20, Screen.height-20, 20, 20), _itemsInCart.Count.ToString());
		
	}
	
	IEnumerator WaitForKeyPress(){
		bool _keyPressed = false;
		while(!_keyPressed){
			if(GUIcontrol.isCheckoutStarted)
			{
				_keyPressed = true;
				break;
			}
			yield return 0;
		}
		
	}
	IEnumerator WaitAndMove() {

		setAppropriateCashierHand();

		float cerealCount2 = 0;
		float canCount = 0;
		float itemHeight = pointOnline.position.y-0.11f;
		//print("WaitAndPrint " + Time.time);
		for (int i = 0; i<_itemsInCart.Count; i++){
			Transform item = _itemsInCart[i];
			Highlight itemScript = item.GetComponent<Highlight>();
			
			if(itemScript.type == Highlight.Type.cereal){
				cerealCount2++;
			}
		}
		
		yield return new WaitForSeconds(4f);
		yield return StartCoroutine("WaitForKeyPress"); //4+2 = 6
		
		for (int i = 0; i<_itemsInCart.Count; i++){
			Transform item = _itemsInCart[i];
			Highlight itemScript = item.GetComponent<Highlight>();
			
			path.Add ( item.position);
			path.Add ( new Vector3(pointOnline.position.x, item.position.y+0.3f, pointOnline.position.z));
			path.Add ( new Vector3(pointOnline.position.x-0.3f, pointOnline.position.y+0.7f, pointOnline.position.z));
			path.Add ( new Vector3(pointOnline.position.x-1.3f, pointOnline.position.y+0.8f, pointOnline.position.z));
			path.Add ( new Vector3(pointOnline.position.x-2.1f, pointOnline.position.y+0.5f, pointOnline.position.z));
			path.Add ( new Vector3(pointOnline.position.x-2.1f, pointOnline.position.y+0.5f, pointOnline.position.z));
			//Debug.Log( item.collider.bounds.size.y);
			
			if(itemScript.type == Highlight.Type.cereal){
				itemHeight = itemHeight + item.GetComponent<Collider>().bounds.size.y;
				path.Add ( new Vector3(pointOnline.position.x-2.1f, itemHeight, pointOnline.position.z));
				//cerealCount2++;
				//Debug.Log(cerealCount2);
				//Debug.Log(item.position.y+item.collider.bounds.size.y*cerealCount2);
			}
			else {
				//if(canCount<3){
				
				if(cerealCount2>0)
					path.Add ( new Vector3(pointOnline.position.x-2.1f +(item.GetComponent<Collider>().bounds.size.x+0.05f)*(int)(canCount/3) + 0.5f, item.position.y, -0.2f+pointOnline.position.z+(item.GetComponent<Collider>().bounds.size.z+0.05f)*(canCount%3)));
				else
					path.Add ( new Vector3(pointOnline.position.x-2.1f +(item.GetComponent<Collider>().bounds.size.x+0.05f)*(int)(canCount/3), item.position.y, -0.2f+pointOnline.position.z+(item.GetComponent<Collider>().bounds.size.z+0.05f)*(canCount%3)));
		
				canCount ++;
				//canCount =0;
				//	path.Add ( new Vector3(pointOnline.position.x-2.1f+(item.collider.bounds.size.x+0.03f)*i, item.position.y, pointOnline.position.z));
				//}
			}
			
			item.GetComponent<Rigidbody>().useGravity = false;
			item.GetComponent<Rigidbody>().isKinematic = true;
			
			if(i>0)
				yield return new WaitForSeconds(3f);
			//iTween.MoveTo(item.gameObject, new Vector3(pointOnline.position.x-2f,item.position.y+0.04f*i,pointOnline.position.z), 10f);
			
			// Play checkout animation for the cashier
			//Transform hand=GameObject.Find("VirtualHuman1").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase");  
			//Transform hand=GameObject.Find("VirtualHuman1").transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/RightShoulder/RightArm/RightArmRoll/RightForeArm/RightForeArmRoll/RightHand/RightFingerBase/RightHandIndex0/RightHandIndex1/RightHandIndex2/RightHandIndex3/RightHandIndex4");  
			item.gameObject.GetComponent<Rigidbody>().isKinematic = true;  
			item.gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			item.gameObject.transform.parent = Hand1.transform; 
			//item.gameObject.transform.localRotation = Quaternion.identity;
			item.gameObject.transform.position = Hand1.transform.position;  
			//this.transform.localRotation = Quaternion.identity; //must point y local, so reset rotation
			//this.transform.localRotation = Quaternion.Euler(0, 0, -120); //and rotate the s


			// PLAY THE CHECKOUT ANIMATION
			//checkout_animation_playing = true;
			//yield return new WaitForSeconds(2.0f);
			Virtual_Cashier.GetComponent<Animation>()["checkout_largeItem 1"].layer = 1;
			Virtual_Cashier.GetComponent<Animation>().CrossFade("checkout_largeItem 1", 0.5F, PlayMode.StopSameLayer);
			yield return new WaitForSeconds(2.0f);

			// play audio of beep
			if(!scan_audio_played)
			{
				scanitem_audio_source.clip = scan_item_audio_clip;
				scanitem_audio_source.Play();
				scan_audio_played = true;
			}

			// REMOVE THE ITEM FROM THE HAND AFTER THE CHECKOUT ANIMATION
			item.gameObject.transform.parent = null;
			item.gameObject.transform.eulerAngles = new Vector3(0,0,0);
			item.gameObject.transform.position = new Vector3(pointOnline.position.x-1.1f, item.position.y-0.1f, pointOnline.position.z - 0.2f);
			//item.gameObject.transform.position = new Vector3(pointOnline.position.x-1.1f +(item.collider.bounds.size.x+0.05f)*(int)(canCount/3), item.position.y+0.1f, -0.2f+pointOnline.position.z+(item.collider.bounds.size.z+0.05f)*(canCount%3));
			yield return new WaitForSeconds(1.85f);

			//iTween.MoveTo(item.gameObject, iTween.Hash("path", path.ToArray(), "time", 6f,  
			//"orienttopath", false,"easetype", iTween.EaseType.linear)); 

			textOnScreen.text += "\n";
			string[] name = itemScript.item.Name.Split(' ');
			string currentLine = (i+1).ToString() + ". ";
			int lineNum = 1;
			for(int k = 0; k<name.Length;k++)
			{
				if (currentLine.Length + name[k].Length < 20 * lineNum)
					currentLine = currentLine + name[k] + " ";
				else { 
					currentLine = currentLine + "\n" + name[k]+" ";
					lineNum++;
				}
			}
			textOnScreen.text += currentLine;
			//string pad = "";
			textOnScreen.text += "\n" + "$".PadLeft(29) +itemScript.item.Price;
			
			textOnScreenString += "\n"+(i + 1).ToString() + ". "+itemScript.item.Name+"\n" +"$".PadLeft(80)+ itemScript.item.Price.ToString();
			/*
            if(itemScript.name.Length > 20){
                textOnScreen.text = textOnScreen.text + "\n" + itemScript.item.Name.Substring(0, 15) + "\n" + itemScript.item.Name.Substring(15) + "  $" + itemScript.item.Price;	
			}else{
				//textOnScreen.text = textOnScreen.text.PadRight(10,' ');
                textOnScreen.text = textOnScreen.text + "\n" + itemScript.item.Name.PadRight(20) + "\n" + "$".PadLeft(30) + itemScript.item.Price;
				//textOnScreenString = textOnScreenString.PadRight(10,' ');
                textOnScreenString = textOnScreenString + "\n" + itemScript.item.Name.PadRight(20) + "$" + itemScript.item.Price;
			}*/
			
			
			if(textOnScreen.text.Length>180){
				textOnScreen.text = textOnScreen.text.Substring(textOnScreen.text.Length-180);
			}
			//textOnScreenString = textOnScreen.text;
			totalPrice = totalPrice + itemScript.item.Price;
			totalPrice = Mathf.Round(totalPrice*100)/100;
			path.Clear();
			//item.rigidbody.useGravity = false;
			//item.rigidbody.isKinematic = true;
			//iTween.MoveTo(GameObject target, Vector3 position, float time);
			//item.position = new Vector3(point.position.x+0.5f*i, point.position.y, point.position.z);

			// ATTACH THE ITEM TO THE HAND AGAIN SO THAT WE CAN BAG THE ITEM INTO THE GROCERY BAG
			Virtual_Cashier.GetComponent<Animation>()["checkout_bagGroceries_revised3"].layer = 1;
			Virtual_Cashier.GetComponent<Animation>().CrossFade("checkout_bagGroceries_revised3", 0.5F, PlayMode.StopSameLayer);
			yield return new WaitForSeconds(1.49f);
			item.gameObject.transform.parent = Hand1.transform; 
			item.gameObject.transform.position = Hand1.transform.position;
			yield return new WaitForSeconds(2.49f);
			
			item.gameObject.transform.eulerAngles = new Vector3(0,0,0);
			//Transform groceryBag = GameObject.Find("groceryBag1").transform;
			
			item.gameObject.transform.position = new Vector3(grocery_bag.position.x, grocery_bag.position.y + 0.3f, grocery_bag.position.z);
			item.gameObject.transform.parent = null;

			scan_audio_played = false;
		}
		
		textOnScreen.text = textOnScreen.text+"\nTotal price:"+"\n"+"$".PadLeft(29)+totalPrice;
		textOnScreenString = textOnScreenString + "\n" + "\nTotal price: \n" + "$".PadLeft(80) + totalPrice;
		if (textOnScreen.text.Length > 180)
		{
			textOnScreen.text = textOnScreen.text.Substring(textOnScreen.text.Length - 180);
		}
		
		yield return new WaitForSeconds(4f);
		Messenger<float>.Broadcast("Display wallet", totalPrice);
	}

	private void OnSettingItemsInCart(){
		for (int i = 0; i<_itemsInCart.Count; i++){
			Transform item = _itemsInCart[i];
			//item.parent = null;
			item.GetComponent<Rigidbody>().useGravity = false;
			if(item.GetComponent<Rigidbody>().isKinematic == false){
				item.GetComponent<Rigidbody>().velocity = Vector3.zero;
				item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				//item.rigidbody.Sleep();
				item.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
	}
	
	
	private void OnUnSettingItemsInCart(){
		for (int i = 0; i<_itemsInCart.Count; i++){
			Transform item = _itemsInCart[i];
			//item.parent = null;
			item.GetComponent<Rigidbody>().useGravity = true;
			item.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
	
	private void OnMovingItemsOnLine(Transform point){
		pointOnline = point;
		screen = pointOnline.GetComponentInChildren<Transform>();
		textOnScreen = screen.GetComponentInChildren<TextMesh>();
		textOnScreen.text = "";
		textOnScreen.font.material.color = Color.black;
		
		float canCount = 0;
		//		float cerealCount = 0;
		float currentLineDistance = point.position.x - 0.1f;
		
		List<Transform> tmp = new List<Transform>();
		Debug.Log("No of items in cart: " + _itemsInCart.Count);

		for (int i = 0; i<_itemsInCart.Count; i++){
			Transform item = _itemsInCart[i];
			Highlight itemScript = item.GetComponent<Highlight>();
			item.parent = null;
			
			/*if(item.collider.bounds.size.x>0.3){
				item.eulerAngles = new Vector3(90f,0f,0f);
				item.position = new Vector3(point.position.x+(item.collider.bounds.size.x+0.05f)*cerealCount, point.position.y, point.position.z);
				cerealCount++;
			}
			else{
				item.eulerAngles = new Vector3(0f,0f,0f);
				//item.position = new Vector3(point.position.x+(item.collider.bounds.size.x+0.1f)*i, point.position.y, point.position.z);
				item.position =  new Vector3(point.position.x +(item.collider.bounds.size.x+0.05f)*(int)(canCount/3) + cerealCount*0.5f, point.position.y, -0.2f+point.position.z+(item.collider.bounds.size.z+0.05f)*(canCount%3));
				canCount ++;
			}*/
			// four condition: cereal following by a can, can following by a cereal, cereal slot is not full, can slot is not full
			
			
			//cereal
			if(itemScript.type == Highlight.Type.cereal){
				item.eulerAngles = new Vector3(90f,0f,0f);
				canCount = 0;
				tmp.Add(item);
				/*if( cerealCount<3){
					item.position = new Vector3(currentLineDistance, point.position.y+item.collider.bounds.size.y*cerealCount, point.position.z);
					cerealCount++;
				}
				else{
					currentLineDistance = currentLineDistance + (item.collider.bounds.size.x+0.05f);
					item.position = new Vector3(currentLineDistance, point.position.y, point.position.z);
					cerealCount = 1;
				}*/
			}
			else if(itemScript.type == Highlight.Type.can || itemScript.type == Highlight.Type.cereal){
				item.eulerAngles = new Vector3(0f,0f,0f);
				//cerealCount = 0;
				if(tmp.Count>0){
					for (int j = 0; j<tmp.Count; j++){
						Transform tmpItem = tmp[j];
						int setCount = (int)(tmp.Count/3);
						if(j%3 == 0)
							currentLineDistance = currentLineDistance + (tmpItem.GetComponent<Collider>().bounds.size.x+0.05f);
						if(j< setCount*3){
							tmpItem.position = new Vector3(currentLineDistance, point.position.y+tmpItem.GetComponent<Collider>().bounds.size.y*(2-j%3), point.position.z);
						}
						else{
							//Debug.Log()
							tmpItem.position = new Vector3(currentLineDistance, point.position.y+tmpItem.GetComponent<Collider>().bounds.size.y*(tmp.Count-setCount*3-1-j%3), point.position.z);
							//currentLineDistance = currentLineDistance + (tmpItem.collider.bounds.size.x+0.05f);
						}
					}
					//currentLineDistance = currentLineDistance + ;
					currentLineDistance = currentLineDistance + (tmp[tmp.Count-1].GetComponent<Collider>().bounds.size.x+0.05f);
					tmp.Clear();
				}
				if( canCount<3){
					//currentLineDistance = currentLineDistance + (item.collider.bounds.size.x+0.05f)*((int)(canCount/3)+1);
					item.position = new Vector3(currentLineDistance, point.position.y, -0.15f+point.position.z+(item.GetComponent<Collider>().bounds.size.z+0.05f)*canCount );
					canCount++;
				}
				else{
					currentLineDistance = currentLineDistance + (item.GetComponent<Collider>().bounds.size.x+0.05f);
					item.position = new Vector3(currentLineDistance, point.position.y, -0.15f+point.position.z);
					canCount = 1;
				}
			}	
		}
		if(tmp.Count>0){
			Debug.Log("Entering Temp");
			for (int j = 0; j<tmp.Count; j++){
				Debug.Log("Entering Temp for loop");
				Transform tmpItem = tmp[j];
				int setCount = (int)(tmp.Count/3);
				if(j%3 == 0)
					currentLineDistance = currentLineDistance + (tmpItem.GetComponent<Collider>().bounds.size.x+0.05f);
				if(j< setCount*3){
					tmpItem.position = new Vector3(currentLineDistance, point.position.y+tmpItem.GetComponent<Collider>().bounds.size.y*(2-j%3), point.position.z);
				}
			}

			setAppropriateCashierHand();
		}

		setAppropriateCashierHand();

		// Next two lines control whether the transcripts for the cashier speech are seen or not
		communicator.RenderGUI = true;
		communicator.InputBox.IsEnabled = true;
		StartCoroutine ("waitAndActivate");
		StartCoroutine ( "WaitAndMove");
		
	}
	
	public IEnumerator waitAndActivate()
	{
		   Debug.Log("Starting WAIT");
   		   //waits until this amount of time has passed.
   		   yield return new WaitForSeconds(3.0f);
		   Debug.Log("Ended WAIT");
	}
	
	
}




