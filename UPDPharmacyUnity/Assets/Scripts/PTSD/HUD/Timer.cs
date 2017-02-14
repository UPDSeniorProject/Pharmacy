using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	//float timer = 0f;
	int minutes;
	int seconds;
	private static float timer;
	public string timerFormatted;
	public UILabel content;

    public static float GetTimer {
        get { return timer; }
    }

	// Use this for initialization
	void Start () {
        timer = 0f;
		 content = GameObject.Find("TimerLabel").GetComponent<UILabel>();
		content.text = "";
	}
	
	// Update is called once per frame
	
 
	void Update()
	{
        if (GameFlow.state == GameFlow.State.Tasks_doing)
        {
            timer += Time.deltaTime;
        }
        minutes = Mathf.FloorToInt(timer / 60f);
        seconds = Mathf.FloorToInt(timer - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        content.text = niceTime;
	}
}
