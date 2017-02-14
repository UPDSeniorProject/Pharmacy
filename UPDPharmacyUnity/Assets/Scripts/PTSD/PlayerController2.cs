using UnityEngine;
using System.Collections;

public class PlayerController2 : MonoBehaviour {
	public float speed;
	public float rotationSpeed;
	private float camRotationSpeed;
	private float camAngle;
	private GameObject cart;
	private bool cartVisible;
	
	void Start() {
		cart = GameObject.Find ("Player/shoppingCart");
		cartVisible = true;
	}
	
	void OnCollisionEnter (Collision col)
	{
		cart.SetActive (false);
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}
	
	void OnTriggerExit (Collider col)
	{
		cart.SetActive (true);
		gameObject.GetComponent<BoxCollider>().isTrigger = false;
	}
	
	// Update is called once per frame
	void Update () {
		camAngle = Camera.main.GetComponent<Transform>().localEulerAngles.y;
		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		float camRot = Input.GetAxis("Horizontal2") * rotationSpeed * Time.deltaTime;
		
		if(camAngle > 70 && camAngle < 80)
		{
			transform.Rotate(0, camRot, 0);
			if(camRot > 0)
				Camera.main.GetComponent<Transform>().Rotate (0, -1 * camRot, 0, Space.World);
		}
		else if(camAngle > 280 && camAngle < 290)
		{
			transform.Rotate(0, camRot, 0);
			if(camRot < 0)
				Camera.main.GetComponent<Transform>().Rotate (0, -1 * camRot, 0, Space.World);
		}
		if(camAngle > 2 && camAngle < 358 && translation != 0)
		{
			if(Mathf.Abs(rotation) < 50)
			{
				if(camAngle < 90)
				{
					rotation = 50;
					camRotationSpeed = -1 * camAngle;
				}
				else if(camAngle > 270)
				{
					rotation = -50;
					camRotationSpeed = 360 - camAngle;
				}
			}
			
			if(Mathf.Abs(camRotationSpeed) < 50)
				if(camRotationSpeed < 0)
					camRotationSpeed = -50;
			else
				camRotationSpeed = 50;
			
			Camera.main.GetComponent<Transform>().Rotate (0, camRotationSpeed * Time.deltaTime, 0, Space.World);
			
		}
		
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		transform.Translate(0, 0, translation);
		transform.Rotate (0, rotation, 0);
	}
}
