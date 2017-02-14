using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BendingSegment
{
    public Transform FirstTransform;
    public Transform LastTransform;
    public float ThresholdAngleDifference = 0;
    public float BendingMultiplier = 0.8f;
    public float MaxAngleDifference = 30;
    public float MaxBendingAngle = 80;
    public float Responsiveness = 0.6f;
    internal float AngleH;
    internal float AngleV;
    internal Vector3 DirUp;
    internal Vector3 ReferenceLookDir;
    internal Vector3 ReferenceUpDir;
    internal int ChainLength;
    internal Quaternion[] OrigRotations;
}

[System.Serializable]
public class UnaffectedJoint
{
    public Transform joint;
    public float effect = 0;
}

public class HeadLookBehaviour : RenBehaviour {
    public Transform RootNode;
    public BendingSegment[] Segments;
    public UnaffectedJoint[] UnaffectedJoints;
    
    public Vector3 HeadLookVector = Vector3.forward;
    public Vector3 HeadUpVector = Vector3.up;
	
	protected Vector3 TargetPosition = Vector3.zero;
	protected GameObject Target = null;	
	public GameObject DefaultLookTarget;
	public List<GameObject> LookTargets = new List<GameObject>();

	// This stores the look target the character should primarily be looking at. 
	// This comes into play used when the character glances away at something else.
	protected GameObject CurrentMainLookTarget;
	public bool DoIdleEyegaze = false;
	public float GlanceDuration = 0.5f;
	public float WanderMinDuration = 1.5f;
	public float WanderMaxDuration = 3.5f;
	public float OnTargetMinDuration = 6.0f;
	public float OnTargetMaxDuration = 16.0f;

    public float Effect = 1;
    public bool OverrideAnimation = false;

	public CranialNerveModel NerveModel;

    public void SetLookTarget(GameObject t)
    {
        Target = t;
		CurrentMainLookTarget = Target;

		if (NerveModel != false)
		{
            Debug.Log("Nerve Model is exists");
            MouseEyeControl[] eyes = NerveModel.GetComponentsInChildren<MouseEyeControl>();
			foreach (MouseEyeControl eye in eyes)
			{
				eye.FollowObject(t);
			}
		}else
        {
            Debug.Log("Nerve Model is false");
        }
    }

    protected override void Start()
    {
        base.Start();
        //Set the transform of the Virtual Human
        if (RootNode == null)
        {
            RootNode = transform;
        }

        //Setup segments
        foreach (BendingSegment segment in Segments)
        {
            Quaternion parentRot = segment.FirstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
            segment.ReferenceLookDir =
                parentRotInv * RootNode.rotation * HeadLookVector.normalized;
            segment.ReferenceUpDir =
                parentRotInv * RootNode.rotation * HeadUpVector.normalized;
            segment.AngleH = 0;
            segment.AngleV = 0;
            segment.DirUp = segment.ReferenceUpDir;

            segment.ChainLength = 1;
            Transform t = segment.LastTransform;
            while (t != segment.FirstTransform && t != t.root)
            {
                segment.ChainLength++;
                t = t.parent;
            }

            segment.OrigRotations = new Quaternion[segment.ChainLength];
            t = segment.LastTransform;
            for (int i = segment.ChainLength - 1; i >= 0; i--)
            {
                segment.OrigRotations[i] = t.localRotation;
                t = t.parent;
            }
        }
		
		if (DefaultLookTarget != null)
		{
			SetLookTarget(DefaultLookTarget);	
		}
		
		if (DoIdleEyegaze && LookTargets.Count > 0)
		{
			StartCoroutine(RandomLook());	
		}
    }

