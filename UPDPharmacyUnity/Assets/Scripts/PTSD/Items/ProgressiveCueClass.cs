using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ProgressiveCueClass : GUIBasic {

	// Use this for initialization
    public enum ProgressiveCue
    {
        None,
        Cue1,
        Cue2,
        Cue3,
        Cue4,
        Cue5,
        Cue6
    }
    public ProgressiveCue progressiveCue;


    static private int playerRegion;

    public static int getPlayerRegion()
    {
        return playerRegion;
    }

    public GameObject player;

    public Transform regionPoint1and2 = null;
    public Transform regionPoint2and3 = null;
    public Transform regionPoint3and4 = null;
    public GameObject TaskWindow;

    private bool closePage = false;


    private static Vector3 itemDestination = new Vector3(0f, 0f, 0f);

    static private List<Vector3> path = new List<Vector3>();
    static public List<Vector3> Lightpath = new List<Vector3>();

    static private List<Vector3> rotation = new List<Vector3>();
    static private List<Vector3> path2 = new List<Vector3>();


    private string[] nextTaskName;
    private string[] nextTaskLocation;

    void CallProgressiveCue1()
    {
        progressiveCue = ProgressiveCue.Cue1;

    }
    void CallProgressiveCue2()
    {
        progressiveCue = ProgressiveCue.Cue2;
    }
    void CallProgressiveCue3()
    {
        progressiveCue = ProgressiveCue.Cue3;
    }
    void CallProgressiveCue4()
    {
        progressiveCue = ProgressiveCue.Cue4;
    }
    void CallProgressiveCue5()
    {
        progressiveCue = ProgressiveCue.Cue5;
    }
    void CallProgressiveCue6()
    {
        progressiveCue = ProgressiveCue.Cue6;
    }
    void CallProgressiveCue7()
    {
        //progressiveCue = ProgressiveCue.Cue6;
        Messenger.Broadcast("move block item");
    }

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        if (regionPoint1and2 == null)
            regionPoint1and2 = GameObject.Find("regionPoint_1_2").transform;
        if (regionPoint2and3 == null)
            regionPoint2and3 = GameObject.Find("regionPoint_2_3").transform;
        if (regionPoint3and4 == null)
            regionPoint3and4 = GameObject.Find("regionPoint_3_4").transform;
      
        if (TaskWindow == null)
            TaskWindow = GameObject.Find("TaskWindow");
    }

    void OnGUI()
    {
        GUI.depth = 2;
        GUI.skin = myskin;
        if (GameFlow.state == GameFlow.State.Tasks_doing) {

            if (GameFlow.nextTask != null && GameFlow.nextTask.TaskGoal.Contains("_"))
            {
                nextTaskName = GameFlow.nextTask.TaskGoal.Split('_');
                nextTaskLocation = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal].transform.root.name.Split('_');
            }

            switch (progressiveCue)
            {

                case ProgressiveCue.None:
                    break;
                case ProgressiveCue.Cue1:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();

                        TaskLabel.text = "Would you like help finding the " + nextTaskName[2] + " ?";
                        if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset, topButtonPos, buttonWidth, buttonHeight), "Yes"))
                        {

                            TaskLabel.text = "The " + nextTaskName[2] + " is in aisle " + nextTaskLocation[2] + " in the " + nextTaskLocation[1] + " of store, along with other " + nextTaskName[0] + "s.";

                            closePage = true;
                        }

                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "No"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue2:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        //TaskLabel.text = "Would you like help finding Aisle !, where the All Bran Original Cereal is located?
                        TaskLabel.text = "Would you like help finding the aisle " + nextTaskLocation[2] + ", where the  " + nextTaskName[2] + " is located?";
                        if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset, topButtonPos, buttonWidth, buttonHeight), "Yes"))
                        {
                            TaskWindow.SetActive(false);
                            GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                            if (Vector3.Distance(player.transform.position, item.transform.position) < 2.5f)
                            {
                                TaskWindow.SetActive(true);
                                TaskLabel.text = "You are already at aisle " + nextTaskLocation[2];
                                closePage = true;
                            }
                            else {
                                TaskWindow.SetActive(true);
                                TaskLabel.text = "Follow the green line to aisle" + nextTaskLocation[2];
                                closePage = true;
                            }

                            HelpMoveToItem( );
                            progressiveCue = ProgressiveCue.None;
                            //path.Clear();
                            //HelpMoveToItem();
                            //ShowPath();
                            //path.Clear();
                        }

                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "No"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue3:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "Would you like us to take you to aisle " + nextTaskLocation[2] + " ?";
                        if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset, topButtonPos, buttonWidth, buttonHeight), "Yes"))
                        {
                            TaskWindow.SetActive(false);
                            path.Clear();
                            //GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                            HelpMoveToItem();
                            Debug.Log("help move to item");
                            //MoveToItem();
                            path.Clear();
                            progressiveCue = ProgressiveCue.None;
                        }

                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "No"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue4:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "Would you like help finding the " + nextTaskName[2] + " ?";
                        GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                        
                        if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset, topButtonPos, buttonWidth, buttonHeight), "Yes"))
                        {
                                if (Vector3.Distance(player.transform.position, item.transform.position) < 2.5f)
                                {
                                    if (player.transform.eulerAngles.y > 0 && player.transform.eulerAngles.y < 180)
                                    {
                                        TaskLabel.text = "The " + nextTaskName[2] + " is on the " + nextTaskLocation[3] + ". ";
                                    }
                                    else if (nextTaskLocation[3] == "left")
                                    {
                                        TaskLabel.text = "The " + nextTaskName[2] + " is on the right. ";
                                    }
                                    else if (nextTaskLocation[3] == "right")
                                    {
                                        TaskLabel.text = "The " + nextTaskName[2] + " is on the left. ";
                                    }

                                    closePage = true;
                                }
                                else
                                {
                                    TaskLabel.text = "Please move to the " + nextTaskLocation[1] + "of asile " + nextTaskLocation[2] + " first.";
                                    closePage = true;
                                }
                         }
                        
                        
                        

                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "No"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue5:
                    TaskWindow.SetActive(true);
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "Would you like us to point out the " + nextTaskName[2] + "?";
                        
                        if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset, topButtonPos, buttonWidth, buttonHeight), "Yes"))
                        {
                            //TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                            GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                            

                            if (Vector3.Distance(player.transform.position, item.transform.position) < 2.5f)
                            {
                                //TaskWindow.SetActive(true);
                                item.AddComponent<Flashing>();
                                TaskLabel.text = nextTaskName[2] + " is now flashing.";
                                closePage = true;

                                if (Movement.selectionModeLeft) 
                                {
                                    if (nextTaskLocation[3] == "right")
                                    {
                                        Messenger.Broadcast("exit selection");
                                        Movement.selectionModeRight = true;
                                    }
                                }
                                else if (Movement.selectionModeRight)
                                {
                                    if (nextTaskLocation[3] == "left")
                                    {
                                        Messenger.Broadcast("exit selection");
                                        Movement.selectionModeLeft = true;
                                    }
                                }
                                else if (player.transform.eulerAngles.y > 0 && player.transform.eulerAngles.y < 180 && (Movement.selectionTarget != null))
                                {
                                    if (nextTaskLocation[3] == "left")
                                        Movement.selectionModeLeft = true;
                                    else if (nextTaskLocation[3] == "right")
                                        Movement.selectionModeRight = true;
                                }
                                else if (player.transform.eulerAngles.y > 180 && player.transform.eulerAngles.y < 360 && (Movement.selectionTarget != null))
                                {
                                    if (nextTaskLocation[3] == "left")
                                    {
                                        //Debug.Log("right");
                                        Movement.selectionModeLeft = true;
                                    }
                                    else if (nextTaskLocation[3] == "right")
                                    {
                                        Debug.Log("left");
                                        Movement.selectionModeRight = true;
                                    }
                                }
                            

                            }
                            else {
                                //GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                                TaskLabel.text = "Please move to the "+ nextTaskLocation[1]+"of asile "+ nextTaskLocation[2]+ " first.";
                                closePage = true;
                            }


                            

                        }

                        if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "No"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue6:
                   TaskWindow.SetActive(true);
                   if (!closePage && GameFlow.nextTask != null)
                   {
                       UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                       TaskLabel.text = "Would you like a clerk to help you find the " + nextTaskName[2] + "?";
                       if (GUI.Button(new Rect(Screen.width / 2 + 2 * buttonWidth - offset, topButtonPos, buttonWidth, buttonHeight), "Yes"))
                       {
                           TaskWindow.SetActive(false);
                           if (Movement.selectionModeLeft || Movement.selectionModeRight)
                               Messenger.Broadcast("exit selection");
                           Messenger.Broadcast("Set cursor to null");
                           
                           GameObject clerk = GameObject.Find("VirtualHuman");
                           GameObject cart = GameObject.Find("shoppingCart");
                           if (cart.transform.eulerAngles.y < 120f)
                               clerk.transform.position = new Vector3(cart.transform.position.x + 2.5f, clerk.transform.position.y, cart.transform.position.z);
                           else if (cart.transform.eulerAngles.y > 240f)
                               clerk.transform.position = new Vector3(cart.transform.position.x - 3f, clerk.transform.position.y, cart.transform.position.z);

                           clerk.transform.LookAt(new Vector3(cart.transform.position.x, clerk.transform.position.y, cart.transform.position.z));

                           //here
                           //OfflineSimulatorCommunicator communicator;
                           //communicator = GameObject.Find("VirtualHuman").GetComponent<OfflineSimulatorCommunicator>();
                           //communicator.RenderGUI = true;

						   PTSDVPF1Communicator communicator;
                           communicator = GameObject.Find("VirtualHuman").GetComponent<PTSDVPF1Communicator>();
                           communicator.RenderGUI = true;
		

                           progressiveCue = ProgressiveCue.None;
                       }
                       if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "No"))
                       {
                           TaskWindow.SetActive(false);
                           progressiveCue = ProgressiveCue.None;
                       }
                   }
                       
                    
                    break;
            }

            if (closePage)
            {
				if (GUI.Button(new Rect(Screen.width / 2 + 3 * buttonWidth, topButtonPos, buttonWidth, buttonHeight), "Press A to Continue"))
                {
                    TaskWindow.SetActive(false);
                    progressiveCue = ProgressiveCue.None;
                    closePage = false;
                }
            }
        }
    
    }

    private static Vector3 secondPoint = Vector3.zero;
    private static Vector3 thirdPoint = Vector3.zero;
    void HelpMoveToItem( )
    {
        GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
        regionDefinition aislePoints = item.transform.root.GetComponent<regionDefinition>();
        int itemRegion = aislePoints.itemRegion;
        int itemAisle = aislePoints.aisle;

        string[] AsileName = player.GetComponent<findClosest>().getClosestAsilePoint().name.Split('_');

        // if robot and item in the same region find the closest one
        // if we are not in the same region, if we are in 1,4 find the closest one, if we are in 2,3 go to point2and3
        // = 3;
        //int playerRegion = 1;
        //int itemRegion;
        Vector3 closestPoint = new Vector3(0f, 0f, 0f);
        if (itemRegion == playerRegion)
        {
            // find the closet regionPoints
            closestPoint = player.GetComponent<findClosest>().getClosestRegionPoint().position;
        }
        else
        {
            if (playerRegion == 1 || playerRegion == 4)
            {
                closestPoint = player.GetComponent<findClosest>().getClosestRegionPoint().position;
            }
            else if (playerRegion == 2 || playerRegion == 3)
            {
                closestPoint = regionPoint2and3.position;
            }

        }

        Transform aislePoint;
        if (closestPoint.x < aislePoints.aisleStart.position.x)
        {
            aislePoint = aislePoints.aisleStart;
        }
        else if (closestPoint.x > aislePoints.aisleEnd.position.x)
        {
            aislePoint = aislePoints.aisleEnd;
        }
        else
        {
            aislePoint = aislePoints.aisleStart;
        }
        path.Clear();
        Lightpath.Clear();
        Lightpath.Add(new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z));
        //path.Add(player.transform.position);
        Vector3 point2 = new Vector3(0f, 0f, 0f);

        if (playerRegion == 1 || playerRegion == 4)
        {
            point2 = new Vector3(closestPoint.x, player.transform.position.y, aislePoint.position.z);
            thirdPoint = point2;
            path.Add(point2);
            Lightpath.Add(new Vector3(point2.x, point2.y + 1, point2.z));
            //closestPoint = new Vector3(closestPoint.x, 0f, aislePoint.position.z);
        }
        else if ((playerRegion == 2 || playerRegion == 3) && (itemAisle == int.Parse(AsileName[2])))
        {
            path.Clear();
            Lightpath.Clear();
            Debug.Log("add1");
            Lightpath.Add(new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z));
        }
        else
        {
            point2 = new Vector3(closestPoint.x, player.transform.position.y, player.transform.position.z);
            path.Add(point2);
            Debug.Log("add2");
            Lightpath.Add(new Vector3(point2.x, point2.y + 1, point2.z));
            Vector3 point3 = new Vector3(closestPoint.x, player.transform.position.y, aislePoint.position.z);
            thirdPoint = point3;
            path.Add(point3);
            Debug.Log("add3");
            Lightpath.Add(new Vector3(point3.x, point3.y + 1, point3.z));
        }

        //Debug.DrawLine(point2, point3);
        //path2.Clear();
        //path2.Add(player.transform.position);
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        itemDestination = new Vector3(item.transform.position.x, player.transform.position.y, aislePoint.position.z);
        Lightpath.Add(new Vector3(itemDestination.x, itemDestination.y + 1, itemDestination.z));

        if (progressiveCue == ProgressiveCue.Cue3)
        {
            //Debug.Log(cue);
            if (path.Count > 1)
            {
                Debug.Log("orientpath false");
                secondPoint = path[1];
                iTween.MoveTo(player, iTween.Hash("position", path[0], "speed", 6f,
                    "orienttopath", false, "oncomplete", "secondMove", "easetype", iTween.EaseType.linear));
            }
            else if (path.Count > 0)
            {
                Debug.Log("orientpath true");
                iTween.MoveTo(player, iTween.Hash("position", path[0], "speed", 6f,
                    "orienttopath", true, "oncomplete", "turnAtConer", "easetype", iTween.EaseType.linear));
            }
            else if (path.Count == 0)
            {
                MoveToItem();
            }
            //"oncomplete", "turnAtConer"
            path.Clear();
        }
        else if (progressiveCue == ProgressiveCue.Cue2)
        {
            if ((player.transform.eulerAngles.y > 0f && player.transform.eulerAngles.y < 180f && player.transform.position.x > Lightpath[1].x)
        || (player.transform.eulerAngles.y > 180f && player.transform.eulerAngles.y < 360f && player.transform.position.x < Lightpath[1].x))
            {
                player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, player.transform.eulerAngles.y + 180f, player.transform.eulerAngles.z);
            }

            DrawLine drawLine = GameObject.Find("LineDraw").GetComponent<DrawLine>();
            drawLine.drawLine(Lightpath);
            Debug.Log(Lightpath.Count);
            Lightpath.Clear();

        }

        //if (progressiveCue == ProgressiveCue.Cue2)
        //  ShowPath();
        //if(progressiveCue == ProgressiveCue.Cue3)
        //MoveToItem();
    }

    void secondMove()
    {
        iTween.MoveTo(player, iTween.Hash("position", secondPoint, "speed", 6f,
                    "orienttopath", true, "oncomplete", "turnAtConer", "easetype", iTween.EaseType.linear));
    }

    void turnAtConer()
    {
        Debug.Log("turn");
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (itemDestination.x > thirdPoint.x)
        {
            Debug.Log("turn");
            rotation.Add(new Vector3(0f, 90f, 0f));
            iTween.RotateTo(player, iTween.Hash("rotation", new Vector3(0f, 90f, 0f), "speed", 60f, "oncomplete", "MoveToItem", "easetype", iTween.EaseType.linear));
        }
        else
        {
            rotation.Add(new Vector3(0f, 270f, 0f));
            iTween.RotateTo(player, iTween.Hash("rotation", new Vector3(0f, 270f, 0f), "speed", 60f, "oncomplete", "MoveToItem", "easetype", iTween.EaseType.linear));
        }
        rotation.Clear();
    }

    void MoveToItem()
    {
        Debug.Log("move to item");
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        path2.Clear();
        path2.Add(player.transform.position);
        path2.Add(itemDestination);
        iTween.MoveTo(player, iTween.Hash("position", itemDestination, "speed", 6f,
            "orienttopath", false, "easetype", iTween.EaseType.linear));
        path2.Clear();
    }
   

	void Start () {
	
	}

    void XboxController()
    {
        if (GameFlow.state == GameFlow.State.Tasks_doing || GameFlow.state == GameFlow.State.Tasks_timeout)
        {

            if (GameFlow.nextTask != null && GameFlow.nextTask.TaskGoal.Contains("_"))
            {
                nextTaskName = GameFlow.nextTask.TaskGoal.Split('_');
                nextTaskLocation = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal].transform.root.name.Split('_');
            }

            switch (progressiveCue)
            {
                case ProgressiveCue.None:
                    break;
                case ProgressiveCue.Cue1:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();

                        TaskLabel.text = "Would you like help finding the " + nextTaskName[2] + " ?";

                        GameFlow.state = GameFlow.State.Tasks_timeout;

                        if (Input.GetButtonDown("A Button"))
                        {
                            TaskLabel.text = "The " + nextTaskName[2] + " is in aisle " + nextTaskLocation[2] + " in the " + nextTaskLocation[1] + " of store, along with other " + nextTaskName[0] + "s.";
                            closePage = true;
                            GameFlow.state = GameFlow.State.Tasks_doing;
                        }

                        if (Input.GetButtonDown("B Button"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                            GameFlow.state = GameFlow.State.Tasks_doing;
                        }
                    }
                    break;
                case ProgressiveCue.Cue2:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        //TaskLabel.text = "Would you like help finding Aisle !, where the All Bran Original Cereal is located?
                        TaskLabel.text = "Would you like help finding the aisle " + nextTaskLocation[2] + ", where the  " + nextTaskName[2] + " is located?";
                        GameFlow.state = GameFlow.State.Tasks_timeout;
                        if (Input.GetButtonDown("A Button"))
                        {
                            TaskWindow.SetActive(false);
                            GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                            if (Vector3.Distance(player.transform.position, item.transform.position) < 2.5f)
                            {
                                TaskWindow.SetActive(true);
                                TaskLabel.text = "You are already at aisle " + nextTaskLocation[2];
                                closePage = true;
                            }
                            else
                            {
                                TaskWindow.SetActive(true);
                                TaskLabel.text = "Follow the green line to aisle" + nextTaskLocation[2];
                                closePage = true;
                            }

                            HelpMoveToItem();
                            progressiveCue = ProgressiveCue.None;
                            GameFlow.state = GameFlow.State.Tasks_doing;

                            //path.Clear();
                            //HelpMoveToItem();
                            //ShowPath();
                            //path.Clear();
                        }

                        if (Input.GetButtonDown("B Button"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue3:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "Would you like us to take you to aisle " + nextTaskLocation[2] + " ?";
                        if (Input.GetButtonDown("A Button"))
                        {
                            TaskWindow.SetActive(false);
                            path.Clear();
//                            GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                            HelpMoveToItem();
                            Debug.Log("help move to item");
                            //MoveToItem();
                            path.Clear();
                            progressiveCue = ProgressiveCue.None;
                        }

                        if (Input.GetButtonDown("B Button"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue4:
                    TaskWindow.SetActive(true);
                    if (Movement.selectionModeLeft || Movement.selectionModeRight)
                        Messenger.Broadcast("exit selection");
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "Would you like help finding the " + nextTaskName[2] + " ?";
                        GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];

                        if (Input.GetButtonDown("A Button"))
                        {
                            if (Vector3.Distance(player.transform.position, item.transform.position) < 2.5f)
                            {
                                if (player.transform.eulerAngles.y > 0 && player.transform.eulerAngles.y < 180)
                                {
                                    TaskLabel.text = "The " + nextTaskName[2] + " is on the " + nextTaskLocation[3] + ". ";
                                }
                                else if (nextTaskLocation[3] == "left")
                                {
                                    TaskLabel.text = "The " + nextTaskName[2] + " is on the right. ";
                                }
                                else if (nextTaskLocation[3] == "right")
                                {
                                    TaskLabel.text = "The " + nextTaskName[2] + " is on the left. ";
                                }

                                closePage = true;
                            }
                            else
                            {
                                TaskLabel.text = "Please move to the " + nextTaskLocation[1] + "of asile " + nextTaskLocation[2] + " first.";
                                closePage = true;
                            }
                        }




                        if (Input.GetButtonDown("B Button"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue5:
                    TaskWindow.SetActive(true);
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "Would you like us to point out the " + nextTaskName[2] + "?";

                        if (Input.GetButtonDown("A Button"))
                        {
                            //TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                            GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];


                            if (Vector3.Distance(player.transform.position, item.transform.position) < 2.5f)
                            {
                                //TaskWindow.SetActive(true);
                                item.AddComponent<Flashing>();
                                TaskLabel.text = nextTaskName[2] + " is now flashing.";
                                closePage = true;

                                if (Movement.selectionModeLeft)
                                {
                                    if (nextTaskLocation[3] == "right")
                                    {
                                        Messenger.Broadcast("exit selection");
                                        Movement.selectionModeRight = true;
                                    }
                                }
                                else if (Movement.selectionModeRight)
                                {
                                    if (nextTaskLocation[3] == "left")
                                    {
                                        Messenger.Broadcast("exit selection");
                                        Movement.selectionModeLeft = true;
                                    }
                                }
                                else if (player.transform.eulerAngles.y > 0 && player.transform.eulerAngles.y < 180 && (Movement.selectionTarget != null))
                                {
                                    if (nextTaskLocation[3] == "left")
                                        Movement.selectionModeLeft = true;
                                    else if (nextTaskLocation[3] == "right")
                                        Movement.selectionModeRight = true;
                                }
                                else if (player.transform.eulerAngles.y > 180 && player.transform.eulerAngles.y < 360 && (Movement.selectionTarget != null))
                                {
                                    if (nextTaskLocation[3] == "left")
                                    {
                                        //Debug.Log("right");
                                        Movement.selectionModeLeft = true;
                                    }
                                    else if (nextTaskLocation[3] == "right")
                                    {
                                        Debug.Log("left");
                                        Movement.selectionModeRight = true;
                                    }
                                }


                            }
                            else
                            {
                                //GameObject item = InputManager.AllPickObjectsDictionary[GameFlow.nextTask.TaskGoal];
                                TaskLabel.text = "Please move to the " + nextTaskLocation[1] + "of asile " + nextTaskLocation[2] + " first.";
                                closePage = true;
                            }




                        }

                        if (Input.GetButtonDown("B Button"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }
                    break;
                case ProgressiveCue.Cue6:
                    TaskWindow.SetActive(true);
                    if (!closePage && GameFlow.nextTask != null)
                    {
                        UILabel TaskLabel = GameObject.Find("TaskLabel").GetComponent<UILabel>();
                        TaskLabel.text = "would you like a clerk to help you find the " + nextTaskName[2] + "?";
                        if (Input.GetButtonDown("A Button"))
                        {
                            TaskWindow.SetActive(false);
                            if(Movement.selectionModeLeft||Movement.selectionModeRight)
                                Messenger.Broadcast("exit selection");
                            GameObject clerk = GameObject.Find("StoreClerk_prefab");
                            GameObject cart = GameObject.Find("shoppingCart");
                            if (cart.transform.eulerAngles.y < 120f)
                                clerk.transform.position = new Vector3(cart.transform.position.x + 2.5f, clerk.transform.position.y, cart.transform.position.z);
                            else if (cart.transform.eulerAngles.y > 240f)
                                clerk.transform.position = new Vector3(cart.transform.position.x - 3f, clerk.transform.position.y, cart.transform.position.z);

                            clerk.transform.LookAt(new Vector3(cart.transform.position.x, clerk.transform.position.y, cart.transform.position.z));

                            //here
                            PTSDVPF1Communicator communicator;
                            communicator = GameObject.Find("VirtualHuman").GetComponent<PTSDVPF1Communicator>();
                            communicator.RenderGUI = true;


                            progressiveCue = ProgressiveCue.None;
                        }
                        if (Input.GetButtonDown("B Button"))
                        {
                            TaskWindow.SetActive(false);
                            progressiveCue = ProgressiveCue.None;
                        }
                    }


                    break;
            }

            if (closePage)
            {
                if (Input.GetButtonDown("A Button"))
               {
                    TaskWindow.SetActive(false);
                    progressiveCue = ProgressiveCue.None;
                    closePage = false;
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {

        XboxController();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            CallProgressiveCue1();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CallProgressiveCue2();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            CallProgressiveCue3();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            CallProgressiveCue4();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CallProgressiveCue5();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            CallProgressiveCue6();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            CallProgressiveCue7();
        }

        if (player.transform.position.x < regionPoint1and2.position.x )
        {
            playerRegion = 1;
        }
        else if (player.transform.position.x > regionPoint1and2.position.x - 2f && player.transform.position.x < regionPoint1and2.position.x)
        {
            Messenger.Broadcast("set asile points");
            Debug.Log("player region 1");
            playerRegion = 1;
        }
        else if ((player.transform.position.x < regionPoint2and3.position.x -2f) && (player.transform.position.x >= regionPoint1and2.position.x))
        {
            //if (playerRegion != 2)
            if( !Movement.selectionModeLeft && !Movement.selectionModeRight && (player.transform.position.x < regionPoint1and2.position.x + 4f || player.transform.position.x > regionPoint2and3.position.x -5f))
                Messenger.Broadcast("set asile points");
            //Debug.Log("player region 2");

            playerRegion = 2;

        }
        else if ((player.transform.position.x >= regionPoint2and3.position.x - 2f) && (player.transform.position.x <= regionPoint2and3.position.x + 2f) )
        {
            Messenger.Broadcast("set asile points");
            playerRegion = 5;
        }
        else if ((player.transform.position.x < regionPoint3and4.position.x-2f) && (player.transform.position.x > regionPoint2and3.position.x+2f))
        {
            //if (playerRegion != 3)
            //{
            if (!Movement.selectionModeLeft && !Movement.selectionModeRight && (player.transform.position.x < regionPoint2and3.position.x + 4f || player.transform.position.x > regionPoint3and4.position.x - 5f))
                Messenger.Broadcast("set asile points");
               // Debug.Log("player region 3");
            //}
            playerRegion = 3;

        }
        else if ((player.transform.position.x >= regionPoint3and4.position.x-2f))
        {
            Messenger.Broadcast("set asile points");
            //Debug.Log("player region 4");
            playerRegion = 4;
        }
        //else if(){}

        if (itemDestination != Vector3.zero && (Vector3.Distance(player.transform.position, itemDestination) < 2f))
        {
            DrawLine drawLine = GameObject.Find("LineDraw").GetComponent<DrawLine>();
            drawLine.resetLine();
            itemDestination = Vector3.zero;
        }
	
	}

   // }
}
