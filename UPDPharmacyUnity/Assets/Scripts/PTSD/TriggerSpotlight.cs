using UnityEngine;
using System.Collections;

public class TriggerSpotlight : MonoBehaviour {

	private bool triggered = false;
	private float t0;
	private bool on = false;

	void OnEnable()
	{
		Messenger.AddListener("turn light on", turnLightOn);
		Messenger.AddListener("turn light off", turnLightOff);
		Messenger.AddListener("turn light bright", turnLightBright);
		Messenger.AddListener("flicker light", flickerLight);
	}
	void OnDisable()
	{
		Messenger.RemoveListener("turn light on", turnLightOn);
		Messenger.RemoveListener("turn light off", turnLightOff);
		Messenger.RemoveListener("turn light bright", turnLightBright);
		Messenger.RemoveListener("flicker light", flickerLight);
	}

	void turnLightOn(){
		on = true;
		gameObject.GetComponent<Light>().intensity = 0.5f;
	}

	void turnLightOff(){
		on = false;
		gameObject.GetComponent<Light>().intensity = 0;
	}

	void turnLightBright(){
		on = true;
		gameObject.GetComponent<Light>().intensity = 5;
	}

	void flickerLight(){
		StartCoroutine (flickerLightStart());
	}

	IEnumerator flickerLightStart(){

		for(int i = 0; i < 20; i++)
		{
			if(on)
				turnLightOff();
			else
				turnLightBright();

			yield return new WaitForSeconds(0);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name.Equals("Player") && !triggered)
		{
			triggered = true;
			t0 = Time.time;
		}
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))
			flickerLight ();
		/*
		if(triggered)
		{
			if(gameObject.GetComponent<Light>().intensity == 0 && (int)(Time.time - t0) < 4)
				gameObject.GetComponent<Light>().intensity = 5;
			else
				gameObject.GetComponent<Light>().intensity = 0;
		}

		/*
		if(on)
		{
			triggered = false;
			if(gameObject.GetComponent<Light>().intensity == 0)
				gameObject.GetComponent<Light>().intensity = 5;
			else
				gameObject.GetComponent<Light>().intensity = 0;
		}
		else
			gameObject.GetComponent<Light>().intensity = 0;
		*/

	}
}
