using UnityEngine;
using System.Collections;

/*public class SceneFadeInOut : MonoBehaviour {
	/*public float fadeSpeed = 1.5f;
	public bool sceneStarting = true;

	void Awake()
	{
		GUITexture.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
	}

	// Use this for initialization
	void Start () {
		FadetoClear ();

		if (GUITexture.color.a <= 0.05f) 
		{
			GUITexture.color = Color.clear;
			GUITexture.enabled = false;
			sceneStarting = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FadetoClear()
	{
		GUITexture.color = Color.Lerp (GUITexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	void FadetoBlack()
	{
		GUITexture.color = Color.Lerp (GUITexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}

	public void EndScene()
	{
		GUITexture.enabled = ture;
		FadetoBlack ();

		if (GUITexture.color.a <= 0.05f) {
			Application.LoadLevel ();
		}
	}*/
//}
