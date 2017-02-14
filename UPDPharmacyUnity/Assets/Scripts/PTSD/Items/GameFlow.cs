using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;


public class GameFlow : GUIBasic
{
	
	public enum State
	{
		//None,
		Intro_page,
		Tasks_page,
		Tasks_doing,
		Tasks_timeout,
		Finish,
		Tutorial
	}
	public static State state;
	
	public GameObject IntroWindow;
	public static GameObject TasksWindow;
	public static GameObject TaskWindow;
	public GameObject HelpWindow;
	
	public GUIStyle AButtonStyle;
	public GUIStyle BButtonStyle;
    public GUIStyle XButtonStyle;
	public GUIStyle MouseButtonStyle;
	
	public GUIStyle VH1Style;
	public GUIStyle VH2Style;
	public GUIStyle VH3Style;
	public GUIStyle VH4Style;
	
	private bool displayTaskWindow = false;
	private bool displayCorrectItem = false;
	private bool displayCorrectPrice = false;
	private bool displayFinishedMoney = false;
	private bool correctChange = false;
	private bool displayEndOfTasks = false;
	private bool displayConfirmCheckout = false;
	private bool displayConfirmCheckoutYes = false;
	private bool displayForNutritionTaskResponse = false;
	private bool displayForNutritionTaskResponseNo = false;
	private bool displayCashierOptions = false;
	private bool displayForCashierOptionsExtra = false;
	private bool displayCheckoutInfo = false;
	private bool displayEndOfNutritionTask = false;
	private bool displayNutritionResponse = false;
	private bool correctNutritionItem = false;
	private bool displayForEndOfUnitPriceComparisonTaskList= false;
	private bool displayForUnitPriceComparisonResponse = false;
	private bool rightUnitPriceItemChosen = false;

	private bool correctPrice = false;
	private bool correctItem = false;
	private bool comparisonTask = false;
	
	private static string _taskNameCurrent = "";
	public static Task nextTask = new Task();
	
	private static Transform itemAtHand = null;
	private bool helpWindowIsActive = false;
	
	public bool showTutorial = false;
	public int currentTutorialStep = -1;

	public static bool tutorialShown = false;
	public static bool appropriate_cashier_set = false;

	public List<GameObject> Overlays;

	public bool movementSatisfied = false;
	public bool lookSatisfied = false;
	public bool leftRightSatisfied = false;
	public bool abuttonPressed = false;
	public bool bbuttonPressed = false;
	public bool xbuttonPressed = false;
	public bool ybuttonPressed = false;
	public bool zoomPressed = false;

	private bool lookLeft = false;
	private bool lookRight = false;
	private bool lookUp = false;
	private bool lookDown = false;
	private float lastX = 0.0f;
	private float lastY = 0.0f;
	private float distanceLeft = 0.0f;
	private float distanceRight = 0.0f;
	private float distanceUp = 0.0f;
	private float distanceDown = 0.0f;
	private bool turnLeft = false;
	private bool turnRight = false;
	private bool moveForward = false;
	private bool moveBackward = false;
	private bool zoomIn = false;
	//private bool inputAllowed = true;

	private bool aButtonDelay = false;
	public int chosenCheckout = 1;

	private bool hasPlayedClip = false;
	public GameObject TutorialAudio;

	public AudioClip baby_crying_audio_clip;
	private AudioSource baby_crying_audio_source;
	public AudioClip bottle_breaking_audio_clip;
	private AudioSource bottle_breaking_audio_source;
	public AudioClip thunder_audio_clip;
	private AudioSource thunder_audio_source;
	private bool enableBabyCrying = false;
	private bool enableBottleBreaking = false;
	private bool enableThunder = false;

	void Awake()
	{
		//GUIBasic basic = new GUIBasic();
		//myStyle4 = basic.myStyle;
		
		
		if (IntroWindow == null)
			IntroWindow = GameObject.Find("IntroWindow");
		if (TasksWindow == null)
			TasksWindow = GameObject.Find("TasksWindow");
		if (TaskWindow == null)
			TaskWindow = GameObject.Find("TaskWindow");
		if (HelpWindow == null)
			HelpWindow = GameObject.Find("HelpWindow");

		Overlays = new List<GameObject> ();
		Overlays.Add (GameObject.Find ("HelpSpriteOverlayButton"));
		Overlays.Add (GameObject.Find ("HelpSpriteOverlayStick"));
		Overlays.Add (GameObject.Find ("HelpSpriteOverlayMap"));

		tutorialShown = showTutorial;

		if(tutorialShown && TutorialAudio == null)
			TutorialAudio = GameObject.Find("TutorialAudio");
        //		float adjust = Screen.width / 8;
//		Vector3 position = GameObject.Find ("HelpSpriteOverlayMap").GetComponent<UISprite> ().transform.localPosition;// += Screen.width / 8;
//		position.x += 50;
//		GameObject.Find ("HelpSpriteOverlayMap").GetComponent<UISprite> ().transform.localPosition = position;

		//Rect r = new Rect(Screen.width - Screen.width/8 - 10 , 100, Screen.width/8, Screen.width/8);
		/*if(HelpOverlayButton == null)
			HelpOverlayButton = GameObject.Find ("HelpSpriteOverlayButton");
		if (HelpOverlayStick == null)
			HelpOverlayStick = GameObject.Find ("HelpSpriteOverlayStick");*/

		//Debug.Log (HelpOverlayButton.transform.name);
		//HelpOverlay.GetComponent<UISprite> ().enabled = false;
	}
	
	// Use this for initialization
	void Start()
	{
		IntroWindow.SetActive(true);
		TasksWindow.SetActive(false);
		TaskWindow.SetActive(false);
		//HelpWindow.SetActive(false);
		Screen.lockCursor = true;
		Screen.lockCursor = false;
		foreach (Task task in InputManager.TasksDictionary.Values)
		{
			if (task!=null)
				nextTask = task;
			else
				nextTask = null;
			break;
		}
		//Debug.Log(nextTask.TaskGoal);
		//state = State.None;

		// Add an audio source to the game object
		baby_crying_audio_source = (AudioSource) gameObject.AddComponent<AudioSource>();
		bottle_breaking_audio_source = (AudioSource) gameObject.AddComponent<AudioSource>();
		thunder_audio_source = (AudioSource) gameObject.AddComponent<AudioSource>();
		baby_crying_audio_source.volume = 6.0f;
		bottle_breaking_audio_source.volume = 6.0f;
		thunder_audio_source.volume = 6.0f;
		baby_crying_audio_source.priority = 255;
		bottle_breaking_audio_source.priority = 255;
		thunder_audio_source.priority = 255;
	}
	
	
	void OnEnable()
	{
		Messenger<string, string>.AddListener("task trigger", OnTaskTrigger);
		Messenger<string>.AddListener("task trigger end", OnTaskTriggerEnd);
		Messenger<Transform>.AddListener("put item into cart", OnItemIntoCart);
		Messenger<string, string, string>.AddListener("finish response", finishResponse);
		Messenger.AddListener("finish money changes", finishMoney);
		Messenger.AddListener("user wants to checkout", confirmCheckout);
		Messenger.AddListener("Respond to nutrition task", nutritionTaskResponse);
		Messenger.AddListener("play breaking bottle audio", triggerBottleBreakingAudio);
		Messenger<float, float, float>.AddListener("MoveAround", CameraMoved);
		Messenger<float, float>.AddListener ("LookAround", HeadMoved);
		Messenger<bool>.AddListener ("ZoomButton", ZoomPressed);
		Messenger.AddListener ("enable baby crying", EnableBabyCryingFn);
		Messenger.AddListener ("enable bottle breaking", EnableBottleBreakingFn);
		Messenger.AddListener ("enable thunder", EnableThunderFn);
	}

	void OnDisable()
	{
		Messenger<string, string>.RemoveListener("task trigger", OnTaskTrigger);
		Messenger<string>.RemoveListener("task trigger end", OnTaskTriggerEnd);
		Messenger<Transform>.RemoveListener("put item into cart", OnItemIntoCart);
		Messenger<string, string, string>.RemoveListener("finish response", finishResponse);
		Messenger.RemoveListener("finish money changes", finishMoney);
		Messenger.RemoveListener("user wants to checkout", confirmCheckout);
		Messenger.RemoveListener("Respond to nutrition task", nutritionTaskResponse);
		Messenger.RemoveListener("play breaking bottle audio", triggerBottleBreakingAudio);
		Messenger<float, float, float>.RemoveListener ("MoveAround", CameraMoved);
		Messenger<float, float>.RemoveListener ("LookAround", HeadMoved);
		Messenger<bool>.RemoveListener ("ZoomButton", ZoomPressed);
		Messenger.RemoveListener ("enable baby crying", EnableBabyCryingFn);
		Messenger.RemoveListener ("enable bottle breaking", EnableBottleBreakingFn);
		Messenger.RemoveListener ("enable thunder", EnableThunderFn);
	}
	void finishMoney()
	{
		//displayFinishedMoney = true;
	}

