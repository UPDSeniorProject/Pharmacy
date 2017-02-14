using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]


public class MouseLook2 : MonoBehaviour
{


    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    public static bool isHolding = false;
    public static bool windowFixed = false;

    public static bool isComparing = false;

    private RaycastHit hit;
    private RaycastHit hit2;

    private Transform pickObj = null;
    private Transform pickObjFirst = null;

    private Vector3 objPositionBeforePick;
    private Quaternion objRotationBeforePick = Quaternion.identity;

    private Vector3 objFirstPositionBeforePick;
    private Quaternion objFirstRotationBeforePick = Quaternion.identity;

    // not used
    private GameObject pickedObject;
    private GameObject pickedObjectFirst;

    private float dist;
    private Vector3 newPos;
    private Transform SpawnTo;

    private GameObject MainCamera;
    private GameObject SecondCamera;
    private GameObject ThirdCamera;
    private Transform ShoppingCart;

    private bool isRotate = false;
    IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        print("WaitAndPrint " + Time.time);
    }
    IEnumerator Wait()
    {
        print("Starting " + Time.time);
        yield return StartCoroutine(WaitAndPrint(2.0F));
        print("Done " + Time.time);
    }
    void Update()
    {

        //jsScript = this.GetComponent<PickUp_mouse.js>;

        if (Input.GetKeyDown("escape"))
        {
            windowFixed = true;
            Screen.lockCursor = false;
        }
        if (Input.GetKeyDown("f"))
        {
            windowFixed = !windowFixed;
            Screen.lockCursor = !Screen.lockCursor;
        }
        if (Input.GetKeyDown("y"))
        {
            windowFixed = false;
            isHolding = false;
        }


        // commpare two objects
        if (Input.GetKeyDown("c") && pickObj != null)
        {
            isComparing = true;
            isHolding = !isHolding;
            SpawnTo.transform.DetachChildren();
            pickObj.GetComponent<Rigidbody>().useGravity = true;
            pickObj.transform.position = objPositionBeforePick;
            pickObj.transform.rotation = objRotationBeforePick;
            pickObjFirst = pickObj;
            objFirstPositionBeforePick = objPositionBeforePick;
            objFirstRotationBeforePick = objRotationBeforePick;

            pickedObjectFirst = pickedObject;
            objRotationBeforePick = Quaternion.identity;
            objPositionBeforePick = Vector3.zero;
            pickObj = null;
            isRotate = false;
        }

        // put into the cart
        if (Input.GetKeyDown("t") && pickObj != null)
        {
            isHolding = false;
            SpawnTo.transform.DetachChildren();
            //pickObj.rigidbody.useGravity = true;
            pickObj.GetComponent<Rigidbody>().useGravity = true;
            pickObj.GetComponent<Rigidbody>().isKinematic = false;

            //+ShoppingCart.transform.forward
            pickObj.transform.position = ShoppingCart.transform.position + ShoppingCart.transform.up * 0.5f - ShoppingCart.transform.right * 0.5f;
            pickObj.transform.rotation = objRotationBeforePick;
            //Wait();
            pickObj.parent = ShoppingCart;
            //objRotationBeforePick = Quaternion.identity;
            objPositionBeforePick = Vector3.zero;
            pickObj = null;
            isRotate = false;
            //SecondCamera.camera.enabled = true;
            //ThirdCamera.camera.enabled = false;
        }


        if (Input.GetKeyDown("p") && pickObj != null)
        {
            if (pickObjFirst != null)
            {
                SpawnTo.transform.DetachChildren();
                pickObjFirst.GetComponent<Rigidbody>().useGravity = true;
                pickObjFirst.transform.position = objFirstPositionBeforePick;
                pickObjFirst.transform.rotation = objFirstRotationBeforePick;
                pickObjFirst = null;
            }
            isHolding = false;
            SpawnTo.transform.DetachChildren();
            pickObj.GetComponent<Rigidbody>().useGravity = true;
            pickObj.transform.position = objPositionBeforePick;
            pickObj.transform.rotation = objRotationBeforePick;
            objRotationBeforePick = Quaternion.identity;
            objPositionBeforePick = Vector3.zero;
            pickObj = null;
            isRotate = false;
            //SecondCamera.camera.enabled = true;
            //ThirdCamera.camera.enabled = false;
        }

        else if (Input.GetKeyDown("p") || pickObj != null)
        { // if left button creates a ray from the mouse
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Ray ray = Camera.main.ViewportPointToRay (new Vector3(0.5f,0.5f, 0));

            if (!pickObj)
            { // if nothing picked yet...
                if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Pick")
                {
                    // if it's a rigidbody, zero its physics velocity
                    //if (hit.rigidbody) hit.rigidbody.velocity = Vector3.zero;

                    pickObj = hit.transform; // now there's an object picked

                    // remember its distance from the camera
                    //hit.transform. 
                    pickedObject = hit.collider.gameObject;

                    // remember the object's position and rotation before pick
                    objPositionBeforePick = pickedObject.transform.position;
                    objRotationBeforePick = pickedObject.transform.rotation;
                    dist = Vector3.Distance(pickObj.position, Camera.main.transform.position);
                    //pickObj.material.color -= Color(0.1f, 0, 0) * Time.deltaTime;
                    isHolding = true;
                    Screen.lockCursor = false;

                }
            }
            //isComparing == true
            else if (pickObjFirst != null)
            {

                pickObjFirst.GetComponent<Rigidbody>().useGravity = false;
                pickObjFirst.parent = SpawnTo;
                pickObjFirst.transform.position = SpawnTo.transform.position + SpawnTo.transform.forward * 0.5f - SpawnTo.transform.right * 0.2f; //sets position
                pickObjFirst.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");

                pickObj.GetComponent<Rigidbody>().useGravity = false;
                pickObj.parent = SpawnTo; //parents the object
                pickObj.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                pickObj.transform.position = SpawnTo.transform.position + SpawnTo.transform.forward * 0.5f + SpawnTo.transform.right * 0.2f; //sets position

                if (Input.GetMouseButtonDown(0))
                {
                    isRotate = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isRotate = false;
                }
                if (isRotate == true && Physics.Raycast(ray, out hit2) && hit2.transform == pickObj)
                {
                    float rotateMouseX = -Input.GetAxis("Mouse X") * Time.deltaTime * 200;
                    pickObj.Rotate(0, rotateMouseX, 0, Space.World);
                }
                else if (isRotate == true && Physics.Raycast(ray, out hit2) && hit2.transform == pickObjFirst)
                {
                    float rotateMouseX = -Input.GetAxis("Mouse X") * Time.deltaTime * 200;
                    pickObjFirst.Rotate(0, rotateMouseX, 0, Space.World);
                }
            }
            else
            { // if object already picked...

                pickObj.GetComponent<Rigidbody>().useGravity = false;
                pickObj.parent = SpawnTo; //parents the object
                //object.layer = 9;
                pickObj.GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                //SecondCamera.camera.enabled = false;
                //ThirdCamera.camera.enabled = true;

                pickObj.transform.position = SpawnTo.transform.position + SpawnTo.transform.forward * 0.5f; //sets position
                //if(isRotate ==false){
                //pickObj.transform.rotation = SpawnTo.transform.rotation;

                if (Input.GetMouseButtonDown(0))
                {
                    isRotate = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isRotate = false;
                }
                if (isRotate == true)
                {
                    float rotateMouseX = -Input.GetAxis("Mouse X") * Time.deltaTime * 200;
                    pickObj.Rotate(0, rotateMouseX, 0, Space.World);
                    //rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    //rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

                    //pickObj.transform.localEulerAngles = new Vector3(rotationY, pickObj.transform.localEulerAngles.y, 0);
                }
            }
            //isHolding = true;
            //if(isHolding==false){
            //SpawnTo.transform.DetachChildren();
            //	pickObj.rigidbody.useGravity = true;
            //}
        }

       /* if (isHolding == false && windowFixed == false)
        {
            //windowFixed = false;
            Screen.lockCursor = true;
            if (axes == RotationAxes.MouseXAndY)
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }

        }*/

    }

    void Start()
    {
        // Make the rigid body not change rotation

        //Screen.lockCursor = true;
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        MainCamera = GameObject.Find("Main Camera");
       // ShoppingCart = GameObject.Find("shoppingCart").transform;
        //SecondCamera = GameObject.Find("Camera");
        //ThirdCamera = GameObject.Find("Camera2");
        //ThirdCamera.camera.enabled = false;

        SpawnTo = GameObject.Find("Main Camera").transform;

    }

}