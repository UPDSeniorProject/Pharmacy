#pragma strict
public var GUIdisplayed : boolean;
private var startColor : Color;
private var localScale : Vector3;
public var pulseOx : GameObject;
private var count : int;

function Start () {
	count = 0;
	GUIdisplayed = false;
	startColor = GetComponent.<Renderer>().material.color;
}

function OnMouseOver() {
	if (count == 0) {
		pulseOx.transform.localScale += Vector3(2,2,2);
	}
	count++;
}

function OnMouseExit() {
	count = 0;
	pulseOx.transform.localScale -= Vector3(2,2,2);
}

function OnMouseDown() {
	GUIdisplayed = true;
}

function OnGUI() {
	if(GUIdisplayed == true) {
		if(GUI.Button(Rect(500,200,300,100), "Oxygen Saturation:  , click to return to patient.")) {
			GUIdisplayed = false;
		}
	}
}