using UnityEngine;
using System.Collections;

public class OphthalmoscopeTool : CranialNerveTool
{
    protected OphthalmoscopeLight OphthalmoscopeLight;


    protected Vector3 OriginalCameraPosition;
    protected Vector3 OriginalOphthalmoscopePosition;

    public RenButton LightButton;
    public bool IsLightOn;

    public RenImage LeftFundus;
    public RenImage RightFundus;

    public GameObject RightEye_Pupil;
    public GameObject LeftEye_Pupil;

    public event FundusEventDelegate FundusEvent;
    public CranialNerveAppActionHandler CranialNerveHandler;

#if UNITY_EDITOR
    public RenButton PrintAngles;
    public string left, right;

    void PrintAngles_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        Debug.Log(string.Format("Right: {0} \nLeft: {1}",right,left));
    }
#endif

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    
        Visible = false;

        //Initialize the Light
        OphthalmoscopeLight = GetComponentInChildren<OphthalmoscopeLight>();
        OphthalmoscopeLight.IsEnabled = IsLightOn = false;

        LightButton.ButtonPressed += LightButton_ButtonPressed;
        LightButton.ShouldRender = false;
        AddGUIElement(LightButton);

        OriginalOphthalmoscopePosition = transform.position;
        OriginalCameraPosition = Camera.main.transform.position;

        AddGUIElements(LeftFundus, RightFundus);
        LeftFundus.ShouldRender = RightFundus.ShouldRender = false;

        if (RightEye_Pupil == null)
        {
            RightEye_Pupil = GameObject.Find("RightEye_Pupil");
            if (RightEye_Pupil == null)
            {
                AddDebugLine("Couldn't find a RightEye_Pupil GameObject");
            }
        }


        if (LeftEye_Pupil == null)
        {
            LeftEye_Pupil = GameObject.Find("LeftEye_Pupil");
            if (LeftEye_Pupil == null)
            {
                AddDebugLine("Couldn't find a LeftEye_Pupil GameObject");
            }
        }

        HelpBox.text = "- Use your mouse to drag the Ophthalmoscope around.\n" +
                       "- Use the up and down arrow keys or your mouse scroll wheel to zoom in and out.";

#if UNITY_EDITOR
        left = right = "";
        PrintAngles.Label = "Print Angles";
        PrintAngles.ButtonPressed += PrintAngles_ButtonPressed;
        AddGUIElement(PrintAngles);
