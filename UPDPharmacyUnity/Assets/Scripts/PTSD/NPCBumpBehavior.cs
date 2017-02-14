using UnityEngine;
using System.Collections;

public class NPCBumpBehavior : MonoBehaviour
{
    Transform[] waypoint;            // The amount of Waypoint you want
    float patrolSpeed = 3;        // The walking speed between Waypoints
    bool loop = true;            // Do you want to keep repeating the Waypoints
    float dampingLook = 6.0f;                // How slowly to turn
    float pauseDuration = 1;        // How long to pause at a Waypoint

    public AudioClip CartCrash;
    public AudioClip SpokenResponse;
    public AudioClip WalkAway;
    private AudioClip SavedClip;

    public AudioClip SpokenResponsePolite;
    public AudioClip WalkAwayPolite;
    public AudioClip SpokenResponseRude;
    public AudioClip WalkAwayRude;

    private float curTime;
    private CharacterController character;
    private GameObject player;
    private bool stopMovement = true;
    private bool reverseWalk = false;
    private bool walkAwayState = false;
    private bool finalState = false;

    private float reverseDistance = 0f;
    private GameObject player2;
    private Movement1 playerMovement;
    private Quaternion originalCameraRotation;
    private GameObject lookTarget;

    private bool hasPlayed = false;

    private UnityEngine.AI.NavMeshAgent agent;
    public Transform ShoppingCart;
    private float pauseTime;

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
        if (gameObject != coll.gameObject)
        {
            if (coll.gameObject.name == "front" || coll.gameObject.name == "left" || coll.gameObject.name == "right" || coll.gameObject.name == "back" || coll.gameObject.name == "Robot_Prefab" || coll.gameObject.name == "shoppingCart")
            {
                if (stopMovement)
                {
                    stopMovement = false;
                    reverseWalk = true;
                    GameObject camera = GameObject.Find("Main Camera");
                    originalCameraRotation = camera.transform.rotation;
                    GetComponent<AudioSource>().loop = false;
                    GetComponent<AudioSource>().Stop();

                    GetComponent<AudioSource>().clip = CartCrash;
                    GetComponent<AudioSource>().Play();

                    if(InputManager.BumpEncounter.EncounterType != EncounterTypes.Conversation) 
                        StartCoroutine(SpeakResponse());
                    
                    iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "time", 2f, "islocal", true, "easetype", iTween.EaseType.easeInOutSine));
                    iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("position", new Vector3(1f, 0f, -1f), "time", 2f,
                                                                       "islocal", true, "orienttopath", false, "easetype", iTween.EaseType.linear));

                    pauseTime = 0;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {

    }

    // Use this for initialization
    void Start()
    {
        character = GetComponent<CharacterController>();
        player = GameObject.Find("shoppingCartBumpTarget");//GameObject.Find ("Robot_Prefab");
        player2 = GameObject.Find("Robot_Prefab");
        playerMovement = player2.GetComponent<Movement1>();
        playerMovement.speed = 0f;
        playerMovement.rotationSpeed = 0f;
        lookTarget = GameObject.Find("NPCLookTarget");
        SavedClip = GetComponent<AudioSource>().clip;
       // agent = GetComponent<NavMeshAgent>();
       // agent.enabled = true;
       // agent.SetDestination(player.transform.position);
     //   agent.autoBraking = false;
        //angularSpeed = agent.angularSpeed;
        //Debug.Log("start bump behavior");

    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        pauseTime += Time.deltaTime;
        if (stopMovement)
        {
            KeepWalking();
        }
        if (reverseWalk && pauseTime > .5f)
        {
            NextStep();
        }
        if (walkAwayState && curTime < 3)
        {
            ResetCamera();
        }
        else if (walkAwayState && curTime > 3)
        {
            walkAwayState = false;
            finalState = true;
        }
        if (finalState)
        {
            finalState = false;
            CleanUp();
            //Debug.Log("final");
           
        }
    }

