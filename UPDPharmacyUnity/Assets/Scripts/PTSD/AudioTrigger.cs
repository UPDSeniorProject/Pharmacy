using UnityEngine;
using System.Collections;

public class AudioTrigger : MonoBehaviour
{
    private float timeSinceLastAudio = 0;
    private bool firstTimePlay = true;

    void OnTriggerEnter (Collider other) {
        //for audio entering store
        if(gameObject.name.Equals("Audio") && other.name == "Robot_Prefab")
        {
            GetComponent<AudioSource>().Play();
            AudioSource[] list = GetComponentsInChildren<AudioSource>();
            foreach(AudioSource a in list)
            {
                if (!a.isPlaying)
                    a.Play();
            }
        }
        //for audio on the people in store
		else if (gameObject.name.Equals("AudioTrigger") && other.name == "Robot_Prefab" && (timeSinceLastAudio > 15 || firstTimePlay)) {
            gameObject.transform.parent.GetComponentInChildren<AudioSource>().Play();
            timeSinceLastAudio = 0;
            firstTimePlay = false;
		}
	}

    void Update()
    {
        timeSinceLastAudio += 1 * Time.deltaTime;
    }
}

