using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIcontrol : GUIBasic {    
    private bool FocusChanged = false;
    public static bool isMenu = false;
    private int ButtonIndex = -1;
    private static int FocusID = -1;
    private float FocusTimer;

    public static bool displayResponseButton = false;
    public static bool isResponsing = false;
    public GameObject responseWindow;
    public GameObject walletWindow;

    public static bool isCheckoutStarted = false;
    public bool isPayingYet = false;
    public static bool isWalletDisplayed = false;

	public GUIStyle AButtonStyle;
	public GUIStyle BButtonStyle;
	public GUIStyle MouseButtonStyle;
	public GUIStyle XButtonStyle;
    public GUIStyle ZButtonStyle;

    private bool centerCursor = false;

	public bool returnWrongChange = false;

	private bool aButtonDelay = false;
	private bool xButtonDelay = false;

    private static List<string> ButtonNames = new List<string>();
	// Use this for initialization
	void Start () {
        if (responseWindow == null)
            responseWindow = GameObject.Find("ResponseWindow");
        if (walletWindow == null)
            walletWindow = GameObject.Find("Wallet Window");
        responseWindow.SetActive(false);
        walletWindow.SetActive(false);
	}
    void OnEnable()
    {
        Messenger.AddListener("Change state", changeState);
        Messenger.AddListener("display response", displayResponseButtonFuc);
        Messenger.AddListener("open response", openResponse);
        Messenger.AddListener("close response", closeResponse);
        Messenger.AddListener("Open wallet", openWallet);
		Messenger.AddListener("Close wallet", closeWallet);
        Messenger.AddListener("Start check out", startCheckout);
        Messenger.AddListener("Pay", pay);
		Messenger.AddListener("return wrong change", returnWrongChangeFn);
		Messenger.AddListener("return right change", returnRightChangeFn);

    }
    void OnDisable()
    {
        Messenger.RemoveListener("Change state", changeState);
        Messenger.RemoveListener("display response", displayResponseButtonFuc);
        Messenger.RemoveListener("open response", openResponse);
        Messenger.RemoveListener("close response", closeResponse);
        Messenger.RemoveListener("Open wallet", openWallet);
		Messenger.RemoveListener("Close wallet", closeWallet);
        Messenger.RemoveListener("Start check out", startCheckout);
        Messenger.RemoveListener("Pay", pay);
		Messenger.RemoveListener("return wrong change", returnWrongChangeFn);
		Messenger.RemoveListener("return right change", returnRightChangeFn);
    }
    

    void pay()
    {
        if (!isPayingYet)
        {
            isPayingYet = true;
			float changeToBeReturned = myGUI.totalAtHand - myGUI.totalNeedToPay;

			if(returnWrongChange){
				Messenger<float>.Broadcast("generateChanges", changeToBeReturned-1.25f);
			}else{
				Messenger<float>.Broadcast("generateChanges", changeToBeReturned);
			}
        }
        else 
        {
            Messenger.Broadcast("putChangesBackToWallet");
            Messenger.Broadcast("finish money changes");
        }
    }
    void startCheckout()
    {
        isCheckoutStarted = true;
    }

	void returnWrongChangeFn()
	{
		returnWrongChange = true;
	}

	void returnRightChangeFn()
	{
		returnWrongChange = false;
	}

    void openWallet()
    {   
		if (!isWalletDisplayed)
        {
            isWalletDisplayed = true;
            walletWindow.SetActive(true);
        }
        else
        {
            walletWindow.SetActive(false);
            isWalletDisplayed = false;
        }
    }

	void closeWallet()
	{   
		walletWindow.SetActive(false);
	}

    void displayResponseButtonFuc()
    {
        displayResponseButton = true;
    }
    void openResponse()
    {
        isResponsing = true;
        responseWindow.SetActive(true);
        //isMenu = false;
        //FocusID = -1;
    }
    void closeResponse()
    {
        GameObject input = GameObject.Find("InputText");
        if(input)
        {
            UILabel inputTextLabel = input.GetComponent<UILabel>();
            if(inputTextLabel)
                inputTextLabel.text = "";
        }
        
        //displayResponseButton = false;
        isResponsing = false;
        responseWindow.SetActive(false);
    }
    void changeState()
    {
        if (isMenu)
        {
            isMenu = false;
            FocusID = -1;
            ButtonNames.Clear();
        }
        centerCursor = true;
        Messenger<bool>.Broadcast("display pick up hint", false);
    }
    void OnGUI()
    { 
        GUI.skin = myskin; 

        if (GameFlow.state == GameFlow.State.Tasks_doing)
        {
             
            switch (PickUpItems.state)
            {
                case PickUpItems.State.idle:
                    
//					if(InputManager.TasksDictionary["dairy_Store_Fat Free Grade A Milk_1 Gallon_LOD0"].TaskType == Task.TaskTypes.Nutrition_task)
//					{
//						ButtonNames.Add("Respond");
//						GUI.SetNextControlName("Respond");
//						if (GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Response"))
//						{
//							Messenger.Broadcast("Respond to nutrition task");
//						}
//					}
                   // if (Movement.directWalkOnly){
                       
                        /*if (!Movement.selectionModeLeft && !Movement.selectionModeRight)
                        {
                            FocusChanged = true;
                            ButtonNames.Clear();
                            ButtonNames.Add("TurnAround");
                            GUI.SetNextControlName("TurnAround");
                            if (GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Turn around"))
                            {
                                Messenger.Broadcast("TurnAround");
                            }

                            //FocusID = ManageFocus(FocusID, ButtonNames.Count);
                        }*/

                        /*if (displayResponseButton)
                        {
                            if (!isResponsing)
                            {
                                //if (Movement.selectionModeLeft || Movement.selectionModeRight)
                                ButtonNames.Clear();
                                ButtonNames.Add("open response");
                                GUI.SetNextControlName("open response");
                                // response button. to be done
                                if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Response"))
                                {
                                    openResponse();
                                }
                            }
                            else
                            {
                                //if (Movement.selectionModeLeft || Movement.selectionModeRight)
                                ButtonNames.Clear();
                                ButtonNames.Add("close response");
                                GUI.SetNextControlName("close response");
                                if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Close response"))
                                {
                                    closeResponse();
                                }
                            }
                        }

                        FocusID = ManageFocus(FocusID, ButtonNames.Count);
                        */
                        /*else 
                        {
                            FocusChanged = true;
                            ButtonNames.Clear();
                            ButtonNames.Add("exit selection");
                            GUI.SetNextControlName("exit selection");
                            if ((GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Exit selection")))
                            {
                                Messenger.Broadcast("exit selection");
                            }
                            FocusID = ManageFocus(FocusID, ButtonNames.Count);
                        }*/
                    //}


					//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                 to bill items", AButtonStyle))
					//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Recenter view", MouseButtonStyle))
					//{
						// Call this function mentioned below if you want to rotate the camera back to zero
					//	Messenger.Broadcast("rotate back to zero");
					//}
                    break;
                case PickUpItems.State.picked:
                     FocusChanged = true;
                     ButtonNames.Clear();
                     
					 /*ButtonNames.Add("Rotate");
                     GUI.SetNextControlName("Rotate");
                     if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth * 2f - 2 * offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Rotate"))
                     {
                         Messenger<int>.Broadcast("Rotate", 0);
                     }

                     ButtonNames.Add("Cart");
                     GUI.SetNextControlName("Cart");
                     if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth * 1f - offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Cart"))
                     {
                         Messenger<int>.Broadcast("Cart", 0);
                     }*/

					if(InputManager.TasksDictionary.ContainsKey("cereal_Kellogg's_Raisin Bran_23.5 Oz_LOD0") || InputManager.TasksDictionary.ContainsKey("soup_Cambell's_Harvest Tomato Soup_18.7oz_LOD0"))
					{
						if(InputManager.TasksDictionary["cereal_Kellogg's_Raisin Bran_23.5 Oz_LOD0"].TaskType == Task.TaskTypes.Nutrition_task || InputManager.TasksDictionary["soup_Cambell's_Harvest Tomato Soup_18.7oz_LOD0"].TaskType == Task.TaskTypes.Nutrition_task)
						{
                            //ButtonNames.Add("Compare");
                            //GUI.SetNextControlName("Compare");
                            //if (GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Compare"))
                            //if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to put in cart", AButtonStyle)))
                            //{
                            //    Messenger<int>.Broadcast("Put in cart", 0);
                            //}
                            //if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back", BButtonStyle))
                            //{
                            //    Messenger<int>.Broadcast("Put back", 0);
                            //}

                            //if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to Zoom", ZButtonStyle))
                            //{
                            //    //Messenger<int>.Broadcast("Zoom", 0);
                            //}
                            //if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.88f, buttonWidth, buttonHeight), "Press                 to compare", XButtonStyle))
                            //{
                            //    Messenger<int>.Broadcast("Compare", 0);
                            //}

                        }
                    }
                    else if(InputManager.TasksDictionary.ContainsKey("cheese_Kraft_2% Milk Sharp Cheddar Cheese Singles_16 Slices_LOD0"))
					{
						if(InputManager.TasksDictionary["cheese_Kraft_2% Milk Sharp Cheddar Cheese Singles_16 Slices_LOD0"].TaskType == Task.TaskTypes.UnitPriceComparison_task)
						{
                            //ButtonNames.Add("Compare");
                            //GUI.SetNextControlName("Compare");
                            //if (GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Compare"))
                            //if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.88f, buttonWidth, buttonHeight), "Press                 to compare", XButtonStyle))
                            //{
                            //    Messenger<int>.Broadcast("Compare", 0);
                            //}
                            //if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back", BButtonStyle))
                            //{
                            //   Messenger<int>.Broadcast("Put back", 0);
                            //}

                        }
					}

                     /*ButtonNames.Add("Put back");
                     GUI.SetNextControlName("Put back");
                     if ((GUI.Button(new Rect(Screen.width / 2 + buttonWidth * 1f + offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Put back")))
                     {
                         Messenger<int>.Broadcast("Put back", 0);
                     }*/
                     FocusID = ManageFocus(FocusID, ButtonNames.Count);
                     break;

                case PickUpItems.State.checkout:
                        FocusChanged = false;
						GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to select/deselect money for payment", AButtonStyle);
						GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.90f, buttonWidth, buttonHeight), "Press                 to pay when you have your money selected", XButtonStyle);
                        ButtonNames.Clear();
                        /*if (!isWalletDisplayed)
                        {
					GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                 to open wallet", AButtonStyle);
                         /*   ButtonNames.Add("Open wallet");
                            GUI.SetNextControlName("Open wallet");
						//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                 to open wallet", AButtonStyle))
						  if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Click to open wallet", MouseButtonStyle))
							{
                                openWallet();
                            }
							/*if(Input.GetButtonDown("A Button")){
						openWallet();
							}*/
                        /*}
                        else
                        {
					GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                 to close wallet", BButtonStyle);
                           /* ButtonNames.Add("Open wallet");
                            GUI.SetNextControlName("Open wallet");
							//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                 to close wallet", BButtonStyle))
							if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Click to close wallet", MouseButtonStyle))
							{
                                openWallet();
                            }
							/*if(Input.GetButtonDown("B Button")){
								openWallet();
							}*/
                     //   }

                      /*  if (!isCheckoutStarted)
                        {
                            //ButtonNames.Add("Start check out");
                            //GUI.SetNextControlName("Start check out");
							//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                 to bill items", AButtonStyle))
							if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Click to bill items", MouseButtonStyle))
							{
						        // Call this function mentioned below if you want to start the checkout
                                startCheckout();
                            }
                        }
                            if (myGUI.totalAtHand >= myGUI.totalNeedToPay && myGUI.totalAtHand != 0)
                            {
                                if (!isPayingYet)
                                {
                                    ButtonNames.Add("Pay");
                                    GUI.SetNextControlName("Pay");
									//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Pay", AButtonStyle))
									if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Click to Pay", MouseButtonStyle))
									{
                                        pay();
                                    }
									if(Input.GetButtonDown("A Button")){
										pay();
									}
                                }
                                else
                                {
                                    ButtonNames.Add("Pay");
                                    GUI.SetNextControlName("Pay");
									//if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to put "+"\n"+"                          change in wallet", BButtonStyle))
									if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Click to put "+"\n"+" change in wallet", MouseButtonStyle))
									{
                                        pay();
                                    }
									if(Input.GetButtonDown("B Button")){
										pay();
									}
                                }
                            }*/
                            //FocusID = ManageFocus(FocusID, ButtonNames.Count);
                     break;

            }

            
        }
    }



    void pressMenu()
    {
        if (!isMenu)
        {
            if (Input.GetButtonDown("X Button"))
            {
                isMenu = true;
                FocusID = 0;
                Debug.Log("FocusID" + FocusID);
            }
        }
        else
        {
            if (Input.GetButtonDown("X Button"))
            {
                isMenu = false;
                FocusID = -1;
                Debug.Log("FocusID" + FocusID);
            }
        }

        if (Input.GetButtonDown("A Button"))
        {
            if (FocusID < 0 || FocusID >= ButtonNames.Count)
                return;
            else
            {
                switch (PickUpItems.state)
                {
                    case PickUpItems.State.idle:
                       // if (displayResponseButton)
                        //{
                           // Messenger.Broadcast(ButtonNames[FocusID]);
                        //}
                       /* if (Movement.directWalkOnly)
                        {
                            if (!Movement.selectionModeLeft && !Movement.selectionModeRight)
                            {
                                Messenger.Broadcast(ButtonNames[FocusID]);
                                Debug.Log("turn around happens");
                            }*/
                            //else
                            //{
                                //Messenger.Broadcast(ButtonNames[FocusID]);
                            //}
                       // }
                        break;
                    case PickUpItems.State.picked:
                        Messenger<int>.Broadcast(ButtonNames[FocusID], 0);
                        break;
                    case PickUpItems.State.checkout:
                        Messenger.Broadcast(ButtonNames[FocusID]);
                        break;
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (!Movement.isCheckout && centerCursor) { 
            Screen.lockCursor = true;
            Screen.lockCursor = false;
            Cursor.visible = true;
            centerCursor = false;
        }
        if (GameFlow.state == GameFlow.State.Tasks_doing)
        {
            switch (PickUpItems.state)
            {
                case PickUpItems.State.idle:
                    //if (Input.GetAxis("Right Trigger") ==1f )
                    //{
                    //    if (!isResponsing)
                    //    {
                    //        isResponsing = true;
                    //        openResponse();
                    //        Debug.Log("open response" + Input.GetAxis("Right Trigger"));
                    //    }
                    //    else 
                    //    {
                    //        isResponsing = false;
                    //        closeResponse();
                    //        Debug.Log("close response" + Input.GetAxis("Right Trigger"));
                    //    }
                    //}
                    
                    
                   // if (Movement.directWalkOnly)
                   // {
                        /*if (!Movement.selectionModeLeft && !Movement.selectionModeRight)
                        {
                                pressMenu();
                        }
                        else
                        {*/
                           // if (displayResponseButton)
                         //   {
                         //       pressMenu();
                         //   }
                        //}
                   // }
                    break;
                case PickUpItems.State.picked:
                       //pressMenu();
						if(InputManager.TasksDictionary.ContainsKey("cereal_Kellogg's_Raisin Bran_23.5 Oz_LOD0") || InputManager.TasksDictionary.ContainsKey("soup_Cambell's_Harvest Tomato Soup_18.7oz_LOD0")){
							if(InputManager.TasksDictionary["cereal_Kellogg's_Raisin Bran_23.5 Oz_LOD0"].TaskType == Task.TaskTypes.Nutrition_task || InputManager.TasksDictionary["soup_Cambell's_Harvest Tomato Soup_18.7oz_LOD0"].TaskType == Task.TaskTypes.Nutrition_task)
							{
								if (Input.GetButtonDown("X Button"))
	            				{
									Debug.Log ("Compare button pressed");
									Messenger<int>.Broadcast("Compare", 0);
								}
							}
						}
						if(InputManager.TasksDictionary.ContainsKey("cheese_Kraft_2% Milk Sharp Cheddar Cheese Singles_16 Slices_LOD0")){
							if(InputManager.TasksDictionary["cheese_Kraft_2% Milk Sharp Cheddar Cheese Singles_16 Slices_LOD0"].TaskType == Task.TaskTypes.UnitPriceComparison_task)
							{
								if (Input.GetButtonDown("X Button"))
								{
									Messenger<int>.Broadcast("Compare", 0);
								}
							}
						}
                    break;
			case PickUpItems.State.checkout:
				FocusChanged = false;
				ButtonNames.Clear();
				if(!aButtonDelay && !Input.GetButtonDown("A Button"))
					aButtonDelay = !aButtonDelay;

				if(!xButtonDelay && !Input.GetButtonDown("X Button"))
					xButtonDelay = !xButtonDelay;
				//if(aButtonDelay)
				//{
				if (!isWalletDisplayed)
				{
					//ButtonNames.Add("Open wallet");
					//GUI.SetNextControlName("Open wallet");

					if(isCheckoutStarted){
						openWallet();
					}
			//	}
			//	else
			//	{
					//ButtonNames.Add("Open wallet");
					//GUI.SetNextControlName("Open wallet");
			//		if(Input.GetButtonDown("B Button")){
			//			openWallet();
			//		}
			//	}
				}
				if (!isCheckoutStarted)
				{
					//startCheckout();
					/*if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth + 50 + buttonWidth + 50, Screen.height * 0.80f, buttonWidth, buttonHeight), "Click to bill items", MouseButtonStyle))
					{
						// Call this function mentioned below if you want to start the checkout
						startCheckout();
					}*/
				}
				if(xButtonDelay && Input.GetButtonDown("X Button") && myGUI.totalAtHand >= myGUI.totalNeedToPay && myGUI.totalAtHand != 0)
				{
					pay();
				}
				/*if (myGUI.totalAtHand >= myGUI.totalNeedToPay && myGUI.totalAtHand != 0)
				{
					if (!isPayingYet)
					{
						ButtonNames.Add("Pay");
						GUI.SetNextControlName("Pay");

						if(Input.GetButtonDown("A Button")){
							pay();
						}
					}
					else
					{
						ButtonNames.Add("Pay");
						GUI.SetNextControlName("Pay");

						if(Input.GetButtonDown("B Button")){
							pay();
						}
					}
				}*/
				break;
            }


        }
	
	}


    // set boundary for focus
    // use a arraylist to store the focus control name
    int ManageFocus(int ID, int Length)
    {
//        Debug.Log("ID" + ID + "Button count" + ButtonNames.Count);
        foreach( string s in ButtonNames)
        {
//            Debug.Log(s);
        }
        if ( (ID < 0 || ID >=ButtonNames.Count) )
        {
            if(!Movement.isCheckout)
                GUI.FocusControl("");
            return -1;
        }
        //&& Time.timeSinceLevelLoad > FocusTimer+0.2fFocusTimer=Time.timeSinceLevelLoad;
        GUI.FocusControl(ButtonNames[ID]);
        if (FocusChanged && Time.timeSinceLevelLoad>FocusTimer + 0.2f) { FocusChanged = false; FocusTimer = Time.timeSinceLevelLoad; }
        //if (FocusChanged) { FocusChanged = false; }
        //if ((Input.GetAxis("Menu X") > 0  && ID < Length && !FocusChanged) || (cInput.GetAxis("Vertical Move") > 0  ID < Lenght  !FocusChanged)) {FocusChanged=true; ID++;} else
        //if ((Input.GetAxis("Menu X") > 0 && ID < Length-1 && !FocusChanged)) { FocusChanged = true; ID++; }
        //else
        //    if ((Input.GetAxis("Menu X") > 0 && ID < 0 && !FocusChanged)) { ID = 0; }
        ////if ((Input.GetAxis("Menu X") < 0  && ID > 0 && !FocusChanged) || (cInput.GetAxis("Vertical Move") < 0  ID > 0  !FocusChanged)) {FocusChanged=true; ID--;} else
        //if ((Input.GetAxis("Menu X") < 0 && ID > 0 && !FocusChanged)) { FocusChanged = true; ID--; }
        //else
        //    if ((Input.GetAxis("Menu X") < 0 && ID < 0 && !FocusChanged)) { ID = 0; }
        return ID;
    }
}


