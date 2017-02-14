using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WalletWindowManager : MonoBehaviour {
	public class ButtonInfo
	{
		public bool mHighlighted = false;
		public bool isPayingOrNot = false;
		public Transform myTransform;
		public Vector3 startPosition;
		public int startDepth;
		public UISprite UI;
		public string buttontype;
		//private List<Vector3> route = new List<Vector3>();

		public ButtonInfo()
		{

		}
	}

	public Transform button1_inwallet;
	public Transform button1_pay;
	public Transform button1cent_inwallet;
	public Transform button1cent_pay;
	public Transform button1dime_inwallet;
	public Transform button1dime_pay;
	public Transform button1nickel_inwallet;
	public Transform button1nickel_pay;
	public Transform button1quarter_inwallet;
	public Transform button1quarter_pay;
	public Transform button5_inwallet;
	public Transform button5_pay;
	public Transform button10_inwallet;
	public Transform button10_pay;
	public Transform button20_inwallet;
	public Transform button20_pay;
	
	public Transform Button1_prefab;
	public Transform Button1cent_prefab;
	public Transform Button1dime_prefab;
	public Transform Button1nickel_prefab;
	public Transform Button1quarter_prefab;
	public Transform Button5_prefab;
	public Transform Button10_prefab;
	public Transform Button20_prefab;
	private Transform myTransform;
	
	/*public  static List<Transform> Button1_Walletlist = new List<Transform>();
	public  static List<Transform> Button1cent_Walletlist = new List<Transform>();
	public  static List<Transform> Button1dime_Walletlist = new List<Transform>();
	public  static List<Transform> Button1nickel_Walletlist = new List<Transform>();
	public  static List<Transform> Button1quarter_Walletlist = new List<Transform>();
	public  static List<Transform> Button5_Walletlist = new List<Transform>();
	public  static List<Transform> Button10_Walletlist = new List<Transform>();
	public  static List<Transform> Button20_Walletlist = new List<Transform>();
*/
	public  static List<ButtonInfo> Button1_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1cent_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1dime_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1nickel_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1quarter_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button5_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button10_Walletlist = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button20_Walletlist = new List<ButtonInfo>();

	public  static List<ButtonInfo> Button1_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1cent_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1dime_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1nickel_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button1quarter_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button5_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button10_list = new List<ButtonInfo>();
	public  static List<ButtonInfo> Button20_list = new List<ButtonInfo>();

/*	public  static List<Transform> Button1_list = new List<Transform>();
	public  static List<Transform> Button1cent_list = new List<Transform>();
	public  static List<Transform> Button1dime_list = new List<Transform>();
	public  static List<Transform> Button1nickel_list = new List<Transform>();
	public  static List<Transform> Button1quarter_list = new List<Transform>();
	public  static List<Transform> Button5_list = new List<Transform>();
	public  static List<Transform> Button10_list = new List<Transform>();
	public  static List<Transform> Button20_list = new List<Transform>();*/
	
	private List<Vector3> route = new List<Vector3>();
	
	public float MoneyInWallet = 0f;
	public float ChangesOffset = 0f;

	private int currentButton = 0;
	private bool stickDelay = false;
	private bool walletOpen = false;
	private List<int> Money = new List<int>();
	private bool aButtonDelay = false;

	void OnEnable()
    {
		
		//Messenger<float>.Broadcast("setMoneyInWallet", 10f);
		//Messenger<float>.Broadcast("setChangesOffsetInWallet", 10f);
		
		
		Messenger<float>.AddListener("setMoneyInWallet", setMoneyInWallet);
		Messenger<float>.AddListener("setChangesOffsetInWallet", setChangesOffsetInWallet);
		
       // Messenger<Transform, string>.AddListener("moveButtonToPay", moveButtonToPay);
    	
		//Messenger<Transform, string>.AddListener("moveButtonBackToWallet", moveButtonBackToWallet);
    	
		Messenger<float>.AddListener("generateChanges", generateChanges);
		
		Messenger.AddListener("putChangesBackToWallet", putChangesBackToWallet);
		
		//Messenger<string>.AddListener("FirstButtonHoverOver", FirstButtonHoverOver);
		//Messenger<string>.AddListener("FirstButtonHoverOverEnd", FirstButtonHoverOverEnd);
	}
	 void OnDisable()
    {
		Messenger<float>.RemoveListener("setMoneyInWallet", setMoneyInWallet);
		Messenger<float>.RemoveListener("setChangesOffsetInWallet", setChangesOffsetInWallet);
		
       // Messenger<Transform, string>.RemoveListener("moveButtonToPay", moveButtonToPay);
   		//Messenger<Transform, string>.RemoveListener("moveButtonBackToWallet", moveButtonBackToWallet);
		Messenger<float>.RemoveListener("generateChanges", generateChanges);
		Messenger.RemoveListener("putChangesBackToWallet", putChangesBackToWallet);
		//Messenger<string>.RemoveListener("FirstButtonHoverOver", FirstButtonHoverOver);
		//Messenger<string>.RemoveListener("FirstButtonHoverOverEnd", FirstButtonHoverOverEnd);
	
	}
		
	void Awake(){
		myTransform = transform;
		MoneyInWallet = 0;
		
		if( button1_inwallet == null)
			button1_inwallet = GameObject.Find("Button1_inwallet").transform;
		if( button1_pay == null)
			button1_pay = GameObject.Find("Button1_pay").transform;
		
		if( button1cent_inwallet == null)
			button1cent_inwallet = GameObject.Find("Button1cent_inwallet").transform;
		if( button1cent_pay == null)
			button1cent_pay = GameObject.Find("Button1cent_pay").transform;
	
		if( button1dime_inwallet == null)
			button1dime_inwallet = GameObject.Find("Button1dime_inwallet").transform;
		if( button1dime_pay == null)
			button1dime_pay = GameObject.Find("Button1dime_pay").transform;
		
		if( button1nickel_inwallet == null)
			button1nickel_inwallet = GameObject.Find("Button1nickel_inwallet").transform;
		if( button1nickel_pay == null)
			button1nickel_pay = GameObject.Find("Button1nickel_pay").transform;
		
		if( button1quarter_inwallet == null)
			button1quarter_inwallet = GameObject.Find("Button1quarter_inwallet").transform;
		if( button1quarter_pay == null)
			button1quarter_pay = GameObject.Find("Button1quarter_pay").transform;
		
		if( button5_inwallet == null)
			button5_inwallet = GameObject.Find("Button5_inwallet").transform;
		if( button5_pay == null)
			button5_pay = GameObject.Find("Button5_pay").transform;
		
		if( button10_inwallet == null)
			button10_inwallet = GameObject.Find("Button10_inwallet").transform;
		if( button10_pay == null)
			button10_pay = GameObject.Find("Button10_pay").transform;
		
		if( button20_inwallet == null)
			button20_inwallet = GameObject.Find("Button20_inwallet").transform;
		if( button20_pay == null)
			button20_pay = GameObject.Find("Button20_pay").transform;
		
		if( Button1_prefab == null){
			Button1_prefab = GameObject.Find("Button1").transform;
			Button1_prefab.gameObject.SetActive(false);
		}
		
		if( Button1cent_prefab == null){
			Button1cent_prefab = GameObject.Find("Button1cent").transform;
			Button1cent_prefab.gameObject.SetActive(false);
		}
		
		if( Button1dime_prefab == null){
			Button1dime_prefab = GameObject.Find("Button1dime").transform;
			Button1dime_prefab.gameObject.SetActive(false);
		}
		
		if( Button1nickel_prefab == null){
			Button1nickel_prefab = GameObject.Find("Button1nickel").transform;
			Button1nickel_prefab.gameObject.SetActive(false);
		}
		
		if( Button1quarter_prefab == null){
			Button1quarter_prefab = GameObject.Find("Button1quarter").transform;
			Button1quarter_prefab.gameObject.SetActive(false);
		}
		
		if( Button5_prefab == null){
			Button5_prefab = GameObject.Find("Button5").transform;
			Button5_prefab.gameObject.SetActive(false);
		}	
		
		if( Button10_prefab == null){
			Button10_prefab = GameObject.Find("Button10").transform;
			Button10_prefab.gameObject.SetActive(false);
		}
		
		if( Button20_prefab == null){
			Button20_prefab = GameObject.Find("Button20").transform;
			Button20_prefab.gameObject.SetActive(false);
		}

	}
	
	// Use this for initialization
	void Start () {
		//setMoneyInWallet(58.8f);
		Messenger<float>.Broadcast("setMoneyInWallet", 38.8f);
		//Messenger<float>.Broadcast("setChangesOffsetInWallet", 10f);
		// Button1 got clicke, send message here and move that button to the location.
		walletOpen = true;
	}
	
	void updateMoneyAtHand(){
		float tmp = 20f*Button20_list.Count + 10f*Button10_list.Count + 5f*Button5_list.Count + Button1_list.Count + 0.25f*Button1quarter_list.Count + 0.1f*Button1dime_list.Count + 0.05f*Button1nickel_list.Count + 0.01f*Button1cent_list.Count;
		myGUI.totalAtHand = Mathf.Round(tmp*100)/100;
		Debug.Log ("Money at hand:"+tmp);
	}
	
	//public void moveButtonToPay(Transform ButtonTransform, string ButtonName){
	public void moveButtonToPay(ButtonInfo ButtonTransform, string ButtonName){
		Transform MoveTo = new GameObject().transform;
		MoveTo.position = Vector3.zero;
		MoveTo.rotation = Quaternion.identity;
		if(ButtonName=="button1"){
			//setButtonUnHighlight(Button1_Walletlist);
			MoveTo.position = new Vector3(button1_pay.position.x, button1_pay.position.y-0.02f*Button1_list.Count, button1_pay.position.z-0.05f*Button1_list.Count);

			Button1_Walletlist.RemoveAt(0);
			if(Button1_Walletlist.Count>0)
				Button1_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			Button1_list.Add(ButtonTransform);
			
			Money[3]--;
			Money[11]++;
			//ResetHighlight();
		}
		else if(ButtonName == "button1cent"){
			//setButtonUnHighlight(Button1cent_Walletlist);
			MoveTo.position = new Vector3(button1cent_pay.position.x, button1cent_pay.position.y-0.02f*Button1cent_list.Count, button1cent_pay.position.z-0.05f*Button1cent_list.Count);
			Button1cent_Walletlist.RemoveAt(0);
			if(Button1cent_Walletlist.Count>0)
				Button1cent_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			Button1cent_list.Add(ButtonTransform);
			Money[7]--;
			Money[15]++;
			//ResetHighlight();
		}
		else if(ButtonName == "button1dime"){
			//setButtonUnHighlight(Button1dime_Walletlist);
			//MoveTo = button1dime_pay;
			MoveTo.position = new Vector3(button1dime_pay.position.x, button1dime_pay.position.y-0.02f*Button1dime_list.Count, button1dime_pay.position.z-0.05f*Button1dime_list.Count);
			Button1dime_Walletlist.RemoveAt(0);
			if(Button1dime_Walletlist.Count>0)
				Button1dime_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			Button1dime_list.Add(ButtonTransform);
			Money[6]--;
			Money[14]++;
			//ResetHighlight();
		}
		else if(ButtonName == "button1nickel"){
			//setButtonUnHighlight(Button1nickel_Walletlist);
			//MoveTo = button1nickel_pay;
			MoveTo.position = new Vector3(button1nickel_pay.position.x, button1nickel_pay.position.y-0.02f*Button1nickel_list.Count, button1nickel_pay.position.z-0.05f*Button1nickel_list.Count);
			Button1nickel_Walletlist.RemoveAt(0);
			if(Button1nickel_Walletlist.Count>0)
				Button1nickel_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			
			Button1nickel_list.Add(ButtonTransform);
			Money[5]--;
			Money[13]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button1quarter"){
			//setButtonUnHighlight(Button1quarter_Walletlist);
			//MoveTo = button1nickel_pay;
			MoveTo.position = new Vector3(button1quarter_pay.position.x, button1quarter_pay.position.y-0.02f*Button1quarter_list.Count, button1quarter_pay.position.z-0.05f*Button1quarter_list.Count);
			Button1quarter_Walletlist.RemoveAt(0);
			if(Button1quarter_Walletlist.Count>0)
				Button1quarter_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			
			Button1quarter_list.Add(ButtonTransform);
			Money[4]--;
			Money[12]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button5"){
			//setButtonUnHighlight(Button5_Walletlist);
			//MoveTo = button5_pay;
			MoveTo.position = new Vector3(button5_pay.position.x, button5_pay.position.y-0.02f*Button5_list.Count, button5_pay.position.z-0.05f*Button5_list.Count);
			Button5_Walletlist.RemoveAt(0);
			if(Button5_Walletlist.Count>0)
				Button5_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			
			Button5_list.Add(ButtonTransform);
			Money[2]--;
			Money[10]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button10"){
			//setButtonUnHighlight(Button10_Walletlist);
			//MoveTo = button10_pay;
			MoveTo.position = new Vector3(button10_pay.position.x, button10_pay.position.y-0.02f*Button10_list.Count, button10_pay.position.z-0.05f*Button10_list.Count);
			Button10_Walletlist.RemoveAt(0);
			if(Button10_Walletlist.Count>0)
				Button10_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			
			Button10_list.Add(ButtonTransform);
			Money[1]--;
			Money[9]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button20"){
			//setButtonUnHighlight(Button20_Walletlist);
			//MoveTo = button20_pay;
			MoveTo.position = new Vector3(button20_pay.position.x, button20_pay.position.y-0.02f*Button20_list.Count, button20_pay.position.z-0.05f*Button20_list.Count);
			Button20_Walletlist.RemoveAt(0);
			if(Button20_Walletlist.Count>0)
				Button20_Walletlist[0].myTransform.gameObject.GetComponent<Collider>().enabled = true;
			
			Button20_list.Add(ButtonTransform);
			Money[0]--;
			Money[8]++;
			//ResetHighlight();
		}
		//else 
		//MoveTo = button20_pay;
		ButtonTransform.startPosition = MoveTo.position;

		string counts = "";
		for(int i = 0; i < Money.Count; i++)
			counts += Money[i] + ",";
		Debug.Log(counts);
		
		route.Add ( ButtonTransform.myTransform.position); 
		//route.Add ( new Vector3(myTransform.position.x , myTransform.position.y + 0.4f, myTransform.position.z - 0.2f));
		//route.Add ( new Vector3(myTransform.position.x , myTransform.position.y +0.4f , myTransform.position.z - 1f));
		
		//route.Add ( new Vector3(myTransform.position.x , myTransform.position.y - 0.2f, myTransform.position.z - 1.5f));//
		route.Add ( new Vector3(MoveTo.position.x , MoveTo.position.y, MoveTo.position.z));
		
		
		iTween.MoveTo(ButtonTransform.myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 1f,  
		                                                      "orienttopath", false,"easetype", iTween.EaseType.linear));
		
		route.Clear();
		MoveTo.position = Vector3.zero;
		updateMoneyAtHand();
		//ResetHighlight();
		/*Transform MoveTo = new GameObject().transform;
		MoveTo.position = Vector3.zero;
		MoveTo.rotation = Quaternion.identity;
		if(ButtonName=="button1"){
			//setButtonUnHighlight(Button1_Walletlist);
			MoveTo.position = new Vector3(button1_pay.position.x, button1_pay.position.y-0.02f*Button1_list.Count, button1_pay.position.z-0.05f*Button1_list.Count);
			Button1_Walletlist.RemoveAt(0);
			if(Button1_Walletlist.Count>0)
				Button1_Walletlist[0].gameObject.collider.enabled = true;
			Button1_list.Add(ButtonTransform);

			Money[3]--;
			Money[11]++;
			//ResetHighlight();
		}
		else if(ButtonName == "button1cent"){
			//setButtonUnHighlight(Button1cent_Walletlist);
			MoveTo.position = new Vector3(button1cent_pay.position.x, button1cent_pay.position.y-0.02f*Button1cent_list.Count, button1cent_pay.position.z-0.05f*Button1cent_list.Count);
			Button1cent_Walletlist.RemoveAt(0);
			if(Button1cent_Walletlist.Count>0)
				Button1cent_Walletlist[0].gameObject.collider.enabled = true;
			Button1cent_list.Add(ButtonTransform);
			Money[7]--;
			Money[15]++;
			//ResetHighlight();
		}
		else if(ButtonName == "button1dime"){
			//setButtonUnHighlight(Button1dime_Walletlist);
			//MoveTo = button1dime_pay;
			MoveTo.position = new Vector3(button1dime_pay.position.x, button1dime_pay.position.y-0.02f*Button1dime_list.Count, button1dime_pay.position.z-0.05f*Button1dime_list.Count);
			Button1dime_Walletlist.RemoveAt(0);
			if(Button1dime_Walletlist.Count>0)
				Button1dime_Walletlist[0].gameObject.collider.enabled = true;
			Button1dime_list.Add(ButtonTransform);
			Money[6]--;
			Money[14]++;
			//ResetHighlight();
		}
		else if(ButtonName == "button1nickel"){
			//setButtonUnHighlight(Button1nickel_Walletlist);
			//MoveTo = button1nickel_pay;
			MoveTo.position = new Vector3(button1nickel_pay.position.x, button1nickel_pay.position.y-0.02f*Button1nickel_list.Count, button1nickel_pay.position.z-0.05f*Button1nickel_list.Count);
			Button1nickel_Walletlist.RemoveAt(0);
			if(Button1nickel_Walletlist.Count>0)
				Button1nickel_Walletlist[0].gameObject.collider.enabled = true;
			
			Button1nickel_list.Add(ButtonTransform);
			Money[5]--;
			Money[13]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button1quarter"){
			//setButtonUnHighlight(Button1quarter_Walletlist);
			//MoveTo = button1nickel_pay;
			MoveTo.position = new Vector3(button1quarter_pay.position.x, button1quarter_pay.position.y-0.02f*Button1quarter_list.Count, button1quarter_pay.position.z-0.05f*Button1quarter_list.Count);
			Button1quarter_Walletlist.RemoveAt(0);
			if(Button1quarter_Walletlist.Count>0)
				Button1quarter_Walletlist[0].gameObject.collider.enabled = true;
			
			Button1quarter_list.Add(ButtonTransform);
			Money[4]--;
			Money[12]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button5"){
			//setButtonUnHighlight(Button5_Walletlist);
			//MoveTo = button5_pay;
			MoveTo.position = new Vector3(button5_pay.position.x, button5_pay.position.y-0.02f*Button5_list.Count, button5_pay.position.z-0.05f*Button5_list.Count);
			Button5_Walletlist.RemoveAt(0);
			if(Button5_Walletlist.Count>0)
				Button5_Walletlist[0].gameObject.collider.enabled = true;
			
			Button5_list.Add(ButtonTransform);
			Money[2]--;
			Money[10]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button10"){
			//setButtonUnHighlight(Button10_Walletlist);
			//MoveTo = button10_pay;
			MoveTo.position = new Vector3(button10_pay.position.x, button10_pay.position.y-0.02f*Button10_list.Count, button10_pay.position.z-0.05f*Button10_list.Count);
			Button10_Walletlist.RemoveAt(0);
			if(Button10_Walletlist.Count>0)
				Button10_Walletlist[0].gameObject.collider.enabled = true;
			
			Button10_list.Add(ButtonTransform);
			Money[1]--;
			Money[9]++;
			//ResetHighlight();
		}
		else if (ButtonName == "button20"){
			//setButtonUnHighlight(Button20_Walletlist);
			//MoveTo = button20_pay;
			MoveTo.position = new Vector3(button20_pay.position.x, button20_pay.position.y-0.02f*Button20_list.Count, button20_pay.position.z-0.05f*Button20_list.Count);
			Button20_Walletlist.RemoveAt(0);
			if(Button20_Walletlist.Count>0)
				Button20_Walletlist[0].gameObject.collider.enabled = true;
			
			Button20_list.Add(ButtonTransform);
			Money[0]--;
			Money[8]++;
			//ResetHighlight();
		}
		//else 
			//MoveTo = button20_pay;

		string counts = "";
		for(int i = 0; i < Money.Count; i++)
			counts += Money[i] + ",";
		Debug.Log(counts);

		route.Add ( ButtonTransform.position); 
			//route.Add ( new Vector3(myTransform.position.x , myTransform.position.y + 0.4f, myTransform.position.z - 0.2f));
			//route.Add ( new Vector3(myTransform.position.x , myTransform.position.y +0.4f , myTransform.position.z - 1f));
			
			//route.Add ( new Vector3(myTransform.position.x , myTransform.position.y - 0.2f, myTransform.position.z - 1.5f));//
			route.Add ( new Vector3(MoveTo.position.x , MoveTo.position.y, MoveTo.position.z));
			
			
			iTween.MoveTo(ButtonTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 1f,  
          	"orienttopath", false,"easetype", iTween.EaseType.linear));
		
		route.Clear();
		MoveTo.position = Vector3.zero;
		updateMoneyAtHand();
		//ResetHighlight();*/
	}
	
	
	void setMoneyInWallet(float num){
		MoneyInWallet = num;
		updateMoneyInWallet();
	}

	void setButtonHighlight(List<ButtonInfo> buttonList)
	{
		buttonList[0].UI.depth = 30;
		buttonList[0].mHighlighted = true;
		if(buttonList[0].buttontype == "button1" || buttonList[0].buttontype == "button5" || buttonList[0].buttontype == "button10" || buttonList[0].buttontype == "button20")
			buttonList[0].myTransform.localScale = new Vector3(1.4f,2.2f,1f);
		else
			buttonList[0].myTransform.localScale = new Vector3(1.1f,1.1f,1f);
		//if(!isPayingOrNot)
		//Messenger<string>.Broadcast("FirstButtonHoverOver",buttontype.ToString());
		//FirstButtonHoverOver(buttonList[0].buttontype.ToString());

		//buttonList[0].UI.depth = 30;
		//buttonList[0].mHighlighted = true;

		/*GameObject go = GameObject.Find (buttonList [0].gameObject.name);
		UIButtonMove other = (UIButtonMove) go.GetComponent(typeof(UIButtonMove));
		other.SetSelected ();
		*///buttonList[0].localScale = new Vector3 (1.4f, 2.2f, 1f );
		Debug.Log (buttonList[0].buttontype);
		for(int i=0; i< buttonList.Count; i++){
			Transform Button = buttonList[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
		}
	}

	void setButtonUnHighlight(List<ButtonInfo> buttonList)
	{
		buttonList[0].UI.depth = buttonList[0].startDepth;
		buttonList[0].mHighlighted = false;
		if(buttonList[0].buttontype == "button1" || buttonList[0].buttontype == "button5" || buttonList[0].buttontype == "button10" || buttonList[0].buttontype == "button20")
			buttonList[0].myTransform.localScale = new Vector3(1.2f,2f,1f);
		else
			buttonList[0].myTransform.localScale = new Vector3(1f,1f,1f);
		//FirstButtonHoverOverEnd(buttonList[0].buttontype.ToString());

		//buttonList[0].UI.depth = buttonList[0].startDepth;
		//buttonList[0].mHighlighted = false;
		/*GameObject go = GameObject.Find (buttonList [0].gameObject.name);
		UIButtonMove other = (UIButtonMove) go.GetComponent(typeof(UIButtonMove));
		other.SetUnSelected ();*/
		//buttonList[0].localScale = new Vector3 (1.2f, 2f, 1f );

		Debug.Log (buttonList[0].buttontype);
		for(int i=0; i< buttonList.Count; i++){
			Transform Button = buttonList[i].myTransform;
			//Button.position = buttonList[i].startPosition;
			Button.position = new Vector3(Button.position.x, Button.position.y-0.03f*i, Button.position.z+0.05f*i);
			}
	}

	void setChangesOffsetInWallet(float num){
		ChangesOffset = Mathf.Round(num*100)/100;
	}
	
	void updateMoneyInWallet(){
		MoneyInWallet = Mathf.Round(MoneyInWallet*100)/100;
		float decimalMoneyInWallet = MoneyInWallet - (int)(MoneyInWallet);
		decimalMoneyInWallet = Mathf.Round(decimalMoneyInWallet*100)/100;
		float	numberOfTwentyDollar = (int) (MoneyInWallet * 0.05f );
		float	numberOfTenDollar = (int) ((MoneyInWallet - numberOfTwentyDollar*20f )*0.1f ) ;
		float	numberOfFiveDollar =(int)  ((MoneyInWallet - numberOfTwentyDollar*20f - numberOfTenDollar*10f)*0.2f );
		float	numberOfOneDollar = (int)  ((MoneyInWallet - numberOfTwentyDollar*20f - numberOfTenDollar*10f - numberOfFiveDollar*5f)*1f );
		
		float	numberOfOneQuarter = (int)  (decimalMoneyInWallet*4f) ;
		float	numberOfOneDime = (int)  ((decimalMoneyInWallet - numberOfOneQuarter*0.25f)*10f) ;
		float	numberOfOneNickel = (int) ((decimalMoneyInWallet - numberOfOneQuarter*0.25f - numberOfOneDime*0.1f)*20f);
		float	numberOfOneCent = (int)  Mathf.Round( ((decimalMoneyInWallet - numberOfOneQuarter*0.25f - numberOfOneDime*0.1f-numberOfOneNickel*0.05f)*100f)*100)/100;
		
		resetMoneyInWalletList();

		for(int i=0; i< numberOfOneDollar; i++){
			Transform Button1_clone = Instantiate(Button1_prefab, button1_inwallet.position, Quaternion.identity)as Transform;
			Button1_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1_clone.parent = myTransform;
			Button1_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button1_clone.gameObject.SetActive(true);
			if(i>0)
				Button1_clone.gameObject.GetComponent<Collider>().enabled = false;
			ButtonInfo bi = new ButtonInfo();
			
			bi.myTransform = Button1_clone.gameObject.transform;
			bi.UI = Button1_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = 1;//bi.UI.depth;  
			bi.buttontype = "button1";
			Debug.Log (bi.startDepth);
			Button1_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfOneCent; i++){
			Transform Button1cent_clone = Instantiate(Button1cent_prefab, button1cent_inwallet.position, Quaternion.identity)as Transform;
			Button1cent_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1cent_clone.parent = myTransform;
			Button1cent_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1cent_clone.gameObject.SetActive(true);
			if(i>0)
				Button1cent_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button1cent_Walletlist.Add(Button1cent_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1cent_clone.gameObject.transform;
			bi.UI = Button1cent_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;
			bi.buttontype = "button1cent";
			bi.startDepth = 7;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button1cent_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfOneDime; i++){
			Transform Button1dime_clone = Instantiate(Button1dime_prefab, button1dime_inwallet.position, Quaternion.identity)as Transform;
			Button1dime_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1dime_clone.parent = myTransform;
			Button1dime_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1dime_clone.gameObject.SetActive(true);
			if(i>0)
				Button1dime_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button1dime_Walletlist.Add(Button1dime_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1dime_clone.gameObject.transform;
			bi.UI = Button1dime_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button1dime";
			//bi.startDepth = 7;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button1dime_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfOneNickel; i++){
			Transform Button1nickel_clone = Instantiate(Button1nickel_prefab, button1nickel_inwallet.position, Quaternion.identity)as Transform;
			Button1nickel_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1nickel_clone.parent = myTransform;
			Button1nickel_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1nickel_clone.gameObject.SetActive(true);
			if(i>0)
				Button1nickel_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button1nickel_Walletlist.Add(Button1nickel_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1nickel_clone.gameObject.transform;
			bi.UI = Button1nickel_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button1nickel";
			//bi.startDepth = 7;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button1nickel_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfOneQuarter; i++){
			Transform Button1quarter_clone = Instantiate(Button1quarter_prefab, button1quarter_inwallet.position, Quaternion.identity)as Transform;
			Button1quarter_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1quarter_clone.parent = myTransform;
			Button1quarter_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1quarter_clone.gameObject.SetActive(true);
			if(i>0)
				Button1quarter_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button1quarter_Walletlist.Add(Button1quarter_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1quarter_clone.gameObject.transform;
			bi.UI = Button1quarter_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button1quarter";
			//bi.startDepth = 7;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button1quarter_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfFiveDollar; i++){
			Transform Button5_clone = Instantiate(Button5_prefab, button5_inwallet.position, Quaternion.identity)as Transform;
			Button5_clone.eulerAngles = new Vector3 (0f, 80f, 0f);
			Button5_clone.parent = myTransform;
			Button5_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button5_clone.gameObject.SetActive(true);
			if(i>0)
				Button5_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button5_Walletlist.Add(Button5_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button5_clone.gameObject.transform;
			bi.UI = Button5_clone.GetComponentInChildren<UISprite>();
			//bi.UI.depth = 5;
			bi.startDepth = bi.UI.depth; 
			bi.buttontype = "button5";
			//bi.startDepth = 5;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button5_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfTenDollar; i++){
			Transform Button10_clone = Instantiate(Button10_prefab, button10_inwallet.position, Quaternion.identity)as Transform;
			Button10_clone.eulerAngles = new Vector3 (0f, 80f, 0f);
			Button10_clone.parent = myTransform;
			Button10_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button10_clone.gameObject.SetActive(true);

			if(i>0)
				Button10_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button10_Walletlist.Add(Button10_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button10_clone.gameObject.transform;
			bi.UI = Button10_clone.GetComponentInChildren<UISprite>();
			//bi.UI.depth = 3;
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button10";
			//bi.startDepth = 3;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button10_Walletlist.Add(bi);
		}
		
		for(int i=0; i< numberOfTwentyDollar; i++){
			Transform Button20_clone = Instantiate(Button20_prefab, button20_inwallet.position, Quaternion.identity)as Transform;
			Button20_clone.eulerAngles = new Vector3 (0f, 80f, 0f);
			Button20_clone.parent = myTransform;
			Button20_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button20_clone.gameObject.SetActive(true);
			if(i>0)
				Button20_clone.gameObject.GetComponent<Collider>().enabled = false;
			//Button20_Walletlist.Add(Button20_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button20_clone.gameObject.transform;
			bi.UI = Button20_clone.GetComponentInChildren<UISprite>();
			//bi.UI.depth = 1;
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button20";
			//bi.startDepth = 1;//bi.UI.depth;  
			Debug.Log (bi.startDepth);
			Button20_Walletlist.Add(bi);
		}

		/*for(int i=0; i< numberOfOneDollar; i++){
			Transform Button1_clone = Instantiate(Button1_prefab, button1_inwallet.position, Quaternion.identity)as Transform;
			Button1_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1_clone.parent = myTransform;
			Button1_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button1_clone.gameObject.SetActive(true);
			if(i>0)
				Button1_clone.gameObject.collider.enabled = false;
			Button1_Walletlist.Add(Button1_clone);
		}
		
		for(int i=0; i< numberOfOneCent; i++){
			Transform Button1cent_clone = Instantiate(Button1cent_prefab, button1cent_inwallet.position, Quaternion.identity)as Transform;
			Button1cent_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1cent_clone.parent = myTransform;
			Button1cent_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1cent_clone.gameObject.SetActive(true);
			if(i>0)
				Button1cent_clone.gameObject.collider.enabled = false;
			Button1cent_Walletlist.Add(Button1cent_clone);
		}
		
		for(int i=0; i< numberOfOneDime; i++){
			Transform Button1dime_clone = Instantiate(Button1dime_prefab, button1dime_inwallet.position, Quaternion.identity)as Transform;
			Button1dime_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1dime_clone.parent = myTransform;
			Button1dime_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1dime_clone.gameObject.SetActive(true);
			if(i>0)
				Button1dime_clone.gameObject.collider.enabled = false;
			Button1dime_Walletlist.Add(Button1dime_clone);
		}
		
		for(int i=0; i< numberOfOneNickel; i++){
			Transform Button1nickel_clone = Instantiate(Button1nickel_prefab, button1nickel_inwallet.position, Quaternion.identity)as Transform;
			Button1nickel_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1nickel_clone.parent = myTransform;
			Button1nickel_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1nickel_clone.gameObject.SetActive(true);
			if(i>0)
				Button1nickel_clone.gameObject.collider.enabled = false;
			Button1nickel_Walletlist.Add(Button1nickel_clone);
		}
		
		for(int i=0; i< numberOfOneQuarter; i++){
			Transform Button1quarter_clone = Instantiate(Button1quarter_prefab, button1quarter_inwallet.position, Quaternion.identity)as Transform;
			Button1quarter_clone.eulerAngles = new Vector3 (0f, 100f, 0f);
			Button1quarter_clone.parent = myTransform;
			Button1quarter_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1quarter_clone.gameObject.SetActive(true);
			if(i>0)
				Button1quarter_clone.gameObject.collider.enabled = false;
			Button1quarter_Walletlist.Add(Button1quarter_clone);
		}
		
		for(int i=0; i< numberOfFiveDollar; i++){
			Transform Button5_clone = Instantiate(Button5_prefab, button5_inwallet.position, Quaternion.identity)as Transform;
			Button5_clone.eulerAngles = new Vector3 (0f, 80f, 0f);
			Button5_clone.parent = myTransform;
			Button5_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button5_clone.gameObject.SetActive(true);
			if(i>0)
				Button5_clone.gameObject.collider.enabled = false;
			Button5_Walletlist.Add(Button5_clone);
		}
		
		for(int i=0; i< numberOfTenDollar; i++){
			Transform Button10_clone = Instantiate(Button10_prefab, button10_inwallet.position, Quaternion.identity)as Transform;
			Button10_clone.eulerAngles = new Vector3 (0f, 80f, 0f);
			Button10_clone.parent = myTransform;
			Button10_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button10_clone.gameObject.SetActive(true);
			if(i>0)
				Button10_clone.gameObject.collider.enabled = false;
			Button10_Walletlist.Add(Button10_clone);
		}
		
		for(int i=0; i< numberOfTwentyDollar; i++){
			Transform Button20_clone = Instantiate(Button20_prefab, button20_inwallet.position, Quaternion.identity)as Transform;
			Button20_clone.eulerAngles = new Vector3 (0f, 80f, 0f);
			Button20_clone.parent = myTransform;
			Button20_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button20_clone.gameObject.SetActive(true);
			if(i>0)
				Button20_clone.gameObject.collider.enabled = false;
			Button20_Walletlist.Add(Button20_clone);
		}
*/
		Money.Add(Button20_Walletlist.Count);
		Money.Add(Button10_Walletlist.Count);
		Money.Add(Button5_Walletlist.Count);
		Money.Add(Button1_Walletlist.Count);
		Money.Add(Button1quarter_Walletlist.Count);
		Money.Add(Button1nickel_Walletlist.Count);
		Money.Add(Button1dime_Walletlist.Count);
		Money.Add(Button1cent_Walletlist.Count);

		Money.Add(0);
		Money.Add(0);
		Money.Add(0);
		Money.Add(0);
		Money.Add(0);
		Money.Add(0);
		Money.Add(0);
		Money.Add(0);

		currentButton = 0;
	}
	
	void resetMoneyInWalletList(){
		ResetButtonList(Button1_Walletlist);
		ResetButtonList(Button1cent_Walletlist);
		ResetButtonList(Button1dime_Walletlist);
		ResetButtonList(Button1nickel_Walletlist);
		ResetButtonList(Button1quarter_Walletlist);
		ResetButtonList(Button5_Walletlist);
		ResetButtonList(Button10_Walletlist);
		ResetButtonList(Button20_Walletlist);
		/*for (int i=0; i<Button1_Walletlist.Count; i++){
			Transform tmp = Button1_Walletlist[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button1_Walletlist.Clear();
		
		for (int i=0; i<Button1cent_Walletlist.Count; i++){
			Transform tmp = Button1cent_Walletlist[i];
			Button1_Walletlist[i].UI.depth = Button1_Walletlist[i].startDepth;
			Button1_Walletlist[i].mHighlighted = false;
			Destroy(tmp.gameObject);
		}
		Button1cent_Walletlist.Clear();
		
		for (int i=0; i<Button1dime_Walletlist.Count; i++){
			Transform tmp = Button1dime_Walletlist[i];
			Destroy(tmp.gameObject);
		}
		Button1dime_Walletlist.Clear();
		
		for (int i=0; i<Button1nickel_Walletlist.Count; i++){
			Transform tmp = Button1nickel_Walletlist[i];
			Destroy(tmp.gameObject);
		}
		Button1nickel_Walletlist.Clear();
		
		for (int i=0; i<Button1quarter_Walletlist.Count; i++){
			Transform tmp = Button1quarter_Walletlist[i];
			Destroy(tmp.gameObject);
		}
		Button1quarter_Walletlist.Clear();
		
		for (int i=0; i<Button5_Walletlist.Count; i++){
			Transform tmp = Button5_Walletlist[i];
			Destroy(tmp.gameObject);
		}
		Button5_Walletlist.Clear();
		
		for (int i=0; i<Button10_Walletlist.Count; i++){
			Transform tmp = Button10_Walletlist[i];
			Destroy(tmp.gameObject);
		}
		Button10_Walletlist.Clear();*/
	}

	void ResetButtonList(List<ButtonInfo> list)
	{
		for (int i=0; i<list.Count; i++){
			Transform tmp = list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		list.Clear();
	}
	
/*	void HighlightButton(List<Transform> ButtonList){
		GameObject go = GameObject.Find(ButtonList[0].name);
		UIButtonMove mo = go.GetComponent<UIButtonMove>();
		mo.SetSelected();
		/*for(int i=0; i< ButtonList.Count; i++){
			Transform Button = ButtonList[i];
			Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
			if(i == 0)
				Button.localScale = new Vector3(1.32f, 2.2f, 1f);
		}*/
		//Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);

//	}
	
/*	void UnHighlightButton(List<Transform> ButtonList){
	/*	for(int i=0; i< ButtonList.Count; i++){
			Transform Button = ButtonList[i];
				Button.position = new Vector3(Button.position.x, Button.position.y-0.03f*i, Button.position.z+0.05f*i);
				if(i==0)
				Button.localScale = new Vector3(1.2f, 2f, 1f);
			}*/
	//}
	
	
	/*public void FirstButtonHoverOver(string ButtonName){
		Debug.Log (ButtonName);
		if(ButtonName=="button1"){
			for(int i=0; i< Button1_Walletlist.Count; i++){
				Transform Button = Button1_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
			}
		}
		else if(ButtonName == "button1cent"){
			for(int i=0; i< Button1cent_Walletlist.Count; i++){
				Transform Button = Button1cent_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
			}
		}
		else if(ButtonName == "button1dime"){
			for(int i=0; i< Button1dime_Walletlist.Count; i++){
				Transform Button = Button1dime_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
			}
		}
		else if(ButtonName == "button1nickel"){
			for(int i=0; i< Button1nickel_Walletlist.Count; i++){
				Transform Button = Button1nickel_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
			}
		}
		else if (ButtonName == "button1quarter"){
			for(int i=0; i< Button1quarter_Walletlist.Count; i++){
				Transform Button = Button1quarter_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z-0.05f*i);
			}
		}
		else if (ButtonName == "button5"){
			for(int i=0; i< Button5_Walletlist.Count; i++){
				Transform Button = Button5_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z+0.05f*i);
			}
		}
		else if (ButtonName == "button10"){
			for(int i=0; i< Button10_Walletlist.Count; i++){
				Transform Button = Button10_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z+0.05f*i);
			}
		}
		else if (ButtonName == "button20"){
			for(int i=0; i< Button20_Walletlist.Count; i++){
				Transform Button = Button20_Walletlist[i].myTransform;
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z+0.05f*i);
				Button.localScale = new Vector3(1.32f, 2.2f, 1f);
			}
		}
	}*/
	
/*	public void FirstButtonHoverOverEnd(string ButtonName){
		Debug.Log (ButtonName);
		if(ButtonName=="button1"){
			for(int i=0; i< Button1_Walletlist.Count; i++){
				Transform Button = Button1_Walletlist[i].myTransform;
				Button.position = button1_inwallet.position;
			}
		}
		else if(ButtonName == "button1cent"){
			for(int i=0; i< Button1cent_Walletlist.Count; i++){
				Transform Button = Button1cent_Walletlist[i].myTransform;
				Button.position = button1cent_inwallet.position;
			}
		}
		else if(ButtonName == "button1dime"){
			for(int i=0; i< Button1dime_Walletlist.Count; i++){
				Transform Button = Button1dime_Walletlist[i].myTransform;
				Button.position = button1dime_inwallet.position;
			}
		}
		else if(ButtonName == "button1nickel"){
			for(int i=0; i< Button1nickel_Walletlist.Count; i++){
				Transform Button = Button1nickel_Walletlist[i].myTransform;
				Button.position = button1nickel_inwallet.position;
			}
		}
		else if (ButtonName == "button1quarter"){
			for(int i=0; i< Button1quarter_Walletlist.Count; i++){
				Transform Button = Button1quarter_Walletlist[i].myTransform;
				Button.position = button1quarter_inwallet.position;
			}
		}
		else if (ButtonName == "button5"){
			for(int i=0; i< Button5_Walletlist.Count; i++){
				Transform Button = Button5_Walletlist[i].myTransform;
				Button.position = button5_inwallet.position;
			}
		}
		else if (ButtonName == "button10"){
			for(int i=0; i< Button10_Walletlist.Count; i++){
				Transform Button = Button10_Walletlist[i].myTransform;
				Button.position = button10_inwallet.position;
			}
		}
		else if (ButtonName == "button20"){
			for(int i=0; i< Button20_Walletlist.Count; i++){
				Transform Button = Button20_Walletlist[i].myTransform;
				Button.position = button20_inwallet.position;
				Button.localScale = new Vector3(1.2f, 2f, 1f);
			}
			/*for(int i=0; i< Button20_list.Count; i++){
				Transform Button = Button20_list[i];
				Button.position = new Vector3(Button.position.x, Button.position.y+0.03f*i, Button.position.z+0.05f*i);
				Button.localScale = new Vector3(1.2f, 2f, 1f);
			}*/
//		}
//	}
	
	//void moveButtonBackToWallet(Transform ButtonTransform, string ButtonName){
	void moveButtonBackToWallet(ButtonInfo ButtonTransform, string ButtonName){
		Transform MoveTo = null;
		if(ButtonName=="button1"){
			MoveTo = button1_inwallet;
			//Button1_list.RemoveAt(Button1_list.Count-1);
			Button1_list.RemoveAt(0);
			if (Button1_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button1_Walletlist.Add(ButtonTransform);
			Money[11]--;
			Money[3]++;
		}
		else if(ButtonName == "button1cent"){
			MoveTo = button1cent_inwallet;
			Button1cent_list.RemoveAt(0);//Button1cent_list.Count-1);
			if (Button1cent_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button1cent_Walletlist.Add(ButtonTransform);
			Money[15]--;
			Money[7]++;
		}
		else if(ButtonName == "button1dime"){
			MoveTo = button1dime_inwallet;
			Button1dime_list.RemoveAt(0);//Button1dime_list.Count-1);	
			if (Button1dime_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button1dime_Walletlist.Add(ButtonTransform);
			Money[14]--;
			Money[6]++;
		}
		else if(ButtonName == "button1nickel"){
			MoveTo = button1nickel_inwallet;
			Button1nickel_list.RemoveAt(0);//Button1nickel_list.Count-1);
			if (Button1nickel_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button1nickel_Walletlist.Add(ButtonTransform);
			Money[13]--;
			Money[5]++;
		}
		else if (ButtonName == "button1quarter"){
			MoveTo = button1quarter_inwallet;
			Button1quarter_list.RemoveAt(0);//Button1quarter_list.Count-1);
			if (Button1quarter_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button1quarter_Walletlist.Add(ButtonTransform);
			Money[12]--;
			Money[4]++;
		}
		else if (ButtonName == "button5"){
			MoveTo = button5_inwallet;
			Button5_list.RemoveAt(0);//Button5_list.Count-1);
			if (Button5_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button5_Walletlist.Add(ButtonTransform);
			Money[10]--;
			Money[2]++;
		}
		else if (ButtonName == "button10"){
			MoveTo = button10_inwallet;
			Button10_list.RemoveAt(0);//Button10_list.Count-1);
			if (Button10_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button10_Walletlist.Add(ButtonTransform);
			Money[9]--;
			Money[1]++;
		}
		else if (ButtonName == "button20"){
			MoveTo = button20_inwallet;
			Button20_list.RemoveAt(0);//Button20_list.Count-1);
			if (Button20_Walletlist.Count>0)
				ButtonTransform.myTransform.gameObject.GetComponent<Collider>().enabled = false;
			Button20_Walletlist.Add(ButtonTransform);
			Money[8]--;
			Money[0]++;
		}
		else 
			MoveTo = button20_inwallet;
		
		//Debug.Log(Button1_list.Count + "dollars");
		string counts = "";
		for(int i = 0; i < Money.Count; i++)
			counts += Money[i] + ",";
		Debug.Log(counts);
		
		route.Add ( ButtonTransform.myTransform.position); 
		route.Add ( new Vector3(MoveTo.position.x , MoveTo.position.y, MoveTo.position.z));
		iTween.MoveTo(ButtonTransform.myTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 1f,  
          	"orienttopath", false,"easetype", iTween.EaseType.linear));  
			
			
		
		route.Clear();
		updateMoneyAtHand();
		/*Transform MoveTo = null;
		if(ButtonName=="button1"){
			MoveTo = button1_inwallet;
			//Button1_list.Remove(myTransform);
			Button1_list.RemoveAt(Button1_list.Count-1);
			if (Button1_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button1_Walletlist.Add(ButtonTransform);
			Money[11]--;
			Money[3]++;
			ResetHighlight();
		}
		else if(ButtonName == "button1cent"){
			MoveTo = button1cent_inwallet;
			Button1cent_list.RemoveAt(Button1cent_list.Count-1);
			if (Button1cent_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button1cent_Walletlist.Add(ButtonTransform);
			Money[15]--;
			Money[7]++;
			ResetHighlight();
		}
		else if(ButtonName == "button1dime"){
			MoveTo = button1dime_inwallet;
			Button1dime_list.RemoveAt(Button1dime_list.Count-1);	
			if (Button1dime_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button1dime_Walletlist.Add(ButtonTransform);
			Money[14]--;
			Money[6]++;
			ResetHighlight();
		}
		else if(ButtonName == "button1nickel"){
			MoveTo = button1nickel_inwallet;
			Button1nickel_list.RemoveAt(Button1nickel_list.Count-1);
			if (Button1nickel_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button1nickel_Walletlist.Add(ButtonTransform);
			Money[13]--;
			Money[5]++;
			ResetHighlight();
		}
		else if (ButtonName == "button1quarter"){
			MoveTo = button1quarter_inwallet;
			Button1quarter_list.RemoveAt(Button1quarter_list.Count-1);
			if (Button1quarter_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button1quarter_Walletlist.Add(ButtonTransform);
			Money[12]--;
			Money[4]++;
			ResetHighlight();
		}
		else if (ButtonName == "button5"){
			MoveTo = button5_inwallet;
			Button5_list.RemoveAt(Button5_list.Count-1);
			if (Button5_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button5_Walletlist.Add(ButtonTransform);
			Money[10]--;
			Money[2]++;
			ResetHighlight();
		}
		else if (ButtonName == "button10"){
			MoveTo = button10_inwallet;
			Button10_list.RemoveAt(Button10_list.Count-1);
			if (Button10_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button10_Walletlist.Add(ButtonTransform);
			Money[9]--;
			Money[1]++;
			ResetHighlight();
		}
		else if (ButtonName == "button20"){
			MoveTo = button20_inwallet;
			Button20_list.RemoveAt(Button20_list.Count-1);
			if (Button20_Walletlist.Count>0)
				ButtonTransform.gameObject.collider.enabled = false;
			Button20_Walletlist.Add(ButtonTransform);
			Money[8]--;
			Money[0]++;
			ResetHighlight();
		}
		else 
			MoveTo = button20_inwallet;
		
		//Debug.Log(Button1_list.Count + "dollars");
		string counts = "";
		for(int i = 0; i < Money.Count; i++)
			counts += Money[i] + ",";
		Debug.Log(counts);
		
		route.Add ( ButtonTransform.position); 
		route.Add ( new Vector3(MoveTo.position.x , MoveTo.position.y, MoveTo.position.z));
		iTween.MoveTo(ButtonTransform.gameObject, iTween.Hash("path", route.ToArray(), "time", 1f,  
          	"orienttopath", false,"easetype", iTween.EaseType.linear));  
			
			
		
		route.Clear();
		updateMoneyAtHand();*/
	}
	
	void generateChanges(float totalAfterPaid){
		for (int i=0; i<Button1_list.Count; i++){
			Transform tmp = Button1_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button1_list.Clear();
		
		for (int i=0; i<Button1cent_list.Count; i++){
			Transform tmp = Button1cent_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button1cent_list.Clear();
		
		for (int i=0; i<Button1dime_list.Count; i++){
			Transform tmp = Button1dime_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button1dime_list.Clear();
		
		for (int i=0; i<Button1nickel_list.Count; i++){
			Transform tmp = Button1nickel_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button1nickel_list.Clear();
		
		for (int i=0; i<Button1quarter_list.Count; i++){
			Transform tmp = Button1quarter_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button1quarter_list.Clear();
		
		for (int i=0; i<Button5_list.Count; i++){
			Transform tmp = Button5_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button5_list.Clear();
		
		for (int i=0; i<Button10_list.Count; i++){
			Transform tmp = Button10_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button10_list.Clear();
		
		
		for (int i=0; i<Button20_list.Count; i++){
			Transform tmp = Button20_list[i].myTransform;
			Destroy(tmp.gameObject);
		}
		Button20_list.Clear();

		for(int i = 8; i < Money.Count; i++)
			Money[i] = 0;
		
		totalAfterPaid = Mathf.Round(totalAfterPaid*100)/100;
		
		totalAfterPaid = totalAfterPaid + ChangesOffset;
		
		float decimalAfterPaid = totalAfterPaid - (int)(totalAfterPaid);
		decimalAfterPaid = Mathf.Round(decimalAfterPaid*100)/100;
		float	numberOfTwentyDollarToPay = (int) (totalAfterPaid * 0.05f );
		float	numberOfTenDollarToPay = (int) ((totalAfterPaid - numberOfTwentyDollarToPay*20f )*0.1f ) ;
		float	numberOfFiveDollarToPay =(int)  ((totalAfterPaid - numberOfTwentyDollarToPay*20f - numberOfTenDollarToPay*10f)*0.2f );
		float	numberOfOneDollarToPay = (int)  ((totalAfterPaid - numberOfTwentyDollarToPay*20f - numberOfTenDollarToPay*10f - numberOfFiveDollarToPay*5f)*1f );
			
		//	Debug.Log(numberOfTwentyDollarToPay+"twenty"+numberOfTenDollarToPay+"ten"+ numberOfFiveDollarToPay+"five"+ numberOfOneDollarToPay+"one");
			
		float	numberOfOneQuarterToPay = (int)  (decimalAfterPaid*4f) ;
		float	numberOfOneDimeToPay = (int)  ((decimalAfterPaid - numberOfOneQuarterToPay*0.25f)*10f) ;
		float	numberOfOneNickelToPay = (int) ((decimalAfterPaid - numberOfOneQuarterToPay*0.25f - numberOfOneDimeToPay*0.1f)*20f);
		float	numberOfOneCentToPay = (int)  Mathf.Round( ((decimalAfterPaid - numberOfOneQuarterToPay*0.25f - numberOfOneDimeToPay*0.1f-numberOfOneNickelToPay*0.05f)*100f)*100)/100;
		
		//one dollar
		for(int i=0; i< numberOfOneDollarToPay; i++){
			Vector3 pos = new Vector3(button1_pay.position.x, button1_pay.position.y-0.02f*i, button1_pay.position.z-0.05f*i);
			Transform Button1_clone = Instantiate(Button1_prefab, pos , Quaternion.identity)as Transform;
			Button1_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button1_clone.parent = myTransform;
			Button1_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button1_clone.gameObject.SetActive(true);
			Button1_clone.GetComponent<Collider>().enabled = false;

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1_clone.gameObject.transform;
			bi.UI = Button1_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = 1;//bi.UI.depth;  
			bi.buttontype = "button1";
			Button1_list.Add(bi);
			//Button1_list.Add(Button1_clone);
			Money[11]++;
		}
		
		//one cent
		for(int i=0; i< numberOfOneCentToPay; i++){
			Vector3 pos = new Vector3(button1cent_pay.position.x, button1cent_pay.position.y-0.02f*i, button1cent_pay.position.z-0.05f*i);
			Transform Button1cent_clone = Instantiate(Button1cent_prefab, pos , Quaternion.identity)as Transform;
			Button1cent_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button1cent_clone.parent = myTransform;
			Button1cent_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1cent_clone.gameObject.SetActive(true);
			Button1cent_clone.GetComponent<Collider>().enabled = false;
			//Button1cent_list.Add(Button1cent_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1cent_clone.gameObject.transform;
			bi.UI = Button1cent_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;
			bi.buttontype = "button1cent";
			bi.startDepth = 7;//bi.UI.depth;  
			Button1cent_list.Add(bi);
			Money[15]++;
		}
		
		// one dime
		for(int i=0; i< numberOfOneDimeToPay; i++){
			Vector3 pos = new Vector3(button1dime_pay.position.x, button1dime_pay.position.y-0.02f*i, button1dime_pay.position.z-0.05f*i);
			Transform Button1dime_clone = Instantiate(Button1dime_prefab, pos , Quaternion.identity)as Transform;
			Button1dime_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button1dime_clone.parent = myTransform;
			Button1dime_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1dime_clone.gameObject.SetActive(true);
			Button1dime_clone.GetComponent<Collider>().enabled = false;
			//Button1dime_list.Add(Button1dime_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1dime_clone.gameObject.transform;
			bi.UI = Button1dime_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button1dime";
			Button1dime_list.Add(bi);
			Money[14]++;
		}
		
		// one nickel
		for(int i=0; i< numberOfOneNickelToPay; i++){
			Vector3 pos = new Vector3(button1nickel_pay.position.x, button1nickel_pay.position.y-0.02f*i, button1nickel_pay.position.z-0.05f*i);
			Transform Button1nickel_clone = Instantiate(Button1nickel_prefab, pos , Quaternion.identity)as Transform;
			Button1nickel_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button1nickel_clone.parent = myTransform;
			Button1nickel_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1nickel_clone.gameObject.SetActive(true);
			Button1nickel_clone.GetComponent<Collider>().enabled = false;
			//Button1nickel_list.Add(Button1nickel_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1nickel_clone.gameObject.transform;
			bi.UI = Button1nickel_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button1nickel";
			Button1nickel_list.Add(bi);
			Money[13]++;
		}
		
		
		// one quarter
		for(int i=0; i< numberOfOneQuarterToPay; i++){
			Vector3 pos = new Vector3(button1quarter_pay.position.x, button1quarter_pay.position.y-0.02f*i, button1quarter_pay.position.z-0.05f*i);
			Transform Button1quarter_clone = Instantiate(Button1quarter_prefab, pos , Quaternion.identity)as Transform;
			Button1quarter_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button1quarter_clone.parent = myTransform;
			Button1quarter_clone.localScale = new Vector3 (1f, 1f, 1f );	
			Button1quarter_clone.gameObject.SetActive(true);
			Button1quarter_clone.GetComponent<Collider>().enabled = false;
//			Button1quarter_list.Add(Button1quarter_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button1quarter_clone.gameObject.transform;
			bi.UI = Button1quarter_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button1quarter";
			Button1quarter_list.Add(bi);
			Money[12]++;
		}
		
		
		// five dollar
		for(int i=0; i< numberOfFiveDollarToPay; i++){
			Vector3 pos = new Vector3(button5_pay.position.x, button5_pay.position.y-0.02f*i, button5_pay.position.z-0.05f*i);
			Transform Button5_clone = Instantiate(Button5_prefab, pos , Quaternion.identity)as Transform;
			Button5_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button5_clone.parent = myTransform;
			Button5_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button5_clone.gameObject.SetActive(true);
			Button5_clone.GetComponent<Collider>().enabled = false;
			//Button5_list.Add(Button5_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button5_clone.gameObject.transform;
			bi.UI = Button5_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button5";
			Button5_list.Add(bi);
			Money[10]++;
		}
		
		// ten dollar
		for(int i=0; i< numberOfTenDollarToPay; i++){
			Vector3 pos = new Vector3(button10_pay.position.x, button10_pay.position.y-0.02f*i, button10_pay.position.z-0.05f*i);
			Transform Button10_clone = Instantiate(Button10_prefab, pos , Quaternion.identity)as Transform;
			Button10_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button10_clone.parent = myTransform;
			Button10_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button10_clone.gameObject.SetActive(true);
			Button10_clone.GetComponent<Collider>().enabled = false;
			//Button10_list.Add(Button10_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button10_clone.gameObject.transform;
			bi.UI = Button10_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button10";
			Button10_list.Add(bi);
			Money[9]++;
		}
		
		//twenty dollar
		for(int i=0; i< numberOfTwentyDollarToPay; i++){
			Vector3 pos = new Vector3(button20_pay.position.x, button20_pay.position.y-0.02f*i, button20_pay.position.z-0.05f*i);
			Transform Button20_clone = Instantiate(Button20_prefab, pos , Quaternion.identity)as Transform;
			Button20_clone.eulerAngles = new Vector3 (0f, 90f, 0f);
			Button20_clone.parent = myTransform;
			Button20_clone.localScale = new Vector3 (1.2f, 2f, 1f );	
			Button20_clone.gameObject.SetActive(true);
			Button20_clone.GetComponent<Collider>().enabled = false;
			//Button20_list.Add(Button20_clone);

			ButtonInfo bi = new ButtonInfo();
			bi.myTransform = Button20_clone.gameObject.transform;
			bi.UI = Button20_clone.GetComponentInChildren<UISprite>();
			bi.startDepth = bi.UI.depth;  
			bi.buttontype = "button20";
			Button20_list.Add(bi);
			Money[8]++;
		}

		string counts = "";
		for(int i = 0; i < Money.Count; i++)
			counts += Money[i] + ",";
		Debug.Log(counts);
	}
	
	
	void putChangesBackToWallet(){
		PutBackMoney(Button1_list, Button1_Walletlist, "button1", new Vector3(0f,100f,0f));
		PutBackMoney(Button1cent_list, Button1cent_Walletlist, "button1cent", new Vector3(0f,100f,0f));
		PutBackMoney(Button1dime_list, Button1dime_Walletlist, "button1dime", new Vector3(0f,100f,0f));
		PutBackMoney(Button1nickel_list, Button1nickel_Walletlist, "button1nickel", new Vector3(0f,100f,0f));
		PutBackMoney(Button1quarter_list, Button1quarter_Walletlist, "button1quarter", new Vector3(0f,100f,0f));
		PutBackMoney(Button5_list, Button5_Walletlist, "button5", new Vector3(0f,80f,0f));
		PutBackMoney(Button10_list, Button10_Walletlist, "button10", new Vector3(0f, 80f, 0f));
		PutBackMoney(Button20_list, Button20_Walletlist, "button20", new Vector3(0f, 80f, 0f));
		string counts = "";
		for(int i = 0; i < Money.Count; i++)
			counts += Money[i] + ",";
		Debug.Log(counts);

		StartCoroutine("CloseWalletAfterChange");
		/*if(Button1_list.Count>0){
			for (int i=0; i<Button1_list.Count; i++){
				Transform button = Button1_list[i];
				button.eulerAngles =  new Vector3(0f, 100f, 0f);
				if(i>0)
					button.parent = Button1_list[0];
				Button1_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button1_list[0], "button1");
		}
		Button1_list.Clear();
		
		if(Button1cent_list.Count>0){
			for (int i=0; i<Button1cent_list.Count; i++){
				Transform button = Button1cent_list[i];
				button.eulerAngles =  new Vector3(0f, 100f, 0f);
				if(i>0)
					button.parent = Button1cent_list[0];
				Button1cent_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button1cent_list[0], "button1cent");
		}
		Button1cent_list.Clear();

		
		if(Button1dime_list.Count>0){
			for (int i=0; i<Button1dime_list.Count; i++){
				Transform button = Button1dime_list[i];
				button.eulerAngles =  new Vector3(0f, 100f, 0f);
				if(i>0)
					button.parent = Button1dime_list[0];
				Button1dime_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button1dime_list[0], "button1dime");
		}
		Button1dime_list.Clear();
		
		
		if(Button1nickel_list.Count>0){
			for (int i=0; i<Button1nickel_list.Count; i++){
				Transform button = Button1nickel_list[i];
				button.eulerAngles =  new Vector3(0f, 100f, 0f);
				if(i>0)
					button.parent = Button1nickel_list[0];
				Button1nickel_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button1nickel_list[0], "button1nickel");
		}
		Button1nickel_list.Clear();
		
		
		//Debug.Log("number of quarter:"+Button1quarter_list.Count);
		if(Button1quarter_list.Count>0){
			for (int i=0; i<Button1quarter_list.Count; i++){
				Transform button = Button1quarter_list[i];
				button.eulerAngles =  new Vector3(0f, 100f, 0f);
				if(i>0)
					button.parent = Button1quarter_list[0];
				Button1quarter_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button1quarter_list[0], "button1quarter");
		}
		Button1quarter_list.Clear();
		
		if(Button5_list.Count>0){
			for (int i=0; i<Button5_list.Count; i++){
				Transform button = Button5_list[i];
				button.eulerAngles =  new Vector3(0f, 80f, 0f);
				if(i>0)
					button.parent = Button5_list[0];
				Button5_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button5_list[0], "button5");
		}
		Button5_list.Clear();
		
		if(Button10_list.Count>0){
			for (int i=0; i<Button10_list.Count; i++){
				Transform button = Button10_list[i];
				button.eulerAngles =  new Vector3(0f, 80f, 0f);
				if(i>0)
					button.parent = Button10_list[0];
				Button10_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button10_list[0], "button10");
		}
		Button10_list.Clear();
		
		
		if(Button20_list.Count>0){
			for (int i=0; i<Button20_list.Count; i++){
				Transform button = Button20_list[i];
				button.eulerAngles =  new Vector3(0f, 80f, 0f);
				if(i>0)
					button.parent = Button20_list[0];
				Button20_Walletlist.Add(button);
			}
			moveButtonBackToWallet(Button20_list[0], "button20");
		}
		Button20_list.Clear();*/
	}

	IEnumerator CloseWalletAfterChange()
	{
		yield return new WaitForSeconds(3f);
		Messenger.Broadcast("Close wallet");
	}

	void PutBackMoney(List<ButtonInfo> from, List<ButtonInfo> to, string buttontype, Vector3 angle)
	{
		if(from.Count>0){
			for (int i=0; i<from.Count; i++){
				Transform button = from[i].myTransform;
				button.eulerAngles =  angle;
				if(i>0)
					button.parent = from[0].myTransform;

				to.Add(from[i]);

			}
			while(from.Count > 0)
				moveButtonBackToWallet(from[0], buttontype);
		}
		from.Clear();
	}

	void SetMenuItem(int direction)
	{
		int newCurrent = currentButton + direction;
		if(newCurrent >= Money.Count)
			newCurrent -= Money.Count;
		else if (newCurrent < 0)
			newCurrent += Money.Count;

		if(currentButton == 0)
			setButtonUnHighlight(Button20_Walletlist);
		else if (currentButton == 1)
			setButtonUnHighlight(Button10_Walletlist);
		else if (currentButton == 2)
			setButtonUnHighlight(Button5_Walletlist);
		else if (currentButton == 3)
			setButtonUnHighlight(Button1_Walletlist);
		else if (currentButton == 4)
			setButtonUnHighlight(Button1quarter_Walletlist);
		else if (currentButton == 5)
			setButtonUnHighlight(Button1nickel_Walletlist);
		else if (currentButton == 6)
			setButtonUnHighlight(Button1dime_Walletlist);
		else if (currentButton == 7)
			setButtonUnHighlight(Button1cent_Walletlist);
		else if (currentButton == 8)
			setButtonUnHighlight(Button20_list);
		else if (currentButton == 9)
			setButtonUnHighlight(Button10_list);
		else if (currentButton == 10)
			setButtonUnHighlight(Button5_list);
		else if (currentButton == 11)
			setButtonUnHighlight(Button1_list);
		else if (currentButton == 12)
			setButtonUnHighlight(Button1quarter_list);
		else if (currentButton == 13)
			setButtonUnHighlight(Button1nickel_list);
		else if (currentButton == 14)
			setButtonUnHighlight(Button1dime_list);
		else if (currentButton == 15)
			setButtonUnHighlight(Button1cent_list);

		while(Money[newCurrent] == 0)
		{
			if(direction < 0)
				newCurrent--;
			if(direction > 0)
				newCurrent++;

			if(newCurrent < 0)
				newCurrent += Money.Count;
			else if(newCurrent >= Money.Count)
				newCurrent -= Money.Count;
		}

		currentButton = newCurrent;

		if(currentButton == 0)
			setButtonHighlight(Button20_Walletlist);
		else if (currentButton == 1)
			setButtonHighlight(Button10_Walletlist);
		else if (currentButton == 2)
			setButtonHighlight(Button5_Walletlist);
		else if (currentButton == 3)
			setButtonHighlight(Button1_Walletlist);
		else if (currentButton == 4)
			setButtonHighlight(Button1quarter_Walletlist);
		else if (currentButton == 5)
			setButtonHighlight(Button1nickel_Walletlist);
		else if (currentButton == 6)
			setButtonHighlight(Button1dime_Walletlist);
		else if (currentButton == 7)
			setButtonHighlight(Button1cent_Walletlist);
		else if (currentButton == 8)
			setButtonHighlight(Button20_list);
		else if (currentButton == 9)
			setButtonHighlight(Button10_list);
		else if (currentButton == 10)
			setButtonHighlight(Button5_list);
		else if (currentButton == 11)
			setButtonHighlight(Button1_list);
		else if (currentButton == 12)
			setButtonHighlight(Button1quarter_list);
		else if (currentButton == 13)
			setButtonHighlight(Button1nickel_list);
		else if (currentButton == 14)
			setButtonHighlight(Button1dime_list);
		else if (currentButton == 15)
			setButtonHighlight(Button1cent_list);
	}

	public void ResetHighlight()
	{
		currentButton = 0;
		while(Money[currentButton] == 0)
			currentButton++;

		if(currentButton == 0)
			setButtonHighlight(Button20_Walletlist);
		else if (currentButton == 1)
			setButtonHighlight(Button10_Walletlist);
		else if (currentButton == 2)
			setButtonHighlight(Button5_Walletlist);
		else if (currentButton == 3)
			setButtonHighlight(Button1_Walletlist);
		else if (currentButton == 4)
			setButtonHighlight(Button1quarter_Walletlist);
		else if (currentButton == 5)
			setButtonHighlight(Button1nickel_Walletlist);
		else if (currentButton == 6)
			setButtonHighlight(Button1dime_Walletlist);
		else if (currentButton == 7)
			setButtonHighlight(Button1cent_Walletlist);
		else if (currentButton == 8)
			setButtonHighlight(Button20_list);
		else if (currentButton == 9)
			setButtonHighlight(Button10_list);
		else if (currentButton == 10)
			setButtonHighlight(Button5_list);
		else if (currentButton == 11)
			setButtonHighlight(Button1_list);
		else if (currentButton == 12)
			setButtonHighlight(Button1quarter_list);
		else if (currentButton == 13)
			setButtonHighlight(Button1nickel_list);
		else if (currentButton == 14)
			setButtonHighlight(Button1dime_list);
		else if (currentButton == 15)
			setButtonHighlight(Button1cent_list);
	}

	void OnGUI()
	{
		if(walletOpen)
		{
			if (Button20_Walletlist.Count > 0) {
			setButtonHighlight(Button20_Walletlist);
				} else if (Button10_Walletlist.Count > 0) {
			setButtonHighlight(Button10_Walletlist);
				} else if (Button5_Walletlist.Count > 0) {
			setButtonHighlight(Button5_Walletlist);
				} else if (Button1_Walletlist.Count > 0) {
			setButtonHighlight(Button1_Walletlist);
				} else if (Button1quarter_Walletlist.Count > 0) {
			setButtonHighlight(Button1quarter_Walletlist);
				} else if (Button1nickel_Walletlist.Count > 0) {
			setButtonHighlight(Button1nickel_Walletlist);
				} else if (Button1dime_Walletlist.Count > 0) {
			setButtonHighlight(Button1dime_Walletlist);
				} else if (Button1cent_Walletlist.Count > 0) {
			setButtonHighlight(Button1cent_Walletlist);
				}
			walletOpen = false;
		}
	}

	// Update is called once per frame
	void Update () {

		if (stickDelay && Mathf.Abs (Input.GetAxis ("Horizontal")) < .2f && Mathf.Abs(Input.GetAxis ("Vertical")) < .2f)
						stickDelay = ! stickDelay;

		if (!stickDelay) {
			if(Mathf.Abs (Input.GetAxis ("Horizontal")) >= Mathf.Abs(Input.GetAxis ("Vertical")))
			{
				if(Input.GetAxis("Horizontal") > .2f)
				{
					stickDelay = true;
					SetMenuItem(1);
					//setButtonUnHighlight(Button20_Walletlist);
					//setButtonHighlight(Button1_Walletlist);
				}
				else if(Input.GetAxis("Horizontal") < -.2f)
				{
					stickDelay = true;
					SetMenuItem(-1);
				}
			}
			else if(Mathf.Abs (Input.GetAxis ("Horizontal")) < Mathf.Abs(Input.GetAxis ("Vertical")))
			{
				if(Input.GetAxis("Vertical") > .2f)
				{
					stickDelay = true;
					SetMenuItem(-1);
				}
				else if(Input.GetAxis("Vertical") < -.2f)
				{
					stickDelay = true;
					SetMenuItem(1);
				}
			}
		}

		//if(mHighlighted)
		//	Debug.Log("ID:" + this.GetInstanceID() + " " + this.name);
		if(aButtonDelay && !Input.GetButtonDown("A Button"))
			aButtonDelay = !aButtonDelay;
		
		if(Input.GetButtonDown("A Button") && !aButtonDelay)
		{
			if(currentButton >= 0 && currentButton <= 7)
			{
				if(currentButton == 0)
				{
					setButtonUnHighlight(Button20_Walletlist);
					moveButtonToPay(Button20_Walletlist[0], Button20_Walletlist[0].buttontype);
				}
				else if(currentButton == 1)
				{
					setButtonUnHighlight(Button10_Walletlist);
					moveButtonToPay(Button10_Walletlist[0], Button10_Walletlist[0].buttontype);
				}
				else if(currentButton == 2)
				{
					setButtonUnHighlight(Button5_Walletlist);
					moveButtonToPay(Button5_Walletlist[0], Button5_Walletlist[0].buttontype);
				}
				else if(currentButton == 3)
				{
					setButtonUnHighlight(Button1_Walletlist);
					moveButtonToPay(Button1_Walletlist[0], Button1_Walletlist[0].buttontype);
				}
				else if(currentButton == 4)
				{
					setButtonUnHighlight(Button1quarter_Walletlist);
					moveButtonToPay(Button1quarter_Walletlist[0], Button1quarter_Walletlist[0].buttontype);
				}
				else if(currentButton == 5)
				{
					setButtonUnHighlight(Button1nickel_Walletlist);
					moveButtonToPay(Button1nickel_Walletlist[0], Button1nickel_Walletlist[0].buttontype);
				}
				else if(currentButton == 6)
				{
					setButtonUnHighlight(Button1dime_Walletlist);
					moveButtonToPay(Button1dime_Walletlist[0], Button1dime_Walletlist[0].buttontype);
				}
				else if(currentButton == 7)
				{
					setButtonUnHighlight(Button1cent_Walletlist);
					moveButtonToPay(Button1cent_Walletlist[0], Button1cent_Walletlist[0].buttontype);
				}
			}
			else
			{
				if(currentButton == 8)
				{
					setButtonUnHighlight(Button20_list);
					moveButtonBackToWallet(Button20_list[0], Button20_list[0].buttontype);
				}
				else if(currentButton == 9)
				{
					setButtonUnHighlight(Button10_list);
					moveButtonBackToWallet(Button10_list[0], Button10_list[0].buttontype);
				}
				else if(currentButton == 10)
				{
					setButtonUnHighlight(Button5_list);
					moveButtonBackToWallet(Button5_list[0], Button5_list[0].buttontype);
				}
				else if(currentButton == 11)
				{
					setButtonUnHighlight(Button1_list);
					moveButtonBackToWallet(Button1_list[0], Button1_list[0].buttontype);
				}
				else if(currentButton == 12)
				{
					setButtonUnHighlight(Button1quarter_list);
					moveButtonBackToWallet(Button1quarter_list[0], Button1quarter_list[0].buttontype);
				}
				else if(currentButton == 13)
				{
					setButtonUnHighlight(Button1nickel_list);
					moveButtonBackToWallet(Button1nickel_list[0], Button1nickel_list[0].buttontype);
				}
				else if(currentButton == 14)
				{
					setButtonUnHighlight(Button1dime_list);
					moveButtonBackToWallet(Button1dime_list[0], Button1dime_list[0].buttontype);
				}
				else if(currentButton == 15)
				{
					setButtonUnHighlight(Button1cent_list);
					moveButtonBackToWallet(Button1cent_list[0], Button1cent_list[0].buttontype);
				}
			}
			ResetHighlight();
			aButtonDelay = true;
			/*if (enabled && !isPayingOrNot && mHighlighted)
			{
				aButtonDelay = true;
				UI.depth = 20;
				myTransform.eulerAngles = new Vector3(0f,90f,0f);
				SetUnSelected();
				isPayingOrNot = true;
				Wallet.moveButtonToPay(myTransform, buttontype.ToString());
				Wallet.ResetHighlight();
				
			}
			else if (enabled && isPayingOrNot && mHighlighted){
				aButtonDelay = true;
				SetUnSelected();
				Messenger<Transform, string>.Broadcast("moveButtonBackToWallet", myTransform, buttontype.ToString());
				isPayingOrNot = false;
				myTransform.eulerAngles = new Vector3(0f,startYRotation,0f);
				UI.depth = startDepth;
			}*/
		}
	}
}
