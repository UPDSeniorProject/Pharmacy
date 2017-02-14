using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using KinectV2 = Windows.Kinect;

public class HeadTracker : MonoBehaviour {
	public GameObject KinectV2Manager;
	public GameObject Nurse;
	public GameObject ScrubTech;
	public GameObject Character;
	public float xScale = 1f;
	public float yScale = 1f;
	public bool TrackNurse = false;
	public bool TrackScrubTech = false;

	private ulong nurseTrackingId;
	private ulong scrubTechTrackingId;
	private BodySourceManager _BodyManager;
	private AudioBodyManager _AudioBodyManager;
	private ReflectiveMarkerManager _ReflectiveMarkerManager;
	private HeadLookBehaviour _headLookBehavior;
	
	// Use  this for initialization
	void Start () {
		nurseTrackingId = 0;
		scrubTechTrackingId = 0;
		GameObject IPS = GameObject.Find("IPSRen");
		RenParameterParser parameterParser = IPS.GetComponent<RenParameterParser>();
		TrackNurse = parameterParser.GetParameterAsBool ("TrackNurse", true);
		TrackScrubTech = parameterParser.GetParameterAsBool ("TrackScrubTech", true);
		//string simulatorAddress = parameterParser.GetParameter("simulatorAddress", "localhost");
	}

	// Update is called once per frame
	void Update () {
		if (KinectV2Manager == null)
		{		
			return;
		}
		
		_BodyManager = KinectV2Manager.GetComponent<BodySourceManager>();
		_AudioBodyManager = KinectV2Manager.GetComponent<AudioBodyManager>();
		_ReflectiveMarkerManager = KinectV2Manager.GetComponent<ReflectiveMarkerManager> ();
		_headLookBehavior = Character.GetComponent<HeadLookBehaviour>();
		if (_BodyManager == null || _AudioBodyManager == null || _ReflectiveMarkerManager == null)
		{
			return;
		}

		if(Input.GetKeyUp (KeyCode.N)){
			_headLookBehavior.SetLookTarget(Nurse);
		}

		if(Input.GetKeyUp (KeyCode.S)){
			_headLookBehavior.SetLookTarget(ScrubTech);
		}

		if(Input.GetKeyUp (KeyCode.H)){
			_headLookBehavior.SetLookTarget (this.gameObject);
		}

		if(Input.GetKeyUp (KeyCode.J)){
			_headLookBehavior.SetLookTarget (GameObject.Find ("Surgeon"));
		}

		if(Input.GetKeyUp(KeyCode.P)){
			_headLookBehavior.SetLookTarget (GameObject.Find ("Patient"));
		}
		if(Input.GetKeyUp (KeyCode.A)){
			_headLookBehavior.SetLookTarget(GameObject.Find ("Anesthesiologist"));
		}

		Vector3 head = transform.localPosition;
		KinectV2.Body[] data = _BodyManager.GetData();
		ulong reflectiveTrackingId = _ReflectiveMarkerManager.GetTrackedId();
		if(reflectiveTrackingId != 0)
		{
			this.nurseTrackingId = reflectiveTrackingId;
		}

		ulong lookWhosTalkingNow = _AudioBodyManager.GetData ();
		if (data == null)
		{
			return;
		}
		int bodyCount = 0;
		foreach(var body in data) {
			if(body != null)
			{		
				if(body.IsTracked)
				{
					if(bodyCount == 2)
						break;

					bodyCount++;

					KinectV2.JointType jt = KinectV2.JointType.Neck;
					
					KinectV2.Joint? targetJoint = null;
					targetJoint = body.Joints[KinectV2.JointType.Head];
				
					if(body.TrackingId == lookWhosTalkingNow)
					{
						this.gameObject.transform.localPosition = GetVector3FromJoint(targetJoint.Value);
					}

					if(body.TrackingId == this.nurseTrackingId)
					{
						Nurse.transform.localPosition = GetVector3FromJoint(targetJoint.Value);
						continue;
					} 
					if(body.TrackingId == this.scrubTechTrackingId)
					{
						ScrubTech.transform.localPosition = GetVector3FromJoint(targetJoint.Value);
						continue;
					}
					if(TrackScrubTech) {
						if(body.TrackingId != this.scrubTechTrackingId)
						{
							this.scrubTechTrackingId = body.TrackingId;
							ScrubTech.transform.localPosition = GetVector3FromJoint(targetJoint.Value);
							continue;
						}
					}
					if(TrackNurse) {
						if(body.TrackingId != this.nurseTrackingId)
						{
							this.nurseTrackingId = body.TrackingId;
							Nurse.transform.localPosition = GetVector3FromJoint(targetJoint.Value);
							continue;
						}
					}

				}
			}
		}
		
	}
	
	private Vector3 GetVector3FromJoint(KinectV2.Joint joint)
	{		
		return new Vector3(joint.Position.X*xScale,joint.Position.Y*yScale, -5f);
	}
}
