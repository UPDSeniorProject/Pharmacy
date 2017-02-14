using UnityEngine;
using System.Collections;

public class NPCBumpResponse : MonoBehaviour {

    public Transform movePoint;

    public AudioClip OriginalAudio;
    public AudioClip SpokenResponse;
    public AudioClip WalkAway;

    public GameObject OtherCharacter;

    private CharacterController character;
    private GameObject player;
    private bool turnAndLook = false, moveOutOfWay = false, returnToOriginal = false, alreadyBumped = false;
    private Movement1 playerMovement;
    private Quaternion originalRotation;
    private BoxCollider boxColl;

    private bool hasPlayed = false;

    private float totalWait = 0f;
    //private Vector3 movePoint2;

    void OnEnable()
    {
        Messenger.AddListener("EndBumpEncounter", ReturnControl);
    }

    void OnDisable()
    {
        Messenger.RemoveListener("EndBumpEncounter", ReturnControl);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (gameObject != coll.gameObject && !alreadyBumped)
        {
            GetComponent<PTSDVHAnimationManager>().StopAllCoroutines();
            GetComponent<PTSDVHAnimationManager>().enabled = false;
            GetComponent<Animation>().Stop();
            GetComponent<Animation>().Play("Walk");
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().loop = false;
            GetComponent<AudioSource>().clip = SpokenResponse;
            GetComponent<AudioSource>().Play();
            playerMovement = player.GetComponent<Movement1>();
            playerMovement.speed = 0f;
            playerMovement.rotationSpeed = 0f;
            turnAndLook = true;
            alreadyBumped = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
       
    }

    // Use this for initialization
    void Start()
    {
        boxColl = GetComponent<BoxCollider>();
        character = GetComponent<CharacterController>();
        player = GameObject.Find("Robot_Prefab");
        originalRotation = transform.rotation;

       // movePoint2 = new Vector3(OtherCharacter.transform.position.x, movePoint.position.y, movePoint.position.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (turnAndLook)
        {
            TurnAndRespond();
        }
        if (moveOutOfWay)
        {
            MoveToPoint();
        }
        if (returnToOriginal)
        {
            ResetCharacters();
        }
    }

    void TurnAndRespond()
    {
        Vector3 target = player.transform.position;
        target.y = 0f;
        Vector3 moveDirection = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(target - transform.position);
            
        //Debug.Log((Quaternion.Inverse(transform.rotation) * rotation).eulerAngles.magnitude);
        //Debug.Log((Quaternion.Inverse(transform.rotation) * rotation).y);
        if ((Quaternion.Inverse(transform.rotation) * rotation).eulerAngles.magnitude < 5)
        {
           // Debug.Log("stop");
            GetComponent<Animation>().Stop();
            GetComponent<Animation>().Play("idle_stand");
            turnAndLook = false;
            StartCoroutine(SimulateTalking());
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 1f);
        }
    }

    void MoveToPoint()
    {
        Vector3 target = movePoint.position;
        target.y = 0f;
       
        Vector3 moveDirection = target - transform.position;
        Quaternion rotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
        character.Move(moveDirection.normalized * 1 * Time.deltaTime);

        //Debug.Log(moveDirection.magnitude);
        totalWait += Time.deltaTime;
        if (totalWait > 1f && !OtherCharacter.GetComponent<NPCSecondaryBumpMove>().isActiveAndEnabled)
        {
            OtherCharacter.GetComponent<NPCSecondaryBumpMove>().enabled = true;
        }
        if (moveDirection.magnitude < 1)
        {
            moveOutOfWay = false;
            returnToOriginal = true;
           
            //transform.rotation = originalRotation;// Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * 2);
        }
    }

    void ResetCharacters()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * 2f);
        if ((Quaternion.Inverse(transform.rotation) * originalRotation).eulerAngles.magnitude < 1)
        {
            //Debug.Log("done");
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
            GetComponent<PTSDVHAnimationManager>().ResumeIdleAnimations();
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().clip = OriginalAudio;
            GetComponent<AudioSource>().Play();
            enabled = false;
        }
    }

    IEnumerator SimulateTalking()
    {
        yield return new WaitForSeconds(2);
        moveOutOfWay = true;
        playerMovement.speed = 5f;
        playerMovement.rotationSpeed = 50f;
        boxColl.enabled = false;
        GetComponent<Animation>().Play("Walk");
    }

    void ReturnControl()
    {
        moveOutOfWay = false;
        playerMovement.speed = 5f;
        playerMovement.rotationSpeed = 50f;
    }
}
