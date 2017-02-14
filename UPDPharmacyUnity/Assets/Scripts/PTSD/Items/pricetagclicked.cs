using UnityEngine;
using System.Collections;

public class pricetagclicked : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    void OnMouseUpAsButton()
    {
        TextMesh PriceTagName = transform.FindChild("PriceTagName").GetComponent<TextMesh>();
        TextMesh PriceTagPrice = transform.FindChild("PriceTagPrice").GetComponent<TextMesh>();
        //Messenger<string, string>.Broadcast("Display price tag on GUI", PriceTagName.text, PriceTagPrice.text);
    }
}
