using UnityEngine;
using System.Collections;

public class Flashing : MonoBehaviour {
    Vector3 initialPos;
   
    IEnumerator FlashingItem() {
        while (true) { 
            GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Diffuse");
            yield return new WaitForSeconds(0.5f);
            GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
            yield return new WaitForSeconds(0.5f);
        }
    }
	// Use this for initialization
    
	void Start () {
        StartCoroutine("FlashingItem");
        initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	    if(transform.position != initialPos)
            StopCoroutine("FlashingItem");

	}
}
