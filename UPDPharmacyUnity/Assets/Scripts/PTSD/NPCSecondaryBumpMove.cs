using UnityEngine;
using System.Collections;

public class NPCSecondaryBumpMove : MonoBehaviour {

    public Transform movePoint;

    private Quaternion originalRotation;
    private bool moveToPoint = true, resetCharacter = false;
	// Use this for initialization
	void Start () {
        originalRotation = transform.rotation;
        GetComponent<PTSDVHAnimationManager>().StopAllCoroutines();
        GetComponent<PTSDVHAnimationManager>().enabled = false;
        GetComponent<Animation>().Stop();
        GetComponent<Animation>().Play("Walk");
	}
	
	// Update is called once per frame
	void Update () {

        if (moveToPoint)
        {
            Vector3 moveDirection = movePoint.transform.position - transform.position;
            Quaternion rotation2 = Quaternion.LookRotation(movePoint.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation2, Time.deltaTime * 2f);
            GetComponent<CharacterController>().Move(moveDirection.normalized * 1 * Time.deltaTime);
            //Debug.Log("move: " + moveDirection.magnitude);
            if (moveDirection.magnitude < 1)
            {
              //  Debug.Log("change");
                resetCharacter = true;
                moveToPoint = false;
            }
        }

        if (resetCharacter)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * 2f);
            //Debug.Log("rot: " + (Quaternion.Inverse(transform.rotation) * originalRotation).eulerAngles.magnitude);
            //Debug.Log("roty: " + (Quaternion.Inverse(transform.rotation) * originalRotation).eulerAngles.y);
            float yValue = (Quaternion.Inverse(transform.rotation) * originalRotation).eulerAngles.y;
            yValue = yValue % 360;
            if (yValue < 5 || (yValue > 355 && yValue <= 360))
            {
              //  Debug.Log("done");
                GetComponent<Animation>().Stop();
                Animation anim = GetComponent<Animation>();
                int clipNum = Random.Range(0, anim.GetClipCount() - 1);
                int i = 0;
                foreach (AnimationState state in anim)
                {
                    if (i == clipNum)
                    {
                        anim.Play(state.name);
                        break;
                    }
                    i++;
                }
                    
                GetComponent<PTSDVHAnimationManager>().enabled = true;
                //GetComponent<PTSDVHAnimationManager>().StartCoroutine(GetComponent<PTSDVHAnimationManager>().PlayIdleAnimationTesting(new System.Collections.Generic.List<AnimationClip>(), 0f));
                GetComponent<PTSDVHAnimationManager>().ResumeIdleAnimations();
                enabled = false;
            }
        }

	}
}
