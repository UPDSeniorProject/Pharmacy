using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(HeadLookBehaviour))]
public class HeadLookBehaviourSetup : Editor {
	
	public HeadLookBehaviour myTarget;
	public GameObject targetGameObject;

	void OnEnable(){
		myTarget = (HeadLookBehaviour)target;
		targetGameObject = myTarget.gameObject;
	}


	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();

		if (GUILayout.Button ("Segment Setup")) {

			Transform firstT = targetGameObject.transform.Find("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/Neck1");
			Transform lastT = targetGameObject.transform.Find ("master/reference/Hips/Spine/Spine1/Spine2/Spine3/Spine4/Neck/Neck1/Head");

			//The new characters have different paths for Neck and Head
			if(firstT == null)
				firstT = targetGameObject.transform.Find ("master/Reference/Hips/Spine/Spine1/Spine2/Neck");
			if(lastT == null)
				lastT = targetGameObject.transform.Find("master/Reference/Hips/Spine/Spine1/Spine2/Neck/Head");

            if (myTarget.Segments.Length < 1)
            {
                myTarget.Segments = new BendingSegment[1];
                myTarget.Segments[0] = new BendingSegment();
                Debug.Log("Had to add a segment to HeadLookBehaviour");
            }

            
			myTarget.Segments[0].FirstTransform = firstT;
			myTarget.Segments[0].LastTransform = lastT;
		}
	}
	
}
