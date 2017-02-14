using UnityEngine;
using System.Collections;

public class IsInSpotlight : RenBehaviour {

    public Transform nerve;
    public Transform target;

    public EyeSide Side;

    // **Important** minDistance is the distance between the spotlight and the plane where eyeSight is %100. for instance, Make sure is smaller than the spot light range.
    public float minDistance = 8;
    public float lightSizeMultiplier = 1.2f;
    public Light ambientLight;

    protected float angle;
    protected float distance;
    protected float eyeSight;
    protected float hSlider;

    protected override void Start()
    {
        base.Start();
        if (nerve == null)
        {
            GameObject obj = GameObject.Find(NerveName());
            if(obj != null)
                nerve = obj.transform;
            else
                throw new System.Exception("Could not find nerve with name: " + NerveName() + ". Did you add the CranialNerveModel?");
        }

        if (target == null)
        {
            GameObject obj = GameObject.Find(TargetName());
            if (obj != null)
                target = obj.transform;
            else
                throw new System.Exception("Could not find nerve with name: " + TargetName() + ". Did you add the CranialNerveModel?");
        }

        if (ambientLight == null)
        {
            GameObject obj = GameObject.Find("Ambient light");
            if (obj != null)
                ambientLight = obj.GetComponent<Light>();
            else
                throw new System.Exception("Could not find the ambient light, if you don't have a GameObject named 'Ambient light' please add it, or add a reference to your ambient light");
        }

    }

    protected string NerveName()
    {
        switch (Side)
        {
            case EyeSide.Left:
                return "leye";
            case EyeSide.Right:
                return "reye";
            default:
                throw new System.Exception("EyeSide not set for IsInSpotlight script");
        }
    }

    protected string TargetName()
    {
        switch (Side)
        {
            case EyeSide.Left:
                return "RightEye";
            case EyeSide.Right:
                return "LeftEye";
            default:
                throw new System.Exception("EyeSide not set for IsInSpotlight script");
        }
    }


    protected override void Update()
    {
 	    base.Update();
        DetectObject(target,GetComponent<Light>().range,GetComponent<Light>().spotAngle*lightSizeMultiplier);
    }



    protected void DetectObject(Transform obj, float range, float sAngle){
    //if(light.enabled){
	    Vector3 v = obj.position - transform.position;

	    distance = Vector3.Distance(obj.position,transform.position);
	    angle = Vector3.Angle(v,transform.forward);
	    float d = ((range - distance)/(range-minDistance)) * 100;
	    eyeSight = ((sAngle - angle * 2)/sAngle) * d;
	    if (distance <=range && angle<=sAngle/2&&GetComponent<Light>().enabled){
            if (!IsIn)
            {
                IsIn = true;
                OnSpotlightEvent(new IsInSpotlightArgs(IsIn));
            }
	    } else {
	        eyeSight = 0;
            if (IsIn)
            {
                IsIn = false;
                OnSpotlightEvent(new IsInSpotlightArgs(IsIn));
            }
	    }
	
		if (eyeSight > 100)
			eyeSight = 100;
	
		nerve.localPosition = new Vector3((eyeSight / 100.0f) + ambientLight.intensity, 0, 0);
    }

    public float NerveLocalPosition()
    {
        return nerve.localPosition.x;
    }

    public bool IsIn = false;
    public event IsInSpotlightEvent SpotlightEvent;

    protected void OnSpotlightEvent(IsInSpotlightArgs args)
    {
        if (SpotlightEvent != null)
        {
            SpotlightEvent(this, args);
        }
        else
        {
        }
    }



}



public class IsInSpotlightArgs : System.EventArgs 
{
    public bool In;

    public IsInSpotlightArgs(bool _in)
        : base()
    {
        In = _in;
    }

}

public delegate void IsInSpotlightEvent(IsInSpotlight spotlight, IsInSpotlightArgs args);
