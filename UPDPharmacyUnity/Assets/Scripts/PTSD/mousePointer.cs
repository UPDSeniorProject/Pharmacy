using UnityEngine;
using System.Collections;
 
public class mousePointer : MonoBehaviour 
{
    public Texture2D cursorImage;
 
    private int cursorWidth = 32;
    private int cursorHeight = 32;
 	
	//private bool wasLocked = false;
    void Start()
    {
		Screen.lockCursor = false;
		Screen.lockCursor = true;
		//Screen.showCursor = true; 
        Cursor.visible = false;
    }
 	void Update(){
		
        //    Screen.lockCursor = true;
			
    //if(Screen.lockCursor == false)
		//Screen.lockCursor = true;
	if (Input.GetKeyDown ("escape"))
        Screen.lockCursor = false;
	/*	if (!Screen.lockCursor && wasLocked) {
        wasLocked = false;
        //Screen.lockCursor = false;
    }
    // Did we gain cursor locking?
    else if (Screen.lockCursor && !wasLocked) {
       wasLocked = true;
        //Screen.lockCursor = true;
   }*/
	
	}
 
    void OnGUI()
    {
        //GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, cursorWidth, cursorHeight), cursorImage);
    }
}