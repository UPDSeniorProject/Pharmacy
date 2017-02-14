using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class priceTag : MonoBehaviour {
	public List<Transform> targets;
	public Transform selectedTarget;
	private Transform myTransform;
	public enum Type {
		price,							
		name									
	}
	public Type type;
	// Use this for initialization
	void Start () {
		targets = new List<Transform>();
		selectedTarget = null;
		myTransform = transform;
		addAllTargets();
		getClosestPoint();
		//UILabel content = gameObject.GetComponent<UILabel>();
		TextMesh text = gameObject.GetComponent<TextMesh>();
		Highlight H = selectedTarget.GetComponent<Highlight>();
		if(H != null){
			if(H.item.Quantity>1){
				if(type == Type.price){
					float price = H.item.Price*H.item.Quantity;
					text.text = price.ToString("00.00");
				}
				else if(type == Type.name && H.item.Quantity>1){
	                text.characterSize = 0.3f;
					text.text = H.item.Quantity.ToString()+'\n'+"For";
				}
			}
			else{
				if(type == Type.price)
					text.text = H.item.Price.ToString("00.00");
				else if(type == Type.name){
					text.characterSize = 0.3f;
	                string[] namesub = H.item.Name.Split(' ');
	                int i = 1;
	                foreach(string sub in namesub)
	                {
	                    text.text += sub+" ";
	                  
	                    if (text.text.Length > 15*i)
	                    {
	                        text.text += "\n";
	                        i ++ ;
	                    }
	                }
	                
	                // problem here
					//text.text = H.Objname.Substring(0,6).ToString()+'\n'+H.Objname.Substring(6).ToString();
				}
			}
		}
	}
	
	public void addAllTargets(){
		//GameObject[] go = GameObject.FindGameObjectsWithTag("Pick");
		
		
		foreach(GameObject pickObj in InputManager.AllPickObjects){
			if(pickObj.transform.position.y > myTransform.position.y){
				addTarget(pickObj.transform);
			}
		}
		
		foreach(GameObject pickObj in InputManager.AllPickableObjects){
			if(pickObj.transform.position.y > myTransform.position.y){
				addTarget(pickObj.transform);
			}
		}
	}
	
	public void addTarget(Transform target){
		targets.Add(target);
	}
	
	
	private void sortTargetsByDistance(){
		
		targets.Sort(delegate(Transform t1, Transform t2) {
				return Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position));
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
		return selectedTarget;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
