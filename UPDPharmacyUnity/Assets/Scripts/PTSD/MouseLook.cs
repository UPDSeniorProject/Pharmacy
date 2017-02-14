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
public class MouseLook : MonoBehaviour
{
    public float smooth = 3f;		// a public variable to adjust smoothing of camera motion
    Transform lookAtPosChild;			// the position to move the camera to when using head look

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    

    Quaternion originalRotation;
    /*
    void OnEnable()
    {
        Messenger.AddListener("set camera rotation to zero", setZero);
        Messenger<float>.AddListener("set cameraX max angle", setAngleX);
    }
    void OnDisable()
    {
        Messenger.RemoveListener("set camera rotation to zero", setZero);
        Messenger<float>.RemoveListener("set cameraX max angle", setAngleX);
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
        Debug.Log(transform.eulerAngles);
    }
    void setRotationZero()
    {
        rotationX = 0f;
        rotationY = 0f;
    }
     */

  /*  void Update()
    {
        
            if (!(PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree))
            {


                if (axes == RotationAxes.MouseXAndY)
                {
                    // Read the mouse input axis
                    float x = Input.GetAxis("Mouse X");
                    float y = Input.GetAxis("Mouse Y");


                    if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f && transform.localEulerAngles != Vector3.zero)
                    {

                        if (!Movement.directWalkOnly)
                        {
                            iTween.RotateTo(transform.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "islocal", true, "speed", 30f, "oncomplete", "setRotationZero", "easetype", iTween.EaseType.linear));
                         
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
                        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
                       
                    }
                }
                else if (axes == RotationAxes.MouseX)
                {
                    rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                    rotationX = ClampAngle(rotationX, minimumX, maximumX);

                    Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                    transform.localRotation = originalRotation * xQuaternion;
                }
                else
                {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = ClampAngle(rotationY, minimumY, maximumY);

                    Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                    transform.localRotation = originalRotation * yQuaternion;
                }
            }
            else if (PickUpItems.state == PickUpItems.State.picked)
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
                        pickObjModeMoveRightDistance = Mathf.Clamp(pickObjModeMoveRightDistance, -0.1f, 0.1f);
                        camera.transform.localPosition = new Vector3(pickObjModeMoveRightDistance, -pickObjModeMoveUpDistance, camera.transform.localPosition.z);
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
                        camera.transform.localPosition = new Vector3(pickObjModeMoveRightDistance, -pickObjModeMoveUpDistance, camera.transform.localPosition.z);
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
    */

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        if (GameObject.Find("LookAtPosChild"))
        {
            lookAtPosChild = GameObject.Find("LookAtPosChild").transform;
            transform.rotation = lookAtPosChild.rotation;
            transform.position = lookAtPosChild.position;
            transform.parent = lookAtPosChild;
        }
        originalRotation = transform.localRotation;
    }

}