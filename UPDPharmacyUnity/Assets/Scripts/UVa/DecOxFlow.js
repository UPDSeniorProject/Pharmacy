#pragma strict

public var mainCam : Camera;
public var oxCam : Camera;
public var object : Transform;
public var duration : float = 5;
private var moving : boolean = false;

function OnGUI() {
	if(oxCam.GetComponent.<Camera>().enabled) {
		if(GUI.Button(Rect(500, 300, 300, 50), "Decrease Oxygen Flow")) {
			MoveObjTo(object, duration);
		}
	}
}



function MoveObjTo(obj : Transform, time : float) {
	if (moving) return;
	moving = true;
	var pointA = obj.position;
	var t : float = 0;
	while (t < 1) {
		t += Time.deltaTime / time;
		obj.position.x = obj.position.x;
		//so ball doesnt leave the cylinder
		if (obj.position.y > 147.9055) {
			obj.position.y = obj.position.y - 0.005;
		}
		yield;
	}
	//prints the absolute world position of the object
	//print(obj.transform.position.y);
	moving = false;
}