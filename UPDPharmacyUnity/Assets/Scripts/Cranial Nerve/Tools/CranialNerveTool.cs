using UnityEngine;
using System.Collections;

public abstract class CranialNerveTool : PhysicalExaminationTool 
{
    protected bool _visible = false;

    public bool RestoreToOriginalPosition = false;
    protected Vector3 OriginalPoistion;

    public virtual bool Visible
    {
        get
        {
            return _visible;
        }
        set
        {
            _visible = value;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = value;
        }
    }

    public virtual bool RequiresEyeCamera()
    {
        return false;
    }


    protected override void Start()
    {
        base.Start();
        OriginalPoistion = gameObject.transform.position;
    }

    public override void ActivateTool()
    {
        if (RestoreToOriginalPosition)
        {
            gameObject.transform.position = OriginalPoistion;
        }
    }

}