	void CameraMoved(float vertical, float lookLeftRight, float horizontal)
	{
		if (vertical > 0)
						moveForward = true;
				else if (vertical < 0)
						moveBackward = true;

		if (horizontal > 0)
						turnRight = true;
				else if (horizontal < 0)
						turnLeft = true;

		if (lookLeftRight > 0)
						lookRight = true;
				else if (lookLeftRight < 0)
						lookLeft = true;

		if (moveForward && moveBackward && turnLeft && turnRight)
						movementSatisfied = true;
		//Debug.Log ("Move:" + vertical + ":" + horizontal + ":" + lookLeftRight);
		//Debug.Log ("Move: " + moveForward + " " + moveBackward + " " + turnLeft + " " + turnRight + " " + leftRightSatisfied);
	}

	void HeadMoved(float x, float y)
	{
		if (x > lastX) {
			distanceUp += (x - lastX);
			if(distanceUp > 0.05)
				lookUp = true;
		} else {
			distanceDown += (lastX - x);
			if(distanceDown > 0.05)
				lookDown = true;
		}
		lastX = x;
		/*Debug.Log (x + " " + y + " " + lastY + " " + distanceUp);
		if (y > lastY) {
			distanceUp += (y - lastY);
			if(distanceUp > 0.05)
						lookUp = true;
				} else {
			distanceDown += (lastY - y);
			if(distanceDown > 0.05)
						lookDown = true;
				}
		lastY = y;
*/
		if (lookLeft && lookRight && lookUp && lookDown)
						lookSatisfied = true;
		//Debug.Log ("Look:" + x + ":" + y);
		//Debug.Log ("look:" + lookUp + " " + lookDown + " " + leftRightSatisfied);
	}

	void ZoomPressed(bool zoom)
	{
		//Debug.Log (zoom + " " + zoomIn);
		if (zoomIn && zoom)
						zoomPressed = true;
			if(!zoom)
				zoomIn = true;


	}

	public void ResetTutorialGoals()
	{
		movementSatisfied = false;
		lookSatisfied = false;
		leftRightSatisfied = false;
		abuttonPressed = false;
		bbuttonPressed = false;
		xbuttonPressed = false;
		ybuttonPressed = false;
		zoomPressed = false;

		lookLeft = false;
		lookRight = false;
		lookUp = false;
		lookDown = false;
		lastX = 0.0f;
		lastY = 0.0f;
		distanceLeft = 0.0f;
		distanceRight = 0.0f;
		distanceUp = 0.0f;
		distanceDown = 0.0f;
		turnLeft = false;
		turnRight = false;
		moveForward = false;
		moveBackward = false;
		zoomIn = false;
	}

	void nutritionTaskResponse()
	{
		displayForNutritionTaskResponse = true;
	}
	
	void confirmCheckout()
	{
		displayConfirmCheckout = true;
	}
	
	void finishResponse(string currentTaskName, string comparisonTaskName, string priceS)
	{
		float price;
		float.TryParse(priceS, out price);
		Highlight HCompItem;
		
		GameObject itemCurrent = InputManager.AllPickObjectsDictionary[currentTaskName];
		if(comparisonTaskName != ""){
			GameObject comparisonItem = InputManager.AllPickObjectsDictionary[comparisonTaskName];
			HCompItem = comparisonItem.GetComponent<Highlight>();
		}else{
			GameObject comparisonItem = InputManager.AllPickObjectsDictionary[currentTaskName];
			HCompItem = comparisonItem.GetComponent<Highlight>();
		}	
		
		Highlight H = itemCurrent.GetComponent<Highlight>();
		
		displayCorrectPrice = true;
		
		// Get the current task and check if it is the price task or unit price task
		Task taskCurrent = InputManager.TasksDictionary[currentTaskName];
		
		Debug.Log("rightorwrongprice is "+ correctPrice + " because item price is "+ H.item.Price + " and entered price is "+ price);
		if (taskCurrent.TaskType == Task.TaskTypes.Price_task){
			Debug.Log("rightorwrongrpice is "+ correctPrice + " because item price is "+ H.item.Price + " and entered price is "+ price);
			
			if (H.item.Price == price)
			{
				correctPrice = true;
			}
			else
			{
				correctPrice = false;
			}
		}else if(taskCurrent.TaskType == Task.TaskTypes.Unitprice_task)
		{
			if (H.item.Unitprice == price)
			{
				correctPrice = true;
			}
			else
			{
				correctPrice = false;
			}
		}else if(taskCurrent.TaskType == Task.TaskTypes.PriceComparison_task)
		{
			if(H.item.Price < HCompItem.item.Price){
				if(priceS == "Item1" || priceS == "item1" || priceS == "Item 1" || priceS == "item 1"){
					correctPrice = true;
				}else{
					correctPrice = false;	
				}
			}else if(H.item.Price == HCompItem.item.Price){
				if(priceS == "Item1" || priceS == "item1" || priceS == "Item 1" || priceS == "item 1"){
					correctPrice = true;
				}else if(priceS == "Item2" || priceS == "item2" || priceS == "Item 2" || priceS == "item 2"){
					correctPrice = true;	
				}else{
					correctPrice = false;	
				}
			}else if(HCompItem.item.Price < H.item.Price){
				if(priceS == "Item2" || priceS == "item2" || priceS == "Item 2" || priceS == "item 2"){
					correctPrice = true;
				}else{
					correctPrice = false;	
				}
			}else{
				correctPrice = false;	
			}
			comparisonTask = true;
		}else if(taskCurrent.TaskType == Task.TaskTypes.UnitPriceComparison_task)
		{
			if(H.item.Unitprice < HCompItem.item.Unitprice){
				if(priceS == "Item1" || priceS == "item1" || priceS == "Item 1" || priceS == "item 1"){
					correctPrice = true;
				}else{
					correctPrice = false;	
				}
			}else if(H.item.Unitprice == HCompItem.item.Unitprice){
				if(priceS == "Item1" || priceS == "item1" || priceS == "Item 1" || priceS == "item 1"){
					correctPrice = true;
				}else if(priceS == "Item2" || priceS == "item2" || priceS == "Item 2" || priceS == "item 2"){
					correctPrice = true;	
				}else{
					correctPrice = false;	
				}
			}else if(HCompItem.item.Unitprice < H.item.Unitprice){
				if(priceS == "Item2" || priceS == "item2" || priceS == "Item 2" || priceS == "item 2"){
					correctPrice = true;
				}else{
					correctPrice = false;	
				}
			}else{
				correctPrice = false;	
			}
			comparisonTask = true;
		}
		
	}
	
	void responseRight(string currentTaskName) {
		Task taskCurrent = InputManager.TasksDictionary[currentTaskName];
		taskCurrent.TaskResult = true;
		if (taskCurrent.TaskType == Task.TaskTypes.Price_task || taskCurrent.TaskType == Task.TaskTypes.Unitprice_task)
			taskCurrent.TaskFinishTime = Timer.GetTimer;
		InputManager.OutputTasks.Add(taskCurrent);
		InputManager.TasksDictionary.Remove(currentTaskName);
	}
	
	void responseWrong(string currentTaskName)
	{
		Task taskCurrent = InputManager.TasksDictionary[currentTaskName];
		taskCurrent.TaskResult = false;
		if (taskCurrent.TaskType == Task.TaskTypes.Price_task || taskCurrent.TaskType == Task.TaskTypes.Unitprice_task)
			taskCurrent.TaskFinishTime = Timer.GetTimer;
		InputManager.OutputTasks.Add(taskCurrent);
		InputManager.TasksDictionary.Remove(currentTaskName);
	}
	
	void OnTaskTrigger(string taskName, string taskComparisonName)
	{
		if (!displayTaskWindow)
		{
			if (InputManager.TasksDictionary.ContainsKey(taskName))
			{
				Task taskCurrent = InputManager.TasksDictionary[taskName];
				_taskNameCurrent = taskName;
				if (taskCurrent.TaskType == Task.TaskTypes.Price_task)
				{
					displayTaskWindow = true;
					Messenger.Broadcast("display response");
					Messenger<string>.Broadcast("response manager task", taskName);
				}else if(taskCurrent.TaskType == Task.TaskTypes.Unitprice_task){
					displayTaskWindow = true;
					Messenger.Broadcast("display response");
					Messenger<string>.Broadcast("response manager unitprice task", taskName);
				}else if(taskCurrent.TaskType == Task.TaskTypes.PriceComparison_task){
					displayTaskWindow = true;
					Messenger.Broadcast("display response");
					Messenger<string, string>.Broadcast("response manager price comparison task", taskName, taskComparisonName);
				}else if(taskCurrent.TaskType == Task.TaskTypes.UnitPriceComparison_task){
					displayTaskWindow = true;
					Messenger.Broadcast("display response");
					Messenger<string, string>.Broadcast("response manager unit price comparison task", taskName, taskComparisonName);
				}
				
				//Debug.Log(_taskNameCurrent);
				//taskWindowCollisionBox.collider.enabled = false;
			}
		}
	}
	
