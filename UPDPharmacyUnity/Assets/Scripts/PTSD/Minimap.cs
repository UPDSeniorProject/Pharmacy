using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	public Texture2D compass; // compass image
	public Texture2D needle;  // needle image 
	//public r = new Rect(10, 10, 200, 200); // rect where to draw compass
	//public Rect r = new Rect(Screen.width - 100 , Screen.width - 100, Screen.width/8, Screen.width/8);
	//private Rect r; 
	//new Rect(200,200,100,100);
	public float angle; // angle to rotate the needle

	// Use this for initialization
	void Start () {

	}

	void OnGUI(){

		// Get the angle of rotation
		//Rect r = new Rect(Screen.width - (Screen.width/8 + 20) , Screen.width - (Screen.width/8 + 20), Screen.width/8, Screen.width/8);
		Rect r = new Rect(Screen.width - Screen.width/8 - 10 , 170, Screen.width/8, Screen.width/8);
		//angle = Camera.main.transform.eulerAngles.y - 90;
		angle = (Camera.main.transform.eulerAngles.y - GameObject.Find ("Robot_Prefab").transform.eulerAngles.y);

		GUI.DrawTexture(r, compass); // draw the compass...
		Vector2 p = new Vector2(r.x+(r.width/2)-10,r.y+r.height/2); // find the center
		Matrix4x4 svMat = GUI.matrix; // save gui matrix
		GUIUtility.RotateAroundPivot(angle,p); // prepare matrix to rotate
		GUI.DrawTexture(new Rect((r.x+r.width/2)-47, r.y, r.width/2, r.height/2), needle); // draw the needle rotated by angle
		GUI.matrix = svMat; // restore gui matrix
	}

	// Update is called once per frame
	void Update () {
	
	}
}