    void KeepWalking()
    {
        Vector3 target = player.transform.position;
        target.y = 0f;
        Vector3 moveDirection = target - transform.position;

        Quaternion rotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampingLook);
        character.Move(moveDirection.normalized * patrolSpeed * Time.deltaTime);
    }

    void NextStep()
    {
        //float totalMagnitude = 0f;
        if (reverseDistance < 5f)
        {
            Vector3 target = player.transform.position;
            target.y = 0f;
            //target.y = transform.position.y; // Keep waypoint at character's height
            Vector3 moveDirection = target - transform.position;
            Quaternion rotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampingLook);
            character.Move(-1 * moveDirection.normalized * patrolSpeed * Time.deltaTime);
            reverseDistance += moveDirection.magnitude;
            //Debug.Log(totalMagnitude);

            GameObject camera = GameObject.Find("Main Camera");
            Quaternion rotation2 = camera.transform.rotation;

            Quaternion targetRotation = Quaternion.LookRotation(lookTarget.transform.position - player2.transform.position);//, Vector3.up);//Quaternion.LookRotation(transform.position - player.transform.position, Vector3.up);
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, Quaternion.Euler(new Vector3(0f, targetRotation.eulerAngles.y, 0f)), Time.deltaTime * 10);
           // Debug.Log(targetRotation.eulerAngles.y);
        }
        else
        {
            iTween.RotateTo(ShoppingCart.gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 0f), "time", 2f, "islocal", true, "easetype", iTween.EaseType.easeInOutSine));
            iTween.MoveTo(ShoppingCart.gameObject, iTween.Hash("position", new Vector3(0f, 0f, 0f), "time", 2f,
                                                               "islocal", true, "orienttopath", false, "easetype", iTween.EaseType.linear));
            GetComponentInChildren<Animation>().Stop();
            //GetComponent<Animation>().Stop();
            if(InputManager.BumpEncounter.EncounterType == EncounterTypes.Polite || InputManager.BumpEncounter.EncounterType == EncounterTypes.Rude)
                StartCoroutine(SimulateTalking());
        }
    }

    void ResetCamera()
    {
        GameObject camera = GameObject.Find("Main Camera");
        Quaternion rotation2 = camera.transform.rotation;
        Quaternion targetRotation = originalCameraRotation;//Quaternion.LookRotation(player.transform.forward, Vector3.up);
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, Time.deltaTime);//, Quaternion.Euler(new Vector3(0f, targetRotation.eulerAngles.y, 0f)), Time.deltaTime);   
    }

    void CleanUp()
    {
        GetComponentInChildren<Animation>().Play();
        //GetComponent<AudioSource>().clip = WalkAway;
        if (InputManager.BumpEncounter.EncounterType == EncounterTypes.Polite)
            GetComponent<AudioSource>().clip = WalkAwayPolite;
        else if (InputManager.BumpEncounter.EncounterType == EncounterTypes.Rude)
            GetComponent<AudioSource>().clip = WalkAwayRude;

        GetComponent<AudioSource>().Play();
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.rotation = originalCameraRotation;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = true;
        agent.SetDestination(player.transform.position);
        agent.autoBraking = false;
        GetComponent<PatrolScript>().enabled = true;
        GetComponent<PatrolScript>().disappear = true;

        StartCoroutine(RestartSound(GetComponent<AudioSource>().clip.length + 1f));
        //enabled = false;
    }

    IEnumerator SpeakResponse()
    {
        yield return new WaitForSeconds(1f);

        if(InputManager.BumpEncounter.EncounterType == EncounterTypes.Polite)
            GetComponent<AudioSource>().clip = SpokenResponsePolite;
        else if(InputManager.BumpEncounter.EncounterType == EncounterTypes.Rude)
            GetComponent<AudioSource>().clip = SpokenResponseRude;

        GetComponent<AudioSource>().Play();
    }

    IEnumerator RestartSound(float wait)
    {
        yield return new WaitForSeconds(wait);
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().clip = SavedClip;
        GetComponent<AudioSource>().Play();
        enabled = false;
    }

    IEnumerator SimulateTalking()
    {
        yield return new WaitForSeconds(2);
        reverseWalk = false;
        walkAwayState = true;
        playerMovement.speed = 5f;
        playerMovement.rotationSpeed = 50f;
        curTime = 0f;
    }

    void ReturnControl()
    {
        reverseWalk = false;
        walkAwayState = true;
        playerMovement.speed = 5f;
        playerMovement.rotationSpeed = 50f;
        curTime = 0f;
    }
}






