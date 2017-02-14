#pragma strict
 
 var waypoint : Transform[];            // The amount of Waypoint you want
 var patrolSpeed : float = 3;        // The walking speed between Waypoints
 var loop : boolean = true;            // Do you want to keep repeating the Waypoints
 var dampingLook = 6.0;                // How slowly to turn
 var pauseDuration : float = 0;        // How long to pause at a Waypoint
 //var wasPaused : boolean = false;
 //var previousDistance : float = 0;
 
 private var curTime : float;
 private var currentWaypoint : int = 0;
 private var character : CharacterController;
 private var player : GameObject;
 private var pausePatrol : boolean = false;
 private var charHeight : float;

 private var timeSinceLastAudio = 0.0;
    
 function Start(){
     character = GetComponent(CharacterController);
     player = GameObject.Find("Robot_Prefab");
    // var audio = GetComponent(AudioSource);
     // audio.Play();
     charHeight = transform.localPosition.y;
 }
 
 function Update(){
     timeSinceLastAudio += 1 * Time.deltaTime;

     if(transform.localPosition.y != charHeight)
         transform.localPosition = new Vector3(transform.localPosition.x,charHeight,transform.localPosition.z);
 	
 	//var distanceToTarget = Vector3.Distance(transform.position, player.transform.position); // Assuming that the target is the player or the audio listener
  
     if(currentWaypoint < waypoint.length && !pausePatrol){
      // if(distanceToTarget < previousDistance)
      // {
     //	if(distanceToTarget > 3.5)
     //	{
     //		if(wasPaused)
     //		{
     //			gameObject.animation.Play();
	//			gameObject.audio.Play();
     //		}
     //		wasPaused = false;
         	patrol();
   	//	}
    //	else
    //	{
    //		wasPaused = true;
      //  	gameObject.animation.Stop();
		//	gameObject.audio.Stop();
    //	}
    //	}
    }else{    
     if(loop){
         currentWaypoint=0;
         } 
     }
     //previousDistance = distanceToTarget;
 }
 
 function OnCollisionEnter(collision : Collision) {
 
//Debug.Log("hit");
 
 }
 function OnTriggerEnter (other : Collider) {
 
    
     if(gameObject != other.gameObject)
     {
       if(other.gameObject.name == "Robot_Prefab" || other.name == "Root")
       {
           gameObject.GetComponent(Animation).Stop();
           if(gameObject.name.Contains("walk"))
               gameObject.GetComponent(AudioSource).Stop();
        if(timeSinceLastAudio > 10 && gameObject.name.Contains("ClerkRoam"))
	     {
		     gameObject.GetComponent(AudioSource).Play();
		     timeSinceLastAudio = 0.0;
	     }
	     pausePatrol = true;
       }
     }
 /*Debug.Log("trigger");
 Debug.Log(gameObject.name);
 Debug.Log(other.gameObject.name);
 Debug.Log("end trigger");
 */
 //if(other.gameObject.name == "Sphere")
//		Debug.Log("triggersphere");
//		else if(other.gameObject.name == "VirtualHumanLoop")
//		Debug.Log("triggerHuman");
		//Debug.Log("trigger");
	}
	
function OnTriggerExit (other : Collider) {
	if(gameObject != other.gameObject)
     {
	    if(other.gameObject.name == "Robot_Prefab" || other.name == "Root")
       {
           if(gameObject.name.Contains("walk"))
               gameObject.GetComponent(AudioSource).Play();

         gameObject.GetComponent(Animation).Play();
	     //gameObject.GetComponent.<AudioSource>().Play();
	     pausePatrol = false;
       }
     }
	}
	
/*function pause()
{
wasPaused = true;
gameObject.animation.Stop();
gameObject.audio.Stop();

	}
*/
		
 function patrol(){
 
         var target : Vector3 = waypoint[currentWaypoint].position;
         target.y = transform.position.y; // Keep waypoint at character's height
         var moveDirection : Vector3 = target - transform.position;
 //Debug.Log ("target:" + target.x + " " + target.y + " " + target.z);
 //Debug.Log ("movedirection:" + moveDirection.x + " " + moveDirection.y + " " + moveDirection.z);
 //Debug.Log("mag:" + moveDirection.magnitude);
     if(moveDirection.magnitude < .5){
         if (curTime == 0)
             curTime = Time.time; // Pause over the Waypoint
         if ((Time.time - curTime) >= pauseDuration){
             currentWaypoint++;
             curTime = 0;
         }
     }else{        
     
         var rotation = Quaternion.LookRotation(target - transform.position);
   //      Debug.Log ("rotation:" + rotation.x + " " + rotation.y + " " + rotation.z);
         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampingLook);
     //    Debug.Log(moveDirection.normalized * patrolSpeed * Time.deltaTime);
         character.Move(moveDirection.normalized * patrolSpeed * Time.deltaTime);
       //  character.gameObject.animation.Play("Walk");
     }    
 }