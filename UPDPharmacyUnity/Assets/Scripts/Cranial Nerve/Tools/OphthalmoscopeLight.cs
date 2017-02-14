using UnityEngine;
using System.Collections;

public class OphthalmoscopeLight : RenBehaviour {

    protected Light _light;

    public Vector3 LightForwardVector;
    public Quaternion LightRotation;

    protected override void Start()
    {
        base.Start();

        _light = gameObject.GetComponent<Light>();
    }

	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
        if (_light.enabled)
        { //we only update the direction if the light is enabled.
            Vector3 lightPosition = gameObject.transform.position;
            Vector3 cameraPosition = Camera.main.transform.position;
            //Compute vector from the camera projection point to the light
            
            LightForwardVector = lightPosition - cameraPosition;
            LightRotation = Quaternion.LookRotation(LightForwardVector);
            gameObject.transform.rotation = Quaternion.LookRotation(LightForwardVector);
        }
	}

    public bool IsEnabled
    {
        get
        {
            return _light.enabled;
        }
        set
        {
            if (_light == null)
                _light = gameObject.GetComponent<Light>();
            _light.enabled = value;
        }
    }
}
