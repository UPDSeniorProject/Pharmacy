using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class MovePoliceCar : MonoBehaviour {
	/*public GameObject CarHolder;
	float MoveSpeed = 3f;

	public Texture2D fadeOutTexture; 	// will overlay screen
	public float fadeSpeed = 0.02f;
	public Text movesText;

	private int drawDepth = -1000;   	// texture's order in draw hierarchy. a low number means it renders on top
	private float alpha = 0.01f; 		// the texture's aplpha between 0 and 1
	private int fadeDir = 1;			// the direction to fade: in = -1 or out = 1
	private bool fading = false;


	// put cop car movements in here
	// private Dictionary<int, string> Movements;
	// private string MOVEMENTS_FILE_NAME = "Assets/Scripts/UPD/MoveCarInstructions.txt";


	// for traversing
	private int LineCounter = 0, lastLine;
	private bool moving = false;

	// Use this for initialization
	void Start () {
		// Movements = new Dictionary<int, string> ();
		// LoadMovementsIntoDictionary ();
	}

	int pausing = 40;
	void OnGUI() {
	}

	void Move(GameObject o, Vector3 v) {
		o.transform.Translate (v * MoveSpeed);
	}

	void Move(Camera c, Vector3 v) {
		c.transform.Translate (v * MoveSpeed);
	}

	void Rotate(GameObject o, Vector3 v) {
		// v is pretty small
		// multiply it by a larger number to rotate it more

		o.transform.Rotate (v , Time.deltaTime * 35);
	}*/
		
	public void ActivateMove() {
	// 	moving = true;
	}

	public void Move() {
		/*string direction;
		if (Movements.TryGetValue(LineCounter, out direction)) {
			string[] AllDirections = direction.Split (' ');
		
			for (int i = 0; i < AllDirections.Length; i++) {
				// idk why, but Unity moves backwards. keep note of that.

				// slow down
				if (AllDirections[i].Equals("BRAKE")) {
					MoveSpeed -= 0.1f;
				}

				// gas
				if (AllDirections[i].Equals ("^")) {
					Move (CarHolder, Vector3.back);
				} 

				// turn left
				if (AllDirections[i].Equals ("<")) {
					Rotate (CarHolder, Vector3.down);
				} 

				// turn right
				if (AllDirections[i].Equals (">")) {
					Rotate (CarHolder, Vector3.up);
				}

				// if it doesn't match, loop continues
			}
		}
		
		LineCounter++;

		if (LineCounter > lastLine) {
			moving = false;
			// fading = true;
			// fading logic is done in ToggleCameras()
			// grab the ToggleCamera script
			GameObject c = GameObject.Find ("Cameras");
			ToggleCameras t = c.GetComponent ("ToggleCameras") as ToggleCameras;

			t.FadeOut ();

			LineCounter = 0;
		}*/
	}

	// Update is called once per frame
	void Update () {
		
		// if (moving)
		// 	Move ();
	}


	public void LoadMovementsIntoDictionary() {
		/*
        // import file
		// StreamReader F = new StreamReader (MOVEMENTS_FILE_NAME);

		// Movements = new Dictionary<int, string> ();

		// read all the movements
		int i;
		string[] text = movesText.text.Split('\n');
		for (i = 0; i < text.Length; i++) {

			// split based on first space
			string[] tokens = text[i].Split (new char[] {' '}, 2);
			
			// just go on if it's not successful
			// these lines are either blank or part of legend
			if (!tokens[0].Equals("MOVE"))
				continue;
			
			// success -- add to dictionary
			Movements.Add (i++, tokens [1]);
		}

		// for movement purposes
		lastLine = i;
        */
	}
}
