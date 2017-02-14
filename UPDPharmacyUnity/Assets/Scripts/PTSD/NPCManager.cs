using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public GameObject[] NPCs;
	public GameObject[] BumpLocations;
	public GameObject[] BumpNPCs;
    public GameObject[] SpecialNPCs;
    private List<GameObject> CurrentNPCs = new List<GameObject>();
    
	private BumpEncounter bumpCart = new BumpEncounter();
    private bool hasBumped = false;

    public int NPCOneAisleCount = 0;
    public int NPCTwoAisleCount = 0;
    public int startingNPCs = 0;
    private int currentActiveNPCs = 0;
    private int test = 0;

	private bool isfloating = false;
	private GameObject storeWalls, cart, girders, lights, signs, gui;
	private GameObject robot;

	private Vector3 position;
	private Quaternion rotation;

	private float speed = 1.5f;
	private GameObject player;

	void OnEnable()
	{
		Messenger<int>.AddListener("NPCChangeCount", OnNPCChange);
	}
	
	void OnDisable()
	{
		Messenger<int>.RemoveListener("NPCChangeCount", OnNPCChange);
	}

	void OnNPCChange(int change)
	{
		/*Messenger<int>.Broadcast("CurrentActiveNPCs", currentActiveNPCs);
		if (change == 1 && currentActiveNPCs < NPCs.Length) 
        { 
            //Debug.Log(NPCs[currentActiveNPCs].transform.childCount);
            NPCs[currentActiveNPCs].SetActive(true);
            if(NPCs[currentActiveNPCs].transform.childCount == 2)
            {
                foreach (Transform child in NPCs[currentActiveNPCs].transform)
                {
                    child.gameObject.SetActive(true);
                    if(child.gameObject.GetComponent<PTSDVHAnimationManager>() != null)
                    {
                        PTSDVHAnimationManager manager = child.gameObject.GetComponent<PTSDVHAnimationManager>();
                        manager.ResumeIdleAnimations();
                    }
                    //child.gameObject.animation.Play();
                }
            }
            else
                NPCs[currentActiveNPCs].GetComponent<Animation>().Play();
            currentActiveNPCs++;
        }
        else if (change == -1 && currentActiveNPCs > 0) 
        {
            currentActiveNPCs--;
            if(NPCs[currentActiveNPCs].transform.childCount == 2)
            {
                foreach (Transform child in NPCs[currentActiveNPCs].transform)
                {
                    child.gameObject.GetComponent<Animation>().Stop();
                    child.gameObject.SetActive(false);
                }
            }
            else
                NPCs[currentActiveNPCs].GetComponent<Animation>().Stop();

            NPCs[currentActiveNPCs].SetActive(false);
		}*/
	}

	// Use this for initialization
	void Start ()
	{
        bumpCart = InputManager.BumpEncounter;
       
        //Dictionary<string, string> currentSwaps = new Dictionary<string, string>();
        List<string> currentSwaps = new List<string>();
        foreach (string s in InputManager.SpecialNPCList)
        {
            if (s == "MilitaryNavyMale")
                currentSwaps.Add("customer_navyMaleAisleSearch");
            else if(s == "MilitaryNavyFemale")
                currentSwaps.Add("CustomerPair3");
            else if (s == "MilitaryArmyMale")
                currentSwaps.Add("customer_armyMaleAisleSearch");
            else if (s == "MilitaryArmyFemale")
                currentSwaps.Add("customer_armyFemale");
            else if (s == "MiddleEasternMale")
                currentSwaps.Add("CustomerAisleSearchb");
            else if (s == "MiddleEasternFemale")
                currentSwaps.Add("customer_ArabFemale");
            else if (s == "MiddleEasternGroup")
                currentSwaps.Add("CustomerPairs");

           // Debug.Log(s);
        }
       // Debug.Log(currentSwaps[0]);
        /*foreach (string s in InputManager.SpecialNPCList)
        {
            if (s == "MilitaryNavyMale")
                currentSwaps.Add("customer_aisleSearch4", "customer_navyMaleAisleSearch");
            else if(s == "MilitaryNavyFemale")
                currentSwaps.Add("CustomerPair3", "CustomerPair3");
            else if (s == "MilitaryArmyMale")
                currentSwaps.Add("customer_armyMaleAisleSearch", "customer_armyMaleAisleSearch");
            else if (s == "MilitaryArmyFemale")
                currentSwaps.Add("Customer_Cellphone", "customer_armyFemale");
            else if (s == "MiddleEasternMale")
                currentSwaps.Add("CustomerAisleSearchb", "customer_indianMaleFormal");
            else if (s == "MiddleEasternFemale")
                currentSwaps.Add("CustomerPair", "CustomerPair");
            else if (s == "MiddleEasternGroup")
                currentSwaps.Add("CustomerPairs", "CustomerPairs");
        }*/

		storeWalls = GameObject.Find ("groceryStore_walls_exterior");
		cart = GameObject.Find ("shoppingCart");
		robot = GameObject.Find("Robot_Prefab");
		girders = GameObject.Find ("groceryStore_girders");
		lights = GameObject.Find ("groceryStore_meshLights");
		signs = GameObject.Find ("aisleSignsSquare_prefab");
		gui = GameObject.Find ("Windows_prefab");
       
        player = GameObject.Find("Robot_Prefab");

        for (int i = 0; i < NPCs.Length; i++)
            CurrentNPCs.Add(NPCs[i]);

        if (InputManager.NPCOptionChoice == NPCOption.OnePerAisle)
            currentActiveNPCs = NPCOneAisleCount;
        else if (InputManager.NPCOptionChoice == NPCOption.TwoPerAisle)
            currentActiveNPCs = NPCTwoAisleCount;
        else if (InputManager.NPCOptionChoice == NPCOption.ThreePerAisle)
            currentActiveNPCs = NPCs.Length;

            for (int i = 0; i < NPCs.Length; i++)
            {
                if(i < currentActiveNPCs)
                    CurrentNPCs[i].SetActive(true);
                else
                    CurrentNPCs[i].SetActive(false);
            }

            for(int i = 0; i < currentSwaps.Count; i++)
            {
                for (int j = 0; j < SpecialNPCs.Length; j++)
                {
                    if (SpecialNPCs[j].name == currentSwaps[i])
                    {
                        SpecialNPCs[j].SetActive(true);
                        CurrentNPCs.Add(SpecialNPCs[j]);
                    }
                }
            }


              /*if (currentSwaps.ContainsKey(CurrentNPCs[i].name))
                {
                    for (int j = 0; j < SpecialNPCs.Length; j++)
                    {
                        if (SpecialNPCs[j].name == currentSwaps[CurrentNPCs[i].name])
                        {
                            if (CurrentNPCs[i].name.Contains("CustomerPair"))
                            {
                                foreach (Transform child in CurrentNPCs[i].transform)
                                {
                                    if (child.gameObject.name == "Person1")
                                    {
                                        child.gameObject.SetActive(false);
                                        child.transform.parent = null;
                                        break;
                                    }
                                }
                                //break;
                            }
                            else
                            {
                                CurrentNPCs[i].SetActive(false);
                                CurrentNPCs[i] = SpecialNPCs[j];
                                currentSwaps.Remove(CurrentNPCs[i].name);
                                break;
                            }
                        }
                    }
                }
                else if (CurrentNPCs[i].name.Contains("CustomerPair"))
                {
                    foreach (Transform child in CurrentNPCs[i].transform)
                    {
                        if (child.gameObject.name.Contains("Alt"))
                        {
                            child.gameObject.SetActive(false);
                            child.transform.parent = null;
                            break;
                        }
                    }
                }
            }
            for (int i = currentActiveNPCs; i < CurrentNPCs.Count; i++)
            {
                for (int j = 0; j < SpecialNPCs.Length; j++)
                {
                    if (CurrentNPCs[i].name == SpecialNPCs[j].name)
                    {
                        if (i != currentActiveNPCs)
                        {
                            GameObject temp = CurrentNPCs[i];
                            CurrentNPCs.Remove(CurrentNPCs[i]);
                            CurrentNPCs.Insert(currentActiveNPCs, temp);

                        }
                        currentActiveNPCs++;
                        break;
                    }
                }

            }

            for (int i = 0; i < currentActiveNPCs; i++)
            {
                CurrentNPCs[i].SetActive(true);
                if (CurrentNPCs[i].transform.childCount >= 2)
                {
                    if (CurrentNPCs[i].name == "CustomerPair")
                        Debug.Log(CurrentNPCs[i].transform.childCount);

                    foreach (Transform child in CurrentNPCs[i].transform)
                    {
                        child.gameObject.SetActive(true);
                        if (child.gameObject.GetComponent<PTSDVHAnimationManager>() != null)
                        {
                            PTSDVHAnimationManager manager = child.gameObject.GetComponent<PTSDVHAnimationManager>();
                            manager.ResumeIdleAnimations();
                        }
                    }
                }
                else
                    CurrentNPCs[i].GetComponent<Animation>().Play();
            }
         */

        //if (InputManager.NPCOptionChoice != NPCOption.None)
        //{
        //    if (InputManager.NPCOptionChoice == NPCOption.OnePerAisle)
        //        currentActiveNPCs = NPCOneAisleCount;

        //    if (InputManager.NPCOptionChoice == NPCOption.TwoPerAisle)
        //        currentActiveNPCs = NPCTwoAisleCount;

        //    if (InputManager.NPCOptionChoice == NPCOption.ThreePerAisle)
        //        currentActiveNPCs = NPCs.Length;
            
        //    for (int i = 0; i < CurrentNPCs.Count; i++)
        //    {
        //        if (currentSwaps.ContainsKey(CurrentNPCs[i].name))
        //        {
        //            for (int j = 0; j < SpecialNPCs.Length; j++)
        //            {
        //                if (SpecialNPCs[j].name == currentSwaps[CurrentNPCs[i].name])
        //                {
        //                    if (CurrentNPCs[i].name.Contains("CustomerPair"))
        //                    {
        //                        foreach (Transform child in CurrentNPCs[i].transform)
        //                        {
        //                            if (child.gameObject.name == "Person1")
        //                            {
        //                                child.gameObject.SetActive(false);
        //                                child.transform.parent = null;
        //                                break;
        //                            }
        //                        }
        //                        //break;
        //                    }
        //                    else
        //                    {
        //                        CurrentNPCs[i].SetActive(false);
        //                        CurrentNPCs[i] = SpecialNPCs[j];
        //                        currentSwaps.Remove(CurrentNPCs[i].name);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        else if (CurrentNPCs[i].name.Contains("CustomerPair"))
        //        {
        //            foreach (Transform child in CurrentNPCs[i].transform)
        //            {
        //                if (child.gameObject.name.Contains("Alt"))
        //                {
        //                    child.gameObject.SetActive(false);
        //                    child.transform.parent = null;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    for (int i = currentActiveNPCs; i < CurrentNPCs.Count; i++)
        //    {
        //        for(int j = 0; j < SpecialNPCs.Length; j++)
        //        {
        //            if (CurrentNPCs[i].name == SpecialNPCs[j].name)
        //            {
        //                if (i != currentActiveNPCs)
        //                {
        //                    GameObject temp = CurrentNPCs[i];
        //                    CurrentNPCs.Remove(CurrentNPCs[i]);
        //                    CurrentNPCs.Insert(currentActiveNPCs, temp);

        //                }
        //                currentActiveNPCs++;
        //                break;
        //            }
        //        }
                
        //    }

        //    for (int i = 0; i < currentActiveNPCs; i++)
        //    {
        //        CurrentNPCs[i].SetActive(true);
        //        if (CurrentNPCs[i].transform.childCount >= 2)
        //        {
        //            if (CurrentNPCs[i].name == "CustomerPair")
        //                Debug.Log(CurrentNPCs[i].transform.childCount);

        //            foreach (Transform child in CurrentNPCs[i].transform)
        //            {
        //                child.gameObject.SetActive(true);
        //                if (child.gameObject.GetComponent<PTSDVHAnimationManager>() != null)
        //                {
        //                    PTSDVHAnimationManager manager = child.gameObject.GetComponent<PTSDVHAnimationManager>();
        //                    manager.ResumeIdleAnimations();
        //                }
        //            }
        //        }
        //        else
        //            CurrentNPCs[i].GetComponent<Animation>().Play();
        //    }
        //    for (int i = currentActiveNPCs; i < NPCs.Length; i++)
        //        CurrentNPCs[i].SetActive(false);
        //}
        //else
        //{
        //    for (int i = 0; i < NPCs.Length; i++)
        //        CurrentNPCs[i].SetActive(false);
        //}

     
       
       
	}

	bool CheckDistance()
	{
		Vector3 target = BumpLocations[0].transform.position;
		target.y = transform.position.y; // Keep waypoint at character's height
		Vector3 moveDirection = target - transform.position;
		//Debug.Log (moveDirection.magnitude);
		if (moveDirection.magnitude < 1) {
			return true;
				}
		return false;
		
	}

	// Update is called once per frame
	void Update ()
	{
		if(bumpCart.EncounterType != EncounterTypes.None && !hasBumped)
		{
			if(CheckDistance())
			{
                hasBumped = true;
                string name = "";
                if (bumpCart.NPCType == NPCTypes.African_American_Female)
                    name = "Customer_withCartBumpAgent_female_black";
                else if (bumpCart.NPCType == NPCTypes.Caucasian_Female)
                    name = "Customer_withCartBumpAgent_female_caucasian";
                else if(bumpCart.NPCType == NPCTypes.African_American_Male)
                    name = "Customer_withCartBumpAgent_male_black";

                for (int i = 0; i < BumpNPCs.Length; i++)
                {
                    if (BumpNPCs[i].name == name)
                    {
                        BumpNPCs[i].SetActive(true);
                        break;
                    }
                }
            }
		}
	 /*   if (Input.GetKeyDown(KeyCode.LeftControl))
        {
			//BumpNPCs[0].SetActive(false);
            //Debug.Log("setactivefalse");
            OnNPCChange(-1);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("setactivetrue");
            OnNPCChange(1);
        }
        
		if (Input.GetKeyDown (KeyCode.Tab)) 
		{
			isfloating = !isfloating;
			if(isfloating)
			{
				position = robot.transform.localPosition;
				rotation = robot.transform.localRotation;

				robot.transform.localPosition = new Vector3(20, 20, 0);
				robot.transform.localRotation = Quaternion.Euler(45, 90, 0);

				storeWalls.SetActive(false);
				cart.SetActive(false);
				girders.SetActive(false);
				lights.SetActive(false);
				signs.SetActive(false);
				gui.SetActive(false);
				GetComponent<Movement1> ().enabled = false;
			}
			else
			{
				storeWalls.SetActive(true);
				cart.SetActive(true);
				girders.SetActive(true);
				lights.SetActive(true);
				signs.SetActive(true);
				gui.SetActive(true);
				GetComponent<Movement1> ().enabled = true;

				robot.transform.localPosition = position;
				robot.transform.localRotation = rotation;
			}
		}
		if (isfloating)
		{
			if(Input.GetKey(KeyCode.RightArrow))
			{
				robot.transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
			}
			if(Input.GetKey(KeyCode.LeftArrow))
			{
				robot.transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
			}
			if(Input.GetKey(KeyCode.DownArrow))
			{
				robot.transform.position += Vector3.left * Time.deltaTime;
			}
			if(Input.GetKey(KeyCode.UpArrow))
			{
				robot.transform.position += Vector3.right * Time.deltaTime;
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
			{
				robot.transform.Translate(new Vector3(0,0,speed * 2 * Time.deltaTime));
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
			{
				robot.transform.Translate(new Vector3(0,0,-speed * 2 * Time.deltaTime));
			}
		}
        */
	}
}

