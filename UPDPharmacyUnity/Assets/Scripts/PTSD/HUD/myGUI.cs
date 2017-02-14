using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class myGUI : MonoBehaviour {
	public float cartItemsWindowHeight = 180.0f;
	
	public float dollarWidth = 155.956f;
	public float dollarHeight =	66.294f;
	private float centSize = 19.05f*2.5f;
	private float nickelSize = 21.21f*2.5f;
	private float dimeSize = 17.91f*2.5f;
	private float quarterSize = 24.26f*2.5f;
	
	public float buttonWidth = 40.0f;
	public float buttonHeight = 40.0f;
	
	public float itemButtonWidth = 80.0f;
	public float itemButtonHeight = 80.0f;
	
	public float closeButtonWidth = 20.0f;
	public float closeButtonHeight = 20.0f;
	
	public int numberOfOneDollar = 0;
	public int numberOfFiveDollar = 0;
	public int numberOfTenDollar = 0;
	public int numberOfTwentyDollar = 0;
	public int numberOfOneDollarToPay = 0;
	public int numberOfFiveDollarToPay = 0;
	public int numberOfTenDollarToPay = 0;
	public int numberOfTwentyDollarToPay = 0;
	public int numberOfOneCent = 0;
	public int numberOfOneNickel = 0;
	public int numberOfOneDime = 0;
	public int numberOfOneQuarter = 0;
	public int numberOfOneCentToPay = 0;
	public int numberOfOneNickelToPay = 0;
	public int numberOfOneDimeToPay = 0;
	public int numberOfOneQuarterToPay = 0;
	
	
	public float totalInPurse = 0;
	public static float totalAtHand = 0;
	public static float totalAfterPaid = 0;
	public static float totalNeedToPay = 0;
	
	public Texture2D oneDollarIcon;
	public Texture2D fiveDollarIcon;
	public Texture2D tenDollarIcon;
	public Texture2D twentyDollarIcon;
	public Texture2D oneCentIcon;
	public Texture2D oneNickelIcon;
	public Texture2D oneDimeIcon;
	public Texture2D oneQuarterIcon;
	
	public GUIStyle myStyle;
	
	public static bool paidOrNot = false;
	private bool _animate = false;
	
	private float _offset = 10.0f;
	
	private float offset = 10.0f;
	
	private List<Item> _cartItems;
	private bool _displayCartItemsWindow = false;
	private const int CARTITEMS_WINDOW_ID = 0;
	private Rect _cartItemsWindowRect = new Rect(0,0,0,0);
	private Vector2 _cartItemWindowSlider = Vector2.zero;
	
	private bool _displayCheckoutWindow = false;
	private const int CHECKOUT_WINDOW_ID = 10;
	private Rect _checkoutWindowRect = new Rect(0,0,0,0);
	private int _checkoutCol = 2;
	private int _checkoutRow = 2;
	
	void OnEnable()
    {
        Messenger<float>.AddListener("Display wallet", OnDisplayModeChanged);
	}
	 void OnDisable()
    {
         Messenger<float>.RemoveListener("Display wallet", OnDisplayModeChanged);
	
    
    }
	
	private void OnDisplayModeChanged(float totalPrice){
		totalNeedToPay = Mathf.Round(totalPrice*100f)/100f;
		_displayCheckoutWindow = true;
	}
	
	// Use this for initialization
	void Start () {
		_cartItems = new List<Item>();
		generateItems();
		buttonWidth = 0.9f * dollarWidth;
		buttonHeight = 0.9f * dollarHeight;
		_checkoutWindowRect = new Rect(5f,Screen.height/2,Screen.width-10f,Screen.height/2-5f);
	
		numberOfOneDollar = Random.Range(1,3);
		numberOfFiveDollar = Random.Range(1,3);
		numberOfTenDollar = Random.Range(1,3);
		numberOfTwentyDollar = Random.Range(1,3);
		numberOfOneCent = Random.Range(1,3);
		numberOfOneNickel = Random.Range(1,3);
		numberOfOneDime = Random.Range(1,3);
		numberOfOneQuarter = Random.Range(1,3);
		
		totalInPurse = 20*numberOfTwentyDollar + 10*numberOfTenDollar + 5*numberOfFiveDollar + numberOfOneDollar + 0.25f*numberOfOneQuarter + 0.1f*numberOfOneDime + 0.05f*numberOfOneNickel + 0.01f*numberOfOneCent;
	}
	
	// Update is called once per frame
	void Update () {

	
	}
	
	
	void OnGUI(){
		//if(_displayCheckoutWindow){
		//	_checkoutWindowRect = GUI.Window(CHECKOUT_WINDOW_ID, _checkoutWindowRect, checkoutWindow, "Wallet");
		//}
		
		
		if(_displayCartItemsWindow){
			_cartItemsWindowRect = GUI.Window(CARTITEMS_WINDOW_ID, new Rect(_offset, 10f , Screen.width - 2*_offset, cartItemsWindowHeight ), cartItemsWindow, "Items in cart");
		}
		
		//Rect rctWindow4 = new Rect(43f + (offset/2f), 20f + (offset/2f), 2f + (offset * 1.5f), 6f + offset);

 		//GUI.Button(rctWindow4, "test");

		//offset = Mathf.Lerp(0f + offset, 320f, Time.deltaTime * .05f);
	}
	
	private void checkoutWindow(int id){
		
		if(GUI.Button(new Rect(_checkoutWindowRect.width-20, 0, closeButtonWidth, closeButtonHeight), "X")){
			_displayCheckoutWindow = false;
		}
		
		totalInPurse = 20*numberOfTwentyDollar + 10*numberOfTenDollar + 5*numberOfFiveDollar + numberOfOneDollar + 0.25f*numberOfOneQuarter + 0.1f*numberOfOneDime + 0.05f*numberOfOneNickel + 0.01f*numberOfOneCent;
	
		totalAtHand = 20*numberOfTwentyDollarToPay + 10*numberOfTenDollarToPay + 5*numberOfFiveDollarToPay + numberOfOneDollarToPay + 0.25f*numberOfOneQuarterToPay + 0.1f*numberOfOneDimeToPay + 0.05f*numberOfOneNickelToPay + 0.01f*numberOfOneCentToPay;
	
			
		GUI.Box(new Rect(5,45,_checkoutWindowRect.width/2-7.5f,_checkoutWindowRect.height-50),"");
		
		GUI.Box(new Rect(2.5f+_checkoutWindowRect.width/2,45,_checkoutWindowRect.width/2-7.5f,_checkoutWindowRect.height-50),"");
		
		GUI.Label(new Rect(5,10,_checkoutWindowRect.width,40), "Money in purse: "+totalInPurse.ToString() + " dollars",myStyle);
		
		
		
		if ( (totalAtHand - totalNeedToPay) >= 0 && !paidOrNot && GUI.Button (new Rect(_checkoutWindowRect.width - 85, 25, 80, 40), "Pay")){
			paidOrNot = true;
			totalAfterPaid = totalAtHand - totalNeedToPay;
			totalAfterPaid = Mathf.Round(totalAfterPaid*100)/100;
			float decimalAfterPaid = totalAfterPaid - (int)(totalAfterPaid);
			decimalAfterPaid = Mathf.Round(decimalAfterPaid*100)/100;
			numberOfTwentyDollarToPay = (int) (totalAfterPaid * 0.05f );
			numberOfTenDollarToPay = (int) ((totalAfterPaid - numberOfTwentyDollarToPay*20f )*0.1f ) ;
			numberOfFiveDollarToPay =(int)  ((totalAfterPaid - numberOfTwentyDollarToPay*20f - numberOfTenDollarToPay*10f)*0.2f );
			numberOfOneDollarToPay = (int)  ((totalAfterPaid - numberOfTwentyDollarToPay*20f - numberOfTenDollarToPay*10f - numberOfFiveDollarToPay*5f)*1f );
			
			Debug.Log(numberOfTwentyDollarToPay+"twenty"+numberOfTenDollarToPay+"ten"+ numberOfFiveDollarToPay+"five"+ numberOfOneDollarToPay+"one");
			
			numberOfOneQuarterToPay = (int)  (decimalAfterPaid*4f) ;
			numberOfOneDimeToPay = (int)  ((decimalAfterPaid - numberOfOneQuarterToPay*0.25f)*10f) ;
			numberOfOneNickelToPay = (int) ((decimalAfterPaid - numberOfOneQuarterToPay*0.25f - numberOfOneDimeToPay*0.1f)*20f);
			numberOfOneCentToPay = (int)  Mathf.Round( ((decimalAfterPaid - numberOfOneQuarterToPay*0.25f - numberOfOneDimeToPay*0.1f-numberOfOneNickelToPay*0.05f)*100f)*100)/100;
			
			Debug.Log(totalAfterPaid + "totalAfterPaid"+decimalAfterPaid+"decimal"+numberOfOneQuarterToPay+numberOfOneDimeToPay+numberOfOneNickelToPay+numberOfOneCentToPay+((decimalAfterPaid - numberOfOneQuarterToPay*0.25 - numberOfOneDimeToPay*0.1-numberOfOneNickelToPay*0.05)*100));
		
		}
		
		if (paidOrNot&& GUI.Button (new Rect(_checkoutWindowRect.width - 85, 25, 80, 40), "Done")){
			numberOfTwentyDollar += numberOfTwentyDollarToPay;
			numberOfTenDollar += numberOfTenDollarToPay;
			numberOfFiveDollar += numberOfFiveDollarToPay;
			numberOfOneDollar += numberOfOneDollarToPay;
			numberOfOneQuarter += numberOfOneQuarterToPay;
			numberOfOneDime += numberOfOneDimeToPay;
			numberOfOneNickel += numberOfOneNickelToPay;
			numberOfOneCent += numberOfOneCentToPay;
			
			numberOfTwentyDollarToPay = 0;
			numberOfTenDollarToPay = 0; 
			numberOfFiveDollarToPay = 0;
			numberOfOneDollarToPay = 0;
			numberOfOneQuarterToPay = 0;
			numberOfOneDimeToPay = 0;
			numberOfOneNickelToPay = 0;
			numberOfOneCentToPay = 0;
			
			//_displayCheckoutWindow = false;
		}
		
		
		if (!paidOrNot){
			GUI.Label(new Rect(5+_checkoutWindowRect.width/2,10,_checkoutWindowRect.width,40), "Money at hand: "+totalAtHand.ToString() + " dollars",myStyle);
		}
		else {
			GUI.Label(new Rect(5+_checkoutWindowRect.width/2,10,_checkoutWindowRect.width,40), "Money returned: "+totalAfterPaid.ToString() + " dollars",myStyle);
		}
		
		// twenty dollar
		for (int count = 0; count < numberOfTwentyDollar; count++ ){

			if(GUIButton.Button(new Rect(15+10*count+offset, 60+10*count, buttonWidth, buttonHeight),twentyDollarIcon, myStyle)){
			
					numberOfTwentyDollar--;
					numberOfTwentyDollarToPay++;
			}
		}
		
		
		
		for (int count = 0; count < numberOfTwentyDollarToPay; count++ ){
			if(GUI.Button(new Rect(20+10*count+_checkoutWindowRect.width/2, 60+10*count, buttonWidth, buttonHeight),twentyDollarIcon, myStyle)){
				numberOfTwentyDollar++;
				numberOfTwentyDollarToPay--;
			}
		}
		
		// ten dollar
		for (int count = 0; count < numberOfTenDollar; count++ ){
			//if(GUI.Button(new Rect(40+buttonWidth+10*count, 80+10*count, buttonWidth, buttonHeight), tenDollarIcon, myStyle)){
			if(GUIButton.Button(new Rect(50+buttonWidth+10*count, 60+10*count, buttonWidth, buttonHeight), tenDollarIcon, myStyle)){
			
				Event.current.Use();
				numberOfTenDollar--;
				numberOfTenDollarToPay++;
				GUI.depth = 0;
			}
		}
		
		for (int count = 0; count < numberOfTenDollarToPay; count++ ){
			if(GUI.Button(new Rect(50+buttonWidth+10*count+_checkoutWindowRect.width/2, 60+10*count, buttonWidth, buttonHeight), tenDollarIcon, myStyle)){
				numberOfTenDollar++;
				numberOfTenDollarToPay--;
			}
		}
		
		// five dollar
		for (int count = 0; count < numberOfFiveDollar; count++ ){
			if(GUI.Button(new Rect(25+10*count, 80+buttonHeight+10*count, buttonWidth, buttonHeight), fiveDollarIcon, myStyle)){
				numberOfFiveDollar--;
				numberOfFiveDollarToPay++;
			}
		}
		
		for (int count = 0; count < numberOfFiveDollarToPay; count++ ){
			if(GUI.Button(new Rect(20+10*count+_checkoutWindowRect.width/2, 80+buttonHeight+10*count, buttonWidth, buttonHeight), fiveDollarIcon, myStyle)){
				numberOfFiveDollar++;
				numberOfFiveDollarToPay--;
			}
		}
		
		// one dollar
		for (int count = 0; count < numberOfOneDollar; count++ ){
			if(GUI.Button(new Rect(50+buttonWidth+10*count, 80+buttonHeight+10*count, buttonWidth, buttonHeight), oneDollarIcon, myStyle)){
				numberOfOneDollar--;
				numberOfOneDollarToPay++;
			}
		}
		
		for (int count = 0; count < numberOfOneDollarToPay; count++ ){
			if(GUI.Button(new Rect(50+buttonWidth+10*count+_checkoutWindowRect.width/2, 80+buttonHeight+10*count, buttonWidth, buttonHeight), oneDollarIcon, myStyle)){
				numberOfOneDollar++;
				numberOfOneDollarToPay--;
			}
		}
		
		
		//One quarter
		for (int count = 0; count < numberOfOneQuarter; count++ ){
			if(GUIButton.Button(new Rect(25+10*count, 100+buttonHeight*2+10*count, quarterSize, quarterSize),oneQuarterIcon, myStyle)){
			
					numberOfOneQuarter--;
					numberOfOneQuarterToPay++;
			}
		}
		
		for (int count = 0; count < numberOfOneQuarterToPay; count++ ){
			if(GUI.Button(new Rect(20+10*count+_checkoutWindowRect.width/2, 100+buttonHeight*2+10*count, quarterSize, quarterSize), oneQuarterIcon, myStyle)){
				numberOfOneQuarter++;
					numberOfOneQuarterToPay--;
			}
		}
		
		//One dime
		for (int count = 0; count < numberOfOneDime; count++ ){
			if(GUIButton.Button(new Rect(125+10*count, 100+buttonHeight*2+10*count, dimeSize, dimeSize),oneDimeIcon, myStyle)){
			
					numberOfOneDime--;
					numberOfOneDimeToPay++;
			}
		}
		
		for (int count = 0; count < numberOfOneDimeToPay; count++ ){
			if(GUI.Button(new Rect(120+10*count+_checkoutWindowRect.width/2, 100+buttonHeight*2+10*count, dimeSize, dimeSize), oneDimeIcon, myStyle)){
				numberOfOneDime++;
					numberOfOneDimeToPay--;
			}
		}
		
		//One nickel
		for (int count = 0; count < numberOfOneNickel; count++ ){
			if(GUIButton.Button(new Rect(225+10*count, 100+buttonHeight*2+10*count, nickelSize, nickelSize),oneNickelIcon, myStyle)){
			
					numberOfOneNickel--;
					numberOfOneNickelToPay++;
			}
		}
		
		for (int count = 0; count < numberOfOneNickelToPay; count++ ){
			if(GUI.Button(new Rect(220+10*count+_checkoutWindowRect.width/2, 100+buttonHeight*2+10*count, nickelSize, nickelSize), oneNickelIcon, myStyle)){
				numberOfOneNickel++;
					numberOfOneNickelToPay--;
			}
		}
		
		//One cent
		for (int count = 0; count < numberOfOneCent; count++ ){
			if(GUIButton.Button(new Rect(325+10*count, 100+buttonHeight*2+10*count, centSize, centSize),oneCentIcon, myStyle)){
			
					numberOfOneCent--;
					numberOfOneCentToPay++;
			}
		}
		
		for (int count = 0; count < numberOfOneCentToPay; count++ ){
			if(GUI.Button(new Rect(320+10*count+_checkoutWindowRect.width/2, 100+buttonHeight*2+10*count, centSize, centSize), oneCentIcon, myStyle)){
				numberOfOneCent++;
					numberOfOneCentToPay--;
			}
		}
		
		
		
		/*for (int y = 0; y < _checkoutRow; y++){
			for (int x = 0; x < _checkoutCol; x++){
				GUI.Button(new Rect(10+x*buttonWidth, 80+y*buttonHeight, buttonWidth, buttonHeight), (x+y*_checkoutCol).ToString());
			}
		}*/
		//GUI.DragWindow();
	}
	
	public void ToggleCheckoutWindow(){
		_displayCheckoutWindow = !_displayCheckoutWindow;
		
	}
	
	private void cartItemsWindow(int id){
		
		if(GUI.Button(new Rect(_cartItemsWindowRect.width-20, 0, closeButtonWidth, closeButtonHeight), "X")){
			_displayCartItemsWindow = false;
		}
		
		GUI.Label(new Rect(5,10,_cartItemsWindowRect.width,70), "Total number of items: "+_cartItems.Count.ToString() + " Total amount: " + totalNeedToPay.ToString() + " dollars",myStyle);
		
		
		_cartItemWindowSlider = GUI.BeginScrollView (new Rect(_offset*0.5f, 90,_cartItemsWindowRect.width-_offset, 90), _cartItemWindowSlider, new Rect(0,0,itemButtonWidth*_cartItems.Count+_offset,itemButtonHeight+_offset));  
			
		for( int count=0; count < _cartItems.Count; count++){
			GUI.Button (new Rect(_offset*0.5f + itemButtonWidth*count, _offset, itemButtonWidth, itemButtonHeight), count.ToString());
		}
		GUI.EndScrollView();
		
	}
	
	private void generateItems(){
		for ( int count=0; count < 30; count++ )
			_cartItems.Add(new Item());
	}
	
	    bool goodButton(Rect bounds, string caption) {

        GUIStyle btnStyle = GUI.skin.FindStyle("button");

        int controlID = GUIUtility.GetControlID(bounds.GetHashCode(), FocusType.Passive);

        

        bool isMouseOver = bounds.Contains(Event.current.mousePosition);

        bool isDown = GUIUtility.hotControl == controlID;

 

        if (GUIUtility.hotControl != 0 && !isDown) {

            // ignore mouse while some other control has it

            // (this is the key bit that GUI.Button appears to be missing)

            isMouseOver = false;

        }

        

        if (Event.current.type == EventType.Repaint) {

            btnStyle.Draw(bounds, new GUIContent(caption), isMouseOver, isDown, false, false);

        }

        switch (Event.current.GetTypeForControl(controlID)) {

            case EventType.mouseDown:

                if (isMouseOver) {  // (note: isMouseOver will be false when another control is hot)

                    GUIUtility.hotControl = controlID;

                }

                break;

                

            case EventType.mouseUp:

                if (GUIUtility.hotControl == controlID) GUIUtility.hotControl = 0;

                if (isMouseOver && bounds.Contains(Event.current.mousePosition)) return true;

                break;

        }

 

        return false;

    }
}
