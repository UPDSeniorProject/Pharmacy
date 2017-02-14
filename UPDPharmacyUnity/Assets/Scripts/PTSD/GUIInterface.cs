using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIInterface : MonoBehaviour
{
    public string text = "Unity";
    public bool isTextSet = false;

    public void Start()
    {
        //this is here because I was too lazy to create another class
        Camera.main.GetComponent<Animation>().clip.SampleAnimation(Camera.main.gameObject, 0);
    }


}