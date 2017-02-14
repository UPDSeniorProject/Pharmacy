	using UnityEngine;
using System.Collections;

public class PickUpItems : MonoBehaviour
{
    public enum State
    {
        idle,
        picked,
        toCompare,
        compared,
        toCompareThree,
        comparedThree,
        inCart,
        checkout
    }
    public static State state;

    public Transform ShoppingCart;

    public Texture2D pickupIcon;

    private Transform pickObj = null;
    private Transform pickObjFirst = null;
    private Transform pickObjSecond = null;

    private Transform cameraTransform;

    private Highlight itemScript;
    private Highlight itemScriptFirst;
    private Highlight itemScriptSecond;

    private Transform parent_item;

    private Highlight highlightScript;

    private int comparePosition = 0;

    private bool isRotate = false;

    private float buttonWidth = Screen.width / 8f;
    private float buttonHeight = Screen.height / 10f;
    private float offset = Screen.width / 80f;

    public Texture2D max;
    public Texture2D min;

    private Transform myTransform;

    public Shader itemShader;

    public Rect r;

    public GameObject PriceTagWindow;
    private static bool displayPriceTagOrNot = false;

    private bool rotating = false;
    private Transform LookAtPos = null;
    //    private bool Joystick1Button1 = false;

    public AudioClip pickup_item_audio_clip;
    private AudioSource pickupitem_audio_source;
    public AudioClip putback_item_audio_clip;
    private AudioSource putbackitem_audio_source;

    private GameFlow gameFlow;

    public GUIStyle XButtonStyle;
    public GUIStyle BButtonStyle;
    public GUIStyle AButtonStyle;
    public GUIStyle ZButtonStyle;
    public GUIStyle RAnalogStyle;
    public GUIStyle LAnalogStyle;

    private Vector3 holdPosition;
    public bool threeCompared = false;

    void Awake()
    {
        myTransform = transform;
        if (LookAtPos == null)
            LookAtPos = GameObject.Find("LookAtPos").transform;
        if (PriceTagWindow == null)
            PriceTagWindow = GameObject.Find("PriceTagWindow");

        gameFlow = (GameFlow)GameObject.Find("GUI_prefab").GetComponent(typeof(GameFlow));
    }

    void Start()
    {
        //cameraTransform = GameObject.Find("Main Camera").transform;
        cameraTransform = GameObject.Find("LookAtPosChild").transform;
        PriceTagWindow.SetActive(false);

        // Add an audio source to the game object
        pickupitem_audio_source = (AudioSource)gameObject.AddComponent<AudioSource>();
        putbackitem_audio_source = (AudioSource)gameObject.AddComponent<AudioSource>();

        // itemShader = Shader.Find("Transparent/Bumped Specular");
        //itemShader = Shader.Find("Unlit/Transparent");
        //itemShader = Shader.Find("Unlit/Transparent");
    }

    void OnEnable()
    {
        Messenger.AddListener("Exit selection mode", exitSelection);
        Messenger<string, string>.AddListener("Display price tag on GUI", displayPriceTag);
        Messenger<Transform>.AddListener("get the right item", OnGetRightItem);
        Messenger<Transform>.AddListener("get the wrong item", OnGetWrongItem);
        Messenger<int>.AddListener("Rotate", rotate);
        Messenger<int>.AddListener("Cart", cart);
        Messenger<int>.AddListener("Compare", compare);
        Messenger<int>.AddListener("Put back", putBack);
    }
    void OnDisable()
    {
        Messenger.RemoveListener("Exit selection mode", exitSelection);
        Messenger<string, string>.RemoveListener("Display price tag on GUI", displayPriceTag);
        Messenger<Transform>.RemoveListener("get the right item", OnGetRightItem);
        Messenger<Transform>.RemoveListener("get the wrong item", OnGetWrongItem);
        Messenger<int>.RemoveListener("Rotate", rotate);
        Messenger<int>.RemoveListener("Cart", cart);
        Messenger<int>.RemoveListener("Compare", compare);
        Messenger<int>.RemoveListener("Put back", putBack);

    }

