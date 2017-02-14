#pragma strict
public var GUIdisplayed : boolean;


function start() {
	GUIdisplayed = false;
}
function OnMouseDown() {	
	GUIdisplayed = true;
}

function OnGUI() {
	if(GUIdisplayed == true) {
		if(GUI.Button(Rect(500,200,300,100), "Blood Pressure:  , click to return to patient.")) {
			GUIdisplayed = false;
		}
	}
}