using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCAudioController : MonoBehaviour {

    private AudioSource AudioSrc;
    private float timeSinceRepeat = 0f;
    private GameObject player;

    public AudioClip[] AudioClips;
    private List<AudioClip> currentAudioClips = new List<AudioClip>();
    private int currentAudioIndex = 0;
    private Movement1 playerMovement;
    private bool crossThreshold = false;

	// Use this for initialization
	void Start () {
        AudioSrc = GetComponent<AudioSource>();
        player = GameObject.Find("Robot_Prefab");
        playerMovement = player.GetComponent<Movement1>();
        timeSinceRepeat = 20f;

        if (InputManager.ShopperConversations.Count > 0 && InputManager.ShopperConversations[0] != ShopperConversationsTypes.None)
        {
            foreach (ShopperConversationsTypes convo in InputManager.ShopperConversations)
            {
                for (int i = 0; i < AudioClips.Length; i++)
                {
                    if (AudioClips[i].name == convo.ToString())
                    {
                        currentAudioClips.Add(AudioClips[i]);
                        break;
                    }
                }
            }
            FisherYates();
        }
	}

    void FisherYates()
    {
        for (int n = currentAudioClips.Count - 1; n > 0; n--)
        {
            int k = Random.Range(0, n + 1);
            AudioClip temp = currentAudioClips[n];
            currentAudioClips[n] = currentAudioClips[k];
            currentAudioClips[k] = temp;
        }
    }

	// Update is called once per frame
	void Update () {

        if (!AudioSrc.isPlaying)
        {
            timeSinceRepeat += 1 * Time.deltaTime;
        }
        
        Vector3 distanceToPlayer = player.transform.position - transform.position;
        //Debug.Log(distanceToPlayer.magnitude);
        if (timeSinceRepeat > 20f && distanceToPlayer.magnitude < 7f && currentAudioClips.Count > 0)
        {
            AudioSrc.clip = currentAudioClips[currentAudioIndex];
            AudioSrc.Play();
            timeSinceRepeat = 0f;
            currentAudioIndex++;
            if (currentAudioIndex == currentAudioClips.Count)
            {
                currentAudioIndex = 0;
                FisherYates();
            }
        }
        if (!crossThreshold)
        {
            if (distanceToPlayer.magnitude <= AudioSrc.maxDistance - .5f)
            {
                crossThreshold = true;
                playerMovement.speed = 2f;
                playerMovement.rotationSpeed = 20f;
            }
        }
        if (crossThreshold)
        {
            if (distanceToPlayer.magnitude > AudioSrc.maxDistance - .5f)
            {
                crossThreshold = false;
                playerMovement.speed = 5f;
                playerMovement.rotationSpeed = 50f;
            }
        }
	}
}