	void OnTaskTriggerEnd(string taskName)
	{
		
		if (displayTaskWindow)
		{
			
			displayTaskWindow = false;
			TaskWindow.SetActive(false);
			_taskNameCurrent = "";
			// Debug.Log(_taskNameCurrent);
			//taskWindowCollisionBox.collider.enabled = true;
		}
	}
	
	void OnItemIntoCart(Transform itemPicked)
	{
		if (InputManager.TasksDictionary.ContainsKey(itemPicked.name) )
		{
			displayCorrectItem = true;
			correctItem = true;
			itemAtHand = itemPicked;
			//Messenger<Transform>.Broadcast("get the right item", itemPicked);
		}
		else
		{
			displayCorrectItem = true;
			correctItem = false;
			itemAtHand = itemPicked;
			//Messenger<Transform>.Broadcast("get the wrong item", itemPicked);
		}
	}
	
	
	void OnGUI() 
	{
		GUI.depth = 2;
		GUI.skin = myskin;

        //simplified code for GUI button display for game flow
        switch(state)
        {
            case State.Intro_page:
                GUI.Button(new Rect(Screen.width/2+2*buttonWidth,Screen.height*.8f,buttonWidth,buttonHeight), "Press                to Continue", AButtonStyle);
                break;
            case State.Tasks_page:
                GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                break;
            case State.Tasks_doing:
                if(displayTaskWindow)
                {
                    GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.50f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                }
                if(displayEndOfTasks)
                {
                    GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                }
                if(displayEndOfNutritionTask)
                {
                    GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.50f, buttonWidth, buttonHeight), "Press                for Item 1", AButtonStyle);
                    GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.56f, buttonWidth, buttonHeight), "Press                for Item 2", BButtonStyle);
                    if(GameObject.Find("Robot_Prefab").GetComponent<PickUpItems>().threeCompared)
                    {
                        GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.62f, buttonWidth, buttonHeight), "Press                for Item 3", XButtonStyle);
                    }
                }
                if (displayNutritionResponse)
                {
                    if (correctNutritionItem)
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                    }
                    else
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to try again", BButtonStyle);
                    }
                }
                if(displayForEndOfUnitPriceComparisonTaskList)
                {
                    GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click for Item 1", MouseButtonStyle);
                    GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click for Item 2", MouseButtonStyle);
                }
                if (displayForUnitPriceComparisonResponse)
                {
                    if (rightUnitPriceItemChosen)
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click to Continue", MouseButtonStyle);
                    }
                    else
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click to try again", MouseButtonStyle);
                        GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click to close", MouseButtonStyle);
                    }
                }

                if (displayCorrectItem)
                {
                    if (correctItem)
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.50f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                    }
                    else 
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to put " + "\n" + "                          into cart", AButtonStyle);
                        GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to put " + "\n" + "                          back", BButtonStyle);
                    }
                }

                if (displayCorrectPrice)
                {
                    if (correctPrice)
                    {
                        if (comparisonTask)
                        {
                            GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                        }
                        else
                        {
                            GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                        }
                    }
                    else
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, topButtonPos, buttonWidth, buttonHeight), "Press                for Yes", AButtonStyle);
                        GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, topButtonPos, buttonWidth, buttonHeight), "Press                for No", BButtonStyle);
                    }
                }

                if (displayFinishedMoney)
                {
                    if (!correctChange)
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, topButtonPos, buttonWidth, buttonHeight), "Press                for Yes", AButtonStyle);
                        GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, topButtonPos, buttonWidth, buttonHeight), "Press                for No", BButtonStyle);
                    }

                    else
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                    }

                }

                if (displayForNutritionTaskResponse)
                {
                    if (!displayForNutritionTaskResponseNo)
                    {
                        GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.72f, buttonWidth, buttonHeight), "Press                to item 1", AButtonStyle);
                        GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Item 2", BButtonStyle);
                    }
                    else
                    {
                        GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                    }
                }

                if (displayConfirmCheckout && !showTutorial)
                {
                    GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                }

                if (displayCheckoutInfo)
                {
                    GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                }

                if (displayCashierOptions)
                {
                    GUI.Button(new Rect(Screen.width / 6, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH1Style);
                    GUI.Button(new Rect(Screen.width / 3, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH2Style);
                    GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH3Style);
                    GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH4Style);
                }
                break;
            case State.Tutorial:
                if (currentTutorialStep == -1)
                {
                    GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                }
                break;
            case State.Finish:
                GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle);
                break;
        }


        //switch (state)
        //{
        //    case State.Intro_page:
        //        UILabel IntroLabel = GameObject.Find("IntroLabel").GetComponent<UILabel>();
        //        IntroLabel.text = InputManager.IntroText;
        //        //if()

        //        if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //        {
        //            IntroWindow.SetActive(false);
        //            TasksWindow.SetActive(true);
        //            Messenger.Broadcast("Change state");
        //            if (showTutorial)
        //            {
        //                state = State.Tutorial;
        //            }
        //            else {


        //                state = State.Tasks_page;
        //            }
        //        }

        //        break;
        //    case State.Tasks_page:
        //        UILabel TasksLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
        //        TasksLabel.text = InputManager.Scenario;
        //        if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //        {

        //            TasksWindow.SetActive(false);
        //            Messenger.Broadcast("Change state");
        //            state = State.Tasks_doing;

        //        }
        //        break;
        //    case State.Tasks_doing:
        //    case State.Tasks_timeout:
        //        if (displayTaskWindow)
        //        {

        //            if (_taskNameCurrent != "")
        //            {
        //                TaskWindow.SetActive(true);

        //                Task taskCurrent = InputManager.TasksDictionary[_taskNameCurrent];
        //                UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //                TaskLabel.text = taskCurrent.Description;
        //                if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.50f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayTaskWindow = false;
        //                }

        //            }
        //        }

        //        if (displayEndOfTasks)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            //if(!showTutorial)
        //            TaskLabel.text = "Congratulations! You got all the items on your shopping list.  \n \n Now you can either checkout the items you have shopped now or continue shopping for more items. To checkout, please go to the front of the store near the checkout counters and you will be prompted to checkout.";
        //            /*else
        //            {
        //                UILabel instructionsLabel = GameObject.Find ("InstructionsLabel").GetComponent<UILabel> ();
        //                instructionsLabel.text = "";
        //                TaskLabel.text = "Congratulations! You got all the items on your shopping list.  \n \n This concludes the tutorial.  If you'd like to play with the movement controls more, you may close this by hitting the [00ff00]A button[-] and do so.  Otherwise, please proceed to the next task as described by the therapist.";//To checkout, you'd normally need to approach the checkout counters and press the [00ff00]A button[-].  You're already close enough, so press the [00ff00]A button[-] to start checking out.";
        //                for(int i = 0; i < Overlays.Count; i++)
        //                {
        //                    Overlays[i].GetComponent<UISprite>().enabled = false;
        //                    Overlays[i].GetComponent<TweenAlpha>().enabled = false;
        //                }
        //            }*/
        //            if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayEndOfTasks = false;
        //                state = State.Tasks_doing;
        //            }
        //        }


        //        if (displayEndOfNutritionTask)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            TaskLabel.text = "You have successfully added all the items on your shopping list to your cart.  \n \n Which of the items had the least CALORIES?" +
        //                    "\n Press \"A\" if Kellog's Raisin-Bran Cereal had the least calories." +
        //                    "\n Press \"B\" if Kellog's All-Bran Cereal had the least calories.";
        //            if (GameObject.Find("Robot_Prefab").GetComponent<PickUpItems>().threeCompared)
        //                TaskLabel.text += "\n Press \"X\" if (insert third item to compare) had the least calories.";
        //            //if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth-30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                for Item 1",AButtonStyle)))
        //            if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.50f, buttonWidth, buttonHeight), "Press                for Item 1", AButtonStyle)) || Input.GetButtonDown("A Button"))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayEndOfNutritionTask = false;
        //                displayNutritionResponse = true;
        //                correctNutritionItem = false;
        //                state = State.Tasks_doing;
        //            }
        //            //if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                for Item 2", BButtonStyle))
        //            if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.56f, buttonWidth, buttonHeight), "Press                for Item 2", BButtonStyle) || Input.GetButtonDown("B Button"))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayEndOfNutritionTask = false;
        //                displayNutritionResponse = true;
        //                correctNutritionItem = true;
        //                state = State.Tasks_doing;
        //            }
        //            if (GameObject.Find("Robot_Prefab").GetComponent<PickUpItems>().threeCompared
        //                    && (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.62f, buttonWidth, buttonHeight), "Press                for Item 3", XButtonStyle) || Input.GetButtonDown("X Button")))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayEndOfNutritionTask = false;
        //                displayNutritionResponse = true;
        //                correctNutritionItem = false;
        //                state = State.Tasks_doing;
        //            }
        //        }

        //        if (displayNutritionResponse)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            if (correctNutritionItem)
        //            {
        //                TaskLabel.text = "Congratulations! You have successfully identified the item with a lower Sodium content. \n You are now done with this simulation. Thank you for trying out our simulation!";
        //                //if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue",AButtonStyle)))
        //                if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayNutritionResponse = false;
        //                    correctNutritionItem = false;
        //                    state = State.Tasks_doing;
        //                }
        //            }
        //            else {
        //                TaskLabel.text = "Sorry, that is not the item with a lower Sodium content. \n Please press B to try again";
        //                //if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth-30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to try again", AButtonStyle))
        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to try again", BButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayNutritionResponse = false;
        //                    correctNutritionItem = false;
        //                    displayEndOfNutritionTask = true;
        //                    state = State.Tasks_doing;
        //                }
        //            }
        //        }



        //        if (displayForEndOfUnitPriceComparisonTaskList)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            TaskLabel.text = "You have successfully added all the items on your shopping list to your cart.  \n \n Which of the two items had a lower UNIT PRICE?" +
        //                    "\n If Kraft 2% American Cheese Singles had a lower Unit Price then choose Item 1" +
        //                    "\n If Kraft 2% Sharp Cheddar Cheese Singles had a lower Unit Price then choose Item 2" +
        //                    "\n \n Please use the MOUSE to click on the correct choice";
        //            //if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth-30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                for Item 1",AButtonStyle)))
        //            if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click for Item 1", MouseButtonStyle)))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayForEndOfUnitPriceComparisonTaskList = false;
        //                displayForUnitPriceComparisonResponse = true;
        //                rightUnitPriceItemChosen = true;
        //                state = State.Tasks_doing;
        //            }
        //            //if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                for Item 2", BButtonStyle))
        //            if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click for Item 2", MouseButtonStyle))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayForEndOfUnitPriceComparisonTaskList = false;
        //                displayForUnitPriceComparisonResponse = true;
        //                rightUnitPriceItemChosen = false;
        //                state = State.Tasks_doing;
        //            }
        //        }

        //        if (displayForUnitPriceComparisonResponse)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            if (rightUnitPriceItemChosen)
        //            {
        //                TaskLabel.text = "Congratulations! You have successfully identified the item with a lower Unit Price. \n You are now done with this simulation. Thank you for trying out our simulation!";
        //                //if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue",AButtonStyle)))
        //                if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click to Continue", MouseButtonStyle)))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayForUnitPriceComparisonResponse = false;
        //                    rightUnitPriceItemChosen = false;
        //                    state = State.Tasks_doing;
        //                }
        //            }
        //            else {
        //                TaskLabel.text = "Sorry, that is not the item with a lower Unit Price. \n Would you like to try again?";
        //                //if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth-30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to try again", AButtonStyle))
        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click to try again", MouseButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayForUnitPriceComparisonResponse = false;
        //                    rightUnitPriceItemChosen = false;
        //                    displayForEndOfUnitPriceComparisonTaskList = true;
        //                    state = State.Tasks_doing;
        //                }
        //                //if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to close", BButtonStyle))
        //                if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Click to close", MouseButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayForUnitPriceComparisonResponse = false;
        //                    rightUnitPriceItemChosen = false;
        //                    state = State.Tasks_doing;
        //                }
        //            }
        //        }



        //        if (displayCorrectItem)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            if (correctItem)
        //            {
        //                TaskLabel.text = "You put the right item into the cart.";
        //                state = State.Tasks_timeout;
        //                if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.50f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayCorrectItem = false;
        //                    if (itemAtHand)
        //                    {
        //                        Messenger<Transform>.Broadcast("get the right item", itemAtHand);
        //                        Task taskCurrent = InputManager.TasksDictionary[itemAtHand.name];
        //                        if (taskCurrent.TaskType == Task.TaskTypes.Search_task)
        //                            taskCurrent.TaskFinishTime = Timer.GetTimer;
        //                        InputManager.OutputTasks.Add(taskCurrent);

        //                        InputManager.TasksDictionary.Remove(itemAtHand.name);
        //                        //InputManager.TasksDictionary[itemAtHand.name] = taskCurrent;
        //                        InputManager.updateShoppingList(itemAtHand.name);
        //                        foreach (Task task in InputManager.TasksDictionary.Values)
        //                        {
        //                            //if (task.TaskFinishTime == 0)
        //                            if (task != null)
        //                                nextTask = task;
        //                            else
        //                                nextTask = null;
        //                            break;
        //                        }
        //                        Debug.Log("Next Task Goal : " + nextTask.TaskGoal);
        //                        Debug.Log("Number of tasks left : " + InputManager.TasksDictionary.Count);

        //                        // Check if we are done with all the tasks in the task list. Then we can ask the user to go to checkout
        //                        if ((InputManager.TasksDictionary.Count == 0 || nextTask == null) && taskCurrent.TaskType == Task.TaskTypes.Nutrition_task)
        //                        {
        //                            //displayCorrectItem = false;
        //                            displayEndOfNutritionTask = true;
        //                        }
        //                        else if (InputManager.TasksDictionary.Count == 0 || nextTask == null)
        //                        {
        //                            displayEndOfTasks = true;
        //                        }
        //                        state = State.Tasks_doing;
        //                    }

        //                }
        //            }
        //            else {
        //                TaskLabel.text = "Are you sure you want this item? This isn¡¯t what you are looking for. ";
        //                state = State.Tasks_timeout;

        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to put " + "\n" + "                          into cart", AButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayCorrectItem = false;
        //                    if (itemAtHand)
        //                        Messenger<Transform>.Broadcast("get the right item", itemAtHand);
        //                    state = State.Tasks_doing;
        //                }

        //                if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to put " + "\n" + "                          back", BButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayCorrectItem = false;
        //                    if (itemAtHand)
        //                        Messenger<Transform>.Broadcast("get the wrong item", itemAtHand);
        //                    state = State.Tasks_doing;
        //                }
        //            }

        //        }

        //        if (displayCorrectPrice)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            if (correctPrice)
        //            {
        //                // First check for the price comparison task
        //                if (comparisonTask)
        //                {
        //                    TaskLabel.text = "Congratulations! You have correctly identified the item with the lower price.";
        //                    state = State.Tasks_timeout;
        //                    if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //                    {
        //                        TaskWindow.SetActive(false);
        //                        displayCorrectPrice = false;
        //                        responseRight(_taskNameCurrent);
        //                        Messenger.Broadcast("close response");
        //                        state = State.Tasks_doing;
        //                    }
        //                }
        //                else {

        //                    if (InputManager.TasksDictionary.Count > 1)
        //                        TaskLabel.text = "Congratulations! You have entered the right price. Proceed to next item on the shopping list.";
        //                    else
        //                        TaskLabel.text = "Congratulations! You have entered the right response. ";
        //                    state = State.Tasks_timeout;
        //                    Messenger.Broadcast("close response");
        //                    if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //                    {
        //                        TaskWindow.SetActive(false);
        //                        displayCorrectPrice = false;
        //                        responseRight(_taskNameCurrent);

        //                        state = State.Tasks_doing;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                TaskLabel.text = "That is not the answer we were looking for. Would you like to try again?";
        //                state = State.Tasks_timeout;
        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, topButtonPos, buttonWidth, buttonHeight), "Press                for Yes", AButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayCorrectPrice = false;
        //                    state = State.Tasks_doing;
        //                }
        //                if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, topButtonPos, buttonWidth, buttonHeight), "Press                for No", BButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayCorrectPrice = false;
        //                    responseWrong(_taskNameCurrent);
        //                    Messenger.Broadcast("close response");
        //                    state = State.Tasks_doing;
        //                }
        //            }

        //        }
        //        if (displayFinishedMoney)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            state = State.Tasks_timeout;

        //            if (!correctChange)
        //            {
        //                TaskLabel.text = "Are you satisfied that you got the right change?";
        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, topButtonPos, buttonWidth, buttonHeight), "Press                for Yes", AButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    displayFinishedMoney = false;
        //                    Messenger.Broadcast("write results");
        //                    state = State.Finish;
        //                }
        //                if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, topButtonPos, buttonWidth, buttonHeight), "Press                for No", BButtonStyle))
        //                {
        //                    correctChange = true;
        //                }
        //            }

        //            else
        //            {
        //                TaskLabel.text = "Sorry, I think you might have made a mistake with your math. Please check your math again.";
        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //                {
        //                    TaskWindow.SetActive(false);
        //                    Messenger.Broadcast("write results");
        //                    state = State.Finish;
        //                }
        //            }

        //        }

        //        if (displayForNutritionTaskResponse)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            state = State.Tasks_timeout;
        //            if (!displayForNutritionTaskResponseNo)
        //            {
        //                TaskLabel.text = "Which of the items has less SODIUM content? Item 1 or Item 2? Please choose from choices below";
        //                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.72f, buttonWidth, buttonHeight), "Press                to item 1", AButtonStyle))
        //                {
        //                    displayForNutritionTaskResponseNo = true;
        //                }
        //                else if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Item 2", BButtonStyle))
        //                {
        //                    TaskLabel.text = "Congratulations! You made the right choice of selecting the item with low SODIUM content.";
        //                    if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //                    {
        //                        state = State.Tasks_doing;
        //                        displayForNutritionTaskResponse = false;
        //                        TaskWindow.SetActive(false);
        //                    }
        //                }
        //            }
        //            else {
        //                TaskWindow.SetActive(false);
        //                TaskLabel.text = "I am sorry, that is not the item with low SODIUM content.";
        //                TaskWindow.SetActive(true);
        //                if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //                {
        //                    state = State.Tasks_doing;
        //                    displayForNutritionTaskResponse = false;
        //                    TaskWindow.SetActive(false);
        //                }
        //            }
        //        }

        //        if (displayConfirmCheckout && !showTutorial)
        //        {
        //            if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //            {

        //            }
        //            /*TasksWindow.SetActive(true);
        //            TasksLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
        //            state = State.Tasks_timeout;

        //            TasksLabel.text = "You have requested to checkout now. Once you proceed to checkout, you will not be able to shop anymore. Do you want to checkout now?" + "\n" + "\n" + " PLEASE PUT DOWN THE CONTROLLER AND USE THE MOUSE AND CLICK ON THE BUTTONS.";
        //            //if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                for Yes", AButtonStyle))
        //            if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset - 40, Screen.height * 0.80f, buttonWidth, buttonHeight), " Click for Yes ", MouseButtonStyle))
        //            {
        //                displayCheckoutInfo = true;
        //                TasksWindow.SetActive(false);
        //                displayConfirmCheckout = false;
        //                state = State.Tasks_doing;
        //            }
        //            //if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                for No", BButtonStyle))
        //            if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth - 30, Screen.height * 0.80f, buttonWidth, buttonHeight),  " Click for No ", MouseButtonStyle))
        //            {
        //                TasksWindow.SetActive(false);
        //                displayConfirmCheckout = false;
        //                state = State.Tasks_doing;
        //            }*/
        //        }

        //        if (displayCheckoutInfo)
        //        {
        //            if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //            {

        //            }
        //            /*TasksWindow.SetActive(true);
        //            TasksLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
        //            state = State.Tasks_timeout;

        //            TasksLabel.text = "You are now going to be moved to the checkout counter for checking out your items. During checkout you can: \n \n " +
        //                "1) Interact with the cashier by speaking out aloud. The cashier will respond to your questions and comments. \n" +
        //                    "2) Checkout items that you have shopped up until now. As each item is checked out, you can see how much it costs on the bill window.\n" +
        //                    "3) Pay for the items you have shopped with money in your wallet. Hover over the bills and coins in your wallet to view them and click to choose them. Once you" +
        //                    " have paid for the items, you will get back change if any to put back into your wallet.";
        //            //if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //            if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), " Click to Continue ", MouseButtonStyle))
        //            {
        //                TasksWindow.SetActive(false);
        //                displayCheckoutInfo = false;
        //                displayConfirmCheckoutYes = false;
        //                displayCashierOptions = true;
        //                state = State.Tasks_doing;
        //            }*/
        //        }

        //        if (displayCashierOptions)
        //        {
        //            //TaskWindow.SetActive(true);
        //            //UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            //state = State.Tasks_timeout;

        //            //TaskLabel.text = "Please choose a cashier to help you with checking out your items.";
        //            if (GUI.Button(new Rect(Screen.width / 6, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH1Style))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayCashierOptions = false;
        //                displayForCashierOptionsExtra = false;
        //                Messenger.Broadcast("checkout counter 1 chosen");
        //                Messenger.Broadcast("set move to checkout");
        //                state = State.Tasks_doing;
        //            }
        //            if (GUI.Button(new Rect(Screen.width / 3, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH2Style))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayCashierOptions = false;
        //                displayForCashierOptionsExtra = false;
        //                Messenger.Broadcast("checkout counter 2 chosen");
        //                Messenger.Broadcast("set move to checkout");
        //                state = State.Tasks_doing;
        //            }
        //            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH3Style))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayCashierOptions = false;
        //                displayForCashierOptionsExtra = false;
        //                Messenger.Broadcast("checkout counter 3 chosen");
        //                Messenger.Broadcast("set move to checkout");
        //                state = State.Tasks_doing;
        //            }
        //            if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height / 2, Screen.width / 6, Screen.height / 2.5f), "", VH4Style))
        //            {
        //                TaskWindow.SetActive(false);
        //                displayCashierOptions = false;
        //                displayForCashierOptionsExtra = false;
        //                Messenger.Broadcast("checkout counter 4 chosen");
        //                Messenger.Broadcast("set move to checkout");
        //                state = State.Tasks_doing;
        //            }
        //        }

        //        // Disable the taskes box collider, otherwise it will cause problem with selection
        //        /* if (Movement.selectionModeLeft || Movement.selectionModeRight)
        //            {
        //                foreach (string taskGoal in InputManager.TasksDictionary.Keys)
        //                {
        //                    GameObject taskGoalObject = InputManager.AllPickObjectsDictionary[taskGoal];
        //                    if (taskGoalObject.transform.parent != null)
        //                    {
        //                        GameObject taskGoalParent = taskGoalObject.transform.parent.gameObject;
        //                        if (taskGoalParent.collider)
        //                            taskGoalParent.collider.enabled = false;
        //                    }
        //                }
        //            }
        //            else 
        //            {
        //                foreach (Task taskGoal in InputManager.TasksDictionary.Values)
        //                {
        //                    //if (taskGoal.TaskFinishTime == 0f) {
        //                        GameObject taskGoalObject = InputManager.AllPickObjectsDictionary[taskGoal.TaskGoal];
        //                        if (taskGoalObject.transform.parent != null) { 
        //                            GameObject taskGoalParent = taskGoalObject.transform.parent.gameObject;
        //                            taskGoalParent.collider.collider.enabled = true;
        //                        }
        //                    //}
        //                }
        //            }*/
        //        // else {
        //        //     Debug.Log("disable2");

        //        // }



        //        //  if (InputManager.TasksDictionary.Count == 0)
        //        //     state = State.Finish;
        //        /*if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.1f, buttonWidth, buttonHeight), "Finish"))
        //            {
        //                Messenger.Broadcast("write results");

        //                //CallProgressiveCue1();

        //            }*/

        //        //ShowPath();
        //        break;
        //    case State.Tutorial:
        //        //Debug.Log("OnGui: " + currentTutorialStep);
        //        if (currentTutorialStep == -1)
        //        {
        //            TasksWindow.SetActive(true);
        //            UILabel tutorialLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
        //            tutorialLabel.text = "You will now run through a simple example scenario to help you learn how to use the controller.  The instructions will appear at the top of the window, and you can use the [00ff00]A button[-] to move to the next step.  A legend that shows the controller buttons and their functions will always be displayed at the bottom left of the window.  When you are ready to begin, press the [00ff00]A button[-] on your controller to begin.";
        //            if ((GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle)))
        //            {
        //                TasksWindow.SetActive(false);
        //                currentTutorialStep++;
        //                hasPlayedClip = false;
        //                TutorialAudio.GetComponent<TutorialAudioManager>().PlayConfirmation();
        //                ResetTutorialGoals();
        //            }
        //        }
        //        else if (currentTutorialStep >= 0 && currentTutorialStep < InputManager.TutorialSteps.Count && InputManager.TutorialSteps[currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Initial)
        //        {//currentTutorialStep >= 0) {


        //            UILabel instructionsLabel = GameObject.Find("InstructionsLabel").GetComponent<UILabel>();
        //            instructionsLabel.text = InputManager.TutorialSteps[currentTutorialStep].Description + Environment.NewLine + InputManager.TutorialSteps[currentTutorialStep].ProgressionText;//"(Press the [00ff00]A button[-] button to continue)";

        //            if (InputManager.TutorialSteps[currentTutorialStep].OverlayName != "" && InputManager.TutorialSteps[currentTutorialStep].SpriteName != "")
        //            {
        //                for (int i = 0; i < Overlays.Count; i++)
        //                {
        //                    if (Overlays[i].transform.name == InputManager.TutorialSteps[currentTutorialStep].SpriteName)
        //                    {
        //                        Overlays[i].GetComponent<UISprite>().enabled = true;
        //                        Overlays[i].GetComponent<UISprite>().spriteName = InputManager.TutorialSteps[currentTutorialStep].OverlayName;
        //                        Overlays[i].GetComponent<TweenAlpha>().enabled = true;
        //                    }
        //                    else
        //                    {
        //                        Overlays[i].GetComponent<UISprite>().enabled = false;
        //                        Overlays[i].GetComponent<TweenAlpha>().enabled = false;
        //                    }
        //                }
        //            }

        //            FieldInfo myf = typeof(GameFlow).GetField(InputManager.TutorialSteps[currentTutorialStep].StepGoal);
        //            //Debug.Log("here:"+myf.GetValue (this));
        //            //if (   (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.80f, buttonWidth, buttonHeight), "Press                to Continue",AButtonStyle)))
        //            if (Convert.ToBoolean(myf.GetValue(this)))
        //            {
        //                currentTutorialStep++;
        //                hasPlayedClip = false;
        //                TutorialAudio.GetComponent<TutorialAudioManager>().PlayConfirmation();
        //                ResetTutorialGoals();
        //                if (InputManager.TutorialSteps[currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task)
        //                {
        //                    Messenger.Broadcast("Change state");
        //                    state = State.Tasks_doing;
        //                }
        //                for (int i = 0; i < Overlays.Count; i++)
        //                {
        //                    Overlays[i].GetComponent<UISprite>().enabled = false;
        //                    Overlays[i].GetComponent<TweenAlpha>().enabled = false;
        //                }
        //            }
        //            //}
        //        }
        //        else if (currentTutorialStep >= 0 && currentTutorialStep < InputManager.TutorialSteps.Count && InputManager.TutorialSteps[currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Final)
        //        {
        //            TaskWindow.SetActive(true);
        //            UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //            UILabel instructionsLabel = GameObject.Find("InstructionsLabel").GetComponent<UILabel>();
        //            instructionsLabel.text = "";

        //            TaskLabel.text = InputManager.TutorialSteps[currentTutorialStep].Description + Environment.NewLine + InputManager.TutorialSteps[currentTutorialStep].ProgressionText;//"(Press the [00ff00]A button[-] button to continue)";
        //            for (int i = 0; i < Overlays.Count; i++)
        //            {
        //                Overlays[i].GetComponent<UISprite>().enabled = false;
        //                Overlays[i].GetComponent<TweenAlpha>().enabled = false;
        //            }

        //            if (Input.GetButtonDown("A Button"))
        //            {
        //                TaskWindow.SetActive(false);
        //                currentTutorialStep++;
        //            }
        //        }
        //        break;
        //    case State.Finish:
        //        TaskWindow.SetActive(true);
        //        UILabel label = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        //        label.text = "Thank you for trying out the virtual grocery store simulation today! You are now done with your interaction. Please press A to close the simulation. ";
        //        if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth, Screen.height * 0.5f, buttonWidth, buttonHeight), "Press                to Continue", AButtonStyle))
        //        {
        //            Application.Quit();
        //        }
        //        break;
        //}

    }
	
	
	// Update is called once per frame
	void Update()
	{
        UILabel TaskLabel = new UILabel();
        if(TaskWindow.activeInHierarchy)
            TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
        switch (state)
		{
		    case State.Intro_page:
                UILabel IntroLabel = GameObject.Find("IntroLabel").GetComponent<UILabel>();
                IntroLabel.text = InputManager.IntroText;
                if (showTutorial && !hasPlayedClip)
                {
                    hasPlayedClip = true;
                    TutorialAudio.GetComponent<TutorialAudioManager>().PlayOpeningClip();

                }
                if (Input.GetButtonDown("A Button"))
                {
                    IntroWindow.SetActive(false);
                    TasksWindow.SetActive(true);
                    if (showTutorial)
                    {
                        hasPlayedClip = false;
                        TutorialAudio.GetComponent<TutorialAudioManager>().PlayConfirmation();
                        state = State.Tutorial;
                    }
                    else {
                        state = State.Tasks_page;
                    }
                }
                break;
            case State.Tasks_page:
                UILabel TasksLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
                TasksLabel.text = InputManager.Scenario;
                if (Input.GetButtonDown("A Button"))
                {
                    TasksWindow.SetActive(false);
                    state = State.Tasks_doing;
                }
                break;
            case State.Tasks_doing:
			if(displayNutritionResponse){
				Debug.Log ("Entering state; Value : " + displayNutritionResponse);
			}
			if (displayTaskWindow)
			{
				
				if (_taskNameCurrent != "")
				{
					TaskWindow.SetActive(true);
					
					Task taskCurrent = InputManager.TasksDictionary[_taskNameCurrent];
					TaskLabel.text = taskCurrent.Description;
					if (Input.GetButtonDown("A Button"))
					{
						TaskWindow.SetActive(false);
						displayTaskWindow = false; 
					}
					
				}
			}
			
			else if(displayEndOfTasks){
				TaskWindow.SetActive(true);

				//Play audio of baby crying
				if (enableBabyCrying) {
					StartCoroutine ("playBabyCryingAudio");
				}
				//Play audio of baby crying
				if (enableThunder) {
					StartCoroutine ("playThunderAudio");
				}

				TaskLabel.text = "Congratulations! You got all the items on your shopping list.  \n \n Now you can either checkout the items you have shopped now or continue shopping for more items. To checkout, please go to the front of the store near the checkout counters and you will be prompted to checkout.";

				if (Input.GetButtonDown("A Button"))
				{
					TaskWindow.SetActive(false);
					displayEndOfTasks = false;
					state = State.Tasks_doing;

				}
			}

			else if(displayEndOfNutritionTask){
				TaskWindow.SetActive(true);
                TaskLabel.text = "You have successfully added all the items on your shopping list to your cart.  \n \n Which of the items had the lowest CALORIE count?" +
                    "\n Press \"A\" if Kellog's Raisin-Bran Cereal had the lowest Calorie count." +
                    "\n Press \"B\" if Kellog's All-Bran Cereal had the lowest Calorie count.";
                if (GameObject.Find("Robot_Prefab").GetComponent<PickUpItems>().threeCompared)
                    TaskLabel.text += "\n Press \"X\" if (insert third item to compare) had the lowest Caloire count.";
                if (Input.GetButtonDown("A Button"))
				{
					TaskWindow.SetActive(false);
					displayEndOfNutritionTask = false;
					displayNutritionResponse = true;
					correctNutritionItem = false;
					state = State.Tasks_doing;
				}
				else if (Input.GetButtonDown("B Button"))
				{
					displayEndOfNutritionTask = false;
					TaskWindow.SetActive(false);
					displayNutritionResponse = true;
					correctNutritionItem = true;
					state = State.Tasks_doing;
				}
                if(GameObject.Find("Robot_Prefab").GetComponent<PickUpItems>().threeCompared && Input.GetButtonDown("X Button"))
                {
                    TaskWindow.SetActive(false);
                    displayEndOfNutritionTask = false;
                    displayNutritionResponse = true;
                    correctNutritionItem = false;
                    state = State.Tasks_doing;
                }
			}

			else if(displayNutritionResponse){
				TaskWindow.SetActive(true);
				if(correctNutritionItem){
					TaskLabel.text = "Congratulations! You have successfully identified the item with the least Calorie count. \n You are now done with this simulation. Thank you for trying out our simulation!";
					if (Input.GetButtonDown("A Button"))
					{
						TaskWindow.SetActive(false);
						displayNutritionResponse = false;
						correctNutritionItem = false;
						displayEndOfTasks = true;
						state = State.Tasks_doing;
					}
				}else{
					TaskLabel.text = "Sorry, that is not the item with the lowest Calorie count. \n Please press B to try again";
					if (Input.GetButtonDown("B Button"))
					{
						displayNutritionResponse = false;
						TaskWindow.SetActive(false);
						correctNutritionItem = false;
						displayEndOfNutritionTask = true;
						state = State.Tasks_doing;
					}
				}
			}
			
			else if (displayCorrectItem)
			{
                bool itemHandled = false;
                if (correctItem && TaskWindow.activeInHierarchy)
				{
					TaskLabel.text = "You put the right item into the cart.";
                        //state = State.Tasks_timeout;
                    if (Input.GetButtonDown("A Button"))
                    {
                        itemHandled = true;
                        TaskWindow.SetActive(false);
                        displayCorrectItem = false;
						if (itemAtHand)
						{
							Messenger<Transform>.Broadcast("get the right item", itemAtHand);
							Task taskCurrent = InputManager.TasksDictionary[itemAtHand.name];
							if (taskCurrent.TaskType == Task.TaskTypes.Search_task)
								taskCurrent.TaskFinishTime = Timer.GetTimer;
							InputManager.OutputTasks.Add(taskCurrent);
							
							InputManager.TasksDictionary.Remove(itemAtHand.name);
							InputManager.updateShoppingList(itemAtHand.name);
							foreach (Task task in InputManager.TasksDictionary.Values)
							{
								if (task != null)
									nextTask = task;
								else
									nextTask = null;
								break;
							}
							Debug.Log("Next Task Goal : "+ nextTask.TaskGoal);
							Debug.Log("Number of tasks left : "+ InputManager.TasksDictionary.Count);

                            // Check if we are done with all the tasks in the task list. Then we can ask the user to go to checkout
                            if ((InputManager.TasksDictionary.Count == 0 || nextTask == null) && taskCurrent.TaskType == Task.TaskTypes.Nutrition_task)
                            {
                                displayEndOfNutritionTask = true;
                            }
                            else if ((InputManager.TasksDictionary.Count == 0 || nextTask == null) && taskCurrent.TaskType == Task.TaskTypes.UnitPriceComparison_task)
                            {
                                displayForEndOfUnitPriceComparisonTaskList = true;
                            }
                            else if (InputManager.TasksDictionary.Count == 0 || nextTask == null)
                            {
                                displayEndOfTasks = true;
                            }
							state = State.Tasks_doing;
						}
						
					}
				}
				else if(TaskWindow.activeInHierarchy)
				{
					TaskLabel.text = "Are you sure you want this item? This isn¡¯t what you are looking for. ";
					//state = State.Tasks_timeout;
					if(Input.GetButtonDown("A Button"))
					{
                            itemHandled = true;
						TaskWindow.SetActive(false);
						displayCorrectItem = false;
						if (itemAtHand)
							Messenger<Transform>.Broadcast("get the right item", itemAtHand);
						//state = State.Tasks_doing;
					}
					
					if(Input.GetButtonDown("B Button"))
					{
                            itemHandled = true;
						TaskWindow.SetActive(false);
						displayCorrectItem = false;
						if (itemAtHand)
							Messenger<Transform>.Broadcast("get the wrong item", itemAtHand);
						//state = State.Tasks_doing;
					}
				}
                if(!itemHandled)
                    TaskWindow.SetActive(true);
            }
			
			else if (displayCorrectPrice)
			{
				TaskWindow.SetActive(true);
				if (correctPrice)
				{
					if (InputManager.TasksDictionary.Count > 1)
						TaskLabel.text = "Congratulations! You have entered the right price. Proceed to next item on the shopping list.";
					else
						TaskLabel.text = "Congratulations! You have entered the right price. ";
					//state = State.Tasks_timeout;
					if(Input.GetButtonDown("A Button"))
					{
						TaskWindow.SetActive(false);
						displayTaskWindow = false;
						displayCorrectPrice = false;
						responseRight(_taskNameCurrent);
						state = State.Tasks_doing;
						Messenger.Broadcast("close response");
					}
				}
				else
				{
					TaskLabel.text = "That is not the answer we were looking for. Would you like to try again?";
					//state = State.Tasks_timeout;
					if(Input.GetButtonDown("A Button"))
					{
						TaskWindow.SetActive(false);
						displayCorrectPrice = false;
						state = State.Tasks_doing;
					}
					if(Input.GetButtonDown("B Button"))
					{
						TaskWindow.SetActive(false);
						displayCorrectPrice = false;
						responseWrong(_taskNameCurrent);
						state = State.Tasks_doing;
						Messenger.Broadcast("close response");
					}
				}
				
			}

            else if (displayFinishedMoney)
			{
				TaskWindow.SetActive(true);
				//state = State.Tasks_timeout;
				
				if (!correctChange)
				{
					TaskLabel.text = "Are you satisfied that you got the right change?";
					if(Input.GetButtonDown("A Button"))
					{
						TaskWindow.SetActive(false);
						displayFinishedMoney = false;
						Messenger.Broadcast("write results");
						state = State.Finish;
					}
					if (Input.GetButtonDown("B Button"))
					{
						correctChange = true;
					}
				}
				
				else
				{
					TaskLabel.text = "Sorry, I think you might have made a mistake with your math. Please check your math again.";
					if (Input.GetButtonDown("A Button"))
					{
						TaskWindow.SetActive(false);
						Messenger.Broadcast("write results");
						state = State.Finish;
					}
				}
				
			}
			
			else if (displayConfirmCheckout && !showTutorial) 
			{
				TasksWindow.SetActive(true);
				TasksLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
				//state = State.Tasks_timeout;
				TasksLabel.text = "You have requested to checkout now. Once you proceed to checkout, you will not be able to shop anymore. Do you want to checkout now?";// + "\n" + "\n" + " PLEASE PUT DOWN THE CONTROLLER AND USE THE MOUSE AND CLICK ON THE BUTTONS.";
				if(Input.GetButtonDown("A Button"))
				{
					displayCheckoutInfo = true;
					TasksWindow.SetActive(false);
					displayConfirmCheckout = false;
					state = State.Tasks_doing;
					aButtonDelay = true;
				}
				if(Input.GetButtonDown("B Button"))
				{
					TasksWindow.SetActive(false);
					displayConfirmCheckout = false;
					state = State.Tasks_doing;
					Messenger.Broadcast("exit checkout");
				}
			}

			if(aButtonDelay && !Input.GetButtonDown("A Button"))
				aButtonDelay = !aButtonDelay;

			if (displayCheckoutInfo && !aButtonDelay) 
			{
				TasksWindow.SetActive(true);
				TasksLabel = GameObject.Find("TasksLabel").GetComponent<UILabel>();
				//state = State.Tasks_timeout;

				TasksLabel.text = "You are now going to be moved to the checkout counter for checking out your items. During checkout you can: \n \n " +
					"1) Interact with the cashier by speaking out aloud. The cashier will respond to your questions and comments. \n" +
						"2) Checkout items that you have shopped up until now. As each item is checked out, you can see how much it costs on the bill window.\n" +
						"3) Pay for the items you have shopped with money in your wallet. Use the controller to highlight bills, the A Button to move bill to/from the wallet, and the X Button to pay."+//Hover over the bills and coins in your wallet to view them and click to choose them. Once you" +
						"Once you have paid for the items, you will get back change if any to put back into your wallet.";
				    if(Input.GetButtonDown("A Button"))
				    {
                        TasksWindow.SetActive(false);
                        displayCheckoutInfo = false;
                        displayConfirmCheckoutYes = false;
                        displayCashierOptions = true;
                        state = State.Tasks_doing;
                        aButtonDelay = true;

                        displayCashierOptions = false;
                        displayForCashierOptionsExtra = false;
                        //Messenger.Broadcast("checkout counter " + chosenCheckout + " chosen");
                        Messenger.Broadcast("set move to checkout");
                    }
			}

			if (displayCashierOptions && !aButtonDelay) 
			{
				TaskWindow.SetActive(true);
				//state = State.Tasks_timeout;

				TaskLabel.text = "Please choose a cashier to help you with checking out your items.";

				if (GUI.Button(new Rect(Screen.width / 6, Screen.height/2, Screen.width / 6, Screen.height/2.5f), "", VH1Style))
				{
					/*TaskWindow.SetActive(false);
					displayCashierOptions = false;
					displayForCashierOptionsExtra = false;
					Messenger.Broadcast("checkout counter 1 chosen");
					Messenger.Broadcast("set move to checkout");
					state = State.Tasks_doing;*/
				}
				if (GUI.Button(new Rect(Screen.width / 3, Screen.height/2, Screen.width / 6, Screen.height/2.5f), "", VH2Style))
				{
					/*TaskWindow.SetActive(false);
					displayCashierOptions = false;
					displayForCashierOptionsExtra = false;
					Messenger.Broadcast("checkout counter 2 chosen");
					Messenger.Broadcast("set move to checkout");
					state = State.Tasks_doing;*/
				}
				if (GUI.Button(new Rect(Screen.width / 2, Screen.height/2, Screen.width / 6, Screen.height/2.5f), "", VH3Style))
				{
					/*TaskWindow.SetActive(false);
					displayCashierOptions = false;
					displayForCashierOptionsExtra = false;
					Messenger.Broadcast("checkout counter 3 chosen");
					Messenger.Broadcast("set move to checkout");
					state = State.Tasks_doing;*/
				}
				if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height/2, Screen.width / 6, Screen.height/2.5f), "", VH4Style))
				{
					/*TaskWindow.SetActive(false);
					displayCashierOptions = false;
					displayForCashierOptionsExtra = false;
					Messenger.Broadcast("checkout counter 4 chosen");
					Messenger.Broadcast("set move to checkout");
					state = State.Tasks_doing;*/
				}

				if (GUI.Button(new Rect(Screen.width / 1.5f, Screen.height/2f + Screen.width / 6f, Screen.width / 15f, Screen.height/5f), "", VH1Style))
				{

				}
				if(Input.GetButtonDown("A Button"))
				{
					TaskWindow.SetActive(false);
					displayCashierOptions = false;
					displayForCashierOptionsExtra = false;
					Messenger.Broadcast("checkout counter " + chosenCheckout + " chosen");
					Messenger.Broadcast("set move to checkout");
					state = State.Tasks_doing;
					aButtonDelay = true;
				}
			}
			// Disable the tasks box collider, otherwise it will cause problem with selection
			if (Movement1.directWalkOnly)
			{
				foreach (string taskGoal in InputManager.TasksDictionary.Keys)
				{
					GameObject taskGoalObject = InputManager.AllPickObjectsDictionary[taskGoal];
					if (taskGoalObject.transform.parent != null)
					{
						GameObject taskGoalParent = taskGoalObject.transform.parent.gameObject;
						if (taskGoalParent.GetComponent<Collider>())
							taskGoalParent.GetComponent<Collider>().enabled = false;
					}
				}
			}
			else
			{
				foreach (Task taskGoal in InputManager.TasksDictionary.Values)
				{
					//if (taskGoal.TaskFinishTime == 0f) {
					GameObject taskGoalObject = InputManager.AllPickObjectsDictionary[taskGoal.TaskGoal];
					if (taskGoalObject.transform.parent != null)
					{
						GameObject taskGoalParent = taskGoalObject.transform.parent.gameObject;
						taskGoalParent.GetComponent<Collider>().GetComponent<Collider>().enabled = true;
					}
					//}
				}
			}
			// else {
			//     Debug.Log("disable2");
			
			// }
			
			//if (InputManager.TasksDictionary.Count == 0)
			//     state = State.Finish;
			/*if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, Screen.height * 0.1f, buttonWidth, buttonHeight), "Finish"))
                {
                    Messenger.Broadcast("write results");

                    //CallProgressiveCue1();

                }*/
			
			//ShowPath();

			if (Input.GetKeyDown (KeyCode.F)) {
				state = State.Finish;
			}
						break;
		case State.Tutorial:
			//if(inputAllowed)
			//{
			/*if (Input.GetKeyDown(KeyCode.Q))
			{
				TasksWindow.SetActive (false);
				currentTutorialStep++;
				hasPlayedClip = false;
				TutorialAudio.GetComponent<TutorialAudioManager>().PlayConfirmation();
				ResetTutorialGoals();
			}*/


			if (Input.GetButtonDown ("A Button"))
				abuttonPressed = true;
			if (Input.GetButtonDown ("B Button"))
				bbuttonPressed = true;
			if (Input.GetButtonDown ("Y Button"))
				ybuttonPressed = true;
			if (Input.GetButtonDown ("X Button"))
				xbuttonPressed = true;
			//}
			if (currentTutorialStep == -1) {
                   
				TasksWindow.SetActive (true);
				UILabel tutorialLabel = GameObject.Find ("TasksLabel").GetComponent<UILabel> ();
				tutorialLabel.text = "You will now run through a simple example scenario to help you learn how to use the controller.  The instructions will appear at the top of the window along with what you need to do to move to the next step.  A legend that shows the controller buttons and their functions will always be displayed at the bottom left of the window.  When you are ready to begin, press the [00ff00]A button[-] on your controller to begin.";
				if (!hasPlayedClip) {
					hasPlayedClip = true;
					TutorialAudio.GetComponent<TutorialAudioManager> ().PlayIntroClip ();
				}
				if (Input.GetButtonDown ("A Button")) {
					TasksWindow.SetActive (false);
					currentTutorialStep++;
					hasPlayedClip = false;
					TutorialAudio.GetComponent<TutorialAudioManager> ().PlayConfirmation ();
                            
					ResetTutorialGoals ();
				}
			} else if (currentTutorialStep >= 0 && currentTutorialStep < InputManager.TutorialSteps.Count && InputManager.TutorialSteps [currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Initial) {
				//Debug.Log(currentTutorialStep + " " + InputManager.TutorialSteps[currentTutorialStep].TutorialGrouping + " " + InputManager.TutorialSteps [currentTutorialStep].StepGoal);
				if (!hasPlayedClip) {
					hasPlayedClip = true;
					TutorialAudio.GetComponent<TutorialAudioManager> ().PlayAudioClip ();
				}
				UILabel instructionsLabel = GameObject.Find ("InstructionsLabel").GetComponent<UILabel> ();
				instructionsLabel.text = InputManager.TutorialSteps [currentTutorialStep].Description + Environment.NewLine + InputManager.TutorialSteps [currentTutorialStep].ProgressionText;//"(Press the [00ff00]A button[-] button to continue)";

				if (InputManager.TutorialSteps [currentTutorialStep].OverlayName != "" && InputManager.TutorialSteps [currentTutorialStep].SpriteName != "") {
					for (int i = 0; i < Overlays.Count; i++) {
						if (Overlays [i].transform.name == InputManager.TutorialSteps [currentTutorialStep].SpriteName) {
							Overlays [i].GetComponent<UISprite> ().enabled = true;
							Overlays [i].GetComponent<UISprite> ().spriteName = InputManager.TutorialSteps [currentTutorialStep].OverlayName;
							Overlays [i].GetComponent<TweenAlpha> ().enabled = true;
						} else {
							Overlays [i].GetComponent<UISprite> ().enabled = false;
							Overlays [i].GetComponent<TweenAlpha> ().enabled = false;
						}
					}
				}

				FieldInfo myf = typeof(GameFlow).GetField (InputManager.TutorialSteps [currentTutorialStep].StepGoal);
				if (Convert.ToBoolean (myf.GetValue (this))) {
					currentTutorialStep++;
					hasPlayedClip = false;
					TutorialAudio.GetComponent<TutorialAudioManager> ().PlayConfirmation ();
					ResetTutorialGoals ();
					if (InputManager.TutorialSteps [currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Task) {
						Messenger.Broadcast ("Change state");
						state = State.Tasks_doing;
					}
					for (int i = 0; i < Overlays.Count; i++) {
						Overlays [i].GetComponent<UISprite> ().enabled = false;
						Overlays [i].GetComponent<TweenAlpha> ().enabled = false;
					}
				}

			} else if (currentTutorialStep >= 0 && currentTutorialStep < InputManager.TutorialSteps.Count && InputManager.TutorialSteps [currentTutorialStep].TutorialGrouping == TutorialStep.TutorialGroup.Final) {
				TaskWindow.SetActive (true);
				UILabel instructionsLabel = GameObject.Find ("InstructionsLabel").GetComponent<UILabel> ();
				instructionsLabel.text = "";
				if (!hasPlayedClip) {
					hasPlayedClip = true;
					TutorialAudio.GetComponent<TutorialAudioManager> ().PlayAudioClip ();
				}

				TaskLabel.text = InputManager.TutorialSteps [currentTutorialStep].Description + Environment.NewLine + InputManager.TutorialSteps [currentTutorialStep].ProgressionText;//"(Press the [00ff00]A button[-] button to continue)";
				for (int i = 0; i < Overlays.Count; i++) {
					Overlays [i].GetComponent<UISprite> ().enabled = false;
					Overlays [i].GetComponent<TweenAlpha> ().enabled = false;
				}
                    
				if (Input.GetButtonDown ("A Button")) {
					TaskWindow.SetActive (false);
					currentTutorialStep++;
					hasPlayedClip = false;
					TutorialAudio.GetComponent<TutorialAudioManager> ().PlayConfirmation ();
				}
			}
                break;
		case State.Finish:
			TaskWindow.SetActive(true);
			UILabel label = GameObject.Find("TaskLabel").GetComponent<UILabel>();
			label.text = "Thank you for trying out the virtual grocery store simulation today! You are now done with your interaction. Please press the A button to close the simulation. ";
			if (Input.GetButtonDown("A Button"))
			{
				Application.Quit();
			}
			break;
		}
		
		
		
	}

	public void triggerBottleBreakingAudio()
	{
		// Play bottle breaking sound
		if (enableBottleBreaking) {
			StartCoroutine ("playBottleBreakingAudio");
		}
	}

	public void triggerBabyCryingAudio()
	{
		// Play bottle breaking sound
		if (enableBabyCrying) {
			StartCoroutine ("playBabyCryingAudio");
		}
	}

	public void triggerThunder()
	{
		// Play bottle breaking sound
		if (enableThunder) {
			StartCoroutine ("playThunderAudio");
		}
	}

	public IEnumerator playBabyCryingAudio()
	{
		Debug.Log("Playing baby crying audio");
		//waits until this amount of time has passed.
		yield return new WaitForSeconds(3.0f);
		// play audio of pickup
		baby_crying_audio_source.clip = baby_crying_audio_clip;
		baby_crying_audio_source.Play();
	}
	
	public IEnumerator playBottleBreakingAudio()
	{
		Debug.Log("Playing baby crying audio");
		//waits until this amount of time has passed.
		yield return new WaitForSeconds(1.0f);
		// play audio of pickup
		bottle_breaking_audio_source.clip = bottle_breaking_audio_clip;
		bottle_breaking_audio_source.Play();
	}

	public IEnumerator playThunderAudio()
	{
		Debug.Log("Playing thunder audio");
		//waits until this amount of time has passed.
		yield return new WaitForSeconds(1.0f);
		// play audio of pickup
		thunder_audio_source.clip = thunder_audio_clip;
		thunder_audio_source.Play();
	}

	public void EnableBabyCryingFn(){
		enableBabyCrying = true;
	}

	public void EnableBottleBreakingFn(){
		enableBottleBreaking = true;
	}

	public void EnableThunderFn(){
		enableThunder = true;
	}
	
}

