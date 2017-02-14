#pragma strict

public var oxCam : Camera;
public var mainCam : Camera;

function OnGUI()
{
	if(oxCam.GetComponent.<Camera>().enabled) {
		if(GUI.Button(Rect(300, 0, 200, 100), "Done")) {
			oxCam.GetComponent.<Camera>().enabled = false;
			mainCam.GetComponent.<Camera>().enabled = true;
		}
	}
}