#pragma strict

public var mainCam : Camera;
public var oxCam : Camera;
public var bed : Transform;
public var pillow : Transform;
public var duration : float = 5;
private var moving : boolean = false;
public var humanLying : GameObject;
public var humanSitting : GameObject;
public var bedUp : boolean = false;

function OnGUI() {
	if(mainCam.GetComponent.<Camera>().enabled) {
		if(GUI.Button(Rect(0,400,100,100), "Move Bed.")) {
			if(!bedUp) {
				//print("bedUp");
				humanLying.SetActive(false);
				humanSitting.SetActive(true);
				MoveBedUp(bed, duration);
				MovePillowUp(pillow, duration);
			}
			if(bedUp) {
				//print("bedDown");
				humanLying.SetActive(true);
				humanSitting.SetActive(false);
				MoveBedDown(bed, duration);	
				MovePillowDown(pillow, duration);	
			}
		}	
	}
}

function MoveBedUp(obj: Transform, time : float) {
		
	moving = true;
	var pointA = obj.position;
	var t : float = 0;
	while (t < 1) {
		t += Time.deltaTime / time;
		bed.transform.rotation.eulerAngles.z = bed.rotation.z - 50;
		bed.position.x = bed.position.x + 0.8;
		yield;
		bedUp = true;
	}
	print("bedUp");
	//prints the absolute world position of the object
	//print(obj.transform.position.y);
}

function MovePillowUp(obj: Transform, time : float) {
	
	var pointA = obj.position;
	var t : float = 0;
	while (t < 1) {
		t += Time.deltaTime / time;
		pillow.transform.rotation.eulerAngles.z = pillow.rotation.z - 50;
		pillow.position.x = pillow.position.x + 6;
		pillow.position.y = pillow.position.y + 6;
		yield;
	}
	print("pillow Up");
	//prints the absolute world position of the object
	//print(obj.transform.position.y);
}

function MoveBedDown(obj: Transform, time : float) {
	
	moving = true;
	var pointA = obj.position;
	var t : float = 0;
	while (t < 1) {
		t += Time.deltaTime / time;
		bed.transform.rotation.eulerAngles.z = bed.rotation.z + 19.5;
		bed.position.x = bed.position.x - 0.8;
		yield;
		bedUp = false;
	}
	print("bedDown");
	
	//prints the absolute world position of the object
	//print(obj.transform.position.y);
}

function MovePillowDown(obj: Transform, time : float) {
	
	var pointA = obj.position;
	var t : float = 0;
	while (t < 1) {
		t += Time.deltaTime / time;
		pillow.transform.rotation.eulerAngles.z = pillow.rotation.z + 20;
		pillow.position.x = pillow.position.x - 6;
		pillow.position.y = pillow.position.y - 6;
		yield;
	}
	print("pillow down");
	//prints the absolute world position of the object
	//print(obj.transform.position.y);
}