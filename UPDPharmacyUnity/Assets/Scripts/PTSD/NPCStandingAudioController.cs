using UnityEngine;
using System.Collections;

public class NPCStandingAudioController : MonoBehaviour {

    private AudioSource AudioSrc;
    private float timeSinceRepeat = 0f;
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        AudioSrc = GetComponent<AudioSource>();
        player = GameObject.Find("Robot_Prefab");
        timeSinceRepeat = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceRepeat += 1 * Time.deltaTime;
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        if (timeSinceRepeat > 20f && distanceToPlayer.magnitude < AudioSrc.maxDistance)
        {
            AudioSrc.Play();
            timeSinceRepeat = 0f;
        }
    }
}
