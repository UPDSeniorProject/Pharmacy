using UnityEngine;
using System.Collections;

public class OxygenMeter : RenBehaviour {
	
	
	public RenButton IncreaseO2 = new RenButton();
	public RenTextBox OxygenFlow = new RenTextBox();
	public float O2Sat = 0.88f ; 
	
	public float TopO2Sat = 0.92f;
	public float O2SatStep = 0.001f;
	public bool IncreasingO2 = false;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		IncreaseO2.Label=" Increase O2 Flow";
		IncreaseO2.ButtonPressed += HandleIncreaseO2ButtonPressed; 
		AddGUIElement(IncreaseO2);
		
		AddGUIElement(OxygenFlow);
	
	}

	void HandleIncreaseO2ButtonPressed (RenButton btn, ButtonPressedEventArgs args)
	{
		IncreasingO2 = true;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		
		///Random.Range(0.0,0.001);
		
		OxygenFlow.text = "SaO2: " + O2Sat.ToString("P");
		if(IncreasingO2 && O2Sat < TopO2Sat) {
			O2Sat += O2SatStep;	
		}
	}
}