    void rotate(int index)
    {
        switch (index)
        {
            case 0:
                if (pickObj)
                    rotateLeftForNinety(pickObj);
                break;
            default:
                break;
        }
    }
    void cart(int index)
    {
        switch (index)
        {
            case 0:
                if (pickObj)
                    Messenger<Transform>.Broadcast("put item into cart", pickObj);
                break;
            default:
                break;
        }
    }
    void compare(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("compare yes");
                if (pickObjFirst == null)
                    putDownItemToCompare();
                break;
            case 1:
                if (pickObjSecond == null)
                    putDownItemToCompareThree();
                break;
            default:
                break;
        }
    }
    void putBack(int index)
    {
        switch (index)
        {
            case 0:
                if (pickObj)
                    putBackItem(pickObj);
                break;
            default:
                break;
        }
    }

    void OnGetRightItem(Transform itemPicked)
    {
        putIntoCart(itemPicked);
    }

    void OnGetWrongItem(Transform itemPicked)
    {
        putBackItem(itemPicked);
    }

    private void displayPriceTag(string tagName, string tagPrice)
    {
        PriceTagWindow.SetActive(true);
        displayPriceTagOrNot = true;
        TextMesh PriceTagWindowName = GameObject.Find("PriceTagWindowName").GetComponent<TextMesh>();
        TextMesh PriceTagWindowPrice = GameObject.Find("PriceTagWindowPrice").GetComponent<TextMesh>();
        PriceTagWindowName.text = tagName;
        PriceTagWindowPrice.text = tagPrice;
    }

    private void exitSelection()
    {
        if (pickObj)
            putBackItem(pickObj);
        if (pickObjFirst)
            putBackItem(pickObjFirst);
        if (pickObjSecond)
            putBackItem(pickObjSecond);
        state = State.idle;
        Messenger.Broadcast("Change state");
    }

    public bool holdingItem()
    {
        if (pickObj || pickObjFirst || pickObjSecond)
            return true;
        return false;
    }

    void OnGUI()
    {

		//Draw the icon for pickup
		if(gameFlow.showTutorial)
		{
			if(gameFlow.currentTutorialStep >= 0 && gameFlow.currentTutorialStep < InputManager.TutorialSteps.Count){// && InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping != TutorialStep.TutorialGroup.Task ) {
				if(state != State.checkout && GameFlow.state != GameFlow.State.Intro_page && GameFlow.state != GameFlow.State.Tasks_page){
					r = new Rect(Screen.width/2 - 20, Screen.height/2 - 20, 40, 40);
					GUI.DrawTexture(r, pickupIcon);
				}
			}
		}
		else
		{
			if(state != State.checkout && GameFlow.state != GameFlow.State.Intro_page && GameFlow.state != GameFlow.State.Tasks_page){
				r = new Rect(Screen.width/2 - 20, Screen.height/2 - 20, 40, 40);
				GUI.DrawTexture(r, pickupIcon);
                if(!GameFlow.TasksWindow.activeInHierarchy)
                    GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle);
            }
		}


        /*if (displayPriceTagOrNot)
        {
            // Debug.Log("jere");
            if (GUI.Button(new Rect(Screen.width * 0.5f + buttonWidth * 1f, Screen.height * 0.65f, buttonWidth, buttonHeight), "Press A to Continue", myStyle2))
            {
                PriceTagWindow.SetActive(false);
                displayPriceTagOrNot = false;
            }
        }
        */

        switch (state)
        {
            case State.idle:

                if (!Movement1.directWalkOnly && !Movement1.isCheckout)
                {
                    //GUI.Box (new Rect(10,10,Screen.width/3, 20f),"left/right/up/down for movement control",myStyle);
                    if (!Screen.fullScreen && GUI.Button(new Rect(Screen.width - 48, 0, 48, 48), new GUIContent(max)))
                        Screen.fullScreen = true;
                    if (Screen.fullScreen && GUI.Button(new Rect(Screen.width - 48, 0, 48, 48), new GUIContent(min)))
                        Screen.fullScreen = false;
                }
                break;
            case State.picked:
                /*
                if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth * 2f - 2 * offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Rotate", myStyle2))
                    rotateLeftForNinety(pickObj);
                //if(GUI.Button(new Rect(Screen.width/2+buttonWidth*1.5f-offset, Screen.height*0.85f, buttonWidth, buttonHeight),"Rotate right", myStyle2))
                //	rotateRightForNinety();
                if (GUI.Button(new Rect(Screen.width / 2 - buttonWidth * 1f - offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Cart", myStyle2))
                {

                    //putIntoCart(pickObj);
                    Messenger<Transform>.Broadcast("put item into cart", pickObj);
                }

                if (GUI.Button(new Rect(Screen.width / 2, Screen.height * 0.85f, buttonWidth, buttonHeight), "Compare", myStyle2))
                    putDownItemToCompare();
                if ( (GUI.Button(new Rect(Screen.width / 2 + buttonWidth * 1f + offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Put back", myStyle2)))
                {
                    
                    putBackItem(pickObj);
                }
                   
                */
                if (GetComponent<Movement1>().zoomed)
                {
                    if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Use                     to view item", LAnalogStyle)))
                    {
                        //Messenger<int>.Broadcast("Put in cart", 0);
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Use                    rotate item", RAnalogStyle))
                    {
                        //Messenger<int>.Broadcast("Put back", 0);
                    }
                }
                else
                {
                    if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to put in cart", AButtonStyle)))
                    {
                        Messenger<int>.Broadcast("Put in cart", 0);
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back", BButtonStyle))
                    {
                        Messenger<int>.Broadcast("Put back", 0);
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle))
                    {
                        //Messenger<int>.Broadcast("Zoom", 0);
                    }
                    if(transform.parent.FindChild("GUI_prefab").GetComponent<InputManager>().inputFile == InputManager.InputFiles.Scenario3 || transform.parent.FindChild("GUI_prefab").GetComponent<InputManager>().inputFile == InputManager.InputFiles.Scenario2)
                    {
                        if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.88f, buttonWidth, buttonHeight), "Press                 to compare", XButtonStyle))
                        {
                            Messenger<int>.Broadcast("Compare", 0);
                        }
                    }
                }
                if (!pickObj && state == State.picked)
                {
                    isRotate = false;
                    state = State.idle;
                    Messenger.Broadcast("Change state");
                }

                break;


            //case State.toCompare:
                //if (Input.GetMouseButton(0) && GUIUtility.hotControl == 0)
                //    pickUpItem();
                //break;
            case State.compared:
                if (InputManager.TasksDictionary.ContainsKey("cereal_Kellogg's_Raisin Bran_23.5 Oz_LOD0") || InputManager.TasksDictionary.ContainsKey("soup_Cambell's_Harvest Tomato Soup_18.7oz_LOD0"))
                {
                    if (InputManager.TasksDictionary["cereal_Kellogg's_Raisin Bran_23.5 Oz_LOD0"].TaskType == Task.TaskTypes.Nutrition_task || InputManager.TasksDictionary["soup_Cambell's_Harvest Tomato Soup_18.7oz_LOD0"].TaskType == Task.TaskTypes.Nutrition_task)
                    {
                        if (pickObj)
                        {
                            if (GetComponent<Movement1>().zoomed)
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Use                    to view item", LAnalogStyle)))
                                {
                                    //Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Use                    rotate item", RAnalogStyle))
                                {
                                    //Messenger<int>.Broadcast("Put back", 0);
                                }
                            }
                            else
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 2nd item", AButtonStyle)))
                                {
                                    Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 2nd item", BButtonStyle))
                                {
                                    Messenger<int>.Broadcast("Put back", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle))
                                {
                                    //Messenger<int>.Broadcast("Zoom", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.88f, buttonWidth, buttonHeight), "Press                 to compare 3 items", XButtonStyle))
                                {
                                    //Messenger<int>.Broadcast("Compare", 0);
                                }
                            }
                        }
                        else if (pickObjFirst)
                        {
                            if (GetComponent<Movement1>().zoomed)
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Use                    to view items", LAnalogStyle)))
                                {
                                    //Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Use                    rotate items", RAnalogStyle))
                                {
                                    //Messenger<int>.Broadcast("Put back", 0);
                                }
                            }
                            else
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 1st item", AButtonStyle)))
                                {
                                    Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 1st item", BButtonStyle))
                                {
                                    Messenger<int>.Broadcast("Put back", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle))
                                {
                                    //Messenger<int>.Broadcast("Zoom", 0);
                                }
                            }
                        }
                    }
                }
                else if (InputManager.TasksDictionary.ContainsKey("cheese_Kraft_2% Milk Sharp Cheddar Cheese Singles_16 Slices_LOD0"))
                {
                    if (InputManager.TasksDictionary["cheese_Kraft_2% Milk Sharp Cheddar Cheese Singles_16 Slices_LOD0"].TaskType == Task.TaskTypes.UnitPriceComparison_task)
                    {
                        if (pickObj)
                        {
                            if (GetComponent<Movement1>().zoomed)
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Use                    to view items", LAnalogStyle)))
                                {
                                    //Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Use                    rotate items", RAnalogStyle))
                                {
                                    //Messenger<int>.Broadcast("Put back", 0);
                                }
                            }
                            else
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 2nd item", AButtonStyle)))
                                {
                                    Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 2nd item", BButtonStyle))
                                {
                                    Messenger<int>.Broadcast("Put back", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle))
                                {
                                    //Messenger<int>.Broadcast("Zoom", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.88f, buttonWidth, buttonHeight), "Press                 to compare 3 items", XButtonStyle))
                                {
                                    //Messenger<int>.Broadcast("Compare", 0);
                                }
                            }
                        }
                        else if (pickObjFirst)
                        {
                            if (GetComponent<Movement1>().zoomed)
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Use                    to view item", LAnalogStyle)))
                                {
                                    //Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Use                    rotate item", RAnalogStyle))
                                {
                                    //Messenger<int>.Broadcast("Put back", 0);
                                }
                            }
                            else
                            {
                                if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 1st item", AButtonStyle)))
                                {
                                    Messenger<int>.Broadcast("Put in cart", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 1st item", BButtonStyle))
                                {
                                    Messenger<int>.Broadcast("Put back", 0);
                                }
                                if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle))
                                {
                                    //Messenger<int>.Broadcast("Zoom", 0);
                                }
                            }
                        }
                    }
                }
                

                /*if (pickObjFirst)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.25f - 2f * buttonWidth + offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Rotate", myStyle2))
                        rotateLeftForNinety(pickObjFirst);
                    if (GUI.Button(new Rect(Screen.width * 0.25f - buttonWidth + 2f * offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Cart", myStyle2))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjFirst);
                        //putIntoCart(pickObjFirst);
                    }

                    if (GUI.Button(new Rect(Screen.width * 0.25f + offset * 3f, Screen.height * 0.85f, buttonWidth, buttonHeight), "Put back", myStyle2))
                        putBackItem(pickObjFirst);
                }
                if (pickObj)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.75f - 2f * buttonWidth - offset * 3f, Screen.height * 0.85f, buttonWidth, buttonHeight), "Rotate", myStyle2))
                        rotateLeftForNinety(pickObj);
                    if (GUI.Button(new Rect(Screen.width * 0.75f - buttonWidth - offset * 2f, Screen.height * 0.85f, buttonWidth, buttonHeight), "Cart", myStyle2))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObj);
                        //putIntoCart(pickObj);
                    }

                    if (GUI.Button(new Rect(Screen.width * 0.75f - offset, Screen.height * 0.85f, buttonWidth, buttonHeight), "Put back", myStyle2))
                        putBackItem(pickObj);
                }
                if (pickObj && pickObjFirst)
                    if (GUI.Button(new Rect(Screen.width * 0.75f + 1f * buttonWidth, Screen.height * 0.85f, buttonWidth, buttonHeight), "Compare one more", myStyle2))
                        putDownItemToCompareThree();
                 */
                if (!pickObj && !pickObjFirst)
                {
                    isRotate = false;
                    state = State.idle;
                    Messenger.Broadcast("Change state");
                }
                

                break;
            /*case State.toCompareThree:
                if (Input.GetMouseButton(0) && GUIUtility.hotControl == 0)
                    pickUpItem();
                break;*/
                
           case State.comparedThree:
                if (GetComponent<Movement1>().zoomed)
                {
                    if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Use                    to view items", LAnalogStyle)))
                    {
                        //Messenger<int>.Broadcast("Put in cart", 0);
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Use                    rotate items", RAnalogStyle))
                    {
                        //Messenger<int>.Broadcast("Put back", 0);
                    }
                }
                else
                {
                    if (pickObj)
                    {
                        if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 3rd item", AButtonStyle)))
                        {
                            //Messenger<int>.Broadcast("Put in cart", 0);
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 3rd item", BButtonStyle))
                        {
                            //Messenger<int>.Broadcast("Put back", 0);
                        }
                    }
                    else if(pickObjSecond)
                    {
                        if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 2nd item", AButtonStyle)))
                        {
                            //Messenger<int>.Broadcast("Put in cart", 0);
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 2nd item", BButtonStyle))
                        {
                            //Messenger<int>.Broadcast("Put back", 0);
                        }
                    }
                    else
                    {
                        if ((GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.70f, buttonWidth, buttonHeight), "Press                 to pick 1st item", AButtonStyle)))
                        {
                            //Messenger<int>.Broadcast("Put in cart", 0);
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.76f, buttonWidth, buttonHeight), "Press                 to put back 1st item", BButtonStyle))
                        {
                            //Messenger<int>.Broadcast("Put back", 0);
                        }
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.75f, Screen.height * 0.82f, buttonWidth, buttonHeight), "Press                 to zoom", ZButtonStyle))
                    {
                        //Messenger<int>.Broadcast("Zoom", 0);
                    }
                }
                break;
           /*     float buttonWidth2 = buttonWidth * 0.75f;
                if (pickObjFirst)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.25f - 2.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Rotate", myStyle2))
                        rotateLeftForNinety(pickObjFirst);
                    if (GUI.Button(new Rect(Screen.width * 0.25f - 1.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Cart", myStyle2))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjFirst);
                        //putIntoCart(pickObjFirst);
                    }

                    if (GUI.Button(new Rect(Screen.width * 0.25f - 0.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Put back", myStyle2))
                        putBackItem(pickObjFirst);
                }
                if (pickObjSecond)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 1.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Rotate", myStyle2))
                        rotateLeftForNinety(pickObjSecond);
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 0.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Cart", myStyle2))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjSecond);
                        //putIntoCart(pickObjSecond);
                    }

                    if (GUI.Button(new Rect(Screen.width * 0.5f + 0.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Put back", myStyle2))
                        putBackItem(pickObjSecond);
                }
                if (pickObj)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.75f - 0.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Rotate", myStyle2))
                        rotateLeftForNinety(pickObj);
                    if (GUI.Button(new Rect(Screen.width * 0.75f + 0.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Cart", myStyle2))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObj);
                        //	putIntoCart(pickObj);
                    }

                    if (GUI.Button(new Rect(Screen.width * 0.75f + 1.5f * buttonWidth2, Screen.height * 0.85f, buttonWidth2, buttonHeight), "Put back", myStyle2))
                        putBackItem(pickObj);
                }
                if (!pickObjSecond && !pickObj && !pickObjFirst)
                {
                    isRotate = false;
                    state = State.idle;
                    Messenger.Broadcast("Change state");
                }
                break;*/
        }
    }

    void Update()
    {
        //GameObject TaskDialogWindow = GameObject.Find ("");
        //Debug.Log ("TasksWindow : "+ GameFlow.TasksWindow.activeSelf);
        //Debug.Log ("TaskWindow : "+ GameFlow.TaskWindow.activeSelf);

        switch (state)
        {
            case State.idle:
                if (!ShoppingCart.GetComponent<Renderer>().enabled)
                    ShoppingCart.GetComponent<Renderer>().enabled = true;
				if (Movement1.directWalkOnly && !GameFlow.TaskWindow.activeSelf && !GameFlow.TasksWindow.activeSelf)
                {
                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                    RaycastHit hit;
                    if (Movement1.directWalkOnly == true)
                    {
                        if (Physics.Raycast(ray, out hit, 5f))
                        {
                            if (hit.transform.tag == "Pick" && (pickObjFirst == null || (pickObjFirst != null && hit.transform != pickObjFirst)))
                            {
                                if (!highlightScript)
                                {
                                    highlightScript = hit.transform.GetComponent<Highlight>();
                                    highlightScript.highlightItem();
                                }
                                else if (hit.transform.GetComponent<Highlight>() != highlightScript)
                                {
                                    highlightScript.unhighlightItem();
                                    highlightScript = hit.transform.GetComponent<Highlight>();
                                    highlightScript.highlightItem();
                                }
                            }
                            else {
                                if (highlightScript)
                                {
                                    highlightScript.unhighlightItem();
                                    highlightScript = null;
                                }
                            }
                        }
                        else {
                            if (highlightScript)
                            {
                                highlightScript.unhighlightItem();
                                highlightScript = null;
                            }
                        }
                    }
                    else {
                        if (highlightScript)
                        {
                            highlightScript.unhighlightItem();
                            highlightScript = null;
                        }
                    }
                    if ((Input.GetMouseButton(0) || Input.GetButtonDown("A Button")))
                    {
					    if(gameFlow.showTutorial)
					    {
						    if(gameFlow.currentTutorialStep >= 0 && gameFlow.currentTutorialStep < InputManager.TutorialSteps.Count)
						    {
							    //if (InputManager.TutorialSteps [gameFlow.currentTutorialStep].TutorialGrouping != TutorialStep.TutorialGroup.Task ) {
								    pickUpItem();
							    //}
						    }
					    }
					    else
					    {
						    //Debug.Log("Entering pickup");
						    pickUpItem();
					    }


                    }
                }
                break;
            case State.picked:
                if (ShoppingCart.GetComponent<Renderer>().enabled)
                    ShoppingCart.GetComponent<Renderer>().enabled = false;
                rotateItem();
                //Debug.Log(Input.GetAxisRaw("Mouse X") + "raw");
                //                Vector3 origialPos = LookAtPos.position;
                //Debug.Log( Mathf.Abs(Vector3.Distance(Camera.main.transform.position, origialPos)) + "distance");
                if (CameraZoom.zoomBack)
                {
                    float rotation = Input.GetAxis("Mouse X");
                    //TO HAVE CONTINUOUS ROTATION UNCOMMENT THE LINE BELOW AND COMMENT OUT THE CONDITIONALS BELOW THAT
                    pickObj.transform.Rotate(0, -1*rotation*150*Time.deltaTime, 0);

                    //if (rotation <= -0.05f && rotating == false)
                    //{
                    //    rotating = true;
                    //    // local scale is relate to the parent transform, because the robot transform is scaled
                    //    Debug.Log("Rotating left");
                    //    rotateLeftForNinety(pickObj);
                    //}
                    //if (rotation >= 0.05f && rotating == false)
                    //{
                    //    rotating = true;
                    //    Debug.Log("Rotating right");
                    //    rotateRightForNinety(pickObj);
                    //}
                    //if (rotation > -0.05f && rotation < 0.05f)
                    //{
                    //    rotating = false;
                    //}
                }
                if (pickObj) 
                {
                    if (GameFlow.TaskWindow.activeInHierarchy == false && !GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Debug.Log("put into cart");
                        Messenger<Transform>.Broadcast("put item into cart", pickObj);
                    }

                    if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObj);
                    }
                
                }
                break;
            case State.toCompare:
                holdPosition = transform.position;
                //happens when compare button pressed
                if (!ShoppingCart.GetComponent<Renderer>().enabled)
                    ShoppingCart.GetComponent<Renderer>().enabled = true;
                if (Movement1.directWalkOnly)
				{
					Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f, 0f));
					RaycastHit hit;
					if (Movement1.directWalkOnly == true)
					{
						if (Physics.Raycast(ray, out hit, 5f) && hit.transform.tag == "Pick" && (pickObjFirst == null || (pickObjFirst != null && hit.transform != pickObjFirst)))
						{
							if(!highlightScript){
								highlightScript = hit.transform.GetComponent<Highlight>();
								highlightScript.highlightItem();
							}else if(hit.transform.GetComponent<Highlight>() != highlightScript){
								highlightScript.unhighlightItem();
								highlightScript = hit.transform.GetComponent<Highlight>();
								highlightScript.highlightItem();
							}
						}else{
							if(highlightScript){
								highlightScript.unhighlightItem();
							}
						}
					}
					//if ((Input.GetMouseButton(0) || Input.GetButtonDown("A Button")) && !GUIcontrol.isMenu && GUIUtility.hotControl == 0)
					if ((Input.GetMouseButton(0) || Input.GetButtonDown("A Button")))
					{
						//Joystick1Button0 = false;
						Debug.Log("Entering second item pickup");
						pickUpItem();
					}
				}
                //if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("A Button") && GUIUtility.hotControl == 0 && !GUIcontrol.isMenu)
                //    pickUpItem();

                /*if (pickObjFirst)
                {
                    if (!GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjFirst);
                    }
                    if (Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObjFirst);
                    }
                }*/
                break;
            case State.compared:
                //happens while comparing two items
                if (ShoppingCart.GetComponent<Renderer>().enabled)
                    ShoppingCart.GetComponent<Renderer>().enabled = false;
                compareItem();
                if (CameraZoom.zoomBack)
                {
                    float rotation = Input.GetAxis("Mouse X");
                    //TO HAVE CONTINUOUS ROTATION UNCOMMENT THE LINE BELOW AND COMMENT OUT THE CONDITIONALS BELOW
                    if (pickObj)
                        pickObj.transform.Rotate(0, -1 * rotation * 150 * Time.deltaTime, 0);
                    if (pickObjFirst)
                        pickObjFirst.transform.Rotate(0, -1 * rotation * 150 * Time.deltaTime, 0);

                    //if (rotation <= -0.05f && rotating == false)
                    //{
                    //    rotating = true;
                    //    if(pickObj)
                    //        rotateLeftForNinety(pickObj);
                    //    if (pickObjFirst)
                    //        rotateLeftForNinety(pickObjFirst);
                    //}
                    //if (rotation >= 0.05f && rotating == false)
                    //{
                    //    rotating = true;
                    //    if(pickObj)
                    //        rotateRightForNinety(pickObj);
                    //    if (pickObjFirst)
                    //        rotateRightForNinety(pickObjFirst);
                    //}
                    //if (rotation > -0.05f && rotation < 0.05f)
                    //{
                    //    rotating = false;
                    //}
                }
                if (pickObj && !GameFlow.TaskWindow.activeSelf && !GameFlow.TasksWindow.activeSelf)
                {
					if (GameFlow.TaskWindow.activeInHierarchy == false && !GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObj);
                       // state = State.toCompare;
                       // Messenger.Broadcast("Change state");
                    }

                    if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObj);
                       // state = State.toCompare;
                       // Messenger.Broadcast("Change state");
                    }
                }
				else if (pickObjFirst && !GameFlow.TaskWindow.activeSelf && !GameFlow.TasksWindow.activeSelf)
                {
					if (GameFlow.TaskWindow.activeInHierarchy == false && !GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjFirst);
                    }
                    if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObjFirst);
                    }
                }

                //going to compare 3 items
                if(pickObj && pickObjFirst && Input.GetButtonDown("X Button"))
                {
                    Messenger<int>.Broadcast("Compare", 1);
                }

                /*if (Input.GetKeyDown("p"))
                    putBackItem();
                if (Input.GetKeyDown("t"))
                    putIntoCart();*/
                break;
            case State.toCompareThree:
                holdPosition = transform.position;
                if (!ShoppingCart.GetComponent<Renderer>().enabled)
                    ShoppingCart.GetComponent<Renderer>().enabled = true;
                if (Movement1.directWalkOnly)
                {
                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                    RaycastHit hit;
                    if (Movement1.directWalkOnly == true)
                    {
                        if (Physics.Raycast(ray, out hit, 5f) && hit.transform.tag == "Pick")
                        {
                            if (!highlightScript)
                            {
                                highlightScript = hit.transform.GetComponent<Highlight>();
                                highlightScript.highlightItem();
                            }
                            else if (hit.transform.GetComponent<Highlight>() != highlightScript)
                            {
                                highlightScript.unhighlightItem();
                                highlightScript = hit.transform.GetComponent<Highlight>();
                                highlightScript.highlightItem();
                            }
                        }
                        else {
                            if (highlightScript)
                            {
                                highlightScript.unhighlightItem();
                            }
                        }
                    }
                    if ((Input.GetMouseButton(0) || Input.GetButtonDown("A Button")))
                    {
                        pickUpItem();
                    }
                }

                //if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("A Button") && GUIUtility.hotControl == 0 && !GUIcontrol.isMenu)
                //    pickUpItem();
                break;
            case State.comparedThree:
                if (ShoppingCart.GetComponent<Renderer>().enabled)
                    ShoppingCart.GetComponent<Renderer>().enabled = false;
                compareThreeItem();

                if(CameraZoom.zoomBack)
                {
                    //item rotation
                    float rotation = Input.GetAxis("Mouse X");
                    if (pickObj)
                        pickObj.transform.Rotate(0, -1 * rotation * 150 * Time.deltaTime, 0);
                    if (pickObjFirst)
                        pickObjFirst.transform.Rotate(0, -1 * rotation * 150 * Time.deltaTime, 0);
                    if (pickObjSecond)
                        pickObjSecond.transform.Rotate(0, -1 * rotation * 150 * Time.deltaTime, 0);
                }

                if (pickObj && !GameFlow.TaskWindow.activeSelf && !GameFlow.TasksWindow.activeSelf)
                {
                    if (GameFlow.TaskWindow.activeInHierarchy == false && !GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObj);
                    }

                    if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObj);
                    }
                }
                else if (pickObjSecond && !GameFlow.TaskWindow.activeSelf && !GameFlow.TasksWindow.activeSelf)
                {
                    if (GameFlow.TaskWindow.activeInHierarchy == false && !GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjSecond);
                    }
                    if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObjSecond);
                    }
                }
                else if (pickObjFirst && !GameFlow.TaskWindow.activeSelf && !GameFlow.TasksWindow.activeSelf)
                {
                    if (GameFlow.TaskWindow.activeInHierarchy == false && !GUIcontrol.isMenu && Input.GetButtonDown("A Button"))
                    {
                        Messenger<Transform>.Broadcast("put item into cart", pickObjFirst);
                    }
                    if (GameFlow.TaskWindow.activeInHierarchy == false && Input.GetButtonDown("B Button"))
                    {
                        putBackItem(pickObjFirst);
                    }
                }
                break;
            //case State.inCart:
            //break;
        }



        /*
        // commpare two objects
        if (Input.GetKeyDown("c") && pickObj != null)
        {
            isComparing = true;
            isHolding = !isHolding;
            cameraTransform.transform.DetachChildren();
            pickObj.rigidbody.useGravity = true;
            pickObj.transform.position = objPositionBeforePick;
            pickObj.transform.rotation = objRotationBeforePick;
            pickObjFirst = pickObj;
            objFirstPositionBeforePick = objPositionBeforePick;
            objFirstRotationBeforePick = objRotationBeforePick;

            pickedObjectFirst = pickedObject;
            objRotationBeforePick = Quaternion.identity;
            objPositionBeforePick = Vector3.zero;
            pickObj = null;
            isRotate = false;
        }

        // put into the cart
        if (Input.GetKeyDown("t") && pickObj != null)
        {
            isHolding = false;
            cameraTransform.transform.DetachChildren();
            //pickObj.rigidbody.useGravity = true;
            pickObj.rigidbody.useGravity = true;
            pickObj.rigidbody.isKinematic = false;

            //+ShoppingCart.transform.forward
            pickObj.transform.position = ShoppingCart.transform.position + ShoppingCart.transform.up * 0.5f - ShoppingCart.transform.right * 0.5f;
            pickObj.transform.rotation = objRotationBeforePick;
            //Wait();
            pickObj.parent = ShoppingCart;
            //objRotationBeforePick = Quaternion.identity;
            objPositionBeforePick = Vector3.zero;
            pickObj = null;
            isRotate = false;
            //SecondCamera.camera.enabled = true;
            //ThirdCamera.camera.enabled = false;
        }

         */



    }

    private void pickUpItem()
    { 
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f, 0f));
        RaycastHit hit;
        if (!pickObj && Movement1.directWalkOnly == true)
        {
            if (Physics.Raycast(ray, out hit, 5f) && hit.transform.tag == "Pick" && (pickObjFirst == null || (pickObjFirst != null && hit.transform != pickObjFirst)))
            {
                Camera.main.transform.localPosition = Vector3.zero;

				// now there's an object picked
                pickObj = hit.transform;
                pickObj.localScale = new Vector3(1f, 1f, 1f);
                itemScript = pickObj.GetComponent<Highlight>();
				itemScript.highlightItem();

				LODGroup lg = pickObj.GetComponentInParent<LODGroup>();
				LODGroup.Destroy(lg);
                //lg.localReferencePoint = Camera.main.transform.localPosition;
                //lg.RecalculateBounds ();

                // play audio of pickup
                pickupitem_audio_source.clip = pickup_item_audio_clip;
				pickupitem_audio_source.Play();

                if (state == State.idle)
                {
                    Messenger.Broadcast("reset rotation Y");
                    Messenger.Broadcast("Change state");
                    state = State.picked;
                }
                else if (state == State.toCompare)
                {
                    Messenger.Broadcast("reset rotation Y");
                    Messenger.Broadcast("Change state");
                    state = State.compared;
                }
                else if (state == State.toCompareThree)
                {
                    Messenger.Broadcast("reset rotation Y");
                    Messenger.Broadcast("Change state");
                    state = State.comparedThree;
                }
            }
        }

    }

    private void rotateLeftForNinety(Transform objectForRotation)
    {

        //StartCoroutine ("rotateLeft");
        objectForRotation.GetComponent<Rigidbody>().isKinematic = false;
		//objectForRotation.GetComponent<Collider>().enabled = false;
        //
        //if(angle>=0 && angle<90)
        //objectForRotation.parent = null;
        //objectForRotation.parent = GameObject.Find("LookAtPosChild").transform;
        //pickObj.localScale = new Vector3(1f, 1f/1.2f, 1f);
        //iTween.RotateAdd (pickObj.gameObject,iTween.Hash("rotation",new Vector3(0f,90f,0f),"time",1f,"easetype",iTween.EaseType.easeInOutSine));
        objectForRotation.Rotate(0, 1, 0, Space.Self);
		//Debug.Log (objectForRotation.name);
        float angle = objectForRotation.localEulerAngles.y;
        //Debug.Log("test" + angle);
        if (angle >= 0 && angle < 90)
           iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
        else if (angle >= 90 && angle < 180)
            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 180f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
        else if (angle >= 180 && angle < 270)
			//objectForRotation.transform.Rotate(new Vector3(0f, 270f, 0f),Space.Self);
            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 270f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
        else if (angle >= 270 && angle < 360)
            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
        //pickObj.localScale = new Vector3(1f, 1f / 1.2f, 1f);
        //Debug.Log("test2" + angle);
        //StartCoroutine("setParent");
    }

	private void rotateLeftForFortyFive(Transform objectForRotation)
	{
		
		//StartCoroutine ("rotateLeft");
		objectForRotation.GetComponent<Rigidbody>().isKinematic = true;
		//
		//if(angle>=0 && angle<90)
		//objectForRotation.parent = null;
		//objectForRotation.parent = GameObject.Find("LookAtPosChild").transform;
		//pickObj.localScale = new Vector3(1f, 1f/1.2f, 1f);
		//iTween.RotateAdd (pickObj.gameObject,iTween.Hash("rotation",new Vector3(0f,90f,0f),"time",1f,"easetype",iTween.EaseType.easeInOutSine));
		objectForRotation.Rotate(0, 1, 0, Space.Self);
		Debug.Log (objectForRotation.name);
		float angle = objectForRotation.localEulerAngles.y;
		Debug.Log("test" + angle);
		if (angle >= 0 && angle < 90)
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 45f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		else if (angle >= 90 && angle < 180)
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		else if (angle >= 180 && angle < 270)
			//objectForRotation.transform.Rotate(new Vector3(0f, 270f, 0f),Space.Self);
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 135f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		else if (angle >= 270 && angle < 360)
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 180f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		//pickObj.localScale = new Vector3(1f, 1f / 1.2f, 1f);
		//Debug.Log("test2" + angle);
		//StartCoroutine("setParent");
	}

    IEnumerator rotate(Transform objectForRotation, float angle)
    {
        while (objectForRotation.localEulerAngles.y != angle)
        {
            objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, angle, 0f), Time.deltaTime);
            yield return null;
        }
       
    }
    private void rotateRightForNinety(Transform objectForRotation)
    {

        //StartCoroutine ("rotateLeft");
        objectForRotation.GetComponent<Rigidbody>().isKinematic = false;
        //
        //if(angle>=0 && angle<90)
        //objectForRotation.parent = null;
        //objectForRotation.parent = GameObject.Find("LookAtPosChild").transform;
        //pickObj.localScale = new Vector3(1f, 1f/1.2f, 1f);
        //iTween.RotateAdd (pickObj.gameObject,iTween.Hash("rotation",new Vector3(0f,90f,0f),"time",1f,"easetype",iTween.EaseType.easeInOutSine));
        objectForRotation.Rotate(0, 1, 0, Space.Self);
        float angle = objectForRotation.localEulerAngles.y;
       
        if (angle >= 0 && angle < 90)
        //{
            //objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 270f, 0f), Time.deltaTime * 0.01f);
            //StartCoroutine(rotate(objectForRotation, 270f));
       // }
            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 270f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
        else if (angle >= 90 && angle < 180)
            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
            //objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 0.01f);
        
        else if (angle >= 180 && angle < 270)
            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
            //objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 90f, 0f), Time.deltaTime * 0.01f);
			//objectForRotation.transform.Rotate(new Vector3(0f, 90f, 0f),Space.Self);
        else if (angle >= 270 && angle < 360)
            //objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * 0.01f);

            iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 180f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
        //pickObj.localScale = new Vector3(1f, 1f / 1.2f, 1f);
        //Debug.Log("test2" + angle);
        //StartCoroutine("setParent");
    }

	private void rotateRightForFortyFive(Transform objectForRotation)
	{
		
		//StartCoroutine ("rotateLeft");
		objectForRotation.GetComponent<Rigidbody>().isKinematic = true;
		//
		//if(angle>=0 && angle<90)
		//objectForRotation.parent = null;
		//objectForRotation.parent = GameObject.Find("LookAtPosChild").transform;
		//pickObj.localScale = new Vector3(1f, 1f/1.2f, 1f);
		//iTween.RotateAdd (pickObj.gameObject,iTween.Hash("rotation",new Vector3(0f,90f,0f),"time",1f,"easetype",iTween.EaseType.easeInOutSine));
		objectForRotation.Rotate(0, 1, 0, Space.Self);
		float angle = objectForRotation.localEulerAngles.y;
		
		if (angle >= 0 && angle < 90)
			//{
			//objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 270f, 0f), Time.deltaTime * 0.01f);
			//StartCoroutine(rotate(objectForRotation, 270f));
			// }
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 135f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		else if (angle >= 90 && angle < 180)
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 180f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		//objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 0.01f);
		
		else if (angle >= 180 && angle < 270)
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 45f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		//objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 90f, 0f), Time.deltaTime * 0.01f);
		//objectForRotation.transform.Rotate(new Vector3(0f, 90f, 0f),Space.Self);
		else if (angle >= 270 && angle < 360)
			//objectForRotation.localRotation = Quaternion.Lerp(objectForRotation.localRotation, Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * 0.01f);
			
			iTween.RotateTo(objectForRotation.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "islocal", true, "time", 0.5f, "easetype", iTween.EaseType.linear));
		//pickObj.localScale = new Vector3(1f, 1f / 1.2f, 1f);
		//Debug.Log("test2" + angle);
		//StartCoroutine("setParent");
	}

    IEnumerator rotateLeft()
    {
        yield return new WaitForSeconds(1f);
        //pickObj.parent=cameraTransform;
    }

    private void rotateItem()
    {
        if (transform.eulerAngles.y > 180 && transform.eulerAngles.y <= 360)
            transform.eulerAngles = new Vector3(0, 270, 0);
        else
            transform.eulerAngles = new Vector3(0, 90, 0);
        if (Movement1.directWalkOnly && Camera.main.transform.eulerAngles != Vector3.zero)
        {
            //Messenger.Broadcast("set camera rotation to zero");
            //Debug.Log("set zero rotate");
            //Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (Movement1.directWalkOnly && Camera.main.transform.eulerAngles != new Vector3(0f, 180f, 0f))
        {
            //Messenger.Broadcast("set camera rotation to zero");
            //Camera.main.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        if (pickObj != null && pickObj.transform.position == itemScript._startPosition)
		//if (pickObj != null)
        {
            pickObj.GetComponent<Rigidbody>().useGravity = false;
            if (pickObj.parent != null)
            {
//                LODGroup lod = pickObj.parent.GetComponent<LODGroup>();
//                if(lod!=null)
//                    lod.ForceLOD(0);
//				if(pickObj.parent.parent != null){
//					LODGroup lod1 = pickObj.parent.parent.GetComponent<LODGroup>();
//					if(lod1!=null)
//						lod1.ForceLOD(0);
//				}
			}

			parent_item = pickObj.parent;

			pickObj.parent = cameraTransform; //parents the object
            pickObj.localPosition = new Vector3(0,.25f,.25f);
            pickObj.GetComponent<Renderer>().material.shader = itemShader;


            float t = pickObj.GetComponent<Collider>().bounds.size.x;
            float t1 = pickObj.GetComponent<Collider>().bounds.size.y;

            //pickObj.position = cameraTransform.position + cameraTransform.forward * (t + 0.23f) + cameraTransform.up * (t1 * 0.1f - 0.035f); //sets position
            //adjusts the cereal boxes to put labels more in center of screen
            if (pickObj.name.Contains("cereal"))
                pickObj.localPosition += new Vector3(0,-.05f,0);
            pickObj.GetComponent<Collider>().enabled = false;
            pickObj.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
        

        if (pickObj != null)
            pickObj.GetComponent<Renderer>().material.shader = itemShader;

        // right click and press for rotation

        if (Input.GetMouseButtonDown(1))
        {
            isRotate = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotate = false;
        }
        if (isRotate == true)
        {
            //float rotateMouseX = -Input.GetAxis("Mouse X") * Time.deltaTime * 300;
            //pickObj.Rotate(0, rotateMouseX, 0, Space.World);
        }
    }

    private void putBackItem(Transform obj)
    {
        threeCompared = false;

		// play audio of putting back item
		putbackitem_audio_source.clip = putback_item_audio_clip;
		putbackitem_audio_source.Play();

        //cameraTransform.transform.DetachChildren();
        obj.parent = parent_item;
		Highlight itemScript_temp = obj.GetComponent<Highlight>();
		itemScript_temp.unhighlightItem();
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
        obj.GetComponent<Rigidbody>().useGravity = true;
		obj.GetComponent<Collider>().enabled = true;
		obj.GetComponent<Rigidbody>().isKinematic = true;

        if (obj == pickObj)
        {
            obj.position = itemScript._startPosition;
            obj.rotation = itemScript._startRotation;
        }
        else if (obj == pickObjFirst)
        {
            obj.position = itemScriptFirst._startPosition;
            obj.rotation = itemScriptFirst._startRotation;
        }
        else if (obj == pickObjSecond)
        {
            obj.position = itemScriptSecond._startPosition;
            obj.rotation = itemScriptSecond._startRotation;

        }
        if (obj == pickObj)
        {
            pickObj = null;
            itemScript = null;
        }
        else if (obj == pickObjFirst)
        {
            pickObjFirst = null;
            itemScriptFirst = null;
        }
        else if (obj == pickObjSecond)
        {
            pickObjSecond = null;
            itemScriptSecond = null;
        }

		highlightScript = null;
    }

    private void putDownItemToCompare()
    {
        //cameraTransform.DetachChildren();
        pickObj.GetComponent<Rigidbody>().useGravity = true;
        //pickObj.position = itemScript._startPosition;
        //pickObj.rotation = itemScript._startRotation;
        pickObj.localEulerAngles = new Vector3(0f, 180f, 0);
        //pickObj.parent = null;
        //pickObj.localScale = new Vector3(1f, 1f, 1f);
        comparePosition = 0;
        pickObj.position = cameraTransform.position + cameraTransform.forward * 1f - cameraTransform.up * 0.3f - cameraTransform.right * 0.7f;

        pickObjFirst = pickObj;
        itemScriptFirst = itemScript;
        pickObjFirst.parent = cameraTransform;
        //pickObjFirst.position = comparePosition;
        //objFirstPositionBeforePick = objPositionBeforePick;
        //objFirstRotationBeforePick = objRotationBeforePick;&& pickObjFirst.position == comparePosition
        itemScript = null;
        pickObj = null;
        isRotate = false;
        state = State.toCompare;
        StartCoroutine("wait");
    }
    IEnumerator wait()
    {
        //fixed the bug with compare press A and aslo pick new obj by pressing A.
        yield return 0;
        Messenger.Broadcast("Change state");

		Messenger.Broadcast ("play breaking bottle audio");
    }



    private void putDownItemToCompareThree()
    {
        //cameraTransform.transform.DetachChildren();
        pickObj.GetComponent<Rigidbody>().useGravity = true;
        //pickObj.position = itemScript._startPosition;
        //pickObj.rotation = itemScript._startRotation;
        comparePosition = 0;
        pickObj.position = cameraTransform.transform.position + cameraTransform.forward * 1f - cameraTransform.up * 0.3f - cameraTransform.transform.right * 0.35f;

        pickObjFirst.GetComponent<Rigidbody>().useGravity = true;
        //pickObj.position = itemScript._startPosition;
        pickObjFirst.rotation = itemScriptFirst._startRotation;
        comparePosition = 0;
        pickObjFirst.position = cameraTransform.transform.position + cameraTransform.forward * 1f - cameraTransform.up * 0.3f - cameraTransform.transform.right * 0.7f;

        pickObjSecond = pickObj;
        itemScriptSecond = itemScript;
        pickObjSecond.parent = cameraTransform;

        itemScript = null;
        pickObj = null;
        isRotate = false;
        state = State.toCompareThree;
        threeCompared = true;
        Messenger.Broadcast("Change state");
    }


    private void compareItem()
    {
        if (transform.eulerAngles.y > 180 && transform.eulerAngles.y <= 360)
            transform.eulerAngles = new Vector3(0, 270, 0);
        else
            transform.eulerAngles = new Vector3(0, 90, 0);
        transform.position = holdPosition;
        Transform robotPrefabTranform = GameObject.Find ("Robot_Prefab").transform;
		robotPrefabTranform.Rotate(0, 0, 0);
//      Transform lookAtPos = GameObject.Find("LookAtPos").transform;
        if (Movement1.directWalkOnly && Camera.main.transform.eulerAngles != Vector3.zero)
        {
			//Messenger.Broadcast("rotate to face aisle");
            //Debug.Log("set zero compare");

        }
        else if (Movement1.directWalkOnly && Camera.main.transform.eulerAngles != new Vector3(0f, 180f, 0f))
        {
            //Messenger.Broadcast("set camera rotation to zero");
            //Camera.main.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

		//myTransform.collider.enabled = false;
		//myTransform.transform.eulerAngles = new Vector3(0f, 90f, 0f);
		//Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);

        if (pickObjFirst != null && comparePosition == 0)
        {
            comparePosition = 1;
            pickObjFirst.GetComponent<Rigidbody>().useGravity = false;
			pickObjFirst.GetComponent<Collider>().enabled = false;
            //pickObjFirst.transform.position = Vector3.zero;
            pickObjFirst.parent = cameraTransform;
            pickObjFirst.localPosition = new Vector3(-.2f, .25f, .33f);
            float t = pickObjFirst.GetComponent<Collider>().bounds.size.x;
            float t1 = pickObjFirst.GetComponent<Collider>().bounds.size.y;
            //pickObj.position = cameraTransform.position + cameraTransform.forward * (t+0.23f)+ cameraTransform.up*(t1*0.1f-0.035f); //sets position

			//pickObjFirst.transform.position = cameraTransform.transform.position + cameraTransform.transform.forward * (t + 0.73f) + cameraTransform.transform.up * (t1 * 0.1f - 0.02f) - cameraTransform.transform.right * (t / 2f + 0.3f); //sets position(t / 2 + 0.08f)

            pickObjFirst.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
        if (pickObj != null && pickObj.position == itemScript._startPosition)
        {
			Debug.Log("Placing second item in compare");
            pickObj.GetComponent<Rigidbody>().useGravity = false;
			pickObj.GetComponent<Collider>().enabled = false;
			pickObj.parent = cameraTransform; //parents the object
            pickObj.localPosition = new Vector3(.2f, .25f, .33f);
            pickObj.GetComponent<Renderer>().material.shader = itemShader;
            float t = pickObj.GetComponent<Collider>().bounds.size.x;
            float t1 = pickObj.GetComponent<Collider>().bounds.size.y;

			//pickObj.transform.position = cameraTransform.transform.position + cameraTransform.transform.forward * (t + 0.73f) + cameraTransform.transform.up * (t1 * 0.1f - 0.02f) + cameraTransform.transform.right * (t / 2f + 0.3f); //sets position(t / 2 - 0.015f)

            pickObj.localEulerAngles = new Vector3(0f, 180f, 0f);
           	//pickObj.transform.position = cameraTransform.transform.position + cameraTransform.forward * 0.63f + cameraTransform.up*0.03f + cameraTransform.transform.right * 0.22f; //sets position
        }
        


        if (pickObjFirst != null)
            pickObjFirst.GetComponent<Renderer>().material.shader = itemShader;
        if (pickObj != null)
            pickObj.GetComponent<Renderer>().material.shader = itemShader;
        if (Input.GetMouseButtonDown(0))
        {
            isRotate = true;
        }
    }



	private void compareThreeItem()
	{
		if (transform.eulerAngles.y > 180 && transform.eulerAngles.y <= 360)
			transform.eulerAngles = new Vector3(0, 270, 0);
		else
			transform.eulerAngles = new Vector3(0, 90, 0);
		transform.position = holdPosition;
		if(!pickObj && !pickObjFirst && !pickObjSecond)
		{
			isRotate = false;
			state = State.idle;
		}
		
		if(pickObjFirst)
		{
			pickObjFirst.GetComponent<Rigidbody>().useGravity = false;
			pickObjFirst.GetComponent<Collider>().enabled = false;
			pickObjFirst.parent = cameraTransform;
			pickObjFirst.localPosition = new Vector3(-.4f, .25f, .4f);
			//pickObjFirst.localPosition = new Vector3(-.4f, 0, .7f);
			
			if (!isRotate)
				pickObjFirst.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		if(pickObjSecond)
		{
			pickObjSecond.GetComponent<Rigidbody>().useGravity = false;
			pickObjSecond.GetComponent<Collider>().enabled = false;
			pickObjSecond.parent = cameraTransform; //parents the object
			pickObjSecond.localPosition = new Vector3(0, .25f, .4f);
			//pickObjSecond.localPosition = new Vector3(0, 0, .7f);
			
			if (!isRotate)
				pickObjSecond.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		if(pickObj)
		{
			pickObj.GetComponent<Rigidbody>().useGravity = false;
			pickObj.GetComponent<Collider>().enabled = false;
			pickObj.parent = cameraTransform; //parents the object
			pickObj.localPosition = new Vector3(.4f, .25f, .4f);
			//pickObj.localPosition = new Vector3(.4f, 0, .7f);
			
			if (!isRotate)
				pickObj.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		
		if (pickObjFirst)
			pickObjFirst.GetComponent<Renderer>().material.shader = itemShader;
		if (pickObjSecond)
			pickObjSecond.GetComponent<Renderer>().material.shader = itemShader;
		if (pickObj)
			pickObj.GetComponent<Renderer>().material.shader = itemShader;
		
		isRotate = true;
	}


    private void putIntoCart(Transform obj)
    {
        //cameraTransform.transform.DetachChildren();
        
        obj.parent = null;
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.GetComponent<Rigidbody>().isKinematic = true;



        obj.tag = "Untagged";
        //  + ShoppingCart.up * 0.5f - ShoppingCart.right * 0.5f
        int ran = Random.Range(0, 10);
        int ran2 = Random.Range(0, 10);
		obj.GetComponent<Collider>().enabled = true;
        obj.position = new Vector3(ShoppingCart.position.x + ran * 0.002f, ShoppingCart.position.y, ShoppingCart.position.z + ran2 * 0.02f);
		obj.GetComponent<Collider>().enabled = false;
        if (obj == pickObj)
            obj.rotation = itemScript._startRotation;
        else if (obj == pickObjFirst)
            obj.rotation = itemScriptFirst._startRotation;
        else if (obj == pickObjSecond)
            obj.rotation = itemScriptSecond._startRotation;
        //obj.localScale = new Vector3(1f, 1f, 1f);
        obj.parent = ShoppingCart;
       // obj.localScale = new Vector3(obj.localScale.x / ShoppingCart.localScale.x, obj.localScale.y / ShoppingCart.localScale.y, obj.localScale.z / ShoppingCart.localScale.z);

		//LODGroup lg = obj.GetComponentInParent<LODGroup>();
		//lg.localReferencePoint = Camera.main.transform.localPosition;
		//lg.RecalculateBounds ();

        //
        ItemsInCartGUI.ItemsInCart.Add(obj);

        if (obj == pickObj)
        {
            pickObj = null;
            itemScript = null;
        }
        else if (obj == pickObjFirst)
        {
            pickObjFirst = null;
            itemScriptFirst = null;
        }
        else if (obj == pickObjSecond)
        {
            pickObjSecond = null;
            itemScriptSecond = null;
        }

    }
}