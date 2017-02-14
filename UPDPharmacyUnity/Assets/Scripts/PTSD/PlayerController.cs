using UnityEngine;
using System.Collections;
//using XInputDotNetPure;

public class PlayerController : MonoBehaviour {
	public float speed;
	public float rotationSpeed;
    public Transform ShoppingCart;
    public float vibIntensity;
    private float startVib = 1.0f;

    private float camRotationSpeed;
	private float camAngle;
	private GameObject cart;
	private bool cartVisible;
    private bool shoppingCartCollided = false;
	
	void Start() {
		cart = GameObject.Find ("Player/shoppingCart");
		cartVisible = true;
	}
    
	void OnEnable()
	{
		Messenger.AddListener("play thunder storm", playThunderStorm);
	}
	void OnDisable()
	{
		Messenger.RemoveListener("play thunder storm", playThunderStorm);
	}

	void playThunderStorm(){
		gameObject.GetComponent<AudioSource>().Play();
	}

    void OnCollisionStay(Collision coll)
    {
        startVib = 0;
    }

    /*
    void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.name);
        Debug.Log("Colliding now");
        if (!shoppingCartCollided && (coll.name.Contains("Collision") || coll.name.Contains("Plane")))
        {
            iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "time", 2f, "islocal", true, "easetype", iTween.EaseType.easeInOutSine));
            iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("position", new Vector3(1f, 0f, -1f), "time", 2f,
                                                               "islocal", true, "orienttopath", false, "easetype", iTween.EaseType.linear));

            // disable all the colliders for the shopping cart
            //front_collider.getcomponent<collider>().enabled = false;
            //back_collider.getcomponent<collider>().enabled = false;
            //left_collider.getcomponent<collider>().enabled = false;
            //right_collider.getcomponent<collider>().enabled = false;
            //bottom_collider.getcomponent<collider>().enabled = false;
            //StartCoroutine("setShoppingCartCollided");
            shoppingCartCollided = true;
        }
    }
    

    void OnTriggerExit(Collider coll)
    {
        Debug.Log("Not colliding");

        if (shoppingCartCollided)
        {
            Debug.Log("Getting out of collision");
            //ShoppingCart.localPosition = new Vector3(0f, 0f, 0f);
            //ShoppingCart.localRotation = Quaternion.identity;
            iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "time", 2f, "islocal", true, "easetype", iTween.EaseType.easeInOutSine));
            iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("position", new Vector3(0f, 0f, 0f), "time", 2f,
                                                               "islocal", true, "orienttopath", false, "easetype", iTween.EaseType.linear));

            shoppingCartCollided = false;
            StartCoroutine("setShoppingCartUnCollided");
            //helpSprite.SetActive(true);
        }
    }
    */


    // Update is called once per frame
    void Update () {
        startVib += Time.deltaTime;
        /*if(vibIntensity-startVib < 0)
            GamePad.SetVibration(0, 0, 0);
        else
            GamePad.SetVibration(0, vibIntensity-startVib, vibIntensity-startVib);
            */
        if (Input.GetKeyDown(KeyCode.Alpha2))
			playThunderStorm();

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			GameObject parent = GameObject.Find ("InStoreLights/RealTime");
			Transform[] trans = parent.GetComponentsInChildren<Transform>();
			
			foreach(Transform t in trans)
				Debug.Log (t.name);
		}

		camAngle = Camera.main.GetComponent<Transform>().localEulerAngles.y;
		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		float camRot = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
		
		if(Input.GetKeyDown(KeyCode.Joystick1Button3))
		{
			cart.SetActive(!cartVisible);
			cartVisible = !cartVisible;
			
			gameObject.GetComponent<BoxCollider>().enabled = cartVisible;
		}
		
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
