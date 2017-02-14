using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement1: MonoBehaviour {
	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;
	public static bool selectionModeLeft = false;
	public static bool selectionModeRight = false;
	public Transform selectionStart;
	public Transform selectionEnd;
	
	public Transform selectionStart2;
	public Transform selectionEnd2;
	
	public static Transform selectionTarget;
	
	public GameObject shoppingList;
	public GameObject storeDoors_entrance;
	public GameObject storeDoors_exit;
	private bool entrance_doors_open = false;
	private bool exit_doors_open = false;

	public GameObject front_collider;
	public GameObject back_collider;
	public GameObject left_collider;
	public GameObject right_collider;
	public GameObject bottom_collider;
	public GameObject helpSprite;
	public GameObject miniMap;

	private float buttonWidth = Screen.width / 8f;
	private float buttonHeight = Screen.height / 10f;
	
	public GUIStyle myStyle1;
	public GUIStyle myStyle2;
	public GUIStyle myStyle3;
	public GUIStyle myStyle4;
	//public GUIStyle myStyle4;
	
	private Vector3 position = Vector3.zero;
	private float startTime = 0f;
	private float journeyLength;
	private float fracJourney = 0F;
	public static bool arrivedLeft = false;
	public static bool arrivedRight = false;
	public bool isLookingAtCart = false;
	public bool isSelectionRegion = false;
	
	
	private Transform myTransform;
	private Vector3 lookPos = Vector3.zero;
	
	public static bool directWalkOnly = false;
	public static bool isItemsActive = false;
	public static bool isItemsQuite = false;
	
	public Transform ShoppingCart;
	
	private findClosest findClosestScript;
	private Transform closestPoint;
	public static bool isCheckout;
	
	public bool isShoppinglistOpen = false;
	
	
	private List<Vector3> route = new List<Vector3>();
	private List<Vector3> route2 = new List<Vector3>();
	private List<Vector3> route3 = new List<Vector3>();
	
	new public Transform camera = null;
	
	private static float selectionEnterAngle = 50f;
    private bool directWalkEnterYet = false;

    private bool ifDisplayPickUpHint = false;

	private bool readyToMoveToCheckout = false;
	private bool inCheckoutDialogs = false;

	private bool cartStartedToMove = false;
	private bool shoppingCartCollided = false;

	private GameFlow gameFlow;
    //private bool directWalkLeaveYet = true;
    //private bool allowToTurn = false;
    //private float selectionRightAngle = 130f;
    private bool colliding;
    private bool cartMoved = false;
    private int zoomSetting;
    public int setting1;
    public int setting2;
    private float fov;
    public bool zoomed;
    Vector3 oldPos;
    private bool zoomMode = false;
    private bool triggerDown = false;

    void Awake(){
        myStyle4 = GetComponent<PickUpItems>().AButtonStyle;
		myTransform = transform;
		camera = Camera.main.transform;
		
		if(selectionStart == null)
			selectionStart = GameObject.Find("selectionPoint_front_1_start").transform;
		if(selectionEnd == null)
			selectionEnd = GameObject.Find("selectionPoint_front_1_end").transform;
		if(selectionStart2 == null)
			selectionStart2 = GameObject.Find("selectionPoint_back_1_start").transform;
		if(selectionEnd2 == null)
			selectionEnd2 = GameObject.Find("selectionPoint_back_1_end").transform;
		if(shoppingList == null)
			shoppingList = GameObject.Find("shoppingList Window");
		if(storeDoors_entrance == null)
			storeDoors_entrance = GameObject.Find("FrontMainDoors_prefab/DoorsMain_prefab_enter/doorUnit_outerDoors");
		if(storeDoors_exit == null)
			storeDoors_exit = GameObject.Find("FrontMainDoors_prefab/DoorsMain_prefab_exit/doorUnit_outerDoors");

		front_collider = GameObject.Find("Will_prefab/Robot_Prefab/items/shoppingCart/front");
		back_collider = GameObject.Find("Will_prefab/Robot_Prefab/items/shoppingCart/back");
		left_collider = GameObject.Find("Will_prefab/Robot_Prefab/items/shoppingCart/left");
		right_collider = GameObject.Find("Will_prefab/Robot_Prefab/items/shoppingCart/right");
		bottom_collider = GameObject.Find("Will_prefab/Robot_Prefab/items/shoppingCart/bottom");

		helpSprite = GameObject.Find("Will_prefab/Windows_prefab/Camera/Anchor/Windows/HelpWindow/HelpSprite");

		gameFlow = (GameFlow)GameObject.Find ("GUI_prefab").GetComponent (typeof(GameFlow));
        //ShoppingCart.rigidbody.Sleep();
        //ShoppingCart.rigidbody.IsSleeping;
    }
	
	
	void Start(){
		findClosestScript = (findClosest)GetComponent<findClosest>();
		isCheckout = false;
        zoomSetting = 0;
        zoomed = false;
		shoppingList.SetActive(false);
        fov = Camera.main.fieldOfView;

        // Trial code to test the wallet
        //Messenger.Broadcast("Open wallet");

        GetComponent<AudioSource>().Play();
	}
	void OnEnable()
	{
		Messenger.AddListener("exit selection", exitSelection);
		Messenger.AddListener("move to checkout", moveToCheckout);
		Messenger.AddListener("set asile points", setAsilePoints);
		Messenger.AddListener("TurnAround", turnAround);
        Messenger<bool>.AddListener("display pick up hint", displayPickUpHint);
		Messenger.AddListener("set move to checkout", setMoveToCheckout);
		Messenger.AddListener ("exit checkout", allowMovement);
	}
	void OnDisable()
	{
		Messenger.RemoveListener("exit selection", exitSelection);
		Messenger.RemoveListener("move to checkout", moveToCheckout);
		Messenger.RemoveListener("set asile points", setAsilePoints);
		Messenger.RemoveListener("TurnAround", turnAround);
        Messenger<bool>.RemoveListener("display pick up hint", displayPickUpHint);
		Messenger.RemoveListener("set move to checkout", setMoveToCheckout);
		Messenger.RemoveListener ("exit checkout", allowMovement);
	}
	void turnAround()
	{
		myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y + 180f, myTransform.eulerAngles.z);
		
	}

	void setMoveToCheckout()
	{
		readyToMoveToCheckout = true;
		Messenger.Broadcast("rotate back to zero");
	}

	void allowMovement()
	{
		inCheckoutDialogs = false;
	}

	void setAsilePoints() 
	{
		selectionStart = myTransform.GetComponent<findClosest>().getClosestAsilePoint();
		//Debug.Log("selectionStart" + selectionStart.name);
		selectionEnd2 = selectionStart.GetChild(0);
		selectionStart2 = selectionStart.GetChild(1);
		selectionEnd = selectionStart.GetChild(2);
	}
	
	
     void displayPickUpHint(bool ifDisplay)
     {
         ifDisplayPickUpHint = ifDisplay;
     }

    void OnTriggerStay(Collider coll)
    {
        if (coll.name.Contains("Collision") || coll.name.Contains("Plane"))
            colliding = true;
        if (!shoppingCartCollided && (coll.name.Contains("Collision") || coll.name.Contains("Plane")))
        {
            iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "time", 2f, "islocal", true, "easetype", iTween.EaseType.easeInOutSine));
            iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("position", new Vector3(1f, 0f, -1f), "time", 2f,
                                                               "islocal", true, "orienttopath", false, "easetype", iTween.EaseType.linear));

            // disable all the colliders for the shopping cart
            //front_collider.GetComponent<Collider>().enabled = false;
            //back_collider.GetComponent<Collider>().enabled = false;
            //left_collider.GetComponent<Collider>().enabled = false;
            //right_collider.GetComponent<Collider>().enabled = false;
            //bottom_collider.GetComponent<Collider>().enabled = false;
            //StartCoroutine("setShoppingCartCollided");
            shoppingCartCollided = true;
            if (GameFlow.tutorialShown)
                Messenger<bool>.Broadcast("CartCollided", shoppingCartCollided);
            helpSprite.SetActive(false);
        }
    }

    void OnTriggerExit(Collider coll)
    {

        if (shoppingCartCollided)
        {
            //Debug.Log ("Getting out of collision");
            //ShoppingCart.localPosition = new Vector3(0f, 0f, 0f);
            //ShoppingCart.localRotation = Quaternion.identity;
            iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "time", 2f, "islocal", true, "easetype", iTween.EaseType.easeInOutSine));
            iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("position", new Vector3(0f, 0f, 0f), "time", 2f,
                                                               "islocal", true, "orienttopath", false, "easetype", iTween.EaseType.linear));

            shoppingCartCollided = false;
            //StartCoroutine("setShoppingCartUnCollided");
            helpSprite.SetActive(true);
        }
    }

    IEnumerator setShoppingCartCollided()
	{
		yield return new WaitForSeconds(0f);
		shoppingCartCollided = true;
		if(GameFlow.tutorialShown)
			Messenger<bool>.Broadcast("CartCollided", shoppingCartCollided);
	}

	IEnumerator setShoppingCartUnCollided()
	{
		yield return new WaitForSeconds(2f);
		// enable all the colliders for the shopping cart
		front_collider.GetComponent<Collider>().enabled =true;
		back_collider.GetComponent<Collider>().enabled =true;
		left_collider.GetComponent<Collider>().enabled =true;
		right_collider.GetComponent<Collider>().enabled =true;
		bottom_collider.GetComponent<Collider>().enabled =true;
	}

	 void OnGUI(){

		if ((Movement1.directWalkOnly) && (PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.toCompare || PickUpItems.state == PickUpItems.State.toCompareThree))
		{
			//GUI.SetNextControlName("Press A");
//			if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height - buttonHeight, buttonWidth, buttonHeight), "Click to reset cart", myStyle3))
//			{
//				//iTween.RotateTo(ShoppingCart.gameObject,iTween.Hash("rotation",new Vector3(0f,myTransform.eulerAngles.y,0f),"time",2f,"easetype",iTween.EaseType.easeInOutSine));
//				//route3.Add ( ShoppingCart.position);
//				//route3.Add ( new Vector3(myTransform.position.x + 1.5f, myTransform.position.y + 0.6334043f, myTransform.position.z));
//				//iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("path", route3.ToArray(), "time", 3f,"orienttopath", false,"easetype", iTween.EaseType.linear));
//				Debug.Log (" Cart reset pressed ");
//				ShoppingCart.localPosition = new Vector3(0f, 0f, 0f);
//				ShoppingCart.localRotation = Quaternion.identity;
//				shoppingCartCollided = false;
//
//				// enable all the colliders for the shopping cart
//				front_collider.collider.enabled =true;
//				back_collider.collider.enabled =true;
//				left_collider.collider.enabled =true;
//				right_collider.collider.enabled =true;
//				bottom_collider.collider.enabled =true;
//			}
		}

		if ((Movement1.directWalkOnly) && (PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.toCompare || PickUpItems.state == PickUpItems.State.toCompareThree) && (ifDisplayPickUpHint))
		{
            //GUI.SetNextControlName("Press A");
            if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick up", myStyle4)))
            {
            }
        }

		if (GameFlow.state == GameFlow.State.Tasks_doing)
		{
			
			
			if (PickUpItems.state == PickUpItems.State.idle)
			{
			}
			if (ItemsInCartGUI.ItemsInCart.Count > 0 && myTransform.position.x < selectionStart.position.x && !isCheckout && !gameFlow.showTutorial && !GameFlow.TasksWindow.activeInHierarchy)
			{
                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to check out", myStyle4)))
                {
                    Messenger<int>.Broadcast("Put in cart", 0);
                }
            }
            directWalkOnly = true;
            if (isSelectionRegion == true)
			{

               // directWalkOnly = true;

                if (!selectionModeLeft && !selectionModeRight)
				{

				}
				else if (directWalkOnly)
				{
					//exitSelection() 
					directWalkOnly = false;
					Camera.main.transform.localRotation = Quaternion.identity;
					isLookingAtCart = false;
					Camera.main.transform.localPosition = Vector3.zero;
					selectionModeRight = false;
					selectionModeLeft = false;
					arrivedLeft = false;
					arrivedRight = false;
				}
			}
		}

		if(readyToMoveToCheckout)
		{
			GUI.FocusControl("RenInputBox");
		}
	}
	
	void exitSelection() {
		selectionModeRight = false;
		selectionModeLeft = false;
		arrivedLeft = false;
		arrivedRight = false;
		Camera.main.transform.localRotation = Quaternion.identity;
		isLookingAtCart = false;
		Camera.main.transform.localPosition = Vector3.zero;
		myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z);
		Debug.Log("set zero exit");
		Messenger.Broadcast("set camera rotation to zero");
		selectionEnterAngle = 1f;
		Messenger<float>.Broadcast("set cameraX max angle", 20f);
		StartCoroutine("setSelectionEnterAngle");
		
		float angle = ShoppingCart.eulerAngles.y;
		if (angle >= 0 && angle < 180)
			myTransform.eulerAngles = new Vector3(0f, 90f, 0f);
		else if (angle >= 180 && angle < 360)
			myTransform.eulerAngles = new Vector3(0f, 270f, 0f);
		Messenger.Broadcast("Exit selection mode");
		Messenger.Broadcast("Change state");
		
	}
	IEnumerator setSelectionEnterAngle()
	{
		//Debug.Log("selectionEnterAngle" + selectionEnterAngle);
		yield return new WaitForSeconds(2f);
		Messenger<float>.Broadcast("set cameraX max angle", 45f);
		selectionEnterAngle = 50f;
		Debug.Log("selectionEnterAngle" + selectionEnterAngle);
	}
	
	void changeLookAtMode(){
		//Debug.Log("test");
		iTween.LookTo( Camera.main.gameObject, iTween.Hash("looktarget",ShoppingCart.position,"time",1f));
		StartCoroutine ("lookAtCart");
		
	}
	IEnumerator lookAtCart(){
		//Debug.Log("test");
		yield return new WaitForSeconds(1f);
		isLookingAtCart = true;
		//yield return null;
	}

	void moveToCheckoutSub(){
		//yield return StartCoroutine("Move1sub");
		ShoppingCart.parent = null;
		ShoppingCart.position = new Vector3 (closestPoint.position.x+1f, ShoppingCart.position.y, closestPoint.position.z+1.0f);
		ShoppingCart.eulerAngles = new Vector3(0f,-90f,0f);
		
		Messenger<Transform>.Broadcast("move items on line", closestPoint);

		//myTransform.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		//Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);

		route.Add ( myTransform.position);
		route.Add ( new Vector3(myTransform.position.x, myTransform.position.y, closestPoint.position.z+1.5f));
		route.Add ( new Vector3(closestPoint.position.x-0.8f, myTransform.position.y, closestPoint.position.z+1.5f));
		iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 3f,  
		                                                  "orienttopath", false,"easetype", iTween.EaseType.linear));  
		
		route.Clear();	
		//iTween.RotateTo(myTransform.gameObject,iTween.Hash("rotation",new Vector3(0f,-180f,0f),"time",3f,"easetype",iTween.EaseType.easeInOutSine));
		iTween.RotateTo(myTransform.gameObject,iTween.Hash("rotation",new Vector3(0f,-181f,0f),"time",3f,"easetype",iTween.EaseType.easeInOutSine));
		//looktime",3.5f,
		//Messenger<int>.Broadcast("StartDialogue", 0);
	}
	
	void moveToCheckout(){

		// Remove the help sprite
		helpSprite.SetActive(false);

		route.Add ( myTransform.position);
		route.Add ( new Vector3(closestPoint.position.x+5f, myTransform.position.y, closestPoint.position.z+1.5f));
		route.Add ( new Vector3(closestPoint.position.x+2f, myTransform.position.y, closestPoint.position.z+1.5f));
		iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 4f,  
		                                                  "orienttopath", true,"looktime",3.6f,"oncomplete","moveToCheckoutSub","easetype", iTween.EaseType.linear));  
		
		route.Clear();	
	}

	// Start of MAIN CODE
	private void SelectionRegion(){
		
		if (myTransform.position.x > selectionStart.position.x && myTransform.position.x < selectionEnd.position.x)
		{
			selectionTarget = selectionStart;
			isSelectionRegion = true;
			//Debug.Log(selectionStart);
		}
		else if (myTransform.position.x > selectionStart2.position.x && myTransform.position.x < selectionEnd2.position.x)
		{
			selectionTarget = selectionStart2;
			isSelectionRegion = true;
			//Debug.Log(selectionStart2);
		}
		else 
			//if ( (ShoppingCart.position.x > selectionEnd.position.x && ShoppingCart.position.x <selectionStart2.position.x) ||  ShoppingCart.position.x > selectionEnd2.position.x || ShoppingCart.position.x < selectionStart.position.x)
		{ 
			selectionTarget = null;
			isSelectionRegion = false;
			//Debug.Log("no selction");
		}
	}
	
	void setArrivedLeft(){
		route2.Clear();
		arrivedLeft = true;
		Debug.Log("end");
	}
	
	void setArrivedRight(){
		route2.Clear();
		arrivedRight = true;
	}
	
	   void setEnterTrue()
    {
        directWalkEnterYet = true;
        Debug.Log("directWalkEnterYet true");
    }
    void setEnterFalse()
    {
        directWalkEnterYet = false;
    }
	
	void LateUpdate(){
		if (GameFlow.state == GameFlow.State.Tasks_doing)
		{
            colliding = false;

			// CODE FOR PLACING AND TURNING SHOPPING CART
			if(directWalkOnly&&!selectionModeLeft&&!selectionModeRight && !isCheckout){
				float angle = myTransform.eulerAngles.y;
				if (angle >= 45f && angle <= 135)
				{
					//ShoppingCart.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y + 0.6334043f, myTransform.position.z);
				}
				else if (angle >= 225f && angle <= 315f)
				{
					//ShoppingCart.position = new Vector3(myTransform.position.x - 1f, myTransform.position.y + 0.6334043f, myTransform.position.z);
				}
				else
				{
					//ShoppingCart.position = new Vector3(myTransform.position.x + 1.5f, myTransform.position.y + 0.6334043f, myTransform.position.z);
				}

				//ShoppingCart.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y + 0.6334043f, myTransform.position.z);

				if(angle>=0&&angle<180){
					//ShoppingCart.eulerAngles = new Vector3(0f,90f,0f);
					// Code that makes the shopping cart move along with the camera while turning
					if(!shoppingCartCollided){
						ShoppingCart.eulerAngles = new Vector3(myTransform.eulerAngles.x,myTransform.eulerAngles.y,myTransform.eulerAngles.z);
					}

					//ShoppingCart.position = new Vector3(myTransform.position.x+1.0f, ShoppingCart.position.y, myTransform.position.z);
				}
				else if(angle>=180&&angle<360){
					//ShoppingCart.eulerAngles = new Vector3(0f,270f,0f);
					// Code that makes the shopping cart move along with the camera while turning
					if(!shoppingCartCollided){
						ShoppingCart.eulerAngles = new Vector3(myTransform.eulerAngles.x,myTransform.eulerAngles.y,myTransform.eulerAngles.z);
					}

					//ShoppingCart.position = new Vector3(myTransform.position.x-1.0f, ShoppingCart.position.y, myTransform.position.z);	
				}
				
				
				if(isItemsActive == true){
					Messenger.Broadcast("get itmes quite");
					isItemsActive = false;
				}
				
				
			}	
		}
	}
	
	//void FixedUpdate(){
	//	if (GameFlow.state == GameFlow.State.Tasks_doing)
	//	{
			
	//		if (directWalkOnly)
	//		{
	//			//This was not moving the shopping cart with the robot in the selection regions
	//			//ShoppingCart.parent = null;
	//		}
	//		else if (!isCheckout && (ShoppingCart.localRotation != Quaternion.identity))
	//		{
	//			myTransform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
	//			myTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				
	//			myTransform.GetComponent<Rigidbody>().isKinematic = true;
				
	//			ShoppingCart.parent = myTransform;
				
	//			myTransform.GetComponent<Rigidbody>().isKinematic = false;
				
	//			myTransform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
	//			myTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				
	//			if (isItemsActive == true)
	//			{
	//				Messenger.Broadcast("get itmes quite");
	//				isItemsActive = false;
	//			}
				
	//			ShoppingCart.localPosition = new Vector3(0f, 0.6334043f, 1f);
	//			ShoppingCart.localRotation = Quaternion.identity;
	//		}
	//	}
		
	//}
	
	void XboxController()
	{
		if (PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.checkout)
		{
			if (!isShoppinglistOpen)
			{
				if (Input.GetButtonDown("Y Button"))
				{
					isShoppinglistOpen = true;
					shoppingList.SetActive(true);
				}
			}
			else
			{
				if (Input.GetButtonDown("Y Button"))
				{
					isShoppinglistOpen = false;
					shoppingList.SetActive(false);
				}
			}
		}
		
		if (ItemsInCartGUI.ItemsInCart.Count > 0 && myTransform.position.x < selectionStart.position.x && !isCheckout)
		{
			if(Input.GetButtonDown("A Button") && !inCheckoutDialogs && !readyToMoveToCheckout)
			{
				Debug.Log("checkout from movement");
				Messenger.Broadcast("user wants to checkout");
				inCheckoutDialogs = true;
			}
			if(readyToMoveToCheckout)
			{
				
				closestPoint = findClosestScript.getClosestPoint();
				isCheckout = true;
				BoxCollider boxcollider = myTransform.GetComponent<BoxCollider>();
				boxcollider.enabled = false;
				myTransform.GetComponent<Rigidbody>().isKinematic = true;
				PickUpItems.state = PickUpItems.State.checkout;
				Messenger.Broadcast("get itmes quite");
				moveToCheckout();
			}
		}
		
		//if (isSelectionRegion == true)
		//{
			
		//	directWalkOnly = true;
			
		//	if (!selectionModeLeft && !selectionModeRight)
		//	{
				
		//		//if(Input.GetButtonDown("Back Button"))
		//		//{
		//		//	//  myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y+180f, myTransform.eulerAngles.z), Time.deltaTime*10f);
		//		//	myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y + 180f, myTransform.eulerAngles.z);
		//		//}
				
		//		if ((ShoppingCart.eulerAngles.y >= 40f && ShoppingCart.eulerAngles.y <= 140f) || (camera.eulerAngles.y >= 220f && camera.eulerAngles.y <= 320f))
		//		{
					
		//			if ((camera.eulerAngles.y <= selectionEnterAngle || camera.eulerAngles.y >= 360f- selectionEnterAngle))
		//			{
		//				//selectionModeLeft = true;
		//				//Messenger.Broadcast("Change state");
		//			}
		//			else if ((camera.eulerAngles.y >= 180f-selectionEnterAngle && camera.eulerAngles.y <= 180f+selectionEnterAngle))
		//			{
		//				//selectionModeRight = true;
		//				//Messenger.Broadcast("Change state");
						
		//			}
					
		//		}
  //              //if ((myTransform.eulerAngles.y >= 45 && myTransform.eulerAngles.y <= 135) || (myTransform.eulerAngles.y <= 315 && myTransform.eulerAngles.y >= 225))

  //          }
  //          else if (PickUpItems.state == PickUpItems.State.idle)
  //          {
  //              if (camera.eulerAngles.y >= 40f && camera.eulerAngles.y <= 140f)
  //              {
  //                  if (ShoppingCart.eulerAngles.y >= 40f && ShoppingCart.eulerAngles.y <= 140f)
  //                  {
  //                      exitSelection();
  //                  }
  //                  else if (ShoppingCart.eulerAngles.y >= 220f && ShoppingCart.eulerAngles.y <= 320f)
  //                  {
  //                      ShoppingCart.eulerAngles = new Vector3(0f, 90f, 0f);
  //                      ShoppingCart.position = new Vector3(myTransform.position.x - 1.5f, myTransform.position.y + 0.6334043f, myTransform.position.z);
  //                      exitSelection();
  //                  }
  //              }


  //              if (camera.eulerAngles.y >= 220f && camera.eulerAngles.y <= 320f)
  //              {
  //                  if (ShoppingCart.eulerAngles.y >= 220f && ShoppingCart.eulerAngles.y <= 320f)
  //                  {
  //                      exitSelection();
  //                  }
  //                  else if (ShoppingCart.eulerAngles.y >= 40f && ShoppingCart.eulerAngles.y <= 140f)
  //                  {
  //                      ShoppingCart.eulerAngles = new Vector3(0f, 270f, 0f);
  //                      ShoppingCart.position = new Vector3(myTransform.position.x + 1.5f, myTransform.position.y + 0.6334043f, myTransform.position.z);
  //                      exitSelection();
  //                  }
  //              }


  //          }

  //      }	
		
	}

	void OpenDoors()
	{
       /* Debug.Log("door");
		Debug.Log (Vector3.Distance( myTransform.position, storeDoors_entrance.transform.position));
        Debug.Log(entrance_doors_open);
        Debug.Log(storeDoors_entrance.GetComponent<Animation>());
        */
		if(Vector3.Distance( myTransform.position, storeDoors_entrance.transform.position) <= 11f){
			if(!entrance_doors_open){
				storeDoors_entrance.GetComponent<Animation>().Play ("doors_open");
				entrance_doors_open = true;
			}
		}else{
			if(entrance_doors_open){
				storeDoors_entrance.GetComponent<Animation>().Play ("doors_close");
				entrance_doors_open = false;
			}
		}

		if(Vector3.Distance( myTransform.position, storeDoors_exit.transform.position) <= 11f){
			if(!exit_doors_open){
				storeDoors_exit.GetComponent<Animation>().Play ("doors_open");
				exit_doors_open = true;
			}
		}else{
			if(exit_doors_open){
				storeDoors_exit.GetComponent<Animation>().Play ("doors_close");
				exit_doors_open = false;
			}
		}
	}

    void Zoom()
    {
        float zoom = 1;
        if(!zoomMode)
            zoom = Input.GetAxis("Left Trigger");

        if (!(PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree))
        {
            Camera.main.fieldOfView = fov - zoom * zoomSetting;
        }
        else if (PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree)
        {
            Camera.main.fieldOfView = fov - zoom * zoomSetting;
            if (!zoomed)
                oldPos = Camera.main.transform.localPosition;

            if (zoomSetting > 0)
            {
                zoomed = true;
                Camera.main.GetComponent<Collider>().enabled = false;
                float moveZoomX = Input.GetAxis("Horizontal");
                float moveZoomY = Input.GetAxis("Vertical");
                if (PickUpItems.state == PickUpItems.State.picked && ((Mathf.Abs(oldPos.x - Camera.main.transform.localPosition.x) < .15 && Mathf.Abs(oldPos.y - Camera.main.transform.localPosition.y) < .15)
                    || Vector3.Distance(oldPos, Camera.main.transform.localPosition) > Vector3.Distance(oldPos, Camera.main.transform.localPosition + new Vector3(moveZoomX * .02f, moveZoomY * .02f))))
                {
                    Camera.main.transform.localPosition = Camera.main.transform.localPosition + new Vector3(moveZoomX * .02f, moveZoomY * .02f);
                }
                else if (PickUpItems.state == PickUpItems.State.compared && ((Mathf.Abs(oldPos.x - Camera.main.transform.localPosition.x) < .3 && Mathf.Abs(oldPos.y - Camera.main.transform.localPosition.y) < .3)
                    || Vector3.Distance(oldPos, Camera.main.transform.localPosition) > Vector3.Distance(oldPos, Camera.main.transform.localPosition + new Vector3(moveZoomX * .01f, moveZoomY * .01f))))
                {
                    Camera.main.transform.localPosition = Camera.main.transform.localPosition + new Vector3(moveZoomX * .01f, moveZoomY * .01f);
                }
                else if ((Mathf.Abs(oldPos.x - Camera.main.transform.localPosition.x) < .75 && Mathf.Abs(oldPos.y - Camera.main.transform.localPosition.y) < .75)
                    || Vector3.Distance(oldPos, Camera.main.transform.localPosition) > Vector3.Distance(oldPos, Camera.main.transform.localPosition + new Vector3(moveZoomX * .01f, moveZoomY * .01f)))
                {
                    Camera.main.transform.localPosition = Camera.main.transform.localPosition + new Vector3(moveZoomX * .01f, moveZoomY * .01f);
                }

            }

            if (zoomMode && zoomed && zoomSetting == 0)
            {
                Camera.main.transform.localPosition = oldPos;
                zoomed = false;
                Camera.main.GetComponent<Collider>().enabled = true;
            }
            else if(zoomed && zoom == 0)
            {
                Camera.main.transform.localPosition = oldPos;
                zoomed = false;
                Camera.main.GetComponent<Collider>().enabled = true;
            }
        }
    }

	void Update()
	{
        //moving camera to correct position
        if (Camera.main.transform.localPosition != new Vector3(0, 0.3f, -0.25f) && !zoomed)
            Camera.main.transform.localPosition = new Vector3(0, 0.3f, -0.25f);

        //new zoom function
        if (GameFlow.state == GameFlow.State.Tasks_doing || GameFlow.state == GameFlow.State.Tutorial)
        {
            if (Input.GetButtonDown("Right Bumper"))
            {
				// This folloiwng line had been modified to disable multi-level zoom. Only the linear zoom feature is enabled now
                //zoomMode = !zoomMode;
                
				zoomMode = false;
				zoomSetting = 0;
                zoomed = false;
            }
            if (Input.GetAxisRaw("Left Trigger") == 0)
                triggerDown = false;
            if (zoomMode)
            {
                if (Input.GetButtonDown("Left Bumper") || Input.GetAxisRaw("Left Trigger") == 1 && !triggerDown)
                {
                    triggerDown = true;
                    if (zoomSetting == 0)
                    {
                        zoomSetting = setting1;
                    }
                    else if (zoomSetting == setting1)
                    {
                        zoomSetting = setting2;
                    }
                    else if (zoomSetting == setting2)
                    {
                        zoomSetting = 0;
                        zoomed = false;
                    }
                }
                Zoom();
                
            }
            else
            {
                if (zoomSetting == 0)
                    zoomSetting = setting1;
                if(Input.GetButtonDown("Left Bumper"))
                {
                    if (zoomSetting == setting1)
                        zoomSetting = setting2;
                    else
                        zoomSetting = setting1;
                }
                Zoom();
            }
        }

        //checks if cart colliding with shelf, turn cart collider off. else turn on
        if (colliding)
        {
            ShoppingCart.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            ShoppingCart.GetComponent<BoxCollider>().enabled = true;
        }
        
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);
		isSelectionRegion = true;

		if (GameFlow.state == GameFlow.State.Tasks_doing || GameFlow.state == GameFlow.State.Tutorial)
		{
			
			// Xbox Controller update functions
			XboxController();
			
			// original update functions
			SelectionRegion();

			// Open the doors of the store
			OpenDoors();
			
			if (PickUpItems.state == PickUpItems.State.picked)
			{
				
				if (!selectionModeLeft && (myTransform.eulerAngles.y <= 90 || myTransform.eulerAngles.y > 270))
				{
					//selectionModeLeft = true;
				}
				else if (!selectionModeRight && (myTransform.eulerAngles.y > 90 && myTransform.eulerAngles.y <= 270))
				{
					//selectionModeRight = true;
				}
			}
			
			if (selectionModeLeft || selectionModeRight)
			{
//				ShoppingCart.parent = null;
//				if (selectionTarget != null)
//				{
//					ShoppingCart.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y + 0.6334043f, selectionTarget.position.z);
//				}
				
				if (isItemsActive == false)
				{
					Messenger.Broadcast("get items active");
					isItemsActive = true;
				}

				
			}

            //Debug.Log("MOVEMENT1: " + selectionModeLeft + ":" + selectionModeRight + ":" + isCheckout);
			if (!selectionModeLeft && !selectionModeRight && !isCheckout && !(PickUpItems.state==PickUpItems.State.picked || PickUpItems.state==PickUpItems.State.compared || PickUpItems.state==PickUpItems.State.comparedThree))
			{
				float translation = Input.GetAxis("Vertical") * speed;
				float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
				float look_rotation = Input.GetAxis("Mouse X") * rotationSpeed;
				translation *= Time.deltaTime;
				rotation *= Time.deltaTime;
				look_rotation *= Time.deltaTime;

                // Play the audio for the cart moving
                GetComponent<AudioSource>().volume = Mathf.Abs(translation) * 1.3f;
                if (Mathf.Abs(translation) == 0)
                    GetComponent<AudioSource>().Pause();
                else
                    GetComponent<AudioSource>().UnPause();

                if (translation == 0f && rotation == 0f && myTransform.GetComponent<Rigidbody>().velocity!=Vector3.zero && look_rotation == 0f)
				{
					myTransform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
					myTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
					//   myTransform.rigidbody.isKinematic = true;
					// myTransform.rigidbody.isKinematic = false;
				}
				
				if (directWalkOnly && selectionTarget != null)
				{
					myTransform.Translate(0, 0, translation);
					myTransform.Rotate(0, rotation, 0);
					myTransform.Rotate(0, look_rotation, 0);

					Vector3 targetPos = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z);

                    // Play the audio for the cart moving
                    //if(translation > 0.2f || rotation > 0.2f){
                    //	if(!cartStartedToMove){
                    //		cartStartedToMove = true;
                    //		GetComponent<AudioSource>().Play();
                    //	}
                    //}else if(translation < 0.2f || rotation < 0.2f){
                    //	cartStartedToMove = false;
                    //	GetComponent<AudioSource>().Stop();
                    //}
                    //					if (Mathf.Abs(Vector3.Distance(myTransform.position, targetPos)) < 0.5f)
                    //						myTransform.position = targetPos;
                    //					else
                    //						myTransform.position = Vector3.Lerp(myTransform.position, targetPos, Time.deltaTime);
                    //					
                    //					float angle = myTransform.eulerAngles.y;
                    //					if (angle >= 1f && angle <= 179f)
                    //					{
                    //						Vector3 target = new Vector3(0f, 90f, 0f);
                    //						if (Mathf.Abs(Vector3.Distance(myTransform.eulerAngles, target)) < 0.5f){
                    //							myTransform.eulerAngles = target;
                    //						}else{
                    //							myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, target, Time.deltaTime);
                    //						}
                    //						
                    //						//myTransform.eulerAngles = new Vector3(0f, 90f, 0f);
                    //						//Debug.Log("angle 90f");
                    //					}
                    //					else if (angle >= 181f && angle <= 359f)
                    //					{
                    //						Debug.Log("angle 270f");
                    //						Vector3 target = new Vector3(0f, 270f, 0f);
                    //						if (Mathf.Abs(Vector3.Distance(myTransform.eulerAngles, target)) < 0.5f){
                    //							myTransform.eulerAngles = target;
                    //						}else{
                    //							myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, target, Time.deltaTime);
                    //						}
                    //						
                    //						myTransform.eulerAngles = new Vector3(0f, 270f, 0f);
                    //					}

                }
				else
				{
					myTransform.Translate(0, 0, translation);
					myTransform.Rotate(0, rotation, 0);
					myTransform.Rotate(0, look_rotation, 0);

                    // Play the audio for the cart moving
                    //if(translation > 0.2f || rotation > 0.2f){
                    //	if(!cartStartedToMove){
                    //		cartStartedToMove = true;
                    //		GetComponent<AudioSource>().Play();
                    //	}
                    //}else if(translation < 0.2f || rotation < 0.2f){
                    //	cartStartedToMove = false;
                    //	GetComponent<AudioSource>().Stop();
                    //}
                }
                //Debug.Log("inMOVEMENT");
				if(GameFlow.tutorialShown || inCheckoutDialogs)
					Messenger<float, float, float>.Broadcast("MoveAround", rotation, look_rotation, translation);
			}

			
			else
			{
				// left from front
				if (selectionModeLeft && PickUpItems.state != PickUpItems.State.picked && PickUpItems.state != PickUpItems.State.compared)
				{
					if (arrivedLeft)
					{
						
						float translationH = Input.GetAxis("Horizontal") * speed;
						translationH *= Time.deltaTime;
						if ( (   ( (selectionStart.position.x < myTransform.position.x && myTransform.position.x < selectionStart.position.x + 0.5f) || (selectionStart2.position.x < myTransform.position.x && myTransform.position.x < selectionStart2.position.x + 0.5f) ) && translationH < 0f)
						    || ((      (myTransform.position.x <selectionEnd.position.x && myTransform.position.x > selectionEnd.position.x - 0.5f) || (myTransform.position.x< selectionEnd2.position.x && myTransform.position.x > selectionEnd2.position.x - 0.5f) )&& translationH > 0f ) )
						{
							translationH = 0f;
						}
						
						myTransform.Translate(translationH, 0, 0, Space.World);
						if (selectionTarget != null)
							myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z - 1.2f);
						
						
					}
					else
					{
						route2.Add(myTransform.position);
						route2.Add(new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z - 1.2f));
						iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route2.ToArray(), "time", 0.5f,
						                                                  "orienttopath", false, "oncomplete", "setArrivedLeft", "easetype", iTween.EaseType.linear));
						
						if(myTransform.eulerAngles.y>=80f &&myTransform.eulerAngles.y<=100f)
							myTransform.eulerAngles = new Vector3(0f, myTransform.eulerAngles.y-45f, 0f);
						else if (myTransform.eulerAngles.y >= 260f && myTransform.eulerAngles.y <= 280f)
							myTransform.eulerAngles = new Vector3(0f, myTransform.eulerAngles.y + 45f, 0f);
						
						Messenger.Broadcast("set camera rotation to zero");
						//Debug.Log("set zero arrviedleft");
						iTween.RotateTo(myTransform.gameObject, new Vector3(0, 0, 0), 1f);
						//camera.eulerAngles = new Vector3(0f, 0f, 0f);
						//myTransform.position = new Vector3 (myTransform.position.x, myTransform.position.y, selectionTarget.position.z - 1.2f );
						//myTransform.localEulerAngles = new Vector3(myTransform.eulerAngles.x, 0, myTransform.eulerAngles.z);
						// if (myTransform.eulerAngles.y < 10f)
						arrivedLeft = true;
					}
					
				}
				else if (selectionModeLeft && (PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared))
				{
					myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z - 1.2f);
					myTransform.localEulerAngles = new Vector3(myTransform.eulerAngles.x, 0, myTransform.eulerAngles.z);
					
				}
				//right from front
				else if (selectionModeRight && PickUpItems.state != PickUpItems.State.picked && PickUpItems.state != PickUpItems.State.compared)
				{
					if (arrivedRight)
					{
						float translationH = Input.GetAxis("Horizontal") * speed;
						translationH *= Time.deltaTime;
						if ( (   ( (selectionStart.position.x < myTransform.position.x && myTransform.position.x < selectionStart.position.x + 0.5f) || (selectionStart2.position.x < myTransform.position.x && myTransform.position.x < selectionStart2.position.x + 0.5f) ) && translationH > 0f)
						    || ((      (myTransform.position.x <selectionEnd.position.x && myTransform.position.x > selectionEnd.position.x - 0.5f) || (myTransform.position.x< selectionEnd2.position.x && myTransform.position.x > selectionEnd2.position.x - 0.5f) )&& translationH < 0f ) )
						{
							translationH = 0f;
						}
						myTransform.Translate(-translationH, 0, 0, Space.World);
						if (selectionTarget != null)
							myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z + 1.2f);
						
					}
					else
					{
						//myTransform.position = new Vector3 (myTransform.position.x, myTransform.position.y, selectionTarget.position.z + 1.2f );
						//myTransform.localEulerAngles = new Vector3(myTransform.eulerAngles.x, 180, myTransform.eulerAngles.z);
						route2.Add(myTransform.position);
						route2.Add(new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z + 1.2f));
						iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route2.ToArray(), "time", 0.5f,
						                                                  "orienttopath", false,  "oncomplete", "setArrivedRight", "easetype", iTween.EaseType.linear));
						//iTween.MoveTo(myTransform.gameObject,new Vector3 (myTransform.position.x, myTransform.position.y, selectionTarget.position.z + 1.2f ),1f);
						//iTween.RotateTo(myTransform.gameObject, new Vector3(0, 180, 0), 1f);
						//camera.eulerAngles = new Vector3(0f, 180f, 0f);
						
						if (myTransform.eulerAngles.y >= 80f && myTransform.eulerAngles.y <= 100f)
							myTransform.eulerAngles = new Vector3(0f, myTransform.eulerAngles.y + 45f, 0f);
						else if (myTransform.eulerAngles.y >= 260f && myTransform.eulerAngles.y <= 280f)
							myTransform.eulerAngles = new Vector3(0f, myTransform.eulerAngles.y - 45f, 0f);
						
						
						Messenger.Broadcast("set camera rotation to zero");
						Debug.Log("set zero arrivedright");
						iTween.RotateTo(myTransform.gameObject, new Vector3(0f, 180f, 0f), 1f);
						
						
						//if(myTransform.eulerAngles.y >170f)
						arrivedRight = true;
					}
				}
				else if (selectionModeRight && (PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared))
				{
					myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z + 1.2f);
					myTransform.localEulerAngles = new Vector3(myTransform.eulerAngles.x, 180f, myTransform.eulerAngles.z);
					
				}
				
				
			}
		}
	}
	
}
