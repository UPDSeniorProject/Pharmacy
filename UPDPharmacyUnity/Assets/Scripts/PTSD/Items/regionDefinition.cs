using UnityEngine;
using System.Collections;

public class regionDefinition : MonoBehaviour
{

    public Transform aisleStart;
    public Transform aisleEnd;
    public int itemRegion;
    public int aisle;

	// Use this for initialization
    void Awake()
    {

        //Asile_back_1_left

        string[] selfname = name.Split('_');
        if (aisleStart == null)
            aisleStart = GameObject.Find("selectionPoint_" + selfname[1] + "_" + selfname[2] + "_" + "start").transform;
         if (aisleEnd == null)
             aisleEnd = GameObject.Find("selectionPoint_" + selfname[1] + "_" + selfname[2] + "_" + "end").transform;

         if (selfname[1].Equals("front"))
             itemRegion = 2;
         else if (selfname[1].Equals("back"))
             itemRegion = 3;

         aisle = int.Parse(selfname[2]);  


    }

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
