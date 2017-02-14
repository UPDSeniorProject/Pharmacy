using UnityEngine;
using System.Collections;

public class PatrolScript : MonoBehaviour
{
    public Transform[] waypoint;        // The amount of Waypoint you want
    public bool disappear = false;
    private int currentWaypoint = 0;

    private float angularSpeed;

    private UnityEngine.AI.NavMeshAgent agent;

    private GameObject player;
    private bool isPlayerBumped = false;

    public AudioClip BumpResponse;
    //private AudioClip DefaultClip;
    private bool notPlayed = true;

    //public AudioClip WalkByResponse;
    //private bool notPlayedWalkBy = true;
    public AudioSource mAudio;
    public AudioClip[] Clips;
    private int currentClip = 0;

    private float timeSinceLastCollision = 0f, timeSinceLastWalkBy = 0f;
    private Random rand = new Random();

    private GameObject MainCamera;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (gameObject != other.gameObject && other.gameObject.name =="Robot_Prefab")
        {
            if (timeSinceLastCollision > 10f && GetComponent<Animation>() != null && BumpResponse != null)
            {
                Debug.Log("trigger");
                mAudio.Stop();
                //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                //GetComponent<Rigidbody>().isKinematic = true;
               // player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                agent.enabled = false;
                /*
                agent.destination = player.transform.position;
                //agent.destination = MainCamera.transform.position;
                agent.angularSpeed = 500f;
                agent.updatePosition = false;*/
                isPlayerBumped = true;
                timeSinceLastCollision = 0f;
                //ResetTimers();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject != other.gameObject)
        {
           
        }
    }

    /*IEnumerator WaitToWalk()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }*/

    void Start()
    {
        player = GameObject.Find("Robot_Prefab");
        MainCamera = GameObject.Find("Main Camera");

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.SetDestination(waypoint[currentWaypoint].position);
        agent.autoBraking = false;
        angularSpeed = agent.angularSpeed;

      //  Shuffle();
    }

    void Update()
    {
        timeSinceLastCollision += 1 * Time.deltaTime;
        timeSinceLastWalkBy += 1 * Time.deltaTime;

        if (!isPlayerBumped)
        {
            Vector3 moveDirection = agent.destination - transform.position;
            if (moveDirection.magnitude < 0.5f)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoint.Length)
                    currentWaypoint = 0;

                agent.SetDestination(waypoint[currentWaypoint].position);
            }
            Vector3 distanceToPlayer = player.transform.position - transform.position;

            if (timeSinceLastWalkBy > 20f && distanceToPlayer.magnitude < 4f && Clips.Length > 0)
            {
                mAudio.clip = Clips[currentClip];
                mAudio.Play();
                ResetTimers();
            }
        }
        else
        {
            var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
                   //Debug.Log ("rotation:" + rotation.x + " " + rotation.y + " " + rotation.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1.0f);
           // Debug.Log(Vector3.Angle(transform.forward, player.transform.position - transform.position));
         //   if (Vector3.Angle(transform.forward, player.transform.position - transform.position) < 10f && notPlayed)
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) < 10f && notPlayed)
            {
                GetComponent<Animation>().Stop();
                //agent.enabled = false;
                StartCoroutine(WaitToResume());
                mAudio.clip = BumpResponse;
                mAudio.Play();
                notPlayed = false;
            }
        }
    }

   /* IEnumerator ResumeWalkingNoise()
    {
        yield return new WaitForSeconds(.5f);
        if (mAudio.isPlaying)
            ResumeWalkingNoise();
        else
        {
            mAudio.clip = DefaultClip;
            mAudio.Play();
        }
    }*/

    IEnumerator WaitToResume()
    {
        yield return new WaitForSeconds(3f);
        agent.enabled = true;
        GetComponent<Animation>().Play();
        //mAudio.clip = DefaultClip;
        //mAudio.Play();
       // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        //GetComponent<Rigidbody>().isKinematic = false;
       // player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        agent.nextPosition = transform.position;
        agent.destination = waypoint[currentWaypoint].position;
        agent.angularSpeed = 120f;
        agent.updatePosition = true;
        isPlayerBumped = false;
        notPlayed = true;
        ResetTimers();
        timeSinceLastCollision = 0f;
    }

    void ResetTimers()
    {
        //timeSinceLastRange = 0f;
        //timeSinceLastCollision = 0f;
        //timeSinceLastWalkBy = 0f;
        timeSinceLastWalkBy = 0f;
                currentClip++;
                if (currentClip >= Clips.Length)
                {
                    currentClip = 0;
                    Shuffle();
                }
    }

    void Shuffle()
    {
        for (int n = Clips.Length - 1; n > 0; --n)
        {
            int k = Random.Range(0, Clips.Length - 1);
            AudioClip temp = Clips[n];
            Clips[n] = Clips[k];
            Clips[k] = temp;
        }
    }
}
