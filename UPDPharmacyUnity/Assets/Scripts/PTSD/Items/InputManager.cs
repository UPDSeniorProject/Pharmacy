using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InputManager : MonoBehaviour {
	
	
	/// <summary>
	/// tag with 'Pick' are all the single item can be picked.
	/// tag with 'Pickable' are the items can be picked during the simulation
	/// </summary>
	/// 
	public enum InputFiles { 
		Scenario1,
		Scenario2,
		Scenario3,
		Scenario4,
		Scenario5,
		Scenario6,
		Tutorial1
	}

	public InputFiles inputFile;
	
	public static GameObject[] AllPickableObjects;
	public static GameObject[] AllPickObjects;
	
	private static List<GameObject> AllPickObjectsNonRepeat = new List<GameObject>();
	public static Dictionary<string, GameObject> AllPickObjectsDictionary = new Dictionary<string, GameObject>();
	
	public static string IntroText = "";
	public static string Scenario = "";
	
	
	private static List<Task> _tasks = new List<Task>();
	public static List<Task> Tasks
	{
		get { return _tasks; }
	}
	private static Dictionary<string, Task> _tasksDictionary = new Dictionary<string, Task>();
	public static Dictionary<string, Task> TasksDictionary
	{
		get { return _tasksDictionary; }
	}
	
	private static List<Item> _pickableItems = new List<Item>();
	public static List<Item> PickableItems
	{
		get { return _pickableItems; }
	}
	
	private static List<Item> _shoppingListItems = new List<Item>();
	public static List<Item> ShoppingListItems
	{
		get { return _shoppingListItems; }
	}
	
	public static List<TutorialStep> TutorialSteps = new List<TutorialStep>();
	
	public static List<Task> OutputTasks = new List<Task>();

    public static BumpEncounter BumpEncounter = new BumpEncounter();
    public static NPCOption NPCOptionChoice = new NPCOption();
    public static List<string> SpecialNPCList = new List<string>();
    public static List<ShopperConversationsTypes> ShopperConversations = new List<ShopperConversationsTypes>();

	public static string VirtualCheckoutCashier = "";
	public static List<string> distractions = new List<string>();

	// set the total amount of money in the wallet
	
	void OnEnable()
	{
		Messenger.AddListener("write results", writeResults);
	}
	void OnDisable()
	{
		Messenger.RemoveListener("write results", writeResults);
	}
	
	public void setWalletMoney(float num){
		Messenger<float>.Broadcast("setMoneyInWallet", num);
	}
	
	//set the offest of changes, for example -1 means return changes-1, +1 means return changes+1
	
	public void setChangesOffset(float num){
		Messenger<float>.Broadcast("setChangesOffsetInWallet", num);
	}
	
	// get all the pickable objects
	
	public void FindAllPickableObjects(){
		AllPickableObjects = GameObject.FindGameObjectsWithTag("Pickable");
	}
	
	public void FindAllPickObjects(){
		AllPickObjects = GameObject.FindGameObjectsWithTag("Pick");
		foreach (GameObject go in AllPickObjects)
		{
			/*Highlight H = go.GetComponent<Highlight>();
            if (!AllPickObjectsDictionary.ContainsKey(H.item.Name))
            { 
                //Item item = new Item();
                //item.Name = H.name;
                //item.Price = H.price;
                AllPickObjectsDictionary[H.item.Name] = go;
            }*/
			
			if (!AllPickObjectsDictionary.ContainsKey(go.name))
			{
				//Item item = new Item();
				//item.Name = H.name;
				//item.Price = H.price;
				AllPickObjectsDictionary[go.name] = go;
			}
		}
	}
	
	/*public void FindAllTutorialSteps(){
		Debug.Log ("steps");
		TutorialSteps = GameObject.FindGameObjectsWithTag ("TutorialStep");
		Debug.Log (TutorialSteps);
	}*/
	// return a string contains all the name of the pickable objects in the scene, use after calling FindAllPickableObjects()
	
	public string ReturnAllPickableObjectsNames(){
		string StringName = "";
		foreach(GameObject go in AllPickableObjects){
			Highlight H = go.GetComponent<Highlight>();
			if(!StringName.Contains(H.item.Name))
				StringName = StringName + H.item.Name +"\n";
		}
		return StringName;
		
	}
	
	// set an object's price with its name, use after calling FindAllPickableObjects()
	
	public void SetObjectPrice(string setObjName, float Price, int quantity){
		//AllPickableObjects = GameObject.FindGameObjectsWithTag("Pick");
		foreach(GameObject go in AllPickObjects){
			Highlight H = go.GetComponent<Highlight>();
			if(H.item.Name.Equals(setObjName)){
				float unityPrice = Mathf.Round((Price/quantity)*100)/100;
				H.item.Price = unityPrice;
				if(quantity>1)
					H.item.Quantity = quantity;
			}
		}
		
	}
	
	
	// set the pickable object to pick with the object name
	
	public void SetPickObjects(string objName){
		foreach(GameObject go in AllPickableObjects){
			Highlight H = go.GetComponent<Highlight>();
			if(H.item.Name.Equals(objName))
				go.tag = "Pick";
		}
	}
	
	
	// set the content of shopping list
	
	private static UILabel shoppingListContent;
	private static UILabel shoppingListContentFinished;
	
	
	void writeResults() {
		OutputClass output = new OutputClass();
		output.tasks = OutputTasks;
		output.Save(Path.Combine(Application.streamingAssetsPath, "output.xml"));
	}
	
	void Awake (){
		FindAllPickableObjects();
		//SetPickObjects("Cheerios");
		FindAllPickObjects();
		//SetObjectPrice("Cheerios", 10.0f, 2);
		//FindAllTutorialSteps();
		/*
        string IntroText = "Hello and welcome to the VETS Grocery Store!  The purpose of this experience is to examine how you navigate a complex environment such as a grocery store.  Examples of some of the things you may be asked to do include: \n"+
       "Locating an object or objects within the store.\n"+
"Comparing features of objects within the grocery store.  For example, price, nutritional information, or volume/weight.\n"+
"In addition to \"doing\" tasks such as those listed above, you may encounter some \"feeling\" tasks involving problems encountered by people within environments such as grocery stores.  \n";

        string Scenario2 = "Find\n(1) A 8 ounce package of bologna containing the lowest fat content of the varieties available.\n(2) A package of bread with at least 22 grams of whole grain per slice\n(3) Kraft singles made with 2% milk. \n";
        
        //var inputXML = InputClass.Load(Path.Combine(Application.dataPath, "input1.xml"));
        InputClass input = new InputClass();
        
        Task task = new Task();
        task.Description = "Find the items on the shopping list.";
        task.TaskGoal = "All Bran";
        //_tasks.Add(task);
        input.IntroText = IntroText;
        input.Scenario = Scenario2;
        input.tasks.Add(task);


        Task task2 = new Task();
        task2.Description = "Find the items on the shopping list.";
        task2.TaskGoal = "Cheerios";
        
        input.tasks.Add(task2);

        Task task3 = new Task();
        task3.Description = "Find the items on the shopping list.";
        task3.TaskGoal = "Raisin Bran";

        input.tasks.Add(task3);


        //input.pickableItems = new List<Item>(AllPickObjectsDictionary.Values);

        Item item1 = new Item();
        item1.Name = "cerealBox_Comp2";
        input.shoppingListItems.Add(item1);

        Item item2 = new Item();
        item2.Name = "cerealBox_Comp3";
        input.shoppingListItems.Add(item2);

        Item item3 = new Item();
        item3.Name = "cerealBox_Comp1";
        input.shoppingListItems.Add(item3);

        input.Save(Path.Combine(Application.streamingAssetsPath, "Scenario3.xml"));
        */
		//Load config file
		//inputFile = InputFiles.Scenario2;
		//string file = "Scripts/PTSD/Input/" + inputFile.ToString() + ".xml";
		
		
		//SetShoppingListContent("Cheerios");
		loadXML();
	}
	
	void loadXML()
	{
		string file = inputFile.ToString() + ".xml";
		//var www = new WWW(Path.Combine(Application.streamingAssetsPath, file));
		//yield return www;
		//InputClass input = InputClass.LoadFromText(www.text);
		
		InputClass input = InputClass.Load(Path.Combine(Application.streamingAssetsPath, file));
		
		Debug.Log(Path.Combine(Application.streamingAssetsPath, file));
		
		TutorialSteps = input.tutorialSteps;

        BumpEncounter = input.bumpEncounter;
        NPCOptionChoice = input.npcOption;
        SpecialNPCList = input.specialNPCList;
        ShopperConversations = input.shopperConversations;

		//Task task2 = new Task();
		IntroText = input.IntroText;
		Scenario = input.Scenario;
		Debug.Log(IntroText);
		_tasks = new List<Task>(input.tasks);
		_pickableItems = new List<Item>(input.pickableItems);

		VirtualCheckoutCashier = input.VirtualCheckoutCashier;
		Debug.Log ("VirtualCheckoutCashier : -"+VirtualCheckoutCashier);
		Debug.Log ("input.VirtualCheckoutCashier : -"+input.VirtualCheckoutCashier);
		if (VirtualCheckoutCashier == "CaucasianMale") {
			Debug.Log ("First cashier chosen");
			Messenger.Broadcast("checkout counter 1 chosen");
		}else if (VirtualCheckoutCashier == "CaucasianFemale") {
			Debug.Log ("2nd cashier chosen");
			Messenger.Broadcast("checkout counter 2 chosen");
		}else if (VirtualCheckoutCashier == "MiddleEasternMale") {
			Debug.Log ("3rd cashier chosen");
			Messenger.Broadcast("checkout counter 3 chosen");
		}else if (VirtualCheckoutCashier == "MiddleEasternFemale") {
			Debug.Log ("4th cashier chosen");
			Messenger.Broadcast("checkout counter 4 chosen");
		}else if (VirtualCheckoutCashier == "AfricanAmericanMale") {
			Debug.Log ("4th cashier chosen");
			Messenger.Broadcast("checkout counter 4 chosen");
		}else if (VirtualCheckoutCashier == "AfricanAmericanFemale") {
			Debug.Log ("4th cashier chosen");
			Messenger.Broadcast("checkout counter 4 chosen");
		}

		distractions = input.distractions;
		foreach (string distraction in distractions) {
			if (distraction == "FlickeringLights") {
				Messenger.Broadcast ("enable flashing lights");
			}
			if (distraction == "BabyCrying") {
				Messenger.Broadcast ("enable baby crying");
			}
			if (distraction == "BottleBreaking") {
				Messenger.Broadcast ("enable bottle breaking");
			}
			if (distraction == "Thunder") {
				Messenger.Broadcast ("enable thunder");
			}
		}
		
		foreach (Item pickItem in _pickableItems)
		{
			//Debug.Log(pickItem.Name.ToString());
			
			foreach (GameObject go in AllPickObjects)
			{
				if (go.name.Equals(pickItem.Name.ToString()))
				{
					//Debug.Log(go.name);
					go.AddComponent<Highlight>();
					Highlight H = go.GetComponent<Highlight>();
					H.item.Price = pickItem.Price;
					//if(pickItem.Unitprice != null){ //DRG: Floats cant be null
					H.item.Unitprice = pickItem.Unitprice;
					//}
					if (pickItem.Standpos == "standup")
						H.type = Highlight.Type.can;
					else
						H.type = Highlight.Type.cereal;
					go.AddComponent<BoxCollider>();
					go.AddComponent<Rigidbody>();
					go.GetComponent<Rigidbody>().isKinematic = true;
				}
			}
		}
		
		_shoppingListItems = new List<Item>(input.shoppingListItems);
		
		
		setupTaskColliders();
		
		shoppingListContent = GameObject.Find("UILabelShoppingListContent").GetComponent<UILabel>();
		shoppingListContentFinished = GameObject.Find("UILabelShoppingListContentFinished").GetComponent<UILabel>();
		
		updateShoppingList("");
	}
	
	static int j = 1;
	public static void updateShoppingList(string finish)
	{
		string list = "";
		int i = 1;
		foreach (Task task in _tasksDictionary.Values)
		{
			string[] array = task.TaskGoal.Split('_');
			
			list = list + i.ToString() +". "+ array[1]+" "+array[2] +"\n";
			i++; 
			
			if(task.TaskGoalComparison != null){
				string[] array1 = task.TaskGoalComparison.Split('_');
				
				list = list + i.ToString() +". "+ array1[1]+" "+array1[2] + "\n";
				i++; 
			}
		}
		list = list.Replace("Store","Publix");
		shoppingListContent.text = list;
		
		
		if (finish.Contains("_"))
		{
			string[] array2 = finish.Split('_');
			array2[1] = array2[1].Replace("Store","Publix");
			array2[2] = array2[2].Replace("Store","Publix");
			shoppingListContentFinished.text = shoppingListContentFinished.text + "\n" + j.ToString() +". "+array2[1]+" "+array2[2];
			j++;
		}
	}
	
	void setupTaskColliders()
	{
		foreach (Task task in _tasks) {
			//Debug.Log(task.TaskGoal);
			_tasksDictionary[task.TaskGoal] = task;
			
			if (AllPickObjectsDictionary.ContainsKey(task.TaskGoal))
			{
				GameObject taskGoalParent = AllPickObjectsDictionary[task.TaskGoal].transform.parent.gameObject;
				//Debug.Log(taskGoalParent.name);
				BoxCollider taskCollider = taskGoalParent.AddComponent<BoxCollider>();
				//Debug.Log(taskGoalParent.transform.forward);
				taskCollider.center = new Vector3(0f, 0f, 1.5f);
				taskCollider.size = new Vector3(1.2f, 2f, 3f);
				taskCollider.isTrigger = true;
				TaskTrigger taskTrigger = taskGoalParent.AddComponent<TaskTrigger>();
				taskTrigger.taskName = task.TaskGoal;
				if(task.TaskGoalComparison != null){
					taskTrigger.taskComparisonName = task.TaskGoalComparison;
				}
			}
			
		}
	}
	
	// Use this for initialization
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
