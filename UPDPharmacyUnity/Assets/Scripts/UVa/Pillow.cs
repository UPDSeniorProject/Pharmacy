using UnityEngine;
using System.Collections;

public class Pillow : RenBehaviour {
	
	public Transform PillowPosition;
    //public RenButton TableButton= new RenButton ();
	//private float CounterP = 0.0f; 
	// Use this for initialization
	//=============================================
	//Method TWO: (part a)
	//public BedMove myBedMove = null;
	//===============================================
	
	protected override void Start () 
	{
	    base.Start ();
		//TableButton = new RenButton () ; 
		
		//=========================================================
		//Method TWO: (part b)
		//if(myBedMove != null){
		//	myBedMove.TableButton.ButtonPressed += HandleTableButtonButtonPressed;
		//}
		//==========================================================
		
		//==========================================================
		//METHOD ONE: 
		//BedMove myBedMove = GameObject.Find("Environment").GetComponentInChildren<BedMove>();
		// This previous line finds an instance of the script with name BedMove in all of the children
		// of the GameObject named "Environment"
		
		//myBedMove.TableButton.ButtonPressed += HandleTableButtonButtonPressed;
		// HandletableButtonButtonPressed is being add to the function of myBedmove.TableButton.ButtonPressed
		// So when myBedMove.TableButton.ButtonPressed is called then it will run the additional function as 
		//  as well. 
		//==================================================================
		
		//TableButton.Label="Move Table";
		//TableButton.ButtonPressed += HandleTableButtonButtonPressed; 
		//AddGUIElement(TableButton);
	}

	
	//void HandleTableButtonButtonPressed (RenButton btn, ButtonPressedEventArgs args)
//	{
	//	Debug.Log("I got inside here");	
	//}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update ();
	}
	
}