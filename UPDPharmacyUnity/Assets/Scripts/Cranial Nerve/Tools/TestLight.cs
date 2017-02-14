using UnityEngine;
using System.Collections.Generic;

public class TestLight : RenBehaviour {

#if UNITY_EDITOR
    public RenButton Test;
    
	// Use this for initialization
	protected override void Start () {
        base.Start();
        AddGUIElement(Test);
        Test.ButtonPressed += TestLight_ButtonPressed;

	}

    void TestLight_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        IsInSpotlight[] spotLights = gameObject.GetComponents<IsInSpotlight>();
        AddDebugLine("Has: " + spotLights);

        foreach (IsInSpotlight spot in spotLights)
        {
            AddDebugLine("" + spot.NerveLocalPosition());
        }
    }
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
#endif
}
