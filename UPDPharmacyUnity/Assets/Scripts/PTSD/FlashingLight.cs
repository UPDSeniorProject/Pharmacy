using UnityEngine;
using System.Collections;

public class FlashingLight : MonoBehaviour {

	private Transform myTransform;

	private static bool flashingLightsEnabled = false;

	public Transform movingObject;
	public Light Light;
	public Transform lightMesh;
	public Material DarkLightMaterial;
	public Material OriginalMaterial;

	void OnEnable()
	{
		Messenger.AddListener("enable flashing lights", enableFlashingLights);
	}
	void OnDisable()
	{
		Messenger.RemoveListener("enable flashing lights", enableFlashingLights);
	}
	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//Debug.Log ("Distance to flashing lights " + Vector3.Distance (movingObject.position, myTransform.position));
		if (flashingLightsEnabled){
			if (Vector3.Distance (movingObject.position, myTransform.position) <= 13f) {

				float RandomNumber = Random.value;

				if (RandomNumber < 0.4) {
					Light.enabled = true;
					lightMesh.GetComponent<MeshRenderer> ().material = OriginalMaterial;
				} else {
					Light.enabled = false;
					lightMesh.GetComponent<MeshRenderer> ().material = DarkLightMaterial;
				}
			} else {
				Light.enabled = false;
				lightMesh.GetComponent<MeshRenderer> ().material = OriginalMaterial;
			}
		}
	}

	public void enableFlashingLights(){
		flashingLightsEnabled = true;
	}
}
