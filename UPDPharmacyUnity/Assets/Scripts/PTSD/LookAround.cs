using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add a rigid body to the capsule
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSWalker script to the capsule

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
public class LookAround : MonoBehaviour
{
    Transform lookAtPos;			// the position to move the camera to when using head look

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    private float sensitivityX = 1.8F;
    private float sensitivityY = 1.8F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

	public Texture2D pickupIcon;

    static float rotationX = 0F;
    static float rotationY = 0F;

	public float checkoutScreenX = Screen.width/2-40;
	public float checkoutScreenY = Screen.height/2-40;
	private Rect r = new Rect(Screen.width/2-40, Screen.height/2-40, 40, 40);
	public bool isPayingOrNot = false;
	public UIButtonMove uibm;

    static bool lockCusor = false;

    Quaternion originalRotation;
    static Quaternion xQuaternion;
    static Quaternion yQuaternion;

    private float pickObjModeMoveUpDistance = 0f;
    private float pickObjModeMoveRightDistance = 0f;

    public static bool zoomAndMove = false;

    private Transform camera;

    void OnEnable()
    {
        Messenger.AddListener("set camera rotation to zero", setZero);
        Messenger<float>.AddListener("set cameraX max angle", setAngleX);
        Messenger.AddListener("reset rotation Y", resetRotationY);
		Messenger.AddListener("rotate back to zero", RotateBackToCenter);
		Messenger.AddListener("rotate to face aisle", RotateToFaceAisle);
    }
    void OnDisable()
    {
        Messenger.RemoveListener("set camera rotation to zero", setZero);
        Messenger<float>.RemoveListener("set cameraX max angle", setAngleX);
        Messenger.RemoveListener("reset rotation Y", resetRotationY);
		Messenger.RemoveListener("rotate back to zero", RotateBackToCenter);
		Messenger.RemoveListener("rotate to face aisle", RotateToFaceAisle);
    }
    void setAngleX(float angle)
    {
        maximumX = angle;
        minimumX = -angle;
    }

    void setZero()
    {
        xQuaternion = Quaternion.identity;
        yQuaternion = Quaternion.identity;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        rotationX = 0f;
        rotationY = 0f;
        //Debug.Log(transform.eulerAngles);
    }
    void resetRotationY()
    {
        rotationY = 0f;
        yQuaternion = Quaternion.identity;
		Debug.Log ("Reset y rotation");
        transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
		//transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z);
        //transform.localRotation = Quaternion.identity;
    }
    void setRotationZero()
    {
        rotationX = 0f;
        rotationY = 0f;
    }
	void setRotationAisle()
	{
		rotationX = 0f;
		rotationY = 0f;
	}
    void Update()
    {


        // if (!Movement.directWalkOnly)
        // {
		if (!(PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree || PickUpItems.state == PickUpItems.State.checkout))
        {

            if (!Movement.directWalkOnly)
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");


                if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f && transform.localEulerAngles != Vector3.zero)
                {

                    if (!Movement.directWalkOnly)
                    {
						// Rotate back to center with head look.
                       // iTween.RotateTo(transform.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "islocal", true, "speed", 30f, "oncomplete", "setRotationZero", "easetype", iTween.EaseType.linear));
                    }
                    
                    x = 0f;
                    y = 0f;

                }

                if (!(transform.localRotation == Quaternion.identity && x == 0 && y == 0))
                {
                    rotationX += x * sensitivityX;
                    rotationY += y * sensitivityY;

                    rotationX = ClampAngle(rotationX, minimumX, maximumX);
                    rotationY = ClampAngle(rotationY, minimumY, maximumY);

                    xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);

					//transform.localRotation = originalRotation * xQuaternion * yQuaternion;
					transform.localRotation = originalRotation * yQuaternion;

					if(GameFlow.tutorialShown)
						Messenger<float, float>.Broadcast("LookAround", transform.localRotation.x, transform.localRotation.y);
                }
            }
            /*else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }*/
            else
            {
                float y = Input.GetAxis("Mouse Y");
                if (Mathf.Abs(y) < 0.05f && transform.localEulerAngles != Vector3.zero)
                {
                    y = 0f;
                }
                rotationY += y * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }
        }
        else if (PickUpItems.state == PickUpItems.State.picked)
        {
            //if (Mathf.Abs(transform.localEulerAngles.y) > 1f)
                //resetRotationY();
            if (!CameraZoom.zoomBack)
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                if (Mathf.Abs(y) > 0.03f || Mathf.Abs(x) > 0.03f)
                {
                    zoomAndMove = true;
                    x *= Time.deltaTime;
                    y *= Time.deltaTime;
                    pickObjModeMoveRightDistance += x;
                    pickObjModeMoveUpDistance += y;
                    pickObjModeMoveUpDistance = Mathf.Clamp(pickObjModeMoveUpDistance, -0.15f, 0.15f);
                    pickObjModeMoveRightDistance = Mathf.Clamp(pickObjModeMoveRightDistance, -0.1f, 0.1f);
                    camera.localPosition = new Vector3(pickObjModeMoveRightDistance, -pickObjModeMoveUpDistance, camera.localPosition.z);
                }
            }
            else
            {
                zoomAndMove = false;
                pickObjModeMoveUpDistance = 0f;
                pickObjModeMoveRightDistance = 0f;
            }
        }
        else if (PickUpItems.state == PickUpItems.State.compared)
        {
            if (!CameraZoom.zoomBack)
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                if (Mathf.Abs(y) > 0.03f || Mathf.Abs(x) > 0.03f)
                {
                    zoomAndMove = true;
                    x *= Time.deltaTime;
                    y *= Time.deltaTime;
                    pickObjModeMoveRightDistance += x;
                    pickObjModeMoveUpDistance += y;
                    pickObjModeMoveUpDistance = Mathf.Clamp(pickObjModeMoveUpDistance, -0.15f, 0.15f);
                    pickObjModeMoveRightDistance = Mathf.Clamp(pickObjModeMoveRightDistance, -0.2f, 0.2f);
                    camera.localPosition = new Vector3(pickObjModeMoveRightDistance, -pickObjModeMoveUpDistance, camera.localPosition.z);
                }
            }
            else
            {
                zoomAndMove = false;
                pickObjModeMoveUpDistance = 0f;
                pickObjModeMoveRightDistance = 0f;
            }
		}
    }

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        if (GameObject.Find("LookAtPos"))
        {
            lookAtPos = GameObject.Find("LookAtPos").transform;
            transform.rotation = lookAtPos.rotation;
            transform.position = lookAtPos.position;
            transform.parent = lookAtPos;
        }
        originalRotation = transform.localRotation;

        camera = Camera.main.transform;
    }

	void RotateBackToCenter()
	{
		Debug.Log ("Rotating camera back to zero");
		iTween.RotateTo(transform.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "islocal", true, "speed", 30f, "oncomplete", "setRotationZero", "easetype", iTween.EaseType.linear));
		//transform.localRotation = originalRotation;
	}

	void RotateToFaceAisle()
	{
		Debug.Log ("Rotating camera to face aisle");
		iTween.RotateTo(transform.gameObject, iTween.Hash("rotation", new Vector3(0f, -90f, 0f), "islocal", true, "speed", 30f, "oncomplete", "setRotationAisle", "easetype", iTween.EaseType.linear));
	}


    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}