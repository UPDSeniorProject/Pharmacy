using UnityEngine;
using System.Collections;

public class walletWindowEnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	}
	void OnEnable(){
		Messenger.AddListener("display wallet", enableWalletWindow);
		Messenger.AddListener("close wallet", disableWalletWindow);
	}
	void OnDisable(){
		Messenger.RemoveListener("display wallet", enableWalletWindow);
		Messenger.RemoveListener("close wallet", disableWalletWindow);
	
	}
	
	
	void enableWalletWindow(){
		gameObject.SetActive(true);
	}
	
	void disableWalletWindow(){
		gameObject.SetActive(false);
	}
	// Update is called once per frame
	void Update () {
		/*if(Movement.isCheckout == true){
			Debug.Log("isCheckout == true");
			gameObject.SetActive(true);
			//NGUITools.SetActive(transform.gameObject,true);
		}
		else if(Movement.isCheckout == false){
			Debug.Log("isCheckout == false");
			NGUITools.SetActive(transform.gameObject,false);
		}*/
	}
}
