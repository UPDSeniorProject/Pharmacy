using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class findClosest : MonoBehaviour {
	
	public List<Transform> targets;
    public List<Transform> targetsRegionPoints;
    public List<Transform> targetsAsilePoints;

	public Transform selectedTarget;
    public Transform selectedRegionPoint;
    public Transform selectedAsilePoint;

	private Transform myTransform;

	private static int chosenCheckoutCounterNumber = 1;


	void OnEnable()
	{
		Messenger.AddListener("checkout counter 1 chosen", setFirstCashierCounter);
		Messenger.AddListener("checkout counter 2 chosen", setSecondCashierCounter);
		Messenger.AddListener("checkout counter 3 chosen", setThirdCashierCounter);
		Messenger.AddListener("checkout counter 4 chosen", setFourthCashierCounter);
	}
	void OnDisable()
	{
		Messenger.RemoveListener("checkout counter 1 chosen", setFirstCashierCounter);
		Messenger.RemoveListener("checkout counter 2 chosen", setSecondCashierCounter);
		Messenger.RemoveListener("checkout counter 3 chosen", setThirdCashierCounter);
		Messenger.RemoveListener("checkout counter 4 chosen", setFourthCashierCounter);
	}
	
	// Use this for initialization
	void Start () {
		targets = new List<Transform>();
		selectedTarget = null;
        selectedRegionPoint = null;
        selectedAsilePoint = null;
		myTransform = transform;
		addAllTargets();

	}
	
	public void addAllTargets(){
		GameObject[] go = GameObject.FindGameObjectsWithTag("Checkoutpoint");
		foreach(GameObject checkoutPoint in go)
            targets.Add(checkoutPoint.transform); 

        GameObject[] points = GameObject.FindGameObjectsWithTag("regionPoint");
        foreach (GameObject point in points)
            targetsRegionPoints.Add(point.transform);

        GameObject[] asilePoints = GameObject.FindGameObjectsWithTag("asilePoint");
        foreach (GameObject point in asilePoints)
            targetsAsilePoints.Add(point.transform);


	}

	
	private void sortTargetsByDistance(){
		
		targets.Sort(delegate(Transform t1, Transform t2) {
				return Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position));
				});
	}

    private void sortTargetRegionPointssByDistance()
    {

        targetsRegionPoints.Sort(delegate(Transform t1, Transform t2)
        {
            return Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position));
        });
    }

    private void sortTargetAsilePointssByDistance()
    {

        targetsAsilePoints.Sort(delegate(Transform t1, Transform t2)
        {
            Vector3 self = new Vector3(0, 0, myTransform.position.z);
            Vector3 t1pos = new Vector3(0,0, t1.position.z);
            Vector3 t2pos = new Vector3(0,0, t2.position.z);

            return Vector3.Distance(t1pos, self).CompareTo(Vector3.Distance(t2pos, self));
        });
    }

	public void targetPoint(){
		if (selectedTarget == null){
			sortTargetsByDistance();
			selectedTarget = targets[0];
		}
	}
	
	public void clearClosestPoint(){
		selectedTarget = null;
	}
	
	public Transform getClosestPoint(){
		if (selectedTarget == null){
			sortTargetsByDistance();
			selectedTarget = targets[0];
		}

		// Hack to set which cashier the user is sent to
		//chosenCheckoutCounterNumber = 2;
		Debug.Log ("In findClosest, checking the value of chosenCheckoutCounterNumber before returning transform : "+chosenCheckoutCounterNumber);

		if(chosenCheckoutCounterNumber == 1){
			return GameObject.Find("Will_prefab/Points Prefab/checkoutPoint1").transform;
		}else if(chosenCheckoutCounterNumber == 2){
			return GameObject.Find("Will_prefab/Points Prefab/checkoutPoint2").transform;
		}else if(chosenCheckoutCounterNumber == 3){
			return GameObject.Find("Will_prefab/Points Prefab/checkoutPoint3").transform;
		}else if(chosenCheckoutCounterNumber == 4){
			return GameObject.Find("Will_prefab/Points Prefab/checkoutPoint4").transform;
		}else{
			return GameObject.Find("Will_prefab/Points Prefab/checkoutPoint1").transform;
		}
		//return selectedTarget;
	}

    public Transform getClosestRegionPoint() {
        //if (selectedRegionPoint == null)
        //{
            sortTargetRegionPointssByDistance();
            selectedRegionPoint = targetsRegionPoints[0];
        //}
        return selectedRegionPoint;
    }

    public Transform getClosestAsilePoint()
    {
        
            sortTargetAsilePointssByDistance();
            selectedAsilePoint = targetsAsilePoints[0];
       
        return selectedAsilePoint;
    }

	public void setFirstCashierCounter()
	{
		chosenCheckoutCounterNumber = 1;
		GameFlow.appropriate_cashier_set = true;
	}
	public void setSecondCashierCounter()
	{
		Debug.Log ("In findClosest, the 2nd checkout counter is set");
		chosenCheckoutCounterNumber = 2;
		GameFlow.appropriate_cashier_set = true;
	}
	public void setThirdCashierCounter()
	{
		chosenCheckoutCounterNumber = 3;
		GameFlow.appropriate_cashier_set = true;
	}
	public void setFourthCashierCounter()
	{
		chosenCheckoutCounterNumber = 4;
		GameFlow.appropriate_cashier_set = true;
	}

	public static int getCheckoutCounterNumber()
	{
		return chosenCheckoutCounterNumber;
	}
	// Update is called once per frame
	/*void Update () {
		if(Input.GetKeyDown(KeyCode.Tab)) {
			targetPoint();
		}
	}*/
}
