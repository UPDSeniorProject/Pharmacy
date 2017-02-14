using UnityEngine;
using System.Collections;

public class RemoveBlockItem : MonoBehaviour {
    private Transform myTransform;
    private Vector3 initialPos;
    public GameObject blockItemPrefab;
    public GameObject blockItem;
	// Use this for initialization
    void Awake ()
    {
        myTransform = transform;
        initialPos = myTransform.position;
    }
	void Start () {
        blockItem = Instantiate(blockItemPrefab, myTransform.position, myTransform.rotation) as GameObject;
        myTransform.position = myTransform.position + myTransform.forward*(-0.2f);

	}
    void OnEnable()
    {
        Messenger.AddListener("move block item", removeBlockItem);
    }
    void OnDisable()
    {
        Messenger.RemoveListener("move block item", removeBlockItem);
    }

    void removeBlockItem()
    {
        Debug.Log("remove item");
        Vector3 blockItemPos = blockItem.transform.position - blockItem.transform.right * 0.2f;
        //+blockItem.transform.right * 0.2f;
        iTween.MoveTo(blockItem, iTween.Hash("position", blockItemPos, "speed", 0.5f,
                    "orienttopath", false, "oncomplete", "moveOriginalItem", "oncompletetarget" , myTransform.gameObject, "easetype", iTween.EaseType.linear));
    }
    void moveOriginalItem()
    {
        iTween.MoveTo(myTransform.gameObject, iTween.Hash("position", initialPos, "speed", 0.5f,
                    "orienttopath", false, "easetype", iTween.EaseType.linear));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
