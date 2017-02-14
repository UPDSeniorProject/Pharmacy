using UnityEngine;
using System.Collections;

public class TaskTrigger : MonoBehaviour {
    public string taskName;
	public string taskComparisonName = "";

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            //Debug.Log("enter");
            Messenger<string, string>.Broadcast("task trigger", taskName, taskComparisonName);
        }
    }
    void OnTriggerExit(Collider other)
    {
        //Debug.Log("disable00");
        if (other.transform.tag == "Player")
        {
                //Debug.Log("enter");
            //Debug.Log("out");
            Messenger<string>.Broadcast("task trigger end", taskName);
        }
    }
    

	// Update is called once per frame
	void Update () {
        
	
	}
}
