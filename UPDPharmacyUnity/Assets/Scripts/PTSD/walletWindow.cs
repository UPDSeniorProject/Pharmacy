using UnityEngine;
using System.Collections;

public class walletWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	}
	void OnEnable(){
		Messenger.AddListener("open wallet", enableWalletWindow);
		Messenger.AddListener("close wallet", disableWalletWindow);
	}
	
	void OnDisable(){
		Messenger.RemoveListener("open wallet", enableWalletWindow);
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
	
	}
}
