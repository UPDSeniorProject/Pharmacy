using UnityEngine;
using System.Collections;

[System.Serializable]
public class FunduscopicExam : SimulationEvent {
    protected bool CheckedLeft = false;
    protected bool CheckedRight = false;

    public FunduscopicExam(AbstractVPFCommunicator comm)
        : base(comm)
    {
    }

    public override bool CheckCompletion()
    {
        return CheckedLeft && CheckedRight;
    }
}


public class PupillaryResponse : SimulationEvent
{
    protected bool CheckedLeft = false;
    protected bool CheckedRight = false;

    public PupillaryResponse(AbstractVPFCommunicator comm)
        : base(comm)
    {
    }

    public override bool CheckCompletion()
    {
        return CheckedLeft && CheckedRight;
    }
}

public class VisualAcuityExam : SimulationEvent
{
    protected bool CheckedRightCovered = false;
    protected bool CheckedLeftCovered = false;

    public VisualAcuityExam(AbstractVPFCommunicator comm)
        : base(comm)
    {
    }


    public void CheckedReadLine(int line, EyeSide Covered)
    {
        switch (Covered)
        {
            case EyeSide.Left:
                CheckedLeftCovered = true;
                break;
            case EyeSide.Right:
                CheckedRightCovered = true;
                break;
            case EyeSide.None:
                //Not sure what to do here.
                break;
        }
    }

    public override bool CheckCompletion()
    {
        return CheckedLeftCovered && CheckedRightCovered;
    }
}

public class ExtraocularOcularMovements : SimulationEvent
{
    bool[,] zones = new bool[3, 3];

    public ExtraocularOcularMovements(AbstractVPFCommunicator comm)
        : base(comm)
    {
        for (int i = 0; i < zones.GetLength(0); i++)
            for (int j = 0; j < zones.GetLength(1); j++)
                zones[i, j] = false;
    }

    public override bool CheckCompletion()
    {
        for (int i = 0; i < zones.GetLength(0); i++)
        {
            for (int j = 0; j < zones.GetLength(1); j++)
            {
                if (!zones[i, j]) 
                    return false;
            }
        }

        return true;
    }

}