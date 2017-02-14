using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AnimateMegan : MonoBehaviour {
	public Animation animation;

	Dictionary<string, string> keyMap = 
		new Dictionary<string, string>() {

			{"a", "idle_answerPhone"},
			{"s", "idle_comparePhone-Item"},
			{"d", "phone_idle_stand_idle"},
			{"f", "phone_idle_getPhone"},
			{"g", "phone_idle_hidePhone"},
			{"h", "phone_walk"}

	};





	// Use this for initialization
	void Start () {
		animation = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
		// go through possible animations. if any are called,
		// play that animation, and then return.

		foreach (KeyValuePair<string, string> pair in keyMap)
		{

			if (Input.GetKeyDown(pair.Key))
			{
				animation.Play(pair.Value);
				break; 		// so multiple are never played
			}

		}
		
	}
}
