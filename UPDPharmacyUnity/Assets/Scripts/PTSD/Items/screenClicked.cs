using UnityEngine;
using System.Collections;

public class screenClicked : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUpAsButton (){
		Messenger.Broadcast("Display Bill on GUI");
	}
}
