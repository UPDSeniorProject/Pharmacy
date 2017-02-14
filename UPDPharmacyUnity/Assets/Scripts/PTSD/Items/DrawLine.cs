using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour {

    private LineRenderer lineRender;
    private float count = 0;
    private float distance;
    static bool isDrawing = false;
    public Vector3 origin;
    public Vector3 desitnation;

    public float lineDrawSpeed = 0.01f;

	// Use this for initialization
	void Start () {
        lineRender = GetComponent<LineRenderer>();
        lineRender.SetWidth(0.45f, 0.45f);
        //lineRender.SetPosition(1, desitnation);

        distance = Vector3.Distance(origin, desitnation);
	
	}
	
	// Update is called once per frame
	void Update () {
      //  if (isDrawing)
       // {
       //     drawLine
        //}
    }

    public void drawLine(List<Vector3> points)
    {
        resetLine();
        lineRender.SetVertexCount(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            lineRender.SetPosition(i, points[i]);
        }
            
            //lineRender.SetPosition(1, point);
    }
    public void resetLine() 
    {
        lineRender.SetVertexCount(0);
       // lineRender.SetPosition(0, new Vector3(0f,0f,0f));
    }
	
}
