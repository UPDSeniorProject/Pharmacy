using UnityEngine;
using System.Collections;

public enum EyeSide
{
    Right,
    Left,
    None
}

public class BlinkBehaviour : RenBehaviour
{
	#region Properties
	#region Public properties
    public Transform leftEyeNerve;
    public Transform rightEyeNerve;
	
    public float blinkDuration;
	#endregion
	
	#region Private properties
    private float realBlinkDuration;

    private bool Winking;
    private EyeSide WinkSide;

    private bool closeEye;
    private EyeSide ClosedEye;
    private float CloseValue;
	
	private float nextBlinkAt;
	
	private float maxVal = 0;
	#endregion
	
	
	#endregion
    
    protected override void Start()
    {
		base.Start ();
        nextBlinkAt = 7;
        realBlinkDuration = blinkDuration; //save the duration!
    }
    
    protected override void Update()
    {
		if(isPaused) return;
		
		base.Update();
		
        if (Time.time > nextBlinkAt)
        {
            float deltaT = Time.time - nextBlinkAt;

            System.Random rand = new System.Random();

            if (deltaT > blinkDuration)
            {
                //eye1nerve.localPosition.y = 0;

                Vector3 vec = new Vector3(0, 0, 0);
                vec.x = 1.0f;

                leftEyeNerve.localPosition = new Vector3(0.00001f, 0, 0);
                rightEyeNerve.localPosition = new Vector3(0.00001f, 0, 0);

                nextBlinkAt = Time.time + (float)rand.NextDouble() * 6 + 2;

                Winking = false;
                blinkDuration = realBlinkDuration; //restore
//                Debug.Log("maxVal = "  + maxVal);
            }
            else
            {
                float val = Mathf.Sin(deltaT / blinkDuration * Mathf.PI);
                maxVal = Mathf.Max(val, maxVal);
                if(!Winking || WinkSide == EyeSide.Left) 
                    leftEyeNerve.localPosition = new Vector3(val, 0, 0);
                if(!Winking || WinkSide == EyeSide.Right)
                    rightEyeNerve.localPosition = new Vector3(val, 0, 0);
            }
        }


        if (closeEye)
        {
            CloseValue = Mathf.Min(CloseValue + 0.1f, 1.2f);
            switch (ClosedEye)
            {//override the winks...
                case EyeSide.Right:
                    rightEyeNerve.localPosition = new Vector3(CloseValue, 0, 0);
                    break;
                case EyeSide.Left:
                    leftEyeNerve.localPosition = new Vector3(CloseValue, 0, 0);
                    break;
            }
        }
    }

    public void Wink(EyeSide side)
    {
        Winking = true;
        WinkSide = side;
        realBlinkDuration = blinkDuration;
        blinkDuration = 2.0f; //(winks take longer than blinks!)

        nextBlinkAt = Time.time;
    }

    public void CloseEye(EyeSide side)
    {
        if (closeEye && side != ClosedEye)
        {
            Resume();
        }
        ClosedEye = side;
        closeEye = true;
        CloseValue = 0.00001f;
    }

    public void CloseBothEyes()
    {
        blinkDuration = 2.0f;
        nextBlinkAt = Time.time;
    }

    public void Resume()
    {
        closeEye = false;
        CloseValue = 0.00001f;
        switch (ClosedEye)
        {//override the winks...
            case EyeSide.Right:
                rightEyeNerve.localPosition = new Vector3(CloseValue, 0, 0);
                break;
            case EyeSide.Left:
                leftEyeNerve.localPosition = new Vector3(CloseValue, 0, 0);
                break;
        }

    }
}
