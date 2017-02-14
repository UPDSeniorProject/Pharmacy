using UnityEngine;
using System.Collections;

public class Dimmer : MonoBehaviour {
    public GameObject spotLight;
	public Texture2D bulb;
	float hSbarValue=0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
	    GUI.skin.box.fontStyle=FontStyle.Bold;
		GUI.Box (new Rect (Screen.width - 45,0,40,120), bulb);
		hSbarValue = GUI.VerticalScrollbar (new Rect (Screen.width - 33, 25, 100, 80), hSbarValue, .01f, 0.5f, 0);
    	spotLight.GetComponent<Light>().intensity= hSbarValue*2.92f;
		this.GetComponent<Light>().intensity = hSbarValue;
	}
}
