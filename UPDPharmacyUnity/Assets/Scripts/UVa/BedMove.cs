using UnityEngine;
using System.Collections;

public class BedMove : RenBehaviour {
	
	public Transform BedPosition;
    public RenButton TableButton= new RenButton ();
	private float CounterT = 0.0f; 
	
	public VHAnimationManager AnimationManager;
	
	// Use this for initialization
	protected override void Start () 
	{
	    base.Start ();
		//TableButton = new RenButton () ; 
		TableButton.Label="Move Table";
		TableButton.ButtonPressed += HandleTableButtonButtonPressed; 
		AddGUIElement(TableButton);
	}

	
	void HandleTableButtonButtonPressed (RenButton btn, ButtonPressedEventArgs args)
	{
		//AddDebugLine("HI");
		if ( CounterT < 1.0f)
		{
			
			transform.Rotate( 0.0f, 0.0f , -50.0f);
			CounterT = CounterT + 1;
			if(AnimationManager != null) {
				AnimationManager.PlayAnimation("SittingUp" );
				AnimationManager.gameObject.transform.Rotate(29.5f, 62.11f, -19.70f)	;
				AnimationManager.gameObject.transform.position = new Vector3(29.5f,32.0f,62.7f);
				//AnimationManager.gameObject.transform.Translate(-29.50f,-32.11f,62.70f);
			}
			
		}
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
	}
}
