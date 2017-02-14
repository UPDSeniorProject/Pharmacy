using UnityEngine;

[System.Serializable]
public class EyeLid
{
    public const float BaseValue = 0.00001f;

    public Transform EyeLidTransform;
    public bool CanBlink = true;
    protected Vector3 LocalPosition;

    public void Setup()
    {
        LocalPosition = new Vector3(BaseValue, 0, 0);
        EyeLidTransform.localPosition = LocalPosition;
    }


    public void Blink(float val)
    {
        if (CanBlink)
        {
            LocalPosition.x = val;
            EyeLidTransform.localPosition = LocalPosition;
        }
    }
}

public class BlinkBehaviour2 : RenBehaviour
{
    public EyeLid Right;
    public EyeLid Left;

    public float BlinkDuration;
    public float FirstBlink;

    public float MinBlinkSeparation = 2.0f;
    public float MaxBlinkSeparation = 8.0f;

    private float NextBlinkAt;
    private System.Random Rand;

    protected override void Start()
    {
        base.Start();
        NextBlinkAt = FirstBlink;
        Rand = new System.Random();

        Right.Setup();
        Left.Setup();
    }

    protected override void Update()
    {
        if (isPaused) return;

        if (Time.time > NextBlinkAt)
        {
            float deltaT = Time.time - NextBlinkAt;
            float value = EyeLid.BaseValue;
            if (deltaT > BlinkDuration)
            {
                UpdateNextBlink();
            }
            else
            {
                value = ComputeBlinkProgress(deltaT);
            }

            Right.Blink(value);
            Left.Blink(value);
        }
    }

    protected void UpdateNextBlink()
    {
        NextBlinkAt = Time.time +
            (float)Rand.NextDouble() * (MaxBlinkSeparation - MinBlinkSeparation) + MinBlinkSeparation;
    }

    protected float ComputeBlinkProgress(float delta)
    {
        return Mathf.Sin(delta / BlinkDuration * Mathf.PI);
    }


}