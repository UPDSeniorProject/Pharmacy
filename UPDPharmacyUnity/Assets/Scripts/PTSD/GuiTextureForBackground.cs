using UnityEngine;
using System.Collections;

public class GuiTextureForBackground : MonoBehaviour {

	void Awake()
	{
		GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		
	}
	void Update()
	{
		Color colPreviousGUIColor = GetComponent<GUITexture>().color;
 
       GetComponent<GUITexture>().color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, 0.3f);
      
	}
}
