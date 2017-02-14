using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KinectV2 = Windows.Kinect;

public class OffAxisProjection_V2 : MonoBehaviour {
	public Transform[] Corners;
	public Transform Television;
	public bool drawNearCone, drawFrustum;
	public GameObject KinectV2Manager;
	private BodySourceManager _BodyManager;
	private Vector3 lastPosition;
	public bool useKinect;
	Camera theCam;

	void Start () {
		theCam = GetComponent<Camera>();
	}
	
	
	void Update () {
		Vector3 head = theCam.transform.localPosition;
		if(useKinect) {
			head = GetHeadPositionFromKinect();
			theCam.transform.localPosition = head;
		} else {
			if (Input.GetMouseButtonDown(0))
			{
				lastPosition = Input.mousePosition;
			}
			
			if (Input.GetMouseButton(0))
			{
				Vector3 delta = Input.mousePosition - lastPosition;
				transform.Translate(delta.x * 0.01f, delta.y * 0.01f, 0);
				lastPosition = Input.mousePosition;
			}
		}


		Vector3 pa, pb, pc, pd;
		pa = Corners[0].localPosition;
		pb = Corners[1].localPosition;
		pc = Corners[2].localPosition;
		pd = Corners[3].localPosition;
		
		Vector3 pe = head;
			
		Vector3 vr = ( pb - pa ).normalized; // right axis of screen
		Vector3 vu = ( pc - pa ).normalized; // up axis of screen
		Vector3 vn = -Vector3.Cross(vr, vu).normalized; // normal vector of screen
		
		Vector3 va = pa - pe; // from pe to pa
		Vector3 vb = pb - pe; // from pe to pb
		Vector3 vc = pc - pe; // from pe to pc
		Vector3 vd = pd - pe; // from pe to pd

		float near = -Television.InverseTransformPoint(theCam.transform.localPosition).z; // distance to the near clip plane (screen)
		float far = theCam.farClipPlane; // distance of far clipping plane
		float d = -Vector3.Dot(va, vn); // distance from eye to screen	
			
		float left = Vector3.Dot(vr, va) * near / d; // distance to left screen edge from the 'center'
		float right = Vector3.Dot(vr, vb) * near / d; // distance to right screen edge from 'center'
		float bottom = Vector3.Dot(vu, va) * near / d; // distance to bottom screen edge from 'center'
		float top = Vector3.Dot(vu, vc) * near / d; // distance to top screen edge from 'center'

		Matrix4x4 projectionMatrix = GeneralizedPerspectiveProjection(left,right,bottom,top,near,far);

		/*Matrix4x4 transformMatrix = new Matrix4x4();
		transformMatrix[0, 0] = vr.x;
		transformMatrix[0, 1] = vr.y;
		transformMatrix[0, 2] = vr.z;
		transformMatrix[0, 3] = 0;
		transformMatrix[1, 0] = vu.x;
		transformMatrix[1, 1] = vu.y;
		transformMatrix[1, 2] = vu.z;
		transformMatrix[1, 3] = 0;
		transformMatrix[2, 0] = vn.x;
		transformMatrix[2, 1] = vn.y;
		transformMatrix[2, 2] = vn.z;
		transformMatrix[2, 3] = 0;
		transformMatrix[3, 0] = 0;
		transformMatrix[3, 1] = 0;
		transformMatrix[3, 2] = 0;
		transformMatrix[3, 3] = 1;
		
		//Now for the eye transform
		Matrix4x4 eyeTranslateM = new Matrix4x4();
		eyeTranslateM[0, 0] = 1;
		eyeTranslateM[0, 1] = 0;
		eyeTranslateM[0, 2] = 0;
		eyeTranslateM[0, 3] = -pe.x;
		eyeTranslateM[1, 0] = 0;
		eyeTranslateM[1, 1] = 1;
		eyeTranslateM[1, 2] = 0;
		eyeTranslateM[1, 3] = -pe.y;
		eyeTranslateM[2, 0] = 0;
		eyeTranslateM[2, 1] = 0;
		eyeTranslateM[2, 2] = 1;
		eyeTranslateM[2, 3] = -pe.z;
		eyeTranslateM[3, 0] = 0;
		eyeTranslateM[3, 1] = 0;
		eyeTranslateM[3, 2] = 0;
		eyeTranslateM[3, 3] = 1f;*/
		
		theCam.projectionMatrix = projectionMatrix; // Assign matrix to camera
		if (drawNearCone) { //Draw lines from the camera to the corners f the screen
			Debug.DrawRay(theCam.transform.position, va, Color.blue);
			Debug.DrawRay(theCam.transform.position, vb, Color.blue);
			Debug.DrawRay(theCam.transform.position, vc, Color.blue);
			Debug.DrawRay(theCam.transform.position, vd, Color.blue);
		}
		
		if (drawFrustum) DrawFrustum(theCam); //Draw actual camera frustum
		
	}

