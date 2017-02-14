using UnityEngine;
using System.Collections;

public class VPF2LocalCommunicator : VPF2Communicator {

    protected override string DefaultServerAddress
    {
        get
        {
            return "http://localhost:42378/";
        }
    }

    public override string ToString()
    {
        return "VPF 2 Local";
    }
}
