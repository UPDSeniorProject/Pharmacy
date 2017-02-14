using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour {
	public enum State {
		atShelf,							
		picked,			
		compared,
		inCart						
	}
	public enum Type {
		cereal,							
		can									
	}
	
	public static State state;
	public Type type;
	
	private Transform myTransform;

    public Item item = new Item("", 0f, 0,"");

	//public string Objname;
	//public float price;
	//public int Quantity = 1;
	
	public Vector3  _startPosition;
	public Quaternion  _startRotation;
    public Vector3 startScale;
    public Vector3 _startLocalPosition;
	
	void Awake(){
        myTransform = transform;
        string[] namesub = name.Split('_');
        if (namesub.Length>2)
            item.Name = namesub[2];
        else
            item.Name = name;
        //item.Price = 4.49f;
        state = State.atShelf;
        _startPosition = myTransform.position;
        _startRotation = myTransform.rotation;
        startScale = myTransform.localScale;
        _startLocalPosition = myTransform.localPosition;
	}
	
	// Use this for initialization
	void Start () {
        //string[] selfname = 
        
		//name = Objname;
	}
	
	public void highlightItem()
	{
		//switch(state) {
		//case State.atShelf:
        if (PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.toCompare || PickUpItems.state == PickUpItems.State.toCompareThree)
        {
            if (Movement1.directWalkOnly)
            {
				if (myTransform.tag == "Pick" && Mathf.Abs(Vector3.Distance( myTransform.position, Camera.main.transform.position) )<=5f)
                {
                    Debug.Log("highlight");
                    //GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
					GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                    //renderer.material.shader = Shader.Find("Self-Illumin/Diffuse");
                    myTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    Messenger<bool>.Broadcast("display pick up hint", true);
                }
            }
        }
        //myTransform.localPosition = myTransform.localPosition - myTransform.forward;
		//	break;
		//}
	}
	public void unhighlightItem()
	{
		//if(state == State.atShelf)
        if (PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.toCompare || PickUpItems.state == PickUpItems.State.toCompareThree)
        {
            if (myTransform.tag == "Pick" && Mathf.Abs(Vector3.Distance( myTransform.position, _startPosition) )<0.1f)
            {
                GetComponent<Renderer>().material.shader = Shader.Find("Diffuse");
                myTransform.localScale = new Vector3(1f, 1f, 1f);
				Messenger<bool>.Broadcast("display pick up hint", false);
                // myTransform.localPosition = myTransform.localPosition;
            }
        }
	}
    void Update()
    {
        if (!( PickUpItems.state == PickUpItems.State.idle || PickUpItems.state == PickUpItems.State.toCompare || PickUpItems.state == PickUpItems.State.toCompareThree) )
        {
             //if(myTransform.localScale != _startScale)
                 //myTransform.localScale = _startScale;
        }
    }
	
	/*public void onPickItem(){
		if(state == State.atShelf){
			state = State.picked;
		Debug.Log ("item picked");	
		_myTransform.rigidbody.useGravity = false;
		_myTransform.renderer.material.shader = Shader.Find("Diffuse");
		_myTransform.parent = _cameraTransform;
		_myTransform.position = _cameraTransform.position + _cameraTransform.forward*0.5f;
		}
	}
	
	public void onPutBackItem(){
		if(state == State.picked){
			state = State.atShelf;
			Debug.Log ("item put back");	
			_myTransform.position = _startPosition;
		}
		
	}*/
	
	
}
