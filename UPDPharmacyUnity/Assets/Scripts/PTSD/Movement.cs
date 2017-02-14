using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement: MonoBehaviour {
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

	
	//public float buttonWidth = 64f;
	//public float buttonHeight = 64f;
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


    new public Transform camera = null;

    private static float selectionEnterAngle = 50f;
    private bool directWalkEnterYet = false;

    private bool ifDisplayPickUpHint = false;

    //private bool directWalkLeaveYet = true;
    //private bool allowToTurn = false;
    //private float selectionRightAngle = 130f;

	void Awake(){
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
		
		//ShoppingCart.rigidbody.Sleep();
		//ShoppingCart.rigidbody.IsSleeping;
	}


	void Start(){
		findClosestScript = (findClosest)GetComponent<findClosest>();
		isCheckout = false;
		
		shoppingList.SetActive(false);
		
	}
	void OnEnable()
    {
        Messenger.AddListener("exit selection", exitSelection);
        //Messenger.AddListener("move to checkout", moveToCheckout);
        Messenger.AddListener("set asile points", setAsilePoints);
        Messenger.AddListener("TurnAround", turnAround);
        Messenger<bool>.AddListener("display pick up hint", displayPickUpHint);
    }
	 void OnDisable()
    {
        Messenger.RemoveListener("exit selection", exitSelection);
        //Messenger.RemoveListener("move to checkout", moveToCheckout);
        Messenger.RemoveListener("set asile points", setAsilePoints);
        Messenger.RemoveListener("TurnAround", turnAround);
        Messenger<bool>.RemoveListener("display pick up hint", displayPickUpHint);
    }
    void turnAround()
    {
        myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y + 180f, myTransform.eulerAngles.z);
                    
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
	void OnGUI(){
		// check outisCheckout &&isCheckout&&
        /*if(GameFlow.state == GameFlow.State.Tasks_page){
         if (!isShoppinglistOpen)
                {
                    if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Open" + "\n" + "shopping list", myStyle4))
                    {
                        isShoppinglistOpen = true;
                        shoppingList.SetActive(true);
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Close" + "\n" + "shopping list", myStyle4))
                    {
                        isShoppinglistOpen = false;
                        shoppingList.SetActive(false);
                    }
                }
        }*/
        if ((Movement.selectionModeLeft || Movement.selectionModeRight) && (PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.toCompare || PickUpItems.state == PickUpItems.State.toCompareThree) && (ifDisplayPickUpHint))
        {
            //GUI.SetNextControlName("Press A");
            if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.6f, buttonWidth, buttonHeight), "Press A" + '\n' + "to pick up", myStyle4))
            {

            }
        }


        if (GameFlow.state == GameFlow.State.Tasks_doing)
        {


            if (PickUpItems.state == PickUpItems.State.idle)
            {

                /*if (displayResponseButton)
                {
                    if (!isResponsing)
                    {
                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Response", myStyle4))
                        {
                            isResponsing = true;
                            responseWindow.SetActive(true);
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Close response", myStyle4))
                        {
                            isResponsing = false;

                            UILabel inputTextLabel = GameObject.Find("InputText").GetComponent<UILabel>();
                            inputTextLabel.text = "";
                            responseWindow.SetActive(false);
                        }
                    }
                }
                */

                //check out code used to be here

                /*if (!isShoppinglistOpen)
                {
                    if(Joystick1Button3|| (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Open" + "\n" + "shopping list", myStyle4)))
                    {
                        Joystick1Button3 = false;
                        isShoppinglistOpen = true;
                        shoppingList.SetActive(true);
                    }
                }
                else
                {
                    if (Joystick1Button3 || (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Close" + "\n" + "shopping list", myStyle4)))
                    {
                        Joystick1Button3 = false;
                        isShoppinglistOpen = false;
                        shoppingList.SetActive(false);
                    }
                }*/

                /*if (ItemsInCartGUI.ItemsInCart.Count > 0f) {
                    if (!isLookingAtCart && !isCheckout )
                    {
                        //lookPos = Camera.main.transform.eulerAngles;
                        if (GUI.Button(new Rect(Screen.width / 2 + buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Look at cart", myStyle4))
                        {
                            changeLookAtMode();
                        }
                    }
                    else if (isLookingAtCart && Camera.main.transform.localRotation == Quaternion.identity)
                    {
                        isLookingAtCart = false;
                    }
                    else
                    {
                        if (!isCheckout && GUI.Button(new Rect(Screen.width / 2 + buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Look up", myStyle4))
                        {
                            //putDownItemToCompare();
                            //if (GUI.Button(new Rect(Screen.width-buttonWidth, Screen.height-buttonHeight, buttonWidth, buttonHeight), "Finish", myStyle4)){

                            Camera.main.transform.localRotation = Quaternion.identity;
                            isLookingAtCart = false;
                        }
                    }
                }*/
            }

            if (ItemsInCartGUI.ItemsInCart.Count > 0 && myTransform.position.x < selectionStart.position.x && !isCheckout)
            {
             if (GUI.Button(new Rect(Screen.width/2f - buttonWidth, Screen.height - buttonHeight, buttonWidth, buttonHeight), "Press A" + '\n' + "to check out", myStyle4))
            {
            }
                //if(GUI.Button(new Rect(Screen.width-buttonWidth, Screen.height-buttonHeight, buttonWidth, buttonHeight), "Check out", myStyle4)){
                /*if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth * 1f, Screen.height * 0.85f, buttonWidth, buttonHeight), "Check out", myStyle4))
                {
                    //if(GUI.Button(new Rect(Screen.width/2-buttonWidth*0.5f, Screen.height*0.85f, buttonWidth, buttonHeight), "Check out", myStyle4)){
                    closestPoint = findClosestScript.getClosestPoint();
                    isCheckout = true;
                    PickUpItems.state = PickUpItems.State.checkout;
                    Messenger.Broadcast("get itmes quite");

                    moveToCheckout();
                }*/
            }
            
            if (isSelectionRegion == true)
            {

                directWalkOnly = true;

                if (!selectionModeLeft && !selectionModeRight)
                {
                    /*
                    if (GUI.Button(new Rect(Screen.width / 2 - 2 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Turn around", myStyle4))
                    {
                        myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y + 180f, myTransform.eulerAngles.z);
                    }
                    

                    if ((myTransform.eulerAngles.y >= 45 && myTransform.eulerAngles.y <= 135) || (myTransform.eulerAngles.y <= 315 && myTransform.eulerAngles.y >= 225))
                    {
                        
                        //if(GUI.Button(new Rect(Screen.width/2-1.5f*buttonWidth, Screen.height/2-buttonHeight, buttonWidth, buttonHeight),"", myStyle1)){
                        if( (GUI.Button(new Rect(Screen.width / 2 - buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Look at"+"\n"+"left aisle", myStyle4)))
                        {
                            if (myTransform.eulerAngles.y >= 0 && myTransform.eulerAngles.y < 180)
                            {
                                selectionModeLeft = true;
                            }
                            else
                            {
                                selectionModeRight = true;
                            }
                        }

                        //if(GUI.Button(new Rect(Screen.width/2+0.5f*buttonWidth, Screen.height/2-buttonHeight, buttonWidth, buttonHeight),"", myStyle2)){
                        if((GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Look at"+"\n"+"right aisle", myStyle4)))
                        {
                            if (myTransform.eulerAngles.y > 0 && myTransform.eulerAngles.y <= 180)
                            {
                                selectionModeRight = true;
                            }
                            else
                            {
                                selectionModeLeft = true;
                            }
                        }

                    }*/
                }
                /*else if (PickUpItems.state != PickUpItems.State.picked && PickUpItems.state != PickUpItems.State.compared && PickUpItems.state != PickUpItems.State.comparedThree && ((myTransform.eulerAngles.y < 5f && myTransform.eulerAngles.y > -5f) || (myTransform.eulerAngles.y < 185f && myTransform.eulerAngles.y > 175f)))
                {
                    if( ( GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Exit selection", myStyle4)))
                    {
                        //if(GUI.Button(new Rect(Screen.width-20f-buttonHeight, 20f, buttonHeight, buttonHeight),"", myStyle3)){
                        exitSelection();

                    }

                }
                */
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

    void exitSelection() {
        selectionModeRight = false;
        selectionModeLeft = false;
        arrivedLeft = false;
        arrivedRight = false;
        //Camera.main.transform.localRotation = Quaternion.identity;
        isLookingAtCart = false;
        Camera.main.transform.localPosition = Vector3.zero;
        myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z);
        //Debug.Log("set zero exit");
        //Messenger.Broadcast("set camera rotation to zero");
        
        //selectionEnterAngle = 1f;
        //StartCoroutine("setSelectionEnterAngle");
        
        
        /*float angle = ShoppingCart.eulerAngles.y;
        if (angle >= 0f && angle < 180f)
        {
            myTransform.eulerAngles = new Vector3(0f, 90f, 0f);
            if (camera.eulerAngles.y > 180f + selectionEnterAngle && camera.eulerAngles.y < 360f - selectionEnterAngle)
            {
                camera.eulerAngles = new Vector3(camera.eulerAngles.x, camera.eulerAngles.y - 180f, camera.eulerAngles.z);
            }
         }
        else if (angle >= 180f && angle < 360f)
        {
            myTransform.eulerAngles = new Vector3(0f, 270f, 0f);
            if (camera.eulerAngles.y > selectionEnterAngle && camera.eulerAngles.y < 180f - selectionEnterAngle)
            {
                camera.eulerAngles = new Vector3(camera.eulerAngles.x, camera.eulerAngles.y - 180f, camera.eulerAngles.z);
            }
        }*/
        Messenger.Broadcast("Exit selection mode");
        Messenger.Broadcast("Change state");
          
    }
    /*IEnumerator setSelectionEnterAngle()
    {
        //Debug.Log("selectionEnterAngle" + selectionEnterAngle);
        yield return new WaitForSeconds(2f);
        Messenger<float>.Broadcast("set cameraX max angle", 45f);
        selectionEnterAngle = 50f;
        Debug.Log("selectionEnterAngle" + selectionEnterAngle);
    }*/

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
		
		route.Add ( myTransform.position);
			route.Add ( new Vector3(myTransform.position.x, myTransform.position.y, closestPoint.position.z+1.7f));
			route.Add ( new Vector3(closestPoint.position.x-0.8f, myTransform.position.y, closestPoint.position.z+1.7f));
		iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 3f,  
          	"orienttopath", false,"easetype", iTween.EaseType.linear));  
			
			route.Clear();	
		iTween.RotateTo(myTransform.gameObject,iTween.Hash("rotation",new Vector3(0f,180f,0f),"time",3f,"easetype",iTween.EaseType.easeInOutSine));
		//looktime",3.5f,
			
	}
	
	/*void move2(){
			route.Add ( myTransform.position);
			route.Add ( new Vector3(closestPoint.position.x+2f, myTransform.position.y, closestPoint.position.z+1.0f));
			iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 1f,  
          	"orienttopath", false,"oncomplete","move1","easetype", iTween.EaseType.linear));  
			
			route.Clear();	
	}*/
	
	void moveToCheckout(){
		
		route.Add ( myTransform.position);
			route.Add ( new Vector3(closestPoint.position.x+5f, myTransform.position.y, closestPoint.position.z));
			route.Add ( new Vector3(closestPoint.position.x+2f, myTransform.position.y, closestPoint.position.z+0.8f));
		iTween.MoveTo(myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 4f,  
          	"orienttopath", true,"looktime",3.6f,"oncomplete","moveToCheckoutSub","easetype", iTween.EaseType.linear));  
			
			route.Clear();	
		//myTransform.position = new Vector3 (closestPoint.position.x-1f, myTransform.position.y, closestPoint.position.z+1.7f);
		//myTransform.eulerAngles = new Vector3(0f,180f,0f);3
		//StartCoroutine("move1");
		
		// Set the RenderGUI variable to true to display the interface to talk to the VH
		
	}
	
	private void SelectionRegion(){

        if (myTransform.position.x > selectionStart.position.x && myTransform.position.x < selectionEnd.position.x)
        {
			selectionTarget = selectionStart;
			isSelectionRegion = true;
            Messenger<float>.Broadcast("set cameraX max angle", 360f);
            //directWalkEnterYet = true;
		}
        else if (myTransform.position.x > selectionStart2.position.x && myTransform.position.x < selectionEnd2.position.x)
        {
			selectionTarget = selectionStart2;
			isSelectionRegion = true;
            Messenger<float>.Broadcast("set cameraX max angle", 360f);
            //directWalkEnterYet = true;
		}
        //else if( !(selectionModeLeft||selectionModeRight) )
        
        else{ 
			selectionTarget = null;
			isSelectionRegion = false;
            Messenger<float>.Broadcast("set cameraX max angle", 45f);
            //directWalkEnterYet = false;
		}
        /*else if (selectionModeLeft || selectionModeRight)
        {
            if(selectionTarget == selectionStart)
            {
                if (myTransform.position.x < selectionStart.position.x)
                    myTransform.position = new Vector3(selectionStart.position.x, myTransform.position.y, myTransform.position.z);
                else if(myTransform.position.x > selectionEnd.position.x)
                    myTransform.position = new Vector3(selectionEnd.position.x, myTransform.position.y, myTransform.position.z);
            }
            else if (selectionTarget == selectionStart2)
            {
                if (myTransform.position.x < selectionStart2.position.x)
                    myTransform.position = new Vector3(selectionStart2.position.x, myTransform.position.y, myTransform.position.z);
                else if (myTransform.position.x > selectionEnd2.position.x)
                    myTransform.position = new Vector3(selectionEnd2.position.x, myTransform.position.y, myTransform.position.z);
            }
        }*/
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
		
		/*if(!directWalkOnly){
			if(isItemsActive == false){
				//Messenger.Broadcast("get itmes quite");
				//myTransform.rigidbody.isKinematic = true;
				//ShoppingCart.parent = myTransform;
				myTransform.rigidbody.isKinematic = false;
				//isItemsActive = false;
			}
		}*/

         if (GameFlow.state == GameFlow.State.Tasks_doing)
        {
		
		if(directWalkOnly&&!selectionModeLeft&&!selectionModeRight){
			float angle = myTransform.eulerAngles.y;
            if (angle >= 45f && angle <= 135)
            {
                ShoppingCart.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y + 0.6334043f, myTransform.position.z);
            }
            else if (angle >= 225f && angle <= 315f)
            {
                ShoppingCart.position = new Vector3(myTransform.position.x - 1f, myTransform.position.y + 0.6334043f, myTransform.position.z);
            }
            else
            {
                ShoppingCart.position = new Vector3(myTransform.position.x + 1.5f, myTransform.position.y + 0.6334043f, myTransform.position.z);
            }
            if (directWalkEnterYet)
            {
                if (angle >= 0 && angle < 180)
                {
                    ShoppingCart.eulerAngles = new Vector3(0f, 90f, 0f);
                    //ShoppingCart.position = new Vector3(myTransform.position.x+1.0f, ShoppingCart.position.y, myTransform.position.z);
                }
                else if (angle >= 180 && angle < 360)
                {
                    ShoppingCart.eulerAngles = new Vector3(0f, 270f, 0f);
                    //ShoppingCart.position = new Vector3(myTransform.position.x-1.0f, ShoppingCart.position.y, myTransform.position.z);
                }
            }
            else {
                if (angle >= 0 && angle < 180)
                {
                    iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "speed", 30f, "oncomplete", "setEnterTrue", "oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
                }
                else if (angle >= 180 && angle < 360)
                {
                    iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 270f, 0f), "speed", 30f, "oncomplete", "setEnterTrue", "oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
                }
            }
			
			if(isItemsActive == true){
				Messenger.Broadcast("get itmes quite");
				isItemsActive = false;
			}
			
				
		}
		//else if(directWalkOnly){
			/*if(myTransform.rigidbody.velocity.z>0 && Input.GetAxis("Vertical")==0){
				ShoppingCart.parent = null;
				myTransform.rigidbody.velocity = Vector3.zero;
				myTransform.rigidbody.isKinematic = true;
				
				//myTransform.Translate(0, 0, 0);
				Debug.Log("run");
			}
			else{
				ShoppingCart.parent = myTransform;
				myTransform.rigidbody.velocity = Vector3.zero;
				//myTransform.rigidbody.isKinematic = true;
				//myTransform.rigidbody.isKinematic = false;
				//myTransform.Translate(0, 0, 0);&& myTransform.rigidbody.velocity == Vector3.zero
				
			}*/
			
		//}
			
				
		}
	}
	
	void FixedUpdate(){
        if (GameFlow.state == GameFlow.State.Tasks_doing)
        {

            if (directWalkOnly)
            {
                ShoppingCart.parent = null;

                //if(selectionTarget!=null&&(selectionModeLeft||selectionModeRight))
                //	ShoppingCart.position = new Vector3(myTransform.position.x + 1f,myTransform.position.y + 0.6334043f, selectionTarget.position.z);
                //	ShoppingCart.eulerAngles = new Vector3(0f,90f,0f);

            }
            else if (!isCheckout && (ShoppingCart.localRotation != Quaternion.identity))
            {
                //ShoppingCart.parent==null &&
                //ShoppingCart.rotation = myTransform.rotation;
                //ShoppingCart.position = new Vector3(myTransform.position.x,myTransform.position.y + 0.6334043f, myTransform.position.z);
                //ShoppingCart.position = ShoppingCart.position+myTransform.forward;
                myTransform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                myTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                myTransform.GetComponent<Rigidbody>().isKinematic = true;

                ShoppingCart.parent = myTransform.FindChild("items").transform;

                myTransform.GetComponent<Rigidbody>().isKinematic = false;

                //myTransform.rigidbody.velocity = new Vector3(0f, 0f, 0f);
                //myTransform.rigidbody.angularVelocity = Vector3.zero;

                //ShoppingCart.Translate(0, 0, 0);

                //Debug.Log("run away" + myTransform.rigidbody.velocity.x);
                //

                if (isItemsActive == true)
                {
                    Messenger.Broadcast("get itmes quite");
                    isItemsActive = false;
                }
                //myTransform.rigidbody.isKinematic = false;


                ShoppingCart.localPosition = Vector3.zero;

                if (directWalkEnterYet)
                {
                    iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", Vector3.zero,"islocal", true, "speed", 30f, "oncomplete", "setEnterFalse", "oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
                }
                else
                 {
                   ShoppingCart.localRotation = Quaternion.identity;
                }


            }


            /*if( Input.GetAxis("Vertical")==0f && myTransform.rigidbody.isKinematic == true && myTransform.rigidbody.velocity.x >0.0001f ){
			
                Messenger.Broadcast("get itmes quite");
                //ShoppingCart.
                //myTransform.rigidbody.isKinematic = false;	
                myTransform.rigidbody.velocity = new Vector3(0f,0f,0f);
                myTransform.rigidbody.angularVelocity = Vector3.zero;
                //myTransform.rigidbody.Sleep();
                float k = myTransform.rigidbody.velocity.z;
                float t = myTransform.rigidbody.velocity.x;
                    Debug.Log("run away"+k.ToString()+"kk" + myTransform.rigidbody.angularVelocity);
                }*/

        }
			
	}

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
        
            if (PickUpItems.state == PickUpItems.State.idle)
            {
                /*if (displayResponseButton)
              {
                  if (!isResponsing)
                  {
                      // response button. to be done
                      
                      if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Response", myStyle4))
                      {
                          isResponsing = true;
                          responseWindow.SetActive(true);
                      }
                  }
                  else
                  {
                        
                      if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Close response", myStyle4))
                      {
                          isResponsing = false;

                          UILabel inputTextLabel = GameObject.Find("InputText").GetComponent<UILabel>();
                          inputTextLabel.text = "";
                          responseWindow.SetActive(false);
                      }
                  }
              }*/

                /* check out buttons. to be done
                if (isCheckout && !isWalletDisplayed)
                {
                    if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Open Wallet", myStyle4))
                    {
                        isWalletDisplayed = true;
                        //Debug.Log("isWalletDisplayed == true");
                        walletWindow.SetActive(true);
                    }
                }
                else if (isCheckout && isWalletDisplayed)
                {
                    if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Close wallet", myStyle4))
                    {
                        walletWindow.SetActive(false);
                        isWalletDisplayed = false;
                        //Debug.Log("isWalletDisplayed == false");
                    }

                }
                if (isCheckout)
                {
                    if (GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Start" + "\n" + "check out", myStyle4))
                    {
                        isCheckoutStarted = true;
                    }
                    if (myGUI.totalAtHand >= myGUI.totalNeedToPay && myGUI.totalAtHand != 0)
                    {
                        if (!isPayingYet)
                        {
                            if (GUI.Button(new Rect(Screen.width / 2 + buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Pay", myStyle4))
                            {
                                isPayingYet = true;
                                Messenger<float>.Broadcast("generateChanges", myGUI.totalAtHand - myGUI.totalNeedToPay);
                            }
                        }
                        else
                        {
                            if (GUI.Button(new Rect(Screen.width / 2 + buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Put money" + "\n" + "in wallet", myStyle4))
                            {
                                Messenger.Broadcast("putChangesBackToWallet");
                                Messenger.Broadcast("finish money changes");
                            }
                        }
                    }

                }
                */

                

                // look at cart we dont need anymore
                /*if (ItemsInCartGUI.ItemsInCart.Count > 0f)
                {
                    if (!isLookingAtCart && !isCheckout)
                    {
                        //lookPos = Camera.main.transform.eulerAngles;
                        if (GUI.Button(new Rect(Screen.width / 2 + buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Look at cart", myStyle4))
                        {
                            changeLookAtMode();
                        }
                    }
                    else if (isLookingAtCart && Camera.main.transform.localRotation == Quaternion.identity)
                    {
                        isLookingAtCart = false;
                    }
                    else
                    {
                        if (!isCheckout && GUI.Button(new Rect(Screen.width / 2 + buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Look up", myStyle4))
                        {
                            //putDownItemToCompare();
                            //if (GUI.Button(new Rect(Screen.width-buttonWidth, Screen.height-buttonHeight, buttonWidth, buttonHeight), "Finish", myStyle4)){

                            Camera.main.transform.localRotation = Quaternion.identity;
                            isLookingAtCart = false;
                        }
                    }
                }*/
            }

            if (ItemsInCartGUI.ItemsInCart.Count > 0 && myTransform.position.x < selectionStart.position.x && !isCheckout)
            {
                //if(GUI.Button(new Rect(Screen.width-buttonWidth, Screen.height-buttonHeight, buttonWidth, buttonHeight), "Check out", myStyle4)){
                if(Input.GetButtonDown("A Button") )
                {
                    
                        closestPoint = findClosestScript.getClosestPoint();
                        isCheckout = true;
                        BoxCollider[] boxcolliders = shoppingList.GetComponentsInChildren<BoxCollider>();
                        foreach(BoxCollider boxcollider in boxcolliders)
                        {
                               boxcollider.enabled = false;
                        }
                        myTransform.GetComponent<Rigidbody>().isKinematic = true;
                        PickUpItems.state = PickUpItems.State.checkout;
                        Messenger.Broadcast("get itmes quite");
                        moveToCheckout();
                    
                }
            }

            if (isSelectionRegion == true)
            {
                directWalkOnly = true;

                if (!selectionModeLeft && !selectionModeRight)
                {
                    //if (Input.GetButtonDown("Back Button"))
                    //{
                    //    //  myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y+180f, myTransform.eulerAngles.z), Time.deltaTime*10f);
                    //    myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myTransform.eulerAngles.y + 180f, myTransform.eulerAngles.z);
                    //}

                    if ((ShoppingCart.eulerAngles.y >= 40f && ShoppingCart.eulerAngles.y <= 140f) || (ShoppingCart.eulerAngles.y >= 220f && ShoppingCart.eulerAngles.y <= 320f))
                    {

                        if ((camera.eulerAngles.y <= selectionEnterAngle || camera.eulerAngles.y >= 360f - selectionEnterAngle))
                        {
                            selectionModeLeft = true;
                            Messenger.Broadcast("Change state");
                        }
                        else if ((camera.eulerAngles.y >= 180f - selectionEnterAngle && camera.eulerAngles.y <= 180f + selectionEnterAngle))
                        {
                            selectionModeRight = true;
                            Messenger.Broadcast("Change state");

                        }

                    }
                    //if ((myTransform.eulerAngles.y >= 45 && myTransform.eulerAngles.y <= 135) || (myTransform.eulerAngles.y <= 315 && myTransform.eulerAngles.y >= 225))

                }
                //  else if (PickUpItems.state != PickUpItems.State.picked && PickUpItems.state != PickUpItems.State.compared && PickUpItems.state != PickUpItems.State.comparedThree && ((myTransform.eulerAngles.y < 5f && myTransform.eulerAngles.y > -5f) || (myTransform.eulerAngles.y < 185f && myTransform.eulerAngles.y > 175f)))
                // {
                else if (PickUpItems.state == PickUpItems.State.idle)
                {
                    //if(Input.GetButtonDown("Back Button"))
                    //{
                    //if(GUI.Button(new Rect(Screen.width-20f-buttonHeight, 20f, buttonHeight, buttonHeight),"", myStyle3)){

                    if (camera.eulerAngles.y > selectionEnterAngle && camera.eulerAngles.y < 180f - selectionEnterAngle)
                    {
                        if (ShoppingCart.eulerAngles.y >= 40f && ShoppingCart.eulerAngles.y <= 140f)
                        {
                            exitSelection();
                        }
                        else if (ShoppingCart.eulerAngles.y >= 220f && ShoppingCart.eulerAngles.y <= 320f)
                        {
                            ShoppingCart.eulerAngles = new Vector3(0f, 90f, 0f);
                            ShoppingCart.position = new Vector3(myTransform.position.x - 1.5f, myTransform.position.y + 0.8188018f, myTransform.position.z);
                            //myTransform.eulerAngles = new Vector3(0f, 90f, 0f);
                           // camera.Rotate(Vector3.up, 180f, Space.Self);
                            exitSelection();
                        }
                    }


                    if (camera.eulerAngles.y > 180f + selectionEnterAngle && camera.eulerAngles.y < 360f - selectionEnterAngle)
                    {
                        if (ShoppingCart.eulerAngles.y >= 220f && ShoppingCart.eulerAngles.y <= 320f)
                        {
                            exitSelection();
                        }
                        else if (ShoppingCart.eulerAngles.y >= 40f && ShoppingCart.eulerAngles.y <= 140f)
                        {
                            ShoppingCart.eulerAngles = new Vector3(0f, 270f, 0f);
                            ShoppingCart.position = new Vector3(myTransform.position.x + 1.5f, myTransform.position.y + 0.6334043f, myTransform.position.z);
                            //camera.Rotate(Vector3.up, 180f, Space.Self);
                            //myTransform.eulerAngles = new Vector3(0f, 270f, 0f);
                            exitSelection();
                        }
                    }


                }

            }


        
    }

    void Update()
    {
        //setAsilePoints();

        if (GameFlow.state == GameFlow.State.Tasks_doing)
        {

            // Xbox Controller update functions
            XboxController();

            // original update functions
            SelectionRegion();

            if (PickUpItems.state == PickUpItems.State.picked)
            {

                if (!selectionModeLeft && (myTransform.eulerAngles.y <= 90 || myTransform.eulerAngles.y > 270))
                {
                    selectionModeLeft = true;
                }
                else if (!selectionModeRight && (myTransform.eulerAngles.y > 90 && myTransform.eulerAngles.y <= 270))
                {
                    selectionModeRight = true;
                }
            }

            if (selectionModeLeft || selectionModeRight)
            {
                ShoppingCart.parent = null;
                if (selectionTarget != null)
                {
                    ShoppingCart.position = new Vector3(myTransform.position.x + 1f, myTransform.position.y + 0.6334043f, selectionTarget.position.z);
                }

                if (isItemsActive == false)
                {
                    Messenger.Broadcast("get itmes active");
                    isItemsActive = true;
                }

            }
            else
            {
                //if(ShoppingCart.parent == null)
                //ShoppingCart.parent = myTransform;

                //ShoppingCart.localPosition = new Vector3 (0f, 0.6f, 1f);

                //ShoppingCart.localRotation = Quaternion.identity;



            }
            //else{
            //ShoppingCart.localPosition = new Vector3 (0f, 0.6f, 1f);
            //if(selectionTarget!=null){
            //	if(ShoppingCart.position.z!=selectionTarget.position.z)
            //}

            //}

            // direct walke only, camera fixed
            if (!directWalkOnly && !isCheckout)
            {
                float translation = Input.GetAxis("Vertical") * speed;
                float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
                translation *= Time.deltaTime;
                rotation *= Time.deltaTime;
                if (translation == 0f && rotation == 0f && myTransform.GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    myTransform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                    myTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
                myTransform.Translate(0, 0, translation);
                myTransform.Rotate(0, rotation, 0);
            }
            else if (directWalkOnly && selectionTarget != null && !(PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree))
            {
                float translation = 0;
                if (selectionModeLeft || selectionModeRight)
                    translation = Input.GetAxis("Horizontal") * speed;
                else
                    translation = Input.GetAxis("Vertical") * speed;
                float rotation = Input.GetAxis("Mouse X") * rotationSpeed;
                // translationH *= Time.
                translation *= Time.deltaTime;
                rotation *= Time.deltaTime;
                if (translation == 0f && rotation == 0f && myTransform.GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    myTransform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                    myTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
                if (selectionModeLeft)
                {
                    //myTransform.Translate(translation, 0, 0);
                    if ((((selectionStart.position.x < myTransform.position.x && myTransform.position.x < selectionStart.position.x + 0.5f) || (selectionStart2.position.x < myTransform.position.x && myTransform.position.x < selectionStart2.position.x + 0.5f)) && translation < 0f)
                           || (((myTransform.position.x < selectionEnd.position.x && myTransform.position.x > selectionEnd.position.x - 0.5f) || (myTransform.position.x < selectionEnd2.position.x && myTransform.position.x > selectionEnd2.position.x - 0.5f)) && translation > 0f))
                    {
                        translation = 0f;
                    }

                    myTransform.Translate(translation, 0, 0, Space.World);
                }
                else if (selectionModeRight)
                {
                    if ((((selectionStart.position.x < myTransform.position.x && myTransform.position.x < selectionStart.position.x + 0.5f) || (selectionStart2.position.x < myTransform.position.x && myTransform.position.x < selectionStart2.position.x + 0.5f)) && translation > 0f)
                            || (((myTransform.position.x < selectionEnd.position.x && myTransform.position.x > selectionEnd.position.x - 0.5f) || (myTransform.position.x < selectionEnd2.position.x && myTransform.position.x > selectionEnd2.position.x - 0.5f)) && translation < 0f))
                    {
                        translation = 0f;
                    }
                    myTransform.Translate(translation, 0, 0, Space.World);
                }
                else
                {
                    myTransform.Translate(0, 0, translation);
                }

                myTransform.Rotate(0, rotation, 0);

                //myTransform.position = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z);
                Vector3 targetPos = new Vector3(myTransform.position.x, myTransform.position.y, selectionTarget.position.z);

                if (Mathf.Abs(Vector3.Distance(myTransform.position, targetPos)) < 0.5f)
                    myTransform.position = targetPos;
                else
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPos, Time.deltaTime);

                /*float angle = myTransform.eulerAngles.y;
                if (angle >= 1f && angle <= 179f)
                {
                    Vector3 target = new Vector3(0f, 90f, 0f);
                    if (Mathf.Abs(Vector3.Distance(myTransform.eulerAngles, target)) < 0.5f)
                    {
                        myTransform.eulerAngles = target;
                    }
                    else
                        myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, target, Time.deltaTime);
                        
                    //myTransform.eulerAngles = new Vector3(0f, 90f, 0f);
                    //Debug.Log("angle 90f");
                }
                else if (angle >= 181f && angle <= 359f)
                {
                    Debug.Log("angle 270f");
                    Vector3 target = new Vector3(0f, 270f, 0f);
                    if (Mathf.Abs(Vector3.Distance(myTransform.eulerAngles, target)) < 0.5f)
                    {
                        myTransform.eulerAngles = target;
                    }
                    else
                        myTransform.eulerAngles = Vector3.Lerp(myTransform.eulerAngles, target, Time.deltaTime);
                        
                    //myTransform.eulerAngles = new Vector3(0f, 270f, 0f);
                }*/

            }

            /*else if (isCheckout)
            {
                myTransform.rigidbody.isKinematic = true;
                float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
                rotation *= Time.deltaTime;
                myTransform.Rotate(0, rotation, 0);
            }*/

               
            /*else
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


            }*/
        }
    }
	
}
