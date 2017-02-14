using UnityEngine;
using System.Collections;

public class VectorHelper : RenBehaviour {

    GameObject End, St, Direction;
    
    public Vector3 EndPosition;
    public Vector3 StartPosition;


    protected Vector3 DirectionVector;

	// Use this for initialization
	protected override void Start() {
        base.Start();
        //Get references 
        End =  transform.FindChild("End").gameObject;
        St = transform.FindChild("Start").gameObject;
        Direction = transform.FindChild("Direction").gameObject;
       
	}
	
	// Update is called once per frame
    protected override void Update()
    {
        base.Update();

        End.transform.position = EndPosition;
        St.transform.position = StartPosition;

        DirectionVector = EndPosition - StartPosition;
        if(DirectionVector.magnitude > 0) {
            Direction.transform.localScale = new Vector3(1.0f, DirectionVector.magnitude / 2.0f, 1.0f);
            Quaternion rotation = new Quaternion();
            rotation.SetFromToRotation(Vector3.up, DirectionVector);
            Direction.transform.localRotation = rotation;

            Vector3 HalfStep = DirectionVector.magnitude/2.0f * DirectionVector.normalized;
            Direction.transform.position = StartPosition + HalfStep;
        }
        
    }
}
