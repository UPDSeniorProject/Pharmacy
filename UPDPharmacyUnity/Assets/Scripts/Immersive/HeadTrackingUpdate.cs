using System;
using System.Collections;

[Serializable]
public class HeadTrackingUpdate
{
    double X;
    double Y;
    double Z;

    public HeadTrackingUpdate(double X, double Y, double Z)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }
}
