//[0] Hours
//[1] Minutes
//[2] Seconds
//(Y,X,Z)
private var FullTime = System.DateTime.Now.ToString("hh-mm-ss");
var HandHours   : Transform;
var HandMinutes : Transform;
var HandSecond  : Transform;

var TickClip: AudioClip;

function Start()
{
	PlaySound();
}

function Update()
{
	FullTime = System.DateTime.Now.ToString("hh-mm-ss");
	var TimeSpliter = FullTime.Split("-"[0]);
	HandHours.eulerAngles.z = (float.Parse(TimeSpliter[0])+float.Parse(TimeSpliter[1])/60)*30;
	HandMinutes.eulerAngles.z = float.Parse(TimeSpliter[1]) * 6;
	HandSecond.eulerAngles.z = float.Parse(TimeSpliter[2]) * 6;
	/****Demo****\
	HandHours.Rotate(0,0,5 * Time.deltaTime);
	HandMinutes.Rotate(0,0,2 * Time.deltaTime);
	HandSecond.Rotate(0,0,10 * Time.deltaTime);
	*/
}

function PlaySound()
{
	yield WaitForSeconds (1);
	var dummy = 0;
	
	while(dummy == 0)
	{
		yield WaitForSeconds (1);	
		GetComponent.<AudioSource>().PlayOneShot(TickClip);
	}
}