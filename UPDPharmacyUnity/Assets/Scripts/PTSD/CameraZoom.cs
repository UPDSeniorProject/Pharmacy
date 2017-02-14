using UnityEngine;
using System.Collections;

public class CameraZoom : GUIBasic {
	private bool zooming;
    private bool closer;

    private bool zoomByController;
    private float distanceMax = 3f;
    private float zoomSensitivity = 3f;
    private float zoomDis = 0f;
    public static bool zoomBack = true;
    //private bool closer;
	private Vector3 cameraOriginalPosition;
	Transform lookAtPosChild;
	//Camera camera;
	public float speed = 10.0F;
	public Texture2D zoomCursor;
    public Texture2D aimCursor;
    new Transform camera;
    public GUIStyle myStyle;
    public Texture magnify;
	// Use this for initialization
    static private Vector3 top = Vector3.zero;
    static private Vector3 middle = Vector3.zero;
    static private Vector3 bottom = Vector3.zero;

    public static bool controllerZoom = false;

    public bool cursorNull = false;
    public enum State
    {
        middle,
        top,
        bottom
    }
    public static State state;

    void OnEnable()
    {
        Messenger.AddListener("Set cursor to null", setCursorNull);
    }
    void OnDisable()
    {
        Messenger.RemoveListener("Set cursor to null", setCursorNull);
    }

    void setCursorNull()
    {
        cursorNull = true;
    }


	void Start () {
		/*if(GameObject.Find ("LookAtPos")){
			lookAtPos = GameObject.Find ("LookAtPos").transform;
			transform.forward = lookAtPos.forward;
			transform.position = lookAtPos.position;
			transform.parent = lookAtPos;
		}*/

		zooming = false;
        closer = false;
		//camera = Camera.main;
        camera = Camera.main.transform;
		cameraOriginalPosition = Camera.main.transform.position;
	}

    void zoomFunction(float dis)
    {
		if (dis > 0.05f)
        {
            zoomBack = false;
        }
        else
        {
           zoomBack = true;
        }
        if (!zoomBack)
        {
            dis *= 0.5f*zoomSensitivity * Time.deltaTime;
            zoomDis += dis;
            zoomDis = Mathf.Clamp(zoomDis, 0f, distanceMax);
            //camera = camera.forward * zoomDis;
            //camera.Translate(camera.forward * zoomDis, Space.World);
            Vector3 originalPos = GameObject.Find("LookAtPosChild").transform.position;
            Vector3 distance = camera.forward * zoomDis;
            if (!LookAround.zoomAndMove)
                camera.position = originalPos + Vector3.ClampMagnitude(distance, distanceMax);
        }
        else
        {
            Vector3 origialPos = GameObject.Find("LookAtPosChild").transform.position;
            //Debug.Log(dis + "input");
            if (Mathf.Abs(Vector3.Distance(camera.transform.position, origialPos)) > 0.05f)
            {
                zoomDis = 0f;
                float journeyLength = Vector3.Distance(camera.position, origialPos);
                float distCovered = (Time.deltaTime) * zoomSensitivity;
                float fracJourney = distCovered / journeyLength;
                // Debug.Log("moving"+fracJourney);
                camera.position = Vector3.Lerp(camera.position, origialPos, fracJourney);
                //camera.position = origialPos;
                //iTween.MoveTo(camera.gameObject, iTween.Hash("position", origialPos, "speed", 20f, "easetype", iTween.EaseType.linear));
            }
            else
            {
                camera.position = origialPos;
            }
           
        }
		if(GameFlow.tutorialShown)
			Messenger<bool>.Broadcast("ZoomButton", zoomBack);
    }

    void OnGUI()
    {
        
    }
	// Update is called once per frame
	void Update () {

        // zoom by controller
        if (GameFlow.state == GameFlow.State.Tasks_doing || GameFlow.state == GameFlow.State.Tutorial)
        {
            if (!(PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree))
            {
                //float distanceMax = 0f;

                if (PickUpItems.state == PickUpItems.State.checkout)
                {
                    distanceMax = 0.8f;
                }
                else if (Movement.selectionModeRight || Movement.selectionModeLeft)
                {
                    distanceMax = 1.2f;
                }
                else
                {
                    distanceMax = 1.2f;
                }
                float dis = Input.GetAxis("Left Trigger");
                zoomFunction(dis);
            }
            else if (PickUpItems.state == PickUpItems.State.picked || PickUpItems.state == PickUpItems.State.compared || PickUpItems.state == PickUpItems.State.comparedThree)
            {
                Transform pickObj = null;

                float dis = Input.GetAxis("Left Trigger");
                //<Transform>();
                Transform lookAtPosChild = GameObject.Find("LookAtPosChild").transform;
                if (lookAtPosChild.childCount > 1)
                {
                    pickObj = lookAtPosChild.GetChild(1);
                    //distanceMax = pickObj.collider.bounds.size.x*0.85f;
					distanceMax = Mathf.Abs(Vector3.Distance( pickObj.position, Camera.main.transform.position))*0.70f;
					//Debug.Log ("DM" + distanceMax);
					//distanceMax = pickObj.*0.85f;
                    //*0.7f
//                    Debug.Log(distanceMax + "maxdis");
                    //camera.DetachChildren();
                 }
                //pickObj.parent = null;
//                Debug.Log(distanceMax + "max distance");
                zoomFunction(dis);
               
            }

            //Debug.Log(zoomDis + "pos " + camera.localPosition);
		Vector3 mousePosition = Input.mousePosition;
		//Debug.Log(mousePosition.y+"  mouse");
       

		if( !closer && (PickUpItems.state==PickUpItems.State.picked ||PickUpItems.state==PickUpItems.State.compared || PickUpItems.state==PickUpItems.State.comparedThree) && mousePosition.y > Screen.height*0.16f){
			Cursor.SetCursor(zoomCursor, Vector2.zero,CursorMode.Auto);

            }
        else if (cursorNull || Movement.isCheckout)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(aimCursor, Vector2.zero, CursorMode.Auto);
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
		
		}
        
	}

}