	Matrix4x4 GeneralizedPerspectiveProjection(float left, float right, float bottom, float top, float near, float far)
	{
		Matrix4x4 projectionMatrix = new Matrix4x4();
		projectionMatrix[0, 0] = 2.0f * near / ( right - left );
		projectionMatrix[0, 2] = ( right + left ) / ( right - left );
		projectionMatrix[1, 1] = 2.0f * near / ( top - bottom );
		projectionMatrix[1, 2] = ( top + bottom ) / ( top - bottom );
		projectionMatrix[2, 2] = -( far + near ) / ( far - near );
		projectionMatrix[2, 3] = -(2.0f * far * near) / ( far - near );
		projectionMatrix[3, 2] = -1.0f;

		return projectionMatrix;
	}
	
	Vector3 ThreePlaneIntersection(Plane p1, Plane p2, Plane p3) { //get the intersection point of 3 planes
		return ( ( -p1.distance * Vector3.Cross(p2.normal, p3.normal) ) +
		        ( -p2.distance * Vector3.Cross(p3.normal, p1.normal) ) +
		        ( -p3.distance * Vector3.Cross(p1.normal, p2.normal) ) ) /
			( Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)) );
	}
	
	void DrawFrustum (Camera cam) {
		Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
		Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
		Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
		Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop
		
		for (int i = 0; i < 4; i++) {
			nearCorners[i] = ThreePlaneIntersection(camPlanes[4], camPlanes[i], camPlanes[( i + 1 ) % 4]); //near corners on the created projection matrix
			farCorners[i] = ThreePlaneIntersection(camPlanes[5], camPlanes[i], camPlanes[( i + 1 ) % 4]); //far corners on the created projection matrix
		}
		
		for (int i = 0; i < 4; i++) {
			Debug.DrawLine(nearCorners[i], nearCorners[( i + 1 ) % 4], Color.red, Time.deltaTime, false); //near corners on the created projection matrix
			Debug.DrawLine(farCorners[i], farCorners[( i + 1 ) % 4], Color.red, Time.deltaTime, false); //far corners on the created projection matrix
			Debug.DrawLine(nearCorners[i], farCorners[i], Color.red, Time.deltaTime, false); //sides of the created projection matrix
		}
	}
	private static Vector3 GetVector3FromJoint(KinectV2.Joint joint)
	{
		//return new Vector3(Mathf.Round (joint.Position.X*10*1000f) / 1000f, Mathf.Round(joint.Position.Y*10*1000f)/1000f, -Mathf.Round (joint.Position.Z*10*1000f)/1000f);
		return new Vector3(joint.Position.X*10.0f, joint.Position.Y*10.0f, -joint.Position.Z*10.0f);
	}

	private Vector3 GetHeadPositionFromKinect()
	{
		Vector3 head = theCam.transform.localPosition;
		if (KinectV2Manager == null)
		{		
			return head;
		}
		
		_BodyManager = KinectV2Manager.GetComponent<BodySourceManager>();
		if (_BodyManager == null)
		{
			return head;
		}


		KinectV2.Body[] data = _BodyManager.GetData();
		if (data == null)
		{
			return head;
		}
		foreach(var body in data) {
			if(body != null)
			{		
				if(body.IsTracked)
				{
					KinectV2.JointType jt = KinectV2.JointType.Neck;
					
					KinectV2.Joint? targetJoint = null;
					targetJoint = body.Joints[KinectV2.JointType.Head];
					head = GetVector3FromJoint(targetJoint.Value);
					return head;
				}
			}
		}

		return head;
	}
}