    protected override void Update()
    {
        base.Update();
        if (Target != null)
        {//this can get done even if paused, no big deal.
            TargetPosition = Target.transform.position;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (isPaused)
            return; //Do nothing

        if (Time.deltaTime == 0)
            return; //Does this ever happen?

        // Remember initial directions of joints that should not be affected
        Vector3[] jointDirections = new Vector3[UnaffectedJoints.Length];
        for (int i = 0; i < UnaffectedJoints.Length; i++)
        {
            foreach (Transform child in UnaffectedJoints[i].joint)
            {
                jointDirections[i] = child.position - UnaffectedJoints[i].joint.position;
                break;
            }
        }

        // Handle each segment
        foreach (BendingSegment segment in Segments)
        {
            Transform t = segment.LastTransform;
            if (OverrideAnimation)
            {
                for (int i = segment.ChainLength - 1; i >= 0; i--)
                {
                    t.localRotation = segment.OrigRotations[i];
                    t = t.parent;
                }
            }

            Quaternion parentRot = segment.FirstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);

            // Desired look direction in world space
            Vector3 lookDirWorld = (TargetPosition - segment.LastTransform.position).normalized;

            // Desired look directions in neck parent space
            Vector3 lookDirGoal = (parentRotInv * lookDirWorld);

            // Get the horizontal and vertical rotation angle to look at the target
            float hAngle = AngleAroundAxis(
                segment.ReferenceLookDir, lookDirGoal, segment.ReferenceUpDir
            );

            Vector3 rightOfTarget = Vector3.Cross(segment.ReferenceUpDir, lookDirGoal);

            Vector3 lookDirGoalinHPlane =
                lookDirGoal - Vector3.Project(lookDirGoal, segment.ReferenceUpDir);

            float vAngle = AngleAroundAxis(
                lookDirGoalinHPlane, lookDirGoal, rightOfTarget
            );

            // Handle threshold angle difference, bending multiplier,
            // and max angle difference here
            float hAngleThr = Mathf.Max(
                0, Mathf.Abs(hAngle) - segment.ThresholdAngleDifference
            ) * Mathf.Sign(hAngle);

            float vAngleThr = Mathf.Max(
                0, Mathf.Abs(vAngle) - segment.ThresholdAngleDifference
            ) * Mathf.Sign(vAngle);

            hAngle = Mathf.Max(
                Mathf.Abs(hAngleThr) * Mathf.Abs(segment.BendingMultiplier),
                Mathf.Abs(hAngle) - segment.MaxAngleDifference
            ) * Mathf.Sign(hAngle) * Mathf.Sign(segment.BendingMultiplier);

            vAngle = Mathf.Max(
                Mathf.Abs(vAngleThr) * Mathf.Abs(segment.BendingMultiplier),
                Mathf.Abs(vAngle) - segment.MaxAngleDifference
            ) * Mathf.Sign(vAngle) * Mathf.Sign(segment.BendingMultiplier);

            // Handle max bending angle here
            hAngle = Mathf.Clamp(hAngle, -segment.MaxBendingAngle, segment.MaxBendingAngle);
            vAngle = Mathf.Clamp(vAngle, -segment.MaxBendingAngle, segment.MaxBendingAngle);

            Vector3 referenceRightDir =
                Vector3.Cross(segment.ReferenceUpDir, segment.ReferenceLookDir);

            // Lerp angles
            segment.AngleH = Mathf.Lerp(
                segment.AngleH, hAngle, Time.deltaTime * segment.Responsiveness
            );
            segment.AngleV = Mathf.Lerp(
                segment.AngleV, vAngle, Time.deltaTime * segment.Responsiveness
            );

            // Get direction
            lookDirGoal = Quaternion.AngleAxis(segment.AngleH, segment.ReferenceUpDir)
                * Quaternion.AngleAxis(segment.AngleV, referenceRightDir)
                * segment.ReferenceLookDir;

            // Make look and up perpendicular
            Vector3 upDirGoal = segment.ReferenceUpDir;
            Vector3.OrthoNormalize(ref lookDirGoal, ref upDirGoal);

            // Interpolated look and up directions in neck parent space
            Vector3 lookDir = lookDirGoal;
            segment.DirUp = Vector3.Slerp(segment.DirUp, upDirGoal, Time.deltaTime * 5);
            Vector3.OrthoNormalize(ref lookDir, ref segment.DirUp);

            // Look rotation in world space
            Quaternion lookRot = (
                (parentRot * Quaternion.LookRotation(lookDir, segment.DirUp))
                * Quaternion.Inverse(
                    parentRot * Quaternion.LookRotation(
                        segment.ReferenceLookDir, segment.ReferenceUpDir
                    )
                )
            );

            // Distribute rotation over all joints in segment
            Quaternion dividedRotation =
                Quaternion.Slerp(Quaternion.identity, lookRot, Effect / segment.ChainLength);
            t = segment.LastTransform;
            for (int i = 0; i < segment.ChainLength; i++)
            {
                t.rotation = dividedRotation * t.rotation;
                t = t.parent;
            }
        }

        // Handle unaffected joints
        for (int i = 0; i < UnaffectedJoints.Length; i++)
        {
            Vector3 newJointDirection = Vector3.zero;

            foreach (Transform child in UnaffectedJoints[i].joint)
            {
                newJointDirection = child.position - UnaffectedJoints[i].joint.position;
                break;
            }

            Vector3 combinedJointDirection = Vector3.Slerp(
                jointDirections[i], newJointDirection, UnaffectedJoints[i].effect
            );

            UnaffectedJoints[i].joint.rotation = Quaternion.FromToRotation(
                newJointDirection, combinedJointDirection
            ) * UnaffectedJoints[i].joint.rotation;
        }
    }


    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);

        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
	
	public IEnumerator RandomLook()
	{
		if (DoIdleEyegaze && LookTargets.Count > 0)
		{
			Debug.Log("Doing idle eyegaze");
			// Wait
			yield return new WaitForSeconds(Random.value * (OnTargetMaxDuration - OnTargetMinDuration) + OnTargetMinDuration);

			// Select a look target
			GameObject otherLookTarget = LookTargets[Random.Range(0, LookTargets.Count)];
			
			// Look at it for a random duration (short or long)
			float selector = Random.value;
			// Glance
			if (selector < 0.6)
			{
				Debug.Log("Glancing at " + otherLookTarget.ToString());
				Target = otherLookTarget;		
				yield return new WaitForSeconds(GlanceDuration);
			}
			// Longer look
			else
			{
				Debug.Log("Gazing at " + otherLookTarget.ToString());
				Target = otherLookTarget;
				yield return new WaitForSeconds(Random.value * (WanderMaxDuration - WanderMinDuration) + WanderMinDuration);
			}
			
			// Look back at main look target
			Target = CurrentMainLookTarget;
			
			// Start the whole thing again
			StartCoroutine(RandomLook());
		}
	}
}