#endif

    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(IsActive)
            CheckFundus();
    }

    public override void ActivateTool()
    {
        base.ActivateTool();

        Visible = true;
        OphthalmoscopeLight.IsEnabled = IsLightOn;
        LightButton.ShouldRender = true;

        transform.position = OriginalOphthalmoscopePosition;
        
        Camera.main.transform.position = OphthalmoscopeCameraPosition;
    }

    public override void DeactivateTool()
    {
        Visible = false;
        //The Light should always be off when desactivated (Weird rendering if not)
        OphthalmoscopeLight.IsEnabled = false;
        LightButton.ShouldRender = false;

        LeftFundus.ShouldRender = false;
        RightFundus.ShouldRender = false;

        //Restore the transform
        Camera.main.transform.position = OriginalCameraPosition;
    }

    public void CheckFundus()
    {
        if (IsLightOn)
        {
            EyeSide side = CranialNerveHandler.CoveredEye();
            Vector3 distanceVector;
            float distance, angle;


            if (side != EyeSide.Right)
            {
                //RightEye 
                distanceVector = RightEye_Pupil.transform.position - OphthalmoscopeLight.gameObject.transform.position;
                distance = distanceVector.magnitude;
                angle = Vector3.Angle(OphthalmoscopeLight.LightForwardVector, distanceVector);

                if (distance < 13 && angle < 4)
                {
                    if (!RightFundus.ShouldRender)
                    {
                        OnFundusEvent(new FundusEventArgs(EyeSide.Right, true));
                    }
                    RightFundus.ShouldRender = true;
                }
                else
                {
                    if (RightFundus.ShouldRender)
                        OnFundusEvent(new FundusEventArgs(EyeSide.Right, false));

                    RightFundus.ShouldRender = false;
                }
#if UNITY_EDITOR
                right = string.Format("Distance: {0} -- Angle: {1} -- Right: {2}", distance, angle, RightFundus.ShouldRender);               
#endif
            }

            if (side != EyeSide.Left)
            {
                //LeftEye 
                distanceVector = LeftEye_Pupil.transform.position - OphthalmoscopeLight.gameObject.transform.position;
                distance = distanceVector.magnitude;
                angle = Vector3.Angle(OphthalmoscopeLight.LightForwardVector, distanceVector);

                if (distance < 13 && angle < 4)
                {
                    if (!LeftFundus.ShouldRender){ //if it wasn't rendering
                        OnFundusEvent(new FundusEventArgs(EyeSide.Left, true));
                    }

                    LeftFundus.ShouldRender = true;
                }
                else
                {
                    if (LeftFundus.ShouldRender) //if it was rendering
                        OnFundusEvent(new FundusEventArgs(EyeSide.Left, false));

                    LeftFundus.ShouldRender = false;
                }
#if UNITY_EDITOR
                left = string.Format("Distance: {0} -- Angle: {1} -- Left: {2}", distance, angle, LeftFundus.ShouldRender);
#endif
            }
        }
        else
        {
            if (LeftFundus.ShouldRender)
                OnFundusEvent(new FundusEventArgs(EyeSide.Left, false));
            else if(RightFundus.ShouldRender)
                OnFundusEvent(new FundusEventArgs(EyeSide.Right, false));

            LeftFundus.ShouldRender = RightFundus.ShouldRender = false;
        }

        //light.gameObject
    }

    public override void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLight();
        }

        CheckZoom();
        
    }

    public void ToggleLight()
    {
        if (IsLightOn)
            TurnLightOff();
        else
            TurnLightOn();

    }

    public void TurnLightOn()
    {
        LightButton.Label = "Turn Light Off";
        //Turn light on.
        OphthalmoscopeLight.IsEnabled = IsLightOn = true;

    }

    public void TurnLightOff()
    {
        LightButton.Label = "Turn Light On";
        //Turn light off
        OphthalmoscopeLight.IsEnabled = IsLightOn = false;
    }

    void LightButton_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
    {
        ToggleLight();
    }

    #region Zooming functions

    protected string SCROLLWHEEL_AXIS = "Mouse ScrollWheel";

    public float MinZoomPlane = -1000;
    public float MaxZoomPlane = 1000;

    public float CameraStep = 0.4f;
    public float ToolStep = 0.3f;

    public Vector3 OphthalmoscopeCameraPosition;

    protected bool ShouldZoomOut()
    {
        return Input.GetAxis(SCROLLWHEEL_AXIS) < 0 || Input.GetKey(KeyCode.DownArrow);
    }

    protected bool ShouldZoomIn()
    {
        return Input.GetAxis(SCROLLWHEEL_AXIS) > 0 || Input.GetKey(KeyCode.UpArrow);
    }

    protected void CheckZoom()
    {
        if (ShouldZoomOut())
        {
            if (Camera.main.transform.position.z < MaxZoomPlane)
            {
                Zoom(CameraStep, ToolStep);
            }
        }
        else if (ShouldZoomIn())
        {
            if (Camera.main.transform.position.z > MinZoomPlane)
            {
                Zoom(-CameraStep, -ToolStep);
            }
        }
    }

    protected void Zoom(float cameraStep, float toolStep)
    {
        Vector3 CameraPosition = Camera.main.transform.position;
        CameraPosition.z += cameraStep;
        Camera.main.transform.position = CameraPosition;

        Vector3 ToolPosition = transform.position;
        ToolPosition.z += toolStep;
        transform.position = ToolPosition;
    }

    #endregion

    public override bool RequiresEyeCamera()
    {
        return true;
    }


    public override string Name
    {
        get { return "OphthalmoscopeTool"; }
    }

    protected void OnFundusEvent(FundusEventArgs args)
    {
        if (FundusEvent != null)
        {
            FundusEvent(this, args);
        }
        else
        {
            //AddDebugLine("FundusEvent : " + args + " on Ophth but no handler");
        }
    }
}


public class FundusEventArgs : System.EventArgs
{
    public EyeSide Side;
    public bool Showing;

    public FundusEventArgs(EyeSide side, bool show) : base()
    {
        Side = side;
        Showing = show;
    }

    public override string ToString()
    {
        return string.Format("[FundusEvent on {0} and {1}]", Side, Showing ? "is showing" : "isn't showing");
    }
}

public delegate void FundusEventDelegate(OphthalmoscopeTool tool, FundusEventArgs args);

